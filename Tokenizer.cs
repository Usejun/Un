using Un.Object;

namespace Un
{
    public class Tokenizer(string[] code)
    {
        Calculator calculator = new();

        string[] code = code;
        List<Token> tokens = [];

        int index = 0;
        int line = 0;
        int nesting = 0;

        public bool TryInterpret()
        {
            if (line >= code.Length) return false;
            if (code[line].Length == 1)
            {
                line++;
                return true;
            }

            Scan();

            Console.WriteLine(string.Join(" ", tokens.Select(i => i.tokenType)));
            Console.WriteLine(string.Join(" ", calculator.Postfix(tokens).Select(i => i.tokenType)));

            Parse();

            line++;

            return true;
        }

        void Scan()
        {
            tokens.Clear();
            index = 0;

            while (index < code[line].Length)
            {
                SkipWhitespace();
                if (index >= code[line].Length) break;
                else if (code[line][index] == '\"') tokens.Add(String());
                else if (char.IsLetter(code[line][index])) tokens.Add(Keyword());
                else if (char.IsDigit(code[line][index])) tokens.Add(Number());
                else if (Process.IsOperator(code[line][index])) tokens.Add(Operator());
                else if (code[line][index] == ',') tokens.Add(new(code[line][index++]));
                else throw new ScanException("scan Error");
            }
        }

        void Parse()
        {
            if (tokens.Count == 0) return;

            if (tokens[0].tokenType == Token.Type.Import)
            {

            }
            else if (tokens[0].tokenType == Token.Type.Func)
            {
                Process.Func.Add(tokens[1].value, line);
                Process.Variable.Add(tokens[3].value, Obj.None);
                nesting++;
                line++;

                while (line < code.Length && IsBody(nesting))
                    line++;

                line--;
                nesting--;
            }
            else if (tokens[0].tokenType == Token.Type.Variable && tokens.Count >= 3 && tokens[1].tokenType == Token.Type.Assign)
            {
                if (Process.IsVariable(tokens[0].value))
                    Process.Variable[tokens[0].value] = calculator.Calculate(tokens[2..]);
                else
                    Process.Variable.Add(tokens[0].value, calculator.Calculate(tokens[2..]));
            }
            else if (Process.IsFunction(tokens[0].value))
            {
                Process.Function[tokens[0].value](calculator.Calculate(tokens[1..]));
            }
            else if (Process.IsFunc(tokens[0].value))
            {
                int last = line;
                Obj parameter = calculator.Calculate(tokens[1..]);

                line = Process.Func[tokens[0].value];
                Scan();

                string arg = tokens[3].value;

                Process.Variable[arg] = parameter;

                nesting++;
                line++;

                while (line < code.Length && IsBody(nesting) && TryInterpret());

                nesting--;
                Process.Variable[arg] = Obj.None;
                line = last;                
            }
            else if (Process.IsControl(tokens[0].value))
            {
                if (tokens[0].tokenType == Token.Type.Else ||
                    calculator.Calculate(tokens[1..]) is Bool b && b.value)
                {
                    nesting++;
                    line++;

                    while (line < code.Length && IsBody(nesting) && TryInterpret());

                    nesting--;

                    SkipConditional();
                }
                else
                {
                    line++;

                    while (IsBody(nesting + 1))
                        line++;

                    line--;
                }
            }
            else if (Process.IsLoop(tokens[0].value))
            {
                int loop = line;

                if (tokens[0].tokenType == Token.Type.For)
                {
                    Iter iter = [];

                    if (tokens[2].tokenType != Token.Type.Iterator && calculator.Calculate(tokens[2..]) is Iter i1)
                        iter = i1;
                    else if (tokens[2].tokenType == Token.Type.Iterator && Obj.Convert(tokens[2].value) is Iter i2)
                        iter = i2;
                    else
                        throw new ObjException("Convert Error");

                    nesting++;
                    string var = tokens[1].value;
                    Process.Variable.Add(var, Obj.None);

                    foreach (var item in iter)
                    {
                        line = loop + 1;
                        Process.Variable[var] = item;

                        while (line < code.Length && IsBody(nesting) && TryInterpret()) ;
                    }

                    nesting--;
                }
                else if (tokens[0].tokenType == Token.Type.While)
                {
                    nesting++;

                    while (true)
                    {
                        line = loop;

                        Scan();

                        if (calculator.Calculate(tokens[1..]) is not Bool b || !b.value)
                            break;

                        line++;

                        while (line < code.Length && IsBody(nesting) && TryInterpret()) ;
                    }

                    nesting--;
                }
                else throw new ParseException("Parse Error");
            }
            else throw new ParseException("Parse Error");
        }

