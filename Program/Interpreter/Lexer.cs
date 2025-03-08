namespace Un.Interpreter;

public static class Lexer
{
    public static List<Token> Lex(List<Token> tokens, Field field)
    {
        List<Token> analyzedTokens = [];

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            switch (token.type)
            {
                case Token.Type.LBrack:
                    i = HandleLBrack(tokens, field, analyzedTokens, i);
                    break;

                case Token.Type.LParen:
                    i = HandleLParen(tokens, analyzedTokens, i);
                    break;

                case Token.Type.LBrace:
                    i = HandleLBrace(tokens, analyzedTokens, i);
                    break;

                case Token.Type.Dot:
                    i = HandleDot(tokens, analyzedTokens, i);
                    break;

                case Token.Type.QuestionDot:
                    i = HandleQuestionDot(tokens, analyzedTokens, i);
                    break;

                case Token.Type.Lambda:
                    HandleLambda(tokens, analyzedTokens, i);
                    i = tokens.Count; 
                    break;

                case Token.Type.Plus:
                    i = HandlePlus(tokens, analyzedTokens, i);
                    break;

                case Token.Type.Minus:
                    i = HandleMinus(tokens, analyzedTokens, i);
                    break;

                case Token.Type.Variable:
                    HandleVariable(token, analyzedTokens);
                    break;

                default:
                    analyzedTokens.Add(token);
                    break;
            }
        }

        return analyzedTokens;
    }

    private static int HandleLBrack(List<Token> tokens, Field field, List<Token> analyzedTokens, int i)
    {
        int j = Token.IndexOfPair(tokens, Token.Type.LBrack, i);

        if (analyzedTokens.Count > 0 && IsIndexable(analyzedTokens[^1].type))
        {
            int colon = Token.IndexOf(tokens, Token.Type.Colon, i);

            if (colon > 0)
            {
                Obj start = i + 1 == colon ? Int.Zero : Calculator.All(tokens[(i + 1)..colon], field);
                Obj end = colon + 1 == j ? Int.MinusOne : Calculator.All(tokens[(colon + 1)..j], field);

                if (!start.As<Int>(out _) || !end.As<Int>(out _)) 
                    throw new SyntaxError("slice indices must be integers or None");    

                analyzedTokens.Add(new Token($"{start.CStr().Value}:{end.CStr().Value}", Token.Type.Slicer));
            }
            else
            {
                Obj index = Calculator.All(tokens[(i + 1)..j], field);
                analyzedTokens.Add(new Token(index is Str ? $"\"{index.CStr().Value}\"" : index.CStr().Value, Token.Type.Indexer));
            }
        }
        else
        {
            analyzedTokens.Add(new(Token.String(tokens[i..(j + 1)]), Token.Type.List));
        }

        return j;
    }

    private static int HandleLParen(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        int j = Token.IndexOfPair(tokens, Token.Type.LParen, i);

        if (analyzedTokens.Count == 0 || !IsCallable(analyzedTokens[^1].type))
        {
            analyzedTokens.Add(new(Token.String(tokens[i..(j + 1)]), Token.Type.Tuple));
        }
        else
        {
            analyzedTokens[^1].type = analyzedTokens[^1].type == Token.Type.Method ? Token.Type.Method : Token.Type.Function;
            analyzedTokens[^1].value.Append(Token.String(tokens[i..(j + 1)]));
        }

        return j;
    }

    private static int HandleLBrace(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        int j = i + 1, depth = 1;
        while (j < tokens.Count)
        {
            if (tokens[j].type == Token.Type.LBrace) depth++;
            if (tokens[j].type == Token.Type.RBrace) depth--;
            if (depth == 0) break;
            j++;
        }

        if (i + 1 == j)
        {
            analyzedTokens.Add(new Token(Literals.LBrace + Literals.RBrace, Token.Type.Dictionary));
        }
        else
        {
            StringBuffer value = new();
            for (int k = i; k <= j; k++) value.Append(tokens[k].value);
            analyzedTokens.Add(new Token(value, Token.Type.Dictionary));
        }

        return j;
    }

    private static int HandleDot(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Class)
            analyzedTokens[^1].type = Token.Type.Variable;

        analyzedTokens.Add(new(tokens[i + 1].value, Token.Type.Property));
        i++;

        if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
            analyzedTokens[^1].type = Token.Type.Method;

        return i;
    }

    private static int HandleQuestionDot(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Class)
            analyzedTokens[^1].type = Token.Type.Variable;

        analyzedTokens.Add(new(tokens[i + 1].value, Token.Type.NullableProperty));
        i++;

        if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
            analyzedTokens[^1].type = Token.Type.NullableMethod;

        return i;
    }

    private static void HandleLambda(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        analyzedTokens[^1] = new Token(new StringBuffer().Append(analyzedTokens[^1].value).Append(Token.String(tokens[i..])), Token.Type.Lambda);
    }

    private static int HandlePlus(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        if (tokens.Count != 0 && (analyzedTokens.Count == 0 || Token.IsBasicOperator(analyzedTokens[^1].type)))
        {
            if (Token.IsNumber(tokens[i + 1].type))
                analyzedTokens.Add(new Token(new StringBuffer(Literals.Plus).Append(tokens[i + 1].value), tokens[i + 1].type));
            else throw new SyntaxError();
            i++;
        }
        else
        {
            analyzedTokens.Add(tokens[i]);
        }

        return i;
    }

    private static int HandleMinus(List<Token> tokens, List<Token> analyzedTokens, int i)
    {
        if (tokens.Count != 0 && (analyzedTokens.Count == 0 || Token.IsBasicOperator(analyzedTokens[^1].type)))
        {
            if (Token.IsNumber(tokens[i + 1].type))
                analyzedTokens.Add(new Token(new StringBuffer(Literals.Minus).Append(tokens[i + 1].value), tokens[i + 1].type));
            else throw new SyntaxError();
            i++;
        }
        else
        {
            analyzedTokens.Add(tokens[i]);
        }

        return i;
    }

    private static void HandleVariable(Token token, List<Token> analyzedTokens)
    {
        if (Process.IsClass(token))
            analyzedTokens.Add(new(token.value, Token.Type.Function));
        else
            analyzedTokens.Add(token);
    }

    private static bool IsCallable(Token.Type type) => type switch
    {
        Token.Type.Variable or Token.Type.Function or Token.Type.Method or Token.Type.Class or Token.Type.Lambda => true,
        _ => false
    };

    private static bool IsIndexable(Token.Type type) => type switch
    {
        Token.Type.Variable or Token.Type.Function or Token.Type.Method or Token.Type.List or Token.Type.String or Token.Type.Indexer or Token.Type.Property => true,
        _ => false
    };
}
