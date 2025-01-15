namespace Un.Interpreter;

public static class Lexer
{
    public static List<Token> Lex(List<Token> tokens, Field field)
    {
        List<Token> analyzedTokens = [];

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (token.type == Token.Type.LBrack)
            {
                int j = Token.IndexOfPair(tokens, Token.Type.LBrack, i);

                if (analyzedTokens.Count > 0 && IsIndexable(analyzedTokens[^1].type))
                {
                    int colon = Token.IndexOf(tokens, Token.Type.Colon, i);

                    if (colon > 0)
                    {
                        Obj start = i + 1 == colon ? Int.Zero : Calculator.All(tokens[(i + 1)..colon], field);
                        Obj end = colon + 1 == j ? Int.MinusOne : Calculator.All(tokens[(colon + 1)..j], field);

                        if (start is not Int || end is not Int) throw new SyntaxError();

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

                i = j;
            }
            else if (token.type == Token.Type.LParen)
            {
                int j = Token.IndexOfPair(tokens, Token.Type.LParen, i);

                if (analyzedTokens.Count == 0 || !IsCallable(analyzedTokens[^1].type))
                {
                    analyzedTokens.Add(new($"{Token.String(tokens[i..(j + 1)])}", Token.Type.Tuple));
                }
                else
                {
                    analyzedTokens[^1].type = analyzedTokens[^1].type == Token.Type.Method ? Token.Type.Method : Token.Type.Function;
                    analyzedTokens[^1].Value += $"{Token.String(tokens[i..(j + 1)])}";
                }

                i = j;
            }
            else if (token.type == Token.Type.LBrace)
            {
                int j = i + 1, depth = 1;
                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LBrace)
                        depth++;
                    if (tokens[j].type == Token.Type.RBrace)
                        depth--;
                    if (depth == 0) break;
                    j++;
                }

                if (i + 1 == j)
                    tokens.Add(new Token("{}", Token.Type.Dictionary));
                else
                {
                    string value = "";

                    for (int k = i; k <= j; k++)
                        value += tokens[k].Value;

                    tokens.Add(new Token(value, Token.Type.Dictionary));
                }

                i = j;
            }
            else if (token.type == Token.Type.Dot)
            {
                if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Class)
                    analyzedTokens[^1].type = Token.Type.Variable;

                analyzedTokens.Add(new(tokens[i + 1].Value, Token.Type.Property));
                i++;

                if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
                    analyzedTokens[^1].type = Token.Type.Method;
            }
            else if (token.type == Token.Type.QuestionDot)
            {
                if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Class)
                    analyzedTokens[^1].type = Token.Type.Variable;

                analyzedTokens.Add(new(tokens[i + 1].Value, Token.Type.NullableProperty));
                i++;

                if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
                    analyzedTokens[^1].type = Token.Type.NullableMethod;
            }
            else if (token.type == Token.Type.Lambda)
            {                
                analyzedTokens[^1] = new Token($"{analyzedTokens[^1].Value}{Token.String(tokens[i..])}", Token.Type.Lambda);
                break;
            }
            else if (token.type == Token.Type.Plus)
            {
                if (analyzedTokens.Count == 0 || analyzedTokens.Count != 0 && Token.IsBasicOperator(analyzedTokens[^1].type))
                {
                    if (tokens[i + 1].type == Token.Type.Integer)
                        analyzedTokens.Add(new Token(Literals.Plus + tokens[i + 1].Value, Token.Type.Integer));
                    else if (tokens[i + 1].type == Token.Type.Float)
                        analyzedTokens.Add(new Token(Literals.Plus + tokens[i + 1].Value, Token.Type.Float));
                    else throw new SyntaxError();
                    i++;
                }
                else analyzedTokens.Add(tokens[i]);
            }
            else if (token.type == Token.Type.Minus)
            {
                if (analyzedTokens.Count == 0 || analyzedTokens.Count != 0 && Token.IsBasicOperator(analyzedTokens[^1].type))
                {
                    if (tokens[i + 1].type == Token.Type.Integer)
                        analyzedTokens.Add(new Token(Literals.Minus + tokens[i + 1].Value, Token.Type.Integer));
                    else if (tokens[i + 1].type == Token.Type.Float)
                        analyzedTokens.Add(new Token(Literals.Minus + tokens[i + 1].Value, Token.Type.Float));
                    else throw new SyntaxError();
                    i++;
                }
                else analyzedTokens.Add(tokens[i]);
            }
            else if (token.type == Token.Type.Variable)
            {
                if (Process.IsClass(token.Value))
                    analyzedTokens.Add(new(token.Value, Token.Type.Function));
                else analyzedTokens.Add(token);
            }
            else analyzedTokens.Add(token);
        }

        return analyzedTokens;
    }

    private static bool IsCallable(Token.Type type) => type switch
    {
        Token.Type.Variable or Token.Type.Function or Token.Type.Method or Token.Type.Class or Token.Type.Lambda => true,
        _ => false
    };

    private static bool IsIndexable(Token.Type type) => type switch
    {
        Token.Type.Variable or Token.Type.Function or Token.Type.Method or
        Token.Type.List or Token.Type.String or Token.Type.Indexer or Token.Type.Property => true,
        _ => false
    };
}
