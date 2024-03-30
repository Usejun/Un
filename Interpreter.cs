using Un.Object;
using Un.Class;
using Un.Function;

namespace Un
{
    public class Interpreter(string[] code, Dictionary<string, Obj>? variable, int index = 0, int line = 0, int nesting = 0)
    {
        public Obj ReturnValue = Obj.None;

        public int index = index;
        public int line = line;
        public int nesting = nesting;
        public string[] code = code;
        public Dictionary<string, Obj> variable = variable; 
        public Calculator calculator = new();

        public bool TryInterpret()
        {
            if (ReturnValue != Obj.None) return false;
            if (line >= code.Length) return false;
            if (string.IsNullOrWhiteSpace(code[line]))
            {
                line++;
                return true;
            }

            Parse(Tokenizer.Analyzation(Scan(code[line]), variable));
            line++;
            return true;
        }

        List<Token> Scan(string code)
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
                else if (code[index] == ',' || code[index] == '.') tokens.Add(new(code[index++]));
                else throw new ScanException("scan Error");
            }

            return tokens;
        }

        void Parse(List<Token> analyzedTokens)
        {
            int assign = -1;

            for (int i = 0; assign == -1 && i < analyzedTokens.Count; i++)
                if (analyzedTokens[i].tokenType == Token.Type.Assign)
                    assign = i;            

            if (analyzedTokens.Count == 0 || analyzedTokens[0].tokenType == Token.Type.Comment) return;
            else if (assign >= 1)
            {
                Token token = analyzedTokens[0];

                if (!variable.ContainsKey(token.value))
                {
                    if (assign == 1) variable.Add(token.value, Obj.None);
                    else throw new ObjException("Parse Error");
                }

                Obj var = variable[token.value];
                Obj value = calculator.Calculate(analyzedTokens[(assign + 1)..], variable);

                for (int i = 1; i < assign - 1; i++)
                {
                    if (analyzedTokens[i].tokenType == Token.Type.Indexer)
                    {
                        Obj index = Obj.Convert(analyzedTokens[i].value, variable);
                        
                        if (var is Iter iter && index is Int iIndex)                        
                            var = iter[iIndex];
                        else throw new ObjException("Parse Error");
                    }
                    else if (analyzedTokens[i].tokenType == Token.Type.Pointer)
                    {
                        if (var is Cla cla)
                        {
                            var = cla.Get(analyzedTokens[i].value);
                        }
                        else throw new ObjException("Parse Error");
                    } 
                    else throw new ObjException("Parse Error");
                }

                if (assign == 1)
                    variable[token.value] = value;
                else
                {
                    Token last = analyzedTokens[assign - 1];

                    if (last.tokenType == Token.Type.Indexer)
                    {
                        if (var is Iter iter && Obj.Convert(last.value, variable) is Int iIndex)
                            iter[iIndex] = value;
                        else throw new ObjException("Parse Error");
                    }
                    else if (last.tokenType == Token.Type.Pointer)
                    {
                        if (var is Cla cla)
                            cla.Get(last.value).Ass(value, variable);
                        else throw new ObjException("Parse Error");
                    }
                    else throw new ObjException("Parse Error");
                }
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Import)
            {

            }
            else if (analyzedTokens[0].tokenType == Token.Type.Class)
            {
                int start = line;

                nesting++;
                line++;

                while (line < code.Length && IsBody())
                {
                    line++;
                    while (line < code.Length && string.IsNullOrWhiteSpace(code[line]))
                        line++;
                }

                Process.Class.Add(analyzedTokens[1].value, new(code[start..line], variable));
                
                line--;
                nesting--;
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Func)
            {
                int start = line;

                nesting++;
                line++;

                while (line < code.Length && IsBody())
                    line++;

                Process.Func.Add(analyzedTokens[1].value, new(code[start..line]));

                line--;
                nesting--;

            }
            else if (analyzedTokens[0].tokenType == Token.Type.Return)
            {
                ReturnValue = calculator.Calculate(analyzedTokens[1..], variable);
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Variable)
            {
                int next = 1;
                Obj var = Obj.Convert(analyzedTokens[0].value, variable);

                while (analyzedTokens.Count > next)
                {
                    if (analyzedTokens[next].tokenType == Token.Type.Indexer)
                    {
                        Obj index = Obj.Convert(analyzedTokens[next].value, variable);

                        if (var is Iter iter && index is Int iIndex)
                            var = iter[iIndex];
                        else throw new ObjException("Parse Error");
                    }
                    else if (analyzedTokens[next].tokenType == Token.Type.Pointer)
                    {
                        if (var is Cla cla)
                            var = cla.Get(analyzedTokens[next].value);
                        else throw new ObjException("Parse Error");

                        if (var is Fun func)
                        {
                            int start = next + 1, last = next + 2, depth = 1;

                            while (depth > 0)
                            {
                                if (analyzedTokens[last].tokenType == Token.Type.LParen)
                                    depth++;
                                else if (analyzedTokens[last].tokenType == Token.Type.RParen)
                                    depth--;
                                last++;
                            }

                            var = func.Call(new Iter([cla, Tokenizer.Calculator.Calculate(analyzedTokens[start..last], variable)]));
                            next = last;
                        }
                    }
                    else throw new ObjException("Parse Error");
                    next++;
                }
            }
            else if (Process.IsClass(analyzedTokens[0].value))
            {       
                
            }
            else if (Process.IsFunc(analyzedTokens[0].value))
            {
                Process.GetFunc(analyzedTokens[0].value).Call(calculator.Calculate(analyzedTokens[1..], variable));
            }
            else if (Process.IsControl(analyzedTokens[0].value))
            {
                if (analyzedTokens[0].tokenType == Token.Type.Else ||
                    calculator.Calculate(analyzedTokens[1..], variable) is Bool b && b.value)
                {
                    nesting++;
                    line++;

                    while (line < code.Length && IsBody() && TryInterpret());

                    if (line < code.Length && IsBody())
                        SkipConditional();

                    line--;
                    nesting--;
                }
                else
                {
                    line++;
                    nesting++;

                    while (line < code.Length && IsBody())
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
                    Obj obj = calculator.Calculate(analyzedTokens[3..], variable);

                    if (obj is not Iter iter) throw new ObjException("Arg Error");

                    nesting++;
                    string var = analyzedTokens[1].value;
                    variable.Add(var, Obj.None);

                    foreach (var item in iter)
                    {
                        line = loop + 1;
                        variable[var] = item;

                        while (line < code.Length && IsBody() && TryInterpret()) ;
                    }

                    variable.Remove(var);
                    line--;
                    nesting--;
                }
                else if (analyzedTokens[0].tokenType == Token.Type.While)
                {
                    nesting++;

                    while (true)
                    {
                        line = loop;

                        Scan(code[line]);

                        if (calculator.Calculate(analyzedTokens[1..], variable) is not Bool b || !b.value)
                            break;
                        line++;

                        while (line < code.Length && IsBody() && TryInterpret()) ;
                    }

                    line++;
                    while (line < code.Length && IsBody())
                        line++;
                    line--;
                    nesting--;
                }
                else throw new ParseException("Parse Error");
            }
            else throw new ParseException("Parse Error");
        }

        bool IsBody()
        {
            bool a = true, b = true;

            for (int i = 0; a && i < nesting; i++)
                if (code[line].Length > i && code[line][i] != '\t')
                    a = false;

            for (int i = 0; b && i < 4 * nesting; i++)
                if (code[line].Length > i && code[line][i] != ' ')
                    b = false;

            return a || b;
        }

        void SkipWhitespace(string code)
        {
            while (index < code.Length && char.IsWhiteSpace(code[index]))
                index++;
        }

        void SkipConditional()
        {
            while (line < code.Length)
            {
                index = 0;

                SkipWhitespace(code[line]);

                List<Token> tokens = Scan(code[line]);

                if (tokens.Count > 0 && !Process.IsControl(tokens[0]))
                    break;

                nesting++;
                line++;

                while (line < code.Length && IsBody())
                    line++;
                nesting--;
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

            if (Process.IsFunc(str))
                return new(str, Token.Type.Function);
            if (Token.GetType(str) != Token.Type.None)
                return new(str);

            return new Token(str, Token.Type.Variable);
        }        
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
