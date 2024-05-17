namespace Un;

public class Parser(string[] code,
                    Dictionary<string, Obj> properties,
                    int index = 0,
                    int line = 0,
                    int nesting = 0)
{
    public Obj? ReturnValue = null;

    public int Index { get; private set; } = index;
    public int Line { get; private set; } = line;
    public int Nesting { get; private set; } = nesting;

    private readonly string[] code = code;
    private readonly Dictionary<string, Obj> properties = properties;
    private readonly List<string> usings = [];

    public bool TryInterpret()
    {
        if (ReturnValue is not null || Line >= code.Length)
        {
            foreach (var name in usings)
                properties[name].Exit();
            return false;
        }
        if (string.IsNullOrWhiteSpace(code[Line]))
        {
            Line++;
            return true;
        }

        Parse(Lexer.Lex(Tokenizer.Tokenize(code[Line]), properties));
        Line++;
        return true;
    }

    void Parse(List<Token> analyzedTokens)
    {
        int assign = IndexOfAssign(analyzedTokens);

        if (analyzedTokens.Count == 0 || Token.IsComment(analyzedTokens[0].type)) return;
        else if (analyzedTokens[0].type == Token.Type.Using)
        {
            usings.Add(analyzedTokens[1].value);
            Parse(analyzedTokens[1..]);

            properties[analyzedTokens[1].value].Entry();
        }
        else if (analyzedTokens[0].type == Token.Type.Import)
        {
            for (int i = 1; i < analyzedTokens.Count; i += 2)
                Process.Import(analyzedTokens[i].value);
        }
        else if (analyzedTokens[0].type == Token.Type.Class)
        {
            Process.Class.Add(analyzedTokens[1].value, new([.. GetBody(includeHeader: true)], properties));
            Line--;
        }
        else if (analyzedTokens[0].type == Token.Type.Enum)
        {
            Process.StaticClass.Add(analyzedTokens[1].value, new Enu([.. GetBody(includeHeader: true)]));
            Line--;
        }
        else if (analyzedTokens[0].type == Token.Type.Func)
        {
            Process.Properties.Add(analyzedTokens[1].value, new Fun(GetBody(includeHeader: true)));
            Line--;
        }
        else if (analyzedTokens[0].type == Token.Type.Return)
        {
            ReturnValue = analyzedTokens[1..].Count == 0 ? Obj.None : Calculator.Calculate(analyzedTokens[1..], properties);
        }
        else if (analyzedTokens[0].type == Token.Type.Break)
        {
            ReturnValue = new Obj("break");
        }
        else if (analyzedTokens[0].type == Token.Type.Continue)
        {
            ReturnValue = new Obj("continue");
        }
        else if (Token.IsControl(analyzedTokens[0].value))
        {
            if (analyzedTokens[0].type == Token.Type.Else ||
                Calculator.Calculate(analyzedTokens[1..], properties) is Bool b && b.value)
            {
                Nesting++;
                Line++;

                while (Line < code.Length && IsBody() && TryInterpret())
                {
                    if (ReturnValue?.ClassName == "break")
                    {
                        ReturnValue = null;
                        break;
                    }
                    if (ReturnValue?.ClassName == "continue")
                        ReturnValue = null;
                }

                Nesting--;

                if (Line < code.Length)
                    SkipConditional();

                Line--;
            }
            else
            {
                Nesting++;
                Line++;

                SkipBody();

                Line--;
                Nesting--;
            }
        }
        else if (Token.IsLoop(analyzedTokens[0].value))
        {
            int loop = Line;

            if (analyzedTokens[0].type == Token.Type.For)
            {
                Obj obj = Calculator.Calculate(analyzedTokens[3..], properties);
                Obj? prev = Obj.None;

                if (obj.CIter() is not Iter iter) throw new("Arg Error");

                string var = analyzedTokens[1].value;

                if (properties.TryGetValue(var, out prev))
                    properties[var] = Obj.None;
                else
                    properties.Add(var, Obj.None);

                Nesting++;

                if (iter.Count != 0)
                {
                    foreach (var item in iter)
                    {
                        Line = loop + 1;
                        properties[var] = item;


                        while (Line < code.Length && IsBody() && TryInterpret()) ;

                        if (ReturnValue?.ClassName == "break")
                        {
                            ReturnValue = null;
                            break;
                        }
                        if (ReturnValue?.ClassName == "continue")
                            ReturnValue = null;
                    }
                }
                else Line = loop + 1;

                SkipBody();

                if (prev == Obj.None)
                    properties.Remove(var);
                else
                    properties[var] = prev is null ? Obj.None : prev;

                Line--;
                Nesting--;
            }
            else if (analyzedTokens[0].type == Token.Type.While)
            {
                Nesting++;

                while (true)
                {
                    Line = loop;

                    if (ReturnValue is not null) break;
                    if (!Calculator.Calculate(analyzedTokens[1..], properties).CBool().value)
                        break;
                    Line++;

                    while (Line < code.Length && IsBody() && TryInterpret()) ;

                    if (ReturnValue?.ClassName == "break")
                    {
                        ReturnValue = null;
                        break;
                    }
                    if (ReturnValue?.ClassName == "continue")
                        ReturnValue = null;
                }

                Line++;
                SkipBody();
                Line--;
                Nesting--;
            }
            else throw new SyntaxError("unreachable");
        }
        else if (assign != -1)
        {
            int i = assign + 1 , j;
            Obj var = Obj.None, v;
            Iter values = [];

            while ((j = NextComma(analyzedTokens, i)) != -1)
            {
                values.Append(Calculator.Calculate(analyzedTokens[i..j], properties));
                i = j + 1;
            }

            v = Calculator.Calculate(analyzedTokens[i..], properties);
            if (v is Map) values.Extend(v);
            else values.Append(v);

            i = 0;

            foreach (var value in values)
            {
                while (i < assign)
                {
                    var token = analyzedTokens[i];

                    if (analyzedTokens[i + 1].type == Token.Type.Comma || Token.IsAssigns(analyzedTokens[i + 1]))
                    {
                        if (Token.IsLiteral(analyzedTokens[i]))
                            throw new SyntaxError();

                        if (Obj.IsNone(var))
                        {
                            if (properties.TryGetValue(token.value, out var local)) var = local;
                            else if (Process.Properties.TryGetValue(token.value, out var global)) var = global;
                            else properties.Add(token.value, var);

                            properties[token.value] = AssignCalculate(var, value, analyzedTokens[assign].type);
                        }
                        else if (token.type == Token.Type.Indexer)
                            var.SetItem(new Iter([Obj.Convert(token.value, properties), 
                                AssignCalculate(
                                    var.GetItem(new Iter([Obj.Convert(token.value, properties)])), 
                                    value, 
                                    analyzedTokens[assign].type).Copy()]));
                        else if (token.type == Token.Type.Property && var.HasProperty(token.value))
                            var.Set(token.value, AssignCalculate(var.Get(token.value), value, analyzedTokens[assign].type).Copy());
                        else throw new SyntaxError("unreachable");

                        i +=2;

                        var = Obj.None;
                        break;
                    }
                    else
                    {
                        if (Obj.IsNone(var))
                        {
                            if (properties.TryGetValue(token.value, out var local))
                                var = local;
                            else if (Process.Properties.TryGetValue(token.value, out var global))
                                var = global;
                            else
                                properties.Add(token.value, var);
                        }
                        else
                        {
                            if (token.type == Token.Type.Indexer)
                                var = var.GetItem(new Iter([Obj.Convert(token.value, properties)]));
                            else if (token.type == Token.Type.Property)
                                var = var.Get(token.value);
                            else throw new SyntaxError("unreachable");
                        }
                    }
                    i++;
                }
            }                             
        }
        else if (analyzedTokens[0].type == Token.Type.Variable)
        {
            int next = 1;
            Obj var = Obj.Convert(analyzedTokens[0].value, properties);

            while (analyzedTokens.Count > next)
            {
                if (analyzedTokens[next].type == Token.Type.Indexer)
                {
                    Obj objIndex = Obj.Convert(analyzedTokens[next].value, properties);
                    var = var.GetItem(new Iter([objIndex]));
                }
                else if (analyzedTokens[next].type == Token.Type.Property ||
                         analyzedTokens[next].type == Token.Type.Method)
                {
                    if (var.Get(analyzedTokens[next].value) is Fun func)
                    {
                        var = func.Call(Obj.Convert(analyzedTokens[next + 1].value, properties).CIter().Insert(var, 0));
                    }
                }
                else break;
                next++;
            }
        }
        else if (analyzedTokens[0].type == Token.Type.Function)
        {
            Calculator.Calculate(analyzedTokens, properties);
        }
        else throw new SyntaxError("unreachable");
    }

    string[] GetBody(bool includeHeader = false)
    {
        List<string> buffer = [];

        if (includeHeader)
            buffer.Add(code[Line]);

        Nesting++;
        Line++;

        while (code.Length > Line && IsBody())
            buffer.Add(code[Line++]);

        Nesting--;
        return [.. buffer];
    }

    Obj AssignCalculate(Obj a, Obj b, Token.Type type) => type switch
    {
        Token.Type.PlusAssign => a.Add(b),
        Token.Type.MinusAssign => a.Sub(b),
        Token.Type.AsteriskAssign => a.Mul(b),
        Token.Type.SlashAssign => a.Div(b),
        Token.Type.DoubleSlashAssign => a.IDiv(b),
        Token.Type.PercentAssign => a.Mod(b),
        Token.Type.DoubleAsteriskAssign => a.Pow(b),
        Token.Type.BOrAssign => a.BOr(b),
        Token.Type.BAndAssign => a.BAnd(b),
        Token.Type.BXorAssign => a.BXor(b),
        Token.Type.LeftShiftAssign => a.LSh(b),
        Token.Type.RightShiftAssign => a.RSh(b),
        _ => b
    };

    int IndexOfAssign(List<Token> analyzedTokens)
    {
        int index = -1;

        for (int i = 0; index == -1 && i < analyzedTokens.Count; i++)
            if (Token.IsAssigns(analyzedTokens[i]))
                index = i;

        return index;
    }

    int NextComma(List<Token> analyzedTokens, int index)
    {
        int k = -1;

        for (int i = index; k == -1 && i < analyzedTokens.Count; i++)
            if (analyzedTokens[i].type == Token.Type.Comma)
                k = i;
        return k;
    }

    bool IsBody()
    {
        if (string.IsNullOrWhiteSpace(code[Line]) || string.IsNullOrEmpty(code[Line])) return true;

        bool a = true, b = true;

        for (int i = 0; a && i < Nesting; i++)
            if (code[Line].Length > i && code[Line][i] != '\t')
                a = false;

        for (int i = 0; b && i < 4 * Nesting; i++)
            if (code[Line].Length > i && code[Line][i] != ' ')
                b = false;

        return a || b;
    }

    void SkipWhitespace(string code)
    {
        while (Index < code.Length && char.IsWhiteSpace(code[Index]))
            Index++;
    }

    void SkipConditional()
    {
        while (Line < code.Length)
        {
            Index = 0;

            SkipWhitespace(code[Line]);

            List<Token> tokens = Tokenizer.Tokenize(code[Line]);

            if (tokens.Count > 0 && !Token.IsControl(tokens[0]))
                break;

            Nesting++;
            Line++;

            while (Line < code.Length && IsBody())
                Line++;

            Nesting--;
        }
    }

    void SkipBody()
    {
        while (Line < code.Length && IsBody())
            Line++;
    }
}
