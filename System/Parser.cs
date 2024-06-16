using System.Xml.Linq;

namespace Un;

public class Parser(string[] code, Field field, int index = 0, int line = 0, int nesting = 0)
{
    public Obj? ReturnValue = null;

    public int Index { get; private set; } = index;
    public int Line { get; private set; } = line;
    public int Nesting { get; private set; } = nesting;
    public string[] Code { get; private set; } = code;
    public Field Field { get; private set; } = field;
    public List<string> Usings { get; private set; } = [];

    public bool TryInterpret()
    {
        if (ReturnValue is not null || Line >= Code.Length)
        {
            foreach (var name in Usings)
                Field[name].Exit();
            return false;
        }
        if (string.IsNullOrWhiteSpace(Code[Line]))
        {
            Line++;
            return true;
        }

        Parse(Lexer.Lex(Tokenizer.Tokenize(Code[Line]), Field));
        Line++;

        return true;
    }

    public void Change(string[] code, Field field, List<string> usings, int index = 0, int line = 0, int nesting = 0)
    {
        (Code, Field, Usings, Index, Line, Nesting) = (code, field, usings, index, line, nesting);
    }

    private void Parse(List<Token> analyzedTokens)
    {
        int comment = IndexOfComment(analyzedTokens);
        analyzedTokens = analyzedTokens[..comment];

        int assign = IndexOfAssign(analyzedTokens);

        if (analyzedTokens.Count == 0) return;
        else if (analyzedTokens[0].type == Token.Type.Using)
        {
            Usings.Add(analyzedTokens[1].Value);
            Parse(analyzedTokens[1..]);

            Field[analyzedTokens[1].Value].Entry();
        }
        else if (analyzedTokens[0].type == Token.Type.Await)
        {
            if (Token.IsAssigns(analyzedTokens[2]))
            {
                string name = analyzedTokens[1].Value;
                var value = Calculator.Calculate(analyzedTokens[3..], Field);

                if (!Fun.Invoke(value, "run", [value], out var result))
                    throw new ValueError($"{name} is not task");

                Field.Set(name, result);
            }
            else
            {
                var value = Calculator.Calculate(analyzedTokens[1..], Field);

                if (!Fun.Invoke(value, "run", [value], out var result))
                    throw new ValueError("it is not task");
            }
        }
        else if (analyzedTokens[0].type == Token.Type.Import)
        {
            for (int i = 1; i < analyzedTokens.Count; i += 2)
                Process.Import(analyzedTokens[i].Value);
        }
        else if (analyzedTokens[0].type == Token.Type.Class)
        {
            Process.Class.Set(analyzedTokens[1].Value, new([.. GetBody(includeHeader: true)], Field));
        }
        else if (analyzedTokens[0].type == Token.Type.Enum)
        {
            Process.Class.Set(analyzedTokens[1].Value, new Enu([.. GetBody(includeHeader: true)]));
            Process.StaticClass.Set(analyzedTokens[1].Value, Process.Class[analyzedTokens[1].Value]);
        }
        else if (analyzedTokens[0].type == Token.Type.Func)
        {          
            Process.Field.Set(analyzedTokens[1].Value, new LocalFun(analyzedTokens[1].Value, GetBody(includeHeader: true)));
        }
        else if (analyzedTokens[0].type == Token.Type.Async)
        {
            Process.Field.Set(analyzedTokens[2].Value, new AsyncFun(analyzedTokens[2].Value, GetBody(includeHeader: true)));
        }
        else if (analyzedTokens[0].type == Token.Type.Return)
        {
            ReturnValue = analyzedTokens[1..].Count == 0 ? Obj.None : Calculator.Calculate(analyzedTokens[1..], Field);
        }
        else if (analyzedTokens[0].type == Token.Type.Break)
        {
            ReturnValue = new Obj("break");
        }
        else if (analyzedTokens[0].type == Token.Type.Continue)
        {
            ReturnValue = new Obj("continue");
        }
        else if (Token.IsControl(analyzedTokens[0].Value))
        {
            Bool condition = analyzedTokens[0].type == Token.Type.Else ? new(true) : Calculator.Calculate(analyzedTokens[1..], Field).CBool();

            if (condition.Value)
            {
                Nesting++;
                Line++;

                while (Line < Code.Length && IsBody() && TryInterpret())
                {
                    if (ReturnValue?.ClassName == "break")
                    {
                        ReturnValue = null;
                        break;
                    }
                    else if (ReturnValue?.ClassName == "continue")
                        ReturnValue = null;
                }

                Nesting--;

                if (Line < Code.Length)
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
        else if (Token.IsLoop(analyzedTokens[0].Value))
        {
            int loop = Line;

            if (analyzedTokens[0].type == Token.Type.For)
            {
                Obj obj, prev;

                if (analyzedTokens[2].type == Token.Type.In)
                {
                    obj = Calculator.Calculate(analyzedTokens[3..], Field);

                    if (obj.CList() is not List list)                  
                        throw new ValueError("it is not iterable");

                    string var = analyzedTokens[1].Value;

                    if (Field.Get(var, out prev)) Field[var] = Obj.None;
                    else Field.Set(var, Obj.None);

                    Nesting++;

                    if (list.Count != 0)
                    {
                        foreach (var item in list)
                        {
                            Line = loop + 1;
                            Field[var] = item;

                            while (Line < Code.Length && IsBody() && TryInterpret()) { }

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
                        Field.Remove(var);
                    else
                        Field[var] = prev is null ? Obj.None : prev;

                    Line--;
                    Nesting--;
                }
                else if (analyzedTokens[2].type == Token.Type.Assign)
                {
                    int comma = NextComma(analyzedTokens, 0);
                    string var = analyzedTokens[1].Value;

                    if (Field.Get(var, out prev)) Field[var] = Obj.None;
                    else Field.Set(var, Obj.None);

                    Field[var] = Calculator.Calculate(analyzedTokens[3..comma], Field);

                    List<Token> condition = analyzedTokens[(comma + 1)..NextComma(analyzedTokens, comma + 1)];
                    comma = NextComma(analyzedTokens, comma + 1);
                    List<Token> expression = analyzedTokens[(comma + 1)..];

                    Nesting++;

                    while (Calculator.Calculate(condition, Field) is Bool b && b.Value)
                    {
                        Line = loop + 1;

                        while (Line < Code.Length && IsBody() && TryInterpret()) ;

                        if (ReturnValue?.ClassName == "break")
                        {
                            ReturnValue = null;
                            break;
                        }
                        if (ReturnValue?.ClassName == "continue")
                            ReturnValue = null;

                        Parse(expression);
                    }

                    Line = loop + 1;

                    SkipBody();

                    if (prev == Obj.None)
                        Field.Remove(var);
                    else
                        Field[var] = prev is null ? Obj.None : prev;

                    Line--;
                    Nesting--;

                }
            }
            else if (analyzedTokens[0].type == Token.Type.While)
            {
                Nesting++;

                while (true)
                {
                    Line = loop;

                    if (ReturnValue is not null) break;
                    if (!Calculator.Calculate(analyzedTokens[1..], Field).CBool().Value)
                        break;
                    Line++;

                    while (Line < Code.Length && IsBody() && TryInterpret()) ;

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
            Obj v;
            List values = [];

            while ((j = NextComma(analyzedTokens, i)) != -1)
            {
                values.Append(Calculator.Calculate(analyzedTokens[i..j], Field));
                i = j + 1;
            }

            v = Calculator.Calculate(analyzedTokens[i..], Field);
            if (v is Map) values.Extend(v);
            else values.Append(v);

            i = 0;

            foreach (var value in values)
            {
                Obj var = Obj.None;

                while (i < assign)
                {
                    var token = analyzedTokens[i];

                    if (analyzedTokens[i + 1].type == Token.Type.Comma || Token.IsAssigns(analyzedTokens[i + 1]))
                    {
                        if (Token.IsLiteral(analyzedTokens[i]))
                            throw new SyntaxError();

                        if (Obj.IsNone(var))
                        {
                            if (Field.Get(token.Value, out var local)) var = local;
                            else if (Process.TryGetGlobalProperty(token.Value, out var global)) var = global;
                            else Field.Set(token.Value, var);

                            Field[token.Value] = AssignCalculate(var, value, analyzedTokens[assign].type);
                        }
                        else if (token.type == Token.Type.Indexer)
                        {
                            Obj index = Obj.Convert(token.Value, Field);

                            var.SetItem(new([index, AssignCalculate(var.GetItem(new([index])), value, analyzedTokens[assign].type).Copy()]));
                        }
                        else if (token.type == Token.Type.Property)
                            var.Set(token.Value, AssignCalculate(var.Get(token.Value), value, analyzedTokens[assign].type).Copy());
                        else throw new SyntaxError("unreachable");

                        i += 2;

                        break;
                    }
                    else
                    {
                        if (Obj.IsNone(var))
                        {
                            if (Field.Get(token.Value, out var local))
                                var = local;
                            else if (Process.TryGetGlobalProperty(token.Value, out var global))
                                var = global;
                            else
                                Field.Set(token.Value, var);
                        }
                        else
                        {
                            if (token.type == Token.Type.Indexer)
                                var = var.GetItem(new List([Obj.Convert(token.Value, Field)]));
                            else if (token.type == Token.Type.Property)
                                var = var.Get(token.Value);
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
            Obj var = Obj.Convert(analyzedTokens[0].Value, Field);

            while (analyzedTokens.Count > next)
            {
                if (analyzedTokens[next].type == Token.Type.Indexer)
                {
                    Obj index = Obj.Convert(analyzedTokens[next].Value, Field);
                    var = var.GetItem(new List([index]));
                }
                else if (analyzedTokens[next].type == Token.Type.Property ||
                         analyzedTokens[next].type == Token.Type.Method)
                {
                    List args = Obj.Convert(analyzedTokens[next + 1].Value, Field).CList();

                    if (Fun.Invoke(var, analyzedTokens[next].Value, var is Data.Object ? args : args.Insert(var, 0), out var result))
                        var = result;
                    else var = var.Get(analyzedTokens[next].Value);
                }
                else break;
                next++;
            }
        }
        else if (analyzedTokens[0].type == Token.Type.Function)
        {
            Calculator.Calculate(analyzedTokens, Field);
        }
        else throw new SyntaxError("unreachable");
    }

    private string[] GetBody(bool includeHeader = false)
    {
        List<string> buffer = [];

        if (includeHeader)
            buffer.Add(Code[Line]);

        Nesting++;
        Line++;

        while (Code.Length > Line && IsBody())
            buffer.Add(Code[Line++]);

        Nesting--;
        Line--;
        return [.. buffer];
    }

    private Obj AssignCalculate(Obj a, Obj b, Token.Type type) => type switch
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

    private int IndexOfAssign(List<Token> analyzedTokens)
    {
        int index = -1;

        for (int i = 0; index == -1 && i < analyzedTokens.Count; i++)
            if (Token.IsAssigns(analyzedTokens[i]))
                index = i;

        return index;
    }

    private int IndexOfComment(List<Token> analyzedTokens)
    {
        int index = analyzedTokens.Count;

        for (int i = 0; index == analyzedTokens.Count && i < analyzedTokens.Count; i++)
            if (analyzedTokens[i].type == Token.Type.Comment)
                index = i;

        return index;
    }

    private int NextComma(List<Token> analyzedTokens, int index)
    {
        int k = -1;

        for (int i = index; k == -1 && i < analyzedTokens.Count; i++)
            if (analyzedTokens[i].type == Token.Type.Comma)
                k = i;
        return k;
    }

    private bool IsBody()
    {
        if (string.IsNullOrWhiteSpace(Code[Line]) || string.IsNullOrEmpty(Code[Line])) return true;

        bool a = true, b = true;

        for (int i = 0; a && i < Nesting; i++)
            if (Code[Line].Length > i && Code[Line][i] != '\t')
                a = false;

        for (int i = 0; b && i < 4 * Nesting; i++)
            if (Code[Line].Length > i && Code[Line][i] != ' ')
                b = false;

        return a || b;
    }

    private void SkipWhitespace(string code)
    {
        while (Index < code.Length && char.IsWhiteSpace(code[Index]))
            Index++;
    }

    private void SkipConditional()
    {
        while (Line < Code.Length)
        {
            Index = 0;

            SkipWhitespace(Code[Line]);

            List<Token> tokens = Tokenizer.Tokenize(Code[Line]);

            if (tokens.Count > 0 && !Token.IsControl(tokens[0]))
                break;

            Nesting++;
            Line++;

            while (Line < Code.Length && IsBody())
                Line++;

            Nesting--;
        }
    }

    private void SkipBody()
    {
        while (Line < Code.Length && IsBody())
            Line++;
    }
}
