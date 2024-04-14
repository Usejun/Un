using Un.Function;

namespace Un.Supporter
{
    public static class Tokenizer
    {
        public static List<Token> Tokenize(string code)
        {
            List<Token> tokens = [];
            int index = 0;

            while (index < code.Length)
            {
                SkipWhitespace(code);
                if (index >= code.Length) break;
                else if (Token.IsComment(code[index])) return [];
                else if (code[index] == '\"') tokens.Add(String(code));
                else if (char.IsLetter(code[index]) || code[index] == '_') tokens.Add(Keyword(code));
                else if (char.IsDigit(code[index])) tokens.Add(Number(code));
                else if (Token.IsOperator(code[index])) tokens.Add(Operator(code));
                else if (code[index] == ',' || code[index] == '.') tokens.Add(new(code[index++]));
                else throw new SyntaxException("Invalid Syntax");
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
                {
                    if (code[index] == '\\')
                    {
                        str += code[index + 1] switch
                        {
                            'n' => "\n",
                            't' => "\t",
                            'r' => "\r",
                            '\\' => "\\",
                            _ => $"\\{code[index + 1]}"
                        };
                        index += 2;
                    }
                    else str += code[index++];
                }

                str += code[index++];

                return new(str, Token.Type.String);
            }

            Token Number(string code)
            {
                string str = $"{code[index++]}";

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
                        if (code.Length > index
                            + 1 && code[index + 1] == c2)
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

                bool TryOperators(char c1, char c2, char c3, out Token token)
                {
                    if (code[index] == c1)
                    {
                        if (code.Length > index + 1 && code[index + 1] == c2)
                        {
                            if (code.Length > index + 2 && code[index + 2] == c3)
                            {
                                index += 3;
                                token = new($"{c1}{c2}{c3}");
                                return true;
                            }
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
                else if (TryOperator('+', '=', out var t6))
                    return t6;
                else if (TryOperator('-', '=', out var t7))
                    return t7;
                else if (TryOperator('*', '=', out var t8))
                    return t8;
                else if (TryOperator('/', '=', out var t9))
                    return t9;
                else if (TryOperator('%', '=', out var t10))
                    return t10;
                else if (TryOperators('/', '/', '=', out var t11))
                    return t11;

                return new(code[index++]);
            }

            Token Keyword(string code)
            {
                string str = "";

                while (index < code.Length && (char.IsLetter(code[index]) || code[index] == '_' || char.IsDigit(code[index])))
                    str += code[index++];

                if (Process.TryGetProperty(str, out var property) && property is Fun)
                    return new(str, Token.Type.Function);
                if (Token.GetType(str) != Token.Type.None)
                    return new(str);

                return new Token(str, Token.Type.Variable);
            }
        }

        public static bool IsBody(string code, int nesting)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrEmpty(code)) return true;

            bool a = true, b = true;

            for (int i = 0; a && i < nesting; i++)
                if (code.Length > i && code[i] != '\t')
                    a = false;

            for (int i = 0; b && i < 4 * nesting; i++)
                if (code.Length > i && code[i] != ' ')
                    b = false;

            return a || b;
        }
    }
}
