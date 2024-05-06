using Un.Data;

namespace Un
{
    public static class Tokenizer
    {
        public static List<string> oper = 
        [
            "==", "<=", ">=", "!=", "+=", "-=", "*=", "/=", "%=", "//", "//=", "=>",
            "<<", ">>", "<<=", ">>=", "**", "**=", "&=", "|=", "^=",
        ];

        public static List<Token> Tokenize(string code)
        {
            List<Token> tokens = [];
            int index = 0;

            while (index < code.Length)
            {
                SkipWhitespace(code);
                if (index >= code.Length) break;
                else if (Token.IsComment(code[index])) return [];
                else if (code[index] == '\"' || code[index] == '\'') tokens.Add(String(code));
                else if (char.IsLetter(code[index]) || code[index] == '_') tokens.Add(Keyword(code));
                else if (char.IsDigit(code[index])) tokens.Add(Number(code));
                else if (Token.IsOperator(code[index])) tokens.Add(Operator(code));
                else if (code[index] == ',' || code[index] == '.') tokens.Add(new(code[index++]));
                else throw new SyntaxException("Invalid Syntax");
            }

            //Console.WriteLine();
            //Console.WriteLine(string.Join("\n", tokens));
            //Console.WriteLine();

            return tokens;

            void SkipWhitespace(string code)
            {
                while (index < code.Length && char.IsWhiteSpace(code[index]))
                    index++;
            }

            Token String(string code)
            {
                string str = $"{code[index++]}";

                while (index < code.Length)
                {
                    if (code[index] == '\\')
                    {
                        index++;
                        str += $"\\{code[index]}";
                        index++;
                    }
                    else str += code[index++];

                    if (str[^1] == str[0]) break;
                }

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
                    while (index < code.Length && char.IsDigit(code[index]))
                        str += code[index++];

                    return new(str, Token.Type.Float);
                }

                return new(str, Token.Type.Integer);
            }

            Token Operator(string code)
            {                
                bool TryOperator(string type, out Token token)
                {
                    token = Token.None;

                    for (int i = 0; i < type.Length; i++)
                        if (code[index + i] != type[i])
                            return false;

                    index += type.Length;
                    token = new Token(type);
                    return true;
                }

                foreach (var o in oper)
                    if (TryOperator(o, out var t))
                        return t;

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