        void SkipWhitespace()
        {
            while (index < code[line].Length && char.IsWhiteSpace(code[line][index]))
                index++;
        }

        void SkipConditional()
        {
            while (line < code.Length)
            {
                index = 0;

                SkipWhitespace();
                if (Keyword().value == "endif")
                    break;

                line++;
            }
        }

        bool IsBody(int nesting)
        {
            bool a = true, b = true;

            for (int i = 0; a && i < nesting; i++)
                if (code[line][i] != '\t')
                    a = false;

            for (int i = 0; b && i < 4 * nesting; i++)
                if (code[line][i] != ' ')
                    b = false;

            return a || b;
        }

        Token String()
        {
            string str = $"{code[line][index++]}";

            while (index < code[line].Length && code[line][index] != '\"')
                str += code[line][index++];
            str += code[line][index++];

            return new(str, Token.Type.String);
        }

        Token Number()
        {
            string str = "";

            while (index < code[line].Length && char.IsDigit(code[line][index]))
                str += code[line][index++];

            if (index < code[line].Length && code[line][index] == '.')
            {
                str += code[line][index++];
                while (char.IsDigit(code[line][index]))
                    str += code[line][index++];

                return new(str, Token.Type.Float);
            }

            return new(str, Token.Type.Integer);
        }

        Token Iterator()
        {
            index++;
            string str = "[";
            int depth = 1;

            while (depth > 0)
            {
                SkipWhitespace();
                while (index < code[line].Length && code[line][index] != ',')
                {
                    if (code[line][index] == '[')
                    {
                        str += code[line][index++];
                        depth++;
                    }
                    else if (code[line][index] == ']')
                    {
                        str += code[line][index++];
                        depth--;
                        break;
                    }
                    else if (!char.IsWhiteSpace(code[line][index]))
                    {
                        str += code[line][index++];
                        if (code[line][index] != ']')
                        {
                            str += ",";
                            index++;
                        }
                    }
                    else break;
                }
                SkipWhitespace();
            }

            return new(str, Token.Type.Iterator);
        }

        Token Operator()
        {
            bool TryOperator(char c1, char c2, out Token token)
            {
                if (code[line][index] == c1)
                {
                    if (code[line].Length > index + 1 && code[line][index + 1] == c2)
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

            return new(code[line][index++]);
        }

        Token Keyword()
        {
            string str = "";

            while (index < code[line].Length && char.IsLetter(code[line][index]))
                str += code[line][index++];

            if (Process.IsFunction(str))
                return new(str, Token.Type.Function);
            if (Token.GetType(str) != Token.Type.None)
                return new(str);

            return new Token(str, Token.Type.Variable);
        }
    }

    public class TokenizerException : Exception
    {
        public TokenizerException() { }
        public TokenizerException(string message) : base(message) { }
        public TokenizerException(string message, Exception inner) : base(message, inner) { }
    }

    public class ScanException : Exception
    {
        public ScanException() { }
        public ScanException(string message) : base(message) { }
        public ScanException(string message, Exception inner) : base(message, inner) { }
    }

    public class ParseException : Exception
    {
        public ParseException() { }
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception inner) : base(message, inner) { }
    } 
}
