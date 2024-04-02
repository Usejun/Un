using Un.Function;
using Un.Object;

namespace Un
{
    public static class Tokenizer
    {
        public static Calculator Calculator = new();

        public static List<Token> Tokenization(string code)
        {
            List<Token> tokens = [];
            int index = 0;

            while (index < code.Length)
            {
                SkipWhitespace(code);
                if (index >= code.Length) break;
                else if (code[index] == '#') return [];
                else if (code[index] == '\"') tokens.Add(String(code));
                else if (char.IsLetter(code[index]) || code[index] == '_') tokens.Add(Keyword(code));
                else if (char.IsDigit(code[index])) tokens.Add(Number(code));
                else if (Process.IsOperator(code[index])) tokens.Add(Operator(code));
                else if (code[index] == ',' || code[index] == '.') tokens.Add(new(code[index++]));
                else throw new ScanException("scan Error");
            }

            return tokens;

            void SkipWhitespace(string code)
            {
                while (index < code.Length && char.IsWhiteSpace(code[index]))
                    index++;
            }

            Token String(string code)
            {
                string str = $"{code[index++]}";

                while (index < code.Length && code[index] != '\"')
                    str += code[index++];
                str += code[index++];

                return new(str, Token.Type.String);
            }

            Token Number(string code)
            {
                string str = "";

                while (index < code.Length && char.IsDigit(code[index]))
                    str += code[index++];

                if (index < code.Length && code[index] == '.')
                {
                    str += code[index++];
                    while (char.IsDigit(code[index]))
                        str += code[index++];

                    return new(str, Token.Type.Float);
                }

                return new(str, Token.Type.Integer);
            }

            Token Operator(string code)
            {
                bool TryOperator(char c1, char c2, out Token token)
                {
                    if (code[index] == c1)
                    {
                        if (code.Length > index + 1 && code[index + 1] == c2)
                        {
                            index += 2;
                            token = new($"{c1}{c2}");
                            return true;
                        }

                        index++;
                        token = new(c1);
                        return true;
                    }

                    token = new("None", Token.Type.None);
                    return false;
                }

                if (TryOperator('=', '=', out var t1))
                    return t1;
                else if (TryOperator('<', '=', out var t2))
                    return t2;
                else if (TryOperator('>', '=', out var t3))
                    return t3;
                else if (TryOperator('!', '=', out var t4))
                    return t4;
                else if (TryOperator('/', '/', out var t5))
                    return t5;

                return new(code[index++]);
            }

            Token Keyword(string code)
            {
                string str = "";

                while (index < code.Length && (char.IsLetter(code[index]) || code[index] == '_'))
                    str += code[index++];

                if (Process.IsFunc(str))
                    return new(str, Token.Type.Function);
                if (Token.GetType(str) != Token.Type.None)
                    return new(str);

                return new Token(str, Token.Type.Variable);
            }
        }

        public static List<Token> Analyzation(List<Token> tokens, Dictionary<string, Obj> variable, Dictionary<string, Fun> method)
        {
            List<Token> analyzedTokens = [];

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].tokenType == Token.Type.LBrack)
                {
                    int j = i + 1, depth = 1;
                    while (j < tokens.Count && depth > 0)
                    {
                        if (tokens[j].tokenType == Token.Type.LBrack)
                            depth++;
                        if (tokens[j].tokenType == Token.Type.RBrack)
                            depth--;
                        j++;
                    }

                    if (analyzedTokens.Count > 0 &&
                       (analyzedTokens[^1].tokenType == Token.Type.Variable ||
                        analyzedTokens[^1].tokenType == Token.Type.Iterator ||
                        analyzedTokens[^1].tokenType == Token.Type.String ||
                        analyzedTokens[^1].tokenType == Token.Type.Indexer ||
                        analyzedTokens[^1].tokenType == Token.Type.Pointer))
                    {
                        j--;
                        Obj index = Calculator.Calculate(Analyzation(tokens[(i + 1)..j], variable, method), variable, method);

                        analyzedTokens.Add(new Token(index.ToString(), Token.Type.Indexer));
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

                            if (token.tokenType == Token.Type.LBrack)
                                depth++;
                            else if (token.tokenType == Token.Type.RBrack)
                                depth--;
                        }

                        j--;
                        analyzedTokens.Add(new(value, Token.Type.Iterator));
                    }

                    i = j;
                }
                else if (tokens[i].tokenType == Token.Type.Dot)
                {
                    analyzedTokens.Add(new(tokens[i + 1].value, Token.Type.Pointer));
                    i++;
                }
                else analyzedTokens.Add(tokens[i]);
            }

            return analyzedTokens;
        }

        public static List<Token> All(string code, Dictionary<string, Obj> variable, Dictionary<string, Fun> method) => Analyzation(Tokenization(code), variable, method);

        public static bool IsBody(string code, int nesting)
        {
            bool a = true, b = true;

            for (int i = 0; a && i < nesting; i++)
                if (code[i] != '\t')
                    a = false;

            for (int i = 0; b && i < 4 * nesting; i++)
                if (code[i] != ' ')
                    b = false;

            return a || b;
        }
    }
}
