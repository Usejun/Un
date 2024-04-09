using Un.Object;
using Un.Function;

namespace Un
{
    public class Interpreter(string[] code, 
                             Dictionary<string, Obj> properties, 
                             int index = 0, 
                             int line = 0, 
                             int nesting = 0)
    {
        public Obj ReturnValue = null;

        private int index = index;
        private int line = line;
        private int nesting = nesting;
        private readonly string[] code = code;
        private readonly Dictionary<string, Obj> properties = properties;
        private readonly Calculator calculator = new();

        public bool TryInterpret()
        {
            if (ReturnValue is not null || line >= code.Length) return false;

            if (string.IsNullOrWhiteSpace(code[line]))
            {
                line++;
                return true;
            }

            Parse(Tokenizer.Analyzation(Scan(code[line]), properties));
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
                else if (Calculator.IsOperator(code[index])) tokens.Add(Operator(code));
                else if (code[index] == ',' || code[index] == '.') tokens.Add(new(code[index++]));
                else throw new SyntaxException("Invalid Syntax");
            }

            return tokens;
        }

        void Parse(List<Token> analyzedTokens)
        {
            int assign = IndexOfAssign(analyzedTokens);       

            if (analyzedTokens.Count == 0 || analyzedTokens[0].tokenType == Token.Type.Comment) return;
            else if (assign >= 1)
            {
                Obj AssignCalculate(Token token, Obj a, Obj b) => token.tokenType switch
                {
                    Token.Type.PlusAssign => a.Add(b),
                    Token.Type.MinusAssign => a.Sub(b),
                    Token.Type.AsteriskAssign => a.Mul(b),
                    Token.Type.SlashAssign => a.Div(b),
                    Token.Type.DoubleSlashAssign => a.IDiv(b),
                    Token.Type.PercentAssign => a.Mod(b),
                    _ => b
                };

                Token token = analyzedTokens[0];
                Obj var = Obj.None;

                if (properties.TryGetValue(token.value, out var local))
                    var = local;
                else if (Process.Properties.TryGetValue(token.value, out var global))
                    var = global;
                else 
                    properties.Add(token.value, var);

                Obj value = calculator.Calculate(analyzedTokens[(assign + 1)..], properties);

                for (int i = 1; i < assign - 1; i++)
                {
                    if (analyzedTokens[i].tokenType == Token.Type.Indexer)
                        var = var.GetByIndex(Obj.Convert(analyzedTokens[i].value, properties));
                    else if (analyzedTokens[i].tokenType == Token.Type.Property)
                        var = var.Get(analyzedTokens[i].value);
                    else throw new InterpreterParseException();
                }

                if (assign == 1)
                    properties[token.value] = AssignCalculate(analyzedTokens[assign], var, value);
                else
                {
                    Token last = analyzedTokens[assign - 1];

                    if (last.tokenType == Token.Type.Indexer)
                    {
                        var.SetByIndex(new Iter([Obj.Convert(last.value, properties), AssignCalculate(analyzedTokens[assign], var, value)]));
                    }
                    else if (last.tokenType == Token.Type.Property)
                    {
                        if (var.HasProperty(last.value))
                            var.Set(last.value, AssignCalculate(analyzedTokens[assign], var.Get(last.value), value));
                        else throw new InterpreterParseException();
                    }
                    else throw new InterpreterParseException();
                }
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Import)
            {
                if (Obj.Convert(analyzedTokens[1].value, properties) is Iter packages)
                {
                    foreach (var package in packages)
                    {
                        if (package is not Str name) continue;

                        Process.Import(name.value);
                    }
                }
                else throw new ArgumentException("Invalid import statement.");
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Class)
            {
                Process.Class.TryAdd(analyzedTokens[1].value, new([..GetBody(includeHeader: true)], properties));            
                line--;
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Func)
            {
                Process.Properties.Add(analyzedTokens[1].value, new Fun(GetBody()));
                line--;
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Return)
            {
                ReturnValue = analyzedTokens[1..].Count == 0 ? Obj.None : calculator.Calculate(analyzedTokens[1..], properties);
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Variable)
            {
                int next = 1;
                Obj var = Obj.Convert(analyzedTokens[0].value, properties);

                while (analyzedTokens.Count > next)
                {
                    if (analyzedTokens[next].tokenType == Token.Type.Indexer)
                    {
                        Obj index = Obj.Convert(analyzedTokens[next].value, properties);

                        if (var is IIndexable indexable)
                            var = indexable.GetByIndex(index);
                        else throw new IndexerException("It is not indexable type");
                    }
                    else if (analyzedTokens[next].tokenType == Token.Type.Property ||
                             analyzedTokens[next].tokenType == Token.Type.Method)
                    {
                        if (var.Get(analyzedTokens[next].value) is Fun func)
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

                            var = func.Call(new Iter([var, Tokenizer.Calculator.Calculate(analyzedTokens[start..last], properties)]));
                            next = last;
                        }
                    }
                    else throw new InvalidOperationException("Invalid assign statement");
                    next++;
                }
            }
            else if (analyzedTokens[0].tokenType == Token.Type.Function)
            {
                if (Process.TryGetProperty(analyzedTokens[0].value, out var value) && value is Fun fun)
                    fun.Call(calculator.Calculate(analyzedTokens[1..], properties));
            }
            else if (Process.IsClass(analyzedTokens[0].value))
            {       
                
            }
            else if (Process.IsControl(analyzedTokens[0].value))
            {
                if (analyzedTokens[0].tokenType == Token.Type.Else ||
                    calculator.Calculate(analyzedTokens[1..], properties) is Bool b && b.value)
                {
                    nesting++;
                    line++;

                    while (line < code.Length && IsBody() && TryInterpret());

                    nesting--;

                    if (line < code.Length)
                        SkipConditional();

                    line--;
                }
                else
                {
                    nesting++;
                    line++;

                    SkipBody();

                    line--;
                    nesting--;
                }
            }
            else if (Process.IsLoop(analyzedTokens[0].value))
            {
                int loop = line;

                if (analyzedTokens[0].tokenType == Token.Type.For)
                {
                    Obj obj = calculator.Calculate(analyzedTokens[3..], properties), prev = Obj.None;

                    if (obj is not Iter iter) throw new ("Arg Error");

                    string var = analyzedTokens[1].value;

                    if (properties.TryGetValue(var, out prev))
                        properties[var] = Obj.None;
                    else
                        properties.Add(var, Obj.None);

                    nesting++;

                    if (iter.Count != 0)
                    {
                        foreach (var item in iter)
                        {
                            line = loop + 1;
                            properties[var] = item;

                            while (line < code.Length && IsBody() && TryInterpret()) ;
                        }
                    }
                    else line = loop + 1;
                    
                    SkipBody();

                    if (prev == Obj.None)
                        properties.Remove(var);
                    else
                        properties[var] = prev;

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

                        if (ReturnValue is not null) break;
                        if (calculator.Calculate(analyzedTokens[1..], properties) is not Bool b || !b.value)
                            break;
                        line++;

                        while (line < code.Length && IsBody() && TryInterpret());                        
                    }

                    line++;
                    SkipBody();
                    line--;
                    nesting--;
                }
                else throw new SyntaxException("It's not a loop syntax");
            }
            else throw new InterpreterParseException();
        }

        string[] GetBody(bool includeHeader = false)
        {
            List<string> buffer = [];

            if (includeHeader)
                buffer.Add(code[line]);

            nesting++;
            line++;

            while (code.Length > line && IsBody())
                buffer.Add(code[line++]);

            nesting--;
            return [..buffer];
        }

        int IndexOfAssign(List<Token> analyzedTokens)
        {
            int index = -1;

            for (int i = 0; index == -1 && i < analyzedTokens.Count; i++)
                if (analyzedTokens[i].tokenType >= Token.Type.Assign && analyzedTokens[i].tokenType <= Token.Type.PercentAssign)
                    index = i;

            return index;
        }

        bool IsBody()
        {
            if (string.IsNullOrWhiteSpace(code[line]) || string.IsNullOrEmpty(code[line])) return true;

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

            while (index < code.Length && (char.IsLetter(code[index]) || code[index] == '_'))
                str += code[index++];

            if (Process.TryGetProperty(str, out var property) && property is Fun)
                return new(str, Token.Type.Function);
            if (Token.GetType(str) != Token.Type.None)
                return new(str);         

            return new Token(str, Token.Type.Variable);
        }        
    }   
}
