namespace Un;

public static class Lexer
{
    public static List<Token> Lex(List<Token> tokens, Field field)
    {
        List<Token> analyzedTokens = [];

        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].type == Token.Type.LBrack)
            {
                int j = i + 1, depth = 1;
                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LBrack)
                        depth++;
                    if (tokens[j].type == Token.Type.RBrack)
                        depth--;
                    if (depth == 0) break;
                    j++;
                }

                if (analyzedTokens.Count > 0 &&
                   (analyzedTokens[^1].type == Token.Type.Variable ||
                    analyzedTokens[^1].type == Token.Type.List ||
                    analyzedTokens[^1].type == Token.Type.String ||
                    analyzedTokens[^1].type == Token.Type.Indexer ||
                    analyzedTokens[^1].type == Token.Type.Property))
                {
                    int colon = Token.IndexOf(tokens, Token.Type.Colon, i);

                    if (colon > 0)
                    {
                        Obj start = i + 1 == colon ? new Int() : Calculator.All(tokens[(i + 1)..colon], field);
                        Obj end = colon + 1 == j ? new Int(-1) : Calculator.All(tokens[(colon + 1)..j], field);

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
                    string value = "[";
                    depth = 1;

                    j = i + 1;

                    while (j < tokens.Count)
                    {
                        if (tokens[j].type == Token.Type.LBrack)
                            depth++;
                        if (tokens[j].type == Token.Type.RBrack)
                            depth--;
                        if (depth <= 0) break;

                        value += Token.IsOperator(tokens[j]) ? $" {tokens[j++].Value} " : tokens[j++].Value;
                    }

                    value += "]";

                    analyzedTokens.Add(new(value, Token.Type.List));
                }

                i = j;
            }
            else if (tokens[i].type == Token.Type.Dot)
            {
                if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Class)
                    analyzedTokens[^1].type = Token.Type.Variable;

                analyzedTokens.Add(new(tokens[i + 1].Value, Token.Type.Property));
                i++;

                if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
                    analyzedTokens[^1].type = Token.Type.Method;
            }
            else if (tokens[i].type == Token.Type.Lambda)
            {
                string keyword = "lambda ";
                string arg = "";
                string code = "";

                while (analyzedTokens[^1].type == Token.Type.Variable)
                {
                    arg += analyzedTokens[^1].Value;
                    analyzedTokens.RemoveAt(analyzedTokens.Count - 1);

                    if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Comma)
                    {
                        arg += ",";
                        analyzedTokens.RemoveAt(analyzedTokens.Count - 1);
                    }
                    else break;
                }

                int j = i, depth = 0;

                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LParen)
                        depth++;
                    if (tokens[j].type == Token.Type.RParen)
                        depth--;
                    code += tokens[j].Value;

                    if (depth <= 0 && tokens[j].type == Token.Type.RParen) break;
                    j++;
                }

                analyzedTokens.Add(new Token(keyword + arg + code, Token.Type.Lambda));

                i = j;
            }
            else if (tokens[i].type == Token.Type.LBrace)
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
                    string Value = "";

                    for (int k = i; k <= j; k++)
                        Value += tokens[k].Value;

                    tokens.Add(new Token(Value, Token.Type.Dictionary));
                }

                i = j;
            }
            else if (tokens[i].type == Token.Type.LParen)
            {
                if (analyzedTokens.Count == 0 || !IsCallable(analyzedTokens[^1].type))
                {
                    analyzedTokens.Add(tokens[i]);
                    continue;
                }

                analyzedTokens[^1].type = analyzedTokens[^1].type == Token.Type.Method ? Token.Type.Method : Token.Type.Function;

                string value = "(";
                int j = i + 1, depth = 1;

                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LParen)
                        depth++;
                    if (tokens[j].type == Token.Type.RParen)
                        depth--;
                    if (depth <= 0) break;
                    value += Token.IsOperator(tokens[j]) ? $" {tokens[j++].Value} " : tokens[j++].Value;
                }

                value += ")";

                analyzedTokens[^1].Value += $"|{value}";

                //analyzedTokens.Add(new Token(value, Token.Type.Tuple));

                i = j;
            }
            else if (tokens[i].type == Token.Type.Plus)
            {
                if (analyzedTokens.Count == 0 || (analyzedTokens.Count != 0 && Token.IsBasicOperator(analyzedTokens[^1])))
                {
                    if (tokens[i + 1].type == Token.Type.Integer)
                        analyzedTokens.Add(new Token("+" + tokens[i + 1].Value, Token.Type.Integer));
                    else if (tokens[i + 1].type == Token.Type.Float)
                        analyzedTokens.Add(new Token("+" + tokens[i + 1].Value, Token.Type.Float));
                    i++;
                }
                else analyzedTokens.Add(tokens[i]);
            }
            else if (tokens[i].type == Token.Type.Minus)
            {
                if (analyzedTokens.Count == 0 || (analyzedTokens.Count != 0 && Token.IsBasicOperator(analyzedTokens[^1])))
                {
                    if (tokens[i + 1].type == Token.Type.Integer)
                        analyzedTokens.Add(new Token("-" + tokens[i + 1].Value, Token.Type.Integer));
                    else if (tokens[i + 1].type == Token.Type.Float)
                        analyzedTokens.Add(new Token("-" + tokens[i + 1].Value, Token.Type.Float));
                    i++;
                }
                else analyzedTokens.Add(tokens[i]);
            }
            else analyzedTokens.Add(tokens[i]);
        }

        return analyzedTokens;
    }

    private static bool IsCallable(Token.Type type) => type switch
    {
        Token.Type.Variable or Token.Type.Function or Token.Type.Method or Token.Type.Class => true,
        _ => false
    };
}
