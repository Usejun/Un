using Un.Object;
using Un.Function;

namespace Un.Supporter
{
    public class Parser(string[] code,
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

        public bool TryInterpret()
        {
            if (ReturnValue is not null || line >= code.Length) return false;

            if (string.IsNullOrWhiteSpace(code[line]))
            {
                line++;
                return true;
            }

            Parse(All(code[line], properties));
            line++;
            return true;
        }

        void Parse(List<Token> analyzedTokens)
        {
            int assign = IndexOfAssign(analyzedTokens);

            if (analyzedTokens.Count == 0 || Token.IsComment(analyzedTokens[0].type)) return;
            else if (assign >= 1)
            {
                Obj AssignCalculate(Token token, Obj a, Obj b) => token.type switch
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

                Obj value = Calculator.Calculate(analyzedTokens[(assign + 1)..], properties);

                for (int i = 1; i < assign - 1; i++)
                {
                    if (analyzedTokens[i].type == Token.Type.Indexer)
                        var = var.GetByIndex(new Iter([Obj.Convert(analyzedTokens[i].value, properties)]));
                    else if (analyzedTokens[i].type == Token.Type.Property)
                        var = var.Get(analyzedTokens[i].value);
                    else throw new InterpreterParseException();
                }

                if (assign == 1)
                    properties[token.value] = AssignCalculate(analyzedTokens[assign], var, value);
                else
                {
                    Token last = analyzedTokens[assign - 1];

                    if (last.type == Token.Type.Indexer)
                    {
                        var.SetByIndex(new Iter([Obj.Convert(last.value, properties), AssignCalculate(analyzedTokens[assign], var, value)]));
                    }
                    else if (last.type == Token.Type.Property)
                    {
                        if (var.HasProperty(last.value))
                            var.Set(last.value, AssignCalculate(analyzedTokens[assign], var.Get(last.value), value));
                        else throw new InterpreterParseException();
                    }
                    else throw new InterpreterParseException();
                }
            }
            else if (analyzedTokens[0].type == Token.Type.Import)
            {
                if (Obj.Convert(analyzedTokens[1].value, properties) is not Iter packages)
                    throw new ArgumentException("Invalid import statement.");

                foreach (var package in packages)
                {
                    if (package is not Str name)
                        throw new ImportException("Package Name is only string type");

                    Process.Import(name.value);
                }
            }
            else if (analyzedTokens[0].type == Token.Type.Class)
            {
                Process.Class.Add(analyzedTokens[1].value, new([.. GetBody(includeHeader: true)], properties));
                line--;
            }
            else if (analyzedTokens[0].type == Token.Type.Func)
            {
                Process.Properties.Add(analyzedTokens[1].value, new Fun(GetBody(includeHeader: true)));
                line--;
            }
            else if (analyzedTokens[0].type == Token.Type.Return)
            {
                ReturnValue = analyzedTokens[1..].Count == 0 ? Obj.None : Calculator.Calculate(analyzedTokens[1..], properties);
            }
            else if (analyzedTokens[0].type == Token.Type.Variable)
            {
                int next = 1;
                Obj var = Obj.Convert(analyzedTokens[0].value, properties);

                while (analyzedTokens.Count > next)
                {
                    if (analyzedTokens[next].type == Token.Type.Indexer)
                    {
                        Obj index = Obj.Convert(analyzedTokens[next].value, properties);
                        var = var.GetByIndex(new Iter([index]));
                    }
                    else if (analyzedTokens[next].type == Token.Type.Property ||
                             analyzedTokens[next].type == Token.Type.Method)
                    {
                        if (var.Get(analyzedTokens[next].value) is Fun func)
                        {
                            var = func.Clone().Call(Obj.Convert(analyzedTokens[next + 1].value, properties).CIter().Insert(var, 0, false));
                        }
                    }
                    next++;
                }
            }
            else if (analyzedTokens[0].type == Token.Type.Function)
            {
                Calculator.Calculate(analyzedTokens, properties);
            }
            else if (Token.IsControl(analyzedTokens[0].value))
            {
                if (analyzedTokens[0].type == Token.Type.Else ||
                    Calculator.Calculate(analyzedTokens[1..], properties) is Bool b && b.value)
                {
                    nesting++;
                    line++;

                    while (line < code.Length && IsBody() && TryInterpret()) ;

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
            else if (Token.IsLoop(analyzedTokens[0].value))
            {
                int loop = line;

                if (analyzedTokens[0].type == Token.Type.For)
                {
                    Obj obj = Calculator.Calculate(analyzedTokens[3..], properties), prev = Obj.None;

                    if (obj.CIter() is not Iter iter) throw new("Arg Error");

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
                else if (analyzedTokens[0].type == Token.Type.While)
                {
                    nesting++;

                    while (true)
                    {
                        line = loop;

                        if (ReturnValue is not null) break;
                        if (Calculator.Calculate(analyzedTokens[1..], properties) is not Bool b || !b.value)
                            break;
                        line++;

                        while (line < code.Length && IsBody() && TryInterpret()) ;
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
            return [.. buffer];
        }

        int IndexOfAssign(List<Token> analyzedTokens)
        {
            int index = -1;

            for (int i = 0; index == -1 && i < analyzedTokens.Count; i++)
                if (Token.IsAssigns(analyzedTokens[i]))
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

                List<Token> tokens = Tokenizer.Tokenize(code[line]);

                if (tokens.Count > 0 && !Token.IsControl(tokens[0]))
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

        public static List<Token> All(string code, Dictionary<string, Obj> properties) => Lexer.Lex(Tokenizer.Tokenize(code), properties);
    }
}
