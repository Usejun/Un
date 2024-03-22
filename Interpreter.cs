using Un.Object;

namespace Un
{
    public class Interpreter(int index = 0, int line = 0, int nesting = 0)
    {
        public Obj ReturnValue = Obj.None;

        int index = index;
        int line = line;
        int nesting = nesting;

        public bool TryInterpret()
        {
            if (ReturnValue != Obj.None) return false;
            if (line >= Process.Code.Length) return false;
            if (Process.Code[line].Length < 1)
            {
                line++;
                return true;
            }

            Parse(Analyze(Scan(Process.Code[line])));
            line++;
            return true;
        }

        public List<Token> Scan(string code)
        {
            List<Token> tokens = [];
            index = 0;

            while (index < code.Length)
            {
                SkipWhitespace(code);
                if (index >= code.Length) break;
                else if (code[index] == '#') return [];
                else if (code[index] == '\"') tokens.Add(String(code));
                else if (char.IsLetter(code[index]) || code[index] == '_') tokens.Add(Keyword(code));
                else if (char.IsDigit(code[index])) tokens.Add(Number(code));
                else if (Process.IsOperator(code[index])) tokens.Add(Operator(code));
                else if (code[index] == ',') tokens.Add(new(code[index++]));
                else throw new ScanException("scan Error");
            }

            return tokens;
        }

        public List<Token> Analyze(List<Token> tokens)
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
                        analyzedTokens[^1].tokenType == Token.Type.Indexer))
                    {
                        j--;
                        Obj index = Calculator.Calculate(tokens[(i + 1)..j]);

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
                else analyzedTokens.Add(tokens[i]);
            }

            return analyzedTokens;
        }

        public void Parse(List<Token> analyzedTokens)
        {
            List<Token.Type> tokenTypes = analyzedTokens.Select(i => i.tokenType).ToList();

            int assign = tokenTypes.IndexOf(Token.Type.Assign);

            if (analyzedTokens.Count == 0 || analyzedTokens[0].tokenType == Token.Type.Comment) return;
            else if (assign >= 1)
            {
                Token token = analyzedTokens[0];

                if (!Process.IsVariable(token.value))
                {
                    if (assign == 1) Process.Variable.Add(token.value, Obj.None);
                    else throw new ObjException("Parse Error");
                }

                Obj var = Process.Variable[token.value];
                Obj value = Calculator.Calculate(analyzedTokens[(assign + 1)..]);

                for (int i = 1; i < assign - 1; i++)
                {
                    if (var is Iter iter1 && Obj.Convert(analyzedTokens[i].value) is Int index1)
                        var = iter1[index1];
                    else throw new ObjException("Parse Error");
                }

                if (assign == 1)
                    Process.Variable[token.value] = value;
                if (var is Iter iter2 && Obj.Convert(analyzedTokens[assign - 1].value) is Int index2)
                    iter2[index2] = value;
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Import)
            {

            }
            else if (analyzedTokens[0].tokenType == Token.Type.Func)
            {
                Process.Func.Add(analyzedTokens[1].value, line);
                nesting++;
                line++;

                while (line < Process.Code.Length && IsBody(Process.Code[line]))
                    line++;

                line--;
                nesting--;
            }            
            else if (analyzedTokens[0].tokenType == Token.Type.Return)
            {
                ReturnValue = Calculator.Calculate(analyzedTokens[1..]);
            }
            else if (Process.IsFunc(analyzedTokens[0].value))
            {
                int last = line;
                Obj parameter = Calculator.Calculate(analyzedTokens[1..]);

                line = Process.Func[analyzedTokens[0].value];
                Scan(Process.Code[line]);

                string arg = analyzedTokens[3].value;

                Process.Variable[arg] = parameter;

                nesting++;
                line++;

                while (line < Process.Code.Length && IsBody(Process.Code[line]) && TryInterpret()) ;

                nesting--;
                Process.Variable[arg] = Obj.None;
                line = last;
            }
            else if (Process.IsFunction(analyzedTokens[0].value))
            {
                Process.Function[analyzedTokens[0].value](Calculator.Calculate(analyzedTokens[1..]));
            }            
            else if (Process.IsControl(analyzedTokens[0].value))
            {
                if (analyzedTokens[0].tokenType == Token.Type.Else ||
                    Calculator.Calculate(analyzedTokens[1..]) is Bool b && b.value)
                {
                    nesting++;
                    line++;

                    while (line < Process.Code.Length && IsBody(Process.Code[line]) && TryInterpret()) ;

                    nesting--;

                    if (line < Process.Code.Length)
                        SkipConditional(Process.Code[line]);
                }
                else
                {
                    line++;
                    nesting++;

                    while (line < Process.Code.Length && IsBody(Process.Code[line]))
                        line++;

                    nesting--;
                    line--;
                }
            }
            else if (Process.IsLoop(analyzedTokens[0].value))
            {
                int loop = line;

                if (analyzedTokens[0].tokenType == Token.Type.For)
                {
                    Obj obj = Calculator.Calculate(analyzedTokens[3..]);

                    if (obj is not Iter iter) throw new ObjException("Arg Error");

                    nesting++;
                    string var = analyzedTokens[1].value;
                    Process.Variable.Add(var, Obj.None);

                    foreach (var item in iter)
                    {
                        line = loop + 1;
                        Process.Variable[var] = item;

                        while (line < Process.Code.Length && IsBody(Process.Code[line]) && TryInterpret()) ;
                    }

                    Process.Variable.Remove(var);
                    line--;
                    nesting--;
                }
                else if (analyzedTokens[0].tokenType == Token.Type.While)
                {
                    nesting++;

                    while (true)
                    {
                        line = loop;

                        Scan(Process.Code[line]);

                        if (Calculator.Calculate(analyzedTokens[1..]) is not Bool b || !b.value)
                            break;

                        line++;

                        while (line < Process.Code.Length && IsBody(Process.Code[line]) && TryInterpret()) ;
                    }

                    nesting--;
                }
                else throw new ParseException("Parse Error");
            }
            else throw new ParseException("Parse Error");
        }

        public bool IsBody(string code)
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

        public bool IsBody(string code, int nesting)
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

        void SkipWhitespace(string code)
        {
            while (index < code.Length && char.IsWhiteSpace(code[index]))
                index++;
        }

        void SkipConditional(string code)
        {
            while (line < code.Length)
            {
                index = 0;

                SkipWhitespace(code);
                if (Keyword(code).value == "endif")
                    break;

                line++;
            }
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
