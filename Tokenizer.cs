using Un.Function;
using Un.Object;

namespace Un
{
    public static class Tokenizer
    {
        public readonly static Calculator Calculator = new();

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
                else if (Calculator.IsOperator(code[index])) tokens.Add(Operator(code));
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

        public static List<Token> Analyzation(List<Token> tokens, Dictionary<string, Obj> properties)
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
                        analyzedTokens[^1].tokenType == Token.Type.Property))
                    {
                        j--;
                        Obj index = Calculator.Calculate(Analyzation(tokens[(i + 1)..j], properties), properties);

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
                    analyzedTokens.Add(new(tokens[i + 1].value, Token.Type.Property));
                    i++;
                    if (tokens.Count > i + 1 && tokens[i + 1].tokenType == Token.Type.LParen)
                        analyzedTokens[^1].tokenType = Token.Type.Method;
                }
                else if (tokens[i].tokenType == Token.Type.Minus)
                {
                    if (Calculator.IsBasicOperator(analyzedTokens[^1].tokenType) ||
                        analyzedTokens[^1].tokenType == Token.Type.Return)
                        analyzedTokens.Add(new Token($"-{tokens[i + 1].value}", tokens[++i].tokenType));
                    else analyzedTokens.Add(tokens[i]);
                }
                else if (tokens[i].tokenType == Token.Type.LParen &&
                        (analyzedTokens[^1].tokenType == Token.Type.Function ||
                         analyzedTokens[^1].tokenType == Token.Type.Method))
                {
                    string value = "[";
                    int j = i + 1, depth = 1;

                    while (j < tokens.Count)
                    {
                        if (tokens[j].tokenType == Token.Type.LParen)
                            depth++;
                        if (tokens[j].tokenType == Token.Type.RParen)
                            depth--;
                        if (depth <= 0) break;
                        value += tokens[j++].value;
                    }

                    value += "]";

                    analyzedTokens.Add(new(value, Token.Type.Iterator));

                    i = j;
                }
                else analyzedTokens.Add(tokens[i]);

                if (analyzedTokens[^1].tokenType == Token.Type.Variable && Process.IsClass(tokens[i]))
                    analyzedTokens[^1].tokenType = Token.Type.Function;
            }

            //Console.WriteLine();
            //Console.WriteLine(string.Join("\n", analyzedTokens));
            //Console.WriteLine();

            return analyzedTokens;
        }

        public static List<Token> All(string code, Dictionary<string, Obj> properties) => Analyzation(Tokenization(code), properties);

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
