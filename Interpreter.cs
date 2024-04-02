using Un.Object;
using Un.Class;
using Un.Function;

namespace Un
{
    public class Interpreter(string[] code, 
                             Dictionary<string, Obj> variable, 
                             Dictionary<string, Fun> method, 
                             string className = "",
                             int index = 0, 
                             int line = 0, 
                             int nesting = 0)
    {
        public Obj ReturnValue = Obj.None;

        public int index = index;
        public int line = line;
        public int nesting = nesting;
        public string[] code = code;
        public string className = className;
        public Dictionary<string, Obj> variable = variable;
        public Dictionary<string, Fun> method = method;
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

            Parse(Tokenizer.Analyzation(Scan(code[line]), variable, method));
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
                else if (char.IsDigit(code[index]) || (code[index] == '-' && char.IsDigit(code[index + 1]))) tokens.Add(Number(code));
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
                if (analyzedTokens[i].tokenType >= Token.Type.Assign && analyzedTokens[i].tokenType <= Token.Type.PercentAssign)
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
                Obj value = calculator.Calculate(analyzedTokens[(assign + 1)..], variable, method, className);

                for (int i = 1; i < assign - 1; i++)
                {
                    if (analyzedTokens[i].tokenType == Token.Type.Indexer)
                    {
                        Obj index = Obj.Convert(analyzedTokens[i].value, variable, method);
                        
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
                    variable[token.value] = analyzedTokens[assign].tokenType switch
                    {
                        Token.Type.PlusAssign => var.Add(value),
                        Token.Type.MinusAssign => var.Sub(value),
                        Token.Type.AsteriskAssign => var.Mul(value),
                        Token.Type.SlashAssign => var.Div(value),
                        Token.Type.DoubleSlashAssign => var.IDiv(value),
                        Token.Type.PercentAssign => var.Mod(value),
                        _ => value
                    };
                else
                {
                    Token last = analyzedTokens[assign - 1];

                    if (last.tokenType == Token.Type.Indexer)
                    {
                        if (var is Iter iter && Obj.Convert(last.value, variable, method) is Int iIndex)
                        {
                            iter[iIndex] = analyzedTokens[assign].tokenType switch
                            {
                                Token.Type.PlusAssign => var.Add(value),
                                Token.Type.MinusAssign => var.Sub(value),
                                Token.Type.AsteriskAssign => var.Mul(value),
                                Token.Type.SlashAssign => var.Div(value),
                                Token.Type.DoubleSlashAssign => var.IDiv(value),
                                Token.Type.PercentAssign => var.Mod(value),
                                _ => value
                            };

                        }
                        else throw new ObjException("Parse Error");
                    }
                    else if (last.tokenType == Token.Type.Pointer)
                    {
                        if (var is Cla cla)
                        {
                            Obj obj = analyzedTokens[assign].tokenType switch
                            {
                                Token.Type.PlusAssign => cla.Get(last.value).Add(value),
                                Token.Type.MinusAssign => cla.Get(last.value).Sub(value),
                                Token.Type.AsteriskAssign => cla.Get(last.value).Mul(value),
                                Token.Type.SlashAssign => cla.Get(last.value).Div(value),
                                Token.Type.DoubleSlashAssign => cla.Get(last.value).IDiv(value),
                                Token.Type.PercentAssign => cla.Get(last.value).Mod(value),
                                _ => value
                            };

                            cla.Get(last.value).Ass(obj, variable, method);
                        }
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

                Process.Class.Add(analyzedTokens[1].value, new(code[start..line], variable, method));
                
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
                ReturnValue = calculator.Calculate(analyzedTokens[1..], variable, method, className);
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Variable)
            {
                int next = 1;
                Obj var = Obj.Convert(analyzedTokens[0].value, variable, method);

                while (analyzedTokens.Count > next)
                {
                    if (analyzedTokens[next].tokenType == Token.Type.Indexer)
                    {
                        Obj index = Obj.Convert(analyzedTokens[next].value, variable, method);

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

                            var = func.Call(new Iter([cla, Tokenizer.Calculator.Calculate(analyzedTokens[start..last], variable, method, className)]));
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
                Process.GetFunc(analyzedTokens[0].value).Call(calculator.Calculate(analyzedTokens[1..], variable, method, className));
            }
            else if (Process.IsControl(analyzedTokens[0].value))
            {
                if (analyzedTokens[0].tokenType == Token.Type.Else ||
                    calculator.Calculate(analyzedTokens[1..], variable, method, className) is Bool b && b.value)
                {
                    nesting++;
                    line++;

                    while (line < code.Length && IsBody() && TryInterpret());

                    nesting--;

                    if (line < code.Length)
                        SkipConditional();
                }
                else
                {
                    line++;
                    nesting++;

                    SkipBody();

                    nesting--;
                    line--;
                }
            }
            else if (Process.IsLoop(analyzedTokens[0].value))
            {
                int loop = line;

                if (analyzedTokens[0].tokenType == Token.Type.For)
                {
                    Obj obj = calculator.Calculate(analyzedTokens[3..], variable, method, className), prev = Obj.None;

                    if (obj is not Iter iter) throw new ObjException("Arg Error");

                    nesting++;
                    string var = analyzedTokens[1].value;

                    if (variable.TryGetValue(var, out prev))
                        variable[var] = Obj.None;
                    else
                        variable.Add(var, Obj.None);

                    if (iter.Count != 0)
                    {
                        foreach (var item in iter)
                        {
                            line = loop + 1;
                            variable[var] = item;

                            while (line < code.Length && IsBody() && TryInterpret()) ;
                        }
                    }
                    
                    SkipBody();

                    if (prev == Obj.None)
                        variable.Remove(var);
                    else
                        variable[var] = prev;

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

                        if (calculator.Calculate(analyzedTokens[1..], variable, method, className) is not Bool b || !b.value)
                            break;
                        line++;

                        while (line < code.Length && IsBody() && TryInterpret()) ;
                    }

                    line++;
                    SkipBody();
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

        void SkipBody()
        {
            while (line < code.Length && IsBody())
                line++;
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
