using Un.Object;

namespace Un.Supporter
{
    public static class Lexer
    {
        public readonly static Calculator Calculator = new();

        public static List<Token> Lex(List<Token> tokens, Dictionary<string, Obj> properties)
        {
            List<Token> analyzedTokens = [];

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].type == Token.Type.LBrack)
                {
                    int j = i + 1, depth = 1;
                    while (j < tokens.Count && depth > 0)
                    {
                        if (tokens[j].type == Token.Type.LBrack)
                            depth++;
                        if (tokens[j].type == Token.Type.RBrack)
                            depth--;
                        j++;
                    }

                    if (analyzedTokens.Count > 0 &&
                       (analyzedTokens[^1].type == Token.Type.Variable ||
                        analyzedTokens[^1].type == Token.Type.Iterator ||
                        analyzedTokens[^1].type == Token.Type.String ||
                        analyzedTokens[^1].type == Token.Type.Indexer ||
                        analyzedTokens[^1].type == Token.Type.Property))
                    {
                        j--;
                        Obj index = Calculator.Calculate(Lex(tokens[(i + 1)..j], properties), properties);

                        if (index is Str)
                            analyzedTokens.Add(new Token($"\"{index.CStr().value}\"", Token.Type.Indexer));
                        else
                            analyzedTokens.Add(new Token(index.CStr().value, Token.Type.Indexer));
                    }
                    else
                    {
                        string value = "[";
                        depth = 1;

                        j = i + 1;

                        while (j < tokens.Count && depth > 0)
                        {
                            Token token = tokens[j];
                            value += token.value;
                            j++;

                            if (token.type == Token.Type.LBrack)
                                depth++;
                            else if (token.type == Token.Type.RBrack)
                                depth--;
                        }

                        j--;
                        analyzedTokens.Add(new(value, Token.Type.Iterator));
                    }

                    i = j;
                }
                else if (tokens[i].type == Token.Type.Dot)
                {
                    analyzedTokens.Add(new(tokens[i + 1].value, Token.Type.Property));
                    i++;
                    if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
                        analyzedTokens[^1].type = Token.Type.Method;
                }
                else if (tokens[i].type == Token.Type.Minus)
                {
                    if (Token.IsBasicOperator(analyzedTokens[^1].type) ||
                        analyzedTokens[^1].type == Token.Type.Return)
                        analyzedTokens.Add(new Token($"-{tokens[i + 1].value}", tokens[++i].type));
                    else analyzedTokens.Add(tokens[i]);
                }
                else if (tokens[i].type == Token.Type.LParen &&
                        (analyzedTokens[^1].type == Token.Type.Function ||
                         analyzedTokens[^1].type == Token.Type.Method))
                {
                    string value = "[";
                    int j = i + 1, depth = 1;

                    while (j < tokens.Count)
                    {
                        if (tokens[j].type == Token.Type.LParen)
                            depth++;
                        if (tokens[j].type == Token.Type.RParen)
                            depth--;
                        if (depth <= 0) break;
                        value += tokens[j++].value;
                    }

                    value += "]";

                    analyzedTokens.Add(new(value, Token.Type.Iterator));

                    i = j;
                }
                else analyzedTokens.Add(tokens[i]);

                if (analyzedTokens[^1].type == Token.Type.Variable && Process.IsClass(tokens[i]))
                    analyzedTokens[^1].type = Token.Type.Function;
            }

            return analyzedTokens;
        }
    }
}
