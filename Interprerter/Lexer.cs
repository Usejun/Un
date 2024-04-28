using Un.Data;


namespace Un
{
    public static class Lexer
    {
        public static List<Token> Lex(List<Token> tokens, Dictionary<string, Obj> properties)
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
                        analyzedTokens[^1].type == Token.Type.Iterator ||
                        analyzedTokens[^1].type == Token.Type.String ||
                        analyzedTokens[^1].type == Token.Type.Indexer ||
                        analyzedTokens[^1].type == Token.Type.Property))
                    {
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

                        while (j < tokens.Count)
                        {
                            if (tokens[j].type == Token.Type.LBrack)
                                depth++;
                            if (tokens[j].type == Token.Type.RBrack)
                                depth--;
                            if (depth <= 0) break;
                            value += Token.IsOperator(tokens[j]) ? $" {tokens[j++].value} " : tokens[j++].value;
                        }

                        value += "]";

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
                else if (tokens[i].type == Token.Type.LParen &&
                         analyzedTokens.Count > 0 &&
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
                        value += Token.IsOperator(tokens[j]) ? $" {tokens[j++].value} " : tokens[j++].value;
                    }

                    value += "]";

                    analyzedTokens.Add(new(value, Token.Type.Iterator));

                    i = j;
                }
                else analyzedTokens.Add(tokens[i]);

                if (analyzedTokens[^1].type == Token.Type.Variable && Process.IsClass(tokens[i]))
                    analyzedTokens[^1].type = Token.Type.Function;
                if (analyzedTokens[^1].type == Token.Type.Variable && properties.TryGetValue(tokens[i].value, out var f) && f is Fun)
                    analyzedTokens[^1].type = Token.Type.Function;
            }

            return analyzedTokens;
        }
    }
}
