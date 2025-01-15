namespace Un.Interpreter;

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
        int comment = Token.IndexOf(analyzedTokens, Token.Type.Comment);
        analyzedTokens = analyzedTokens[..(comment < 0 ? analyzedTokens.Count : comment)];

        int assign = Token.IndexOf(analyzedTokens, Token.IsAssigns);

        if (analyzedTokens.Count == 0) return;
        else if (analyzedTokens[0].type == Token.Type.Using)
        {
            if (assign < 0)
                throw new SyntaxError();

            Usings.Add(analyzedTokens[1].Value);
            

            Field[analyzedTokens[1].Value].Entry();
        }
        else if (analyzedTokens[0].type == Token.Type.Await)
        {
            if (Token.IsAssigns(analyzedTokens[2].type))
            {
                var name = analyzedTokens[1].Value;
                var value = Calculator.On(analyzedTokens[3..], Field);

                if (!Fun.Invoke(value, Literals.Run, new(value), new(), out var result))
                    throw new ValueError($"{name} is not task");

                Field.Set(name, result);
            }
            else
            {
                var value = Calculator.On(analyzedTokens[1..], Field);

                if (!Fun.Invoke(value, Literals.Run, new(value), new(), out var result))
                    throw new ValueError("it is not task");
            }
        }
        else if (analyzedTokens[0].type == Token.Type.Import)
        {
            if (analyzedTokens.Count == 4 && analyzedTokens[2].type == Token.Type.As)
                Process.Import(analyzedTokens[1].Value, analyzedTokens[3].Value);
            else            
                for (int i = 1; i < analyzedTokens.Count; i += 2)
                    Process.Import(analyzedTokens[i].Value);
            
        }
        else if (analyzedTokens[0].type == Token.Type.Try)
        {
            Obj error;
            Parser parser;

            try
            {
                parser = new(GetBody(), Field, Index, 0, Nesting) { Usings = Usings };

                while (parser.TryInterpret()) ;

                error = parser.ReturnValue;
            }
            catch (Exception e)
            {
                List<Token> tokens = Tokenizer.Tokenize(Code[Line + 1]);
                if (tokens.Count > 0 && tokens[0].type == Token.Type.Catch)
                {
                    Line++;
                    if (tokens.Count == 2)
                        Field.Set(tokens[1].Value, new Error(e));

                    parser = new(GetBody(), Field, Index, 0, Nesting) { Usings = Usings };

                    while (parser.TryInterpret()) ;

                    error = parser.ReturnValue;

                    if (tokens.Count == 2)
                        Field.Remove(tokens[1].Value);
                }                
            }
            finally
            {
                if (Code.Length > Line + 1)
                {
                    List<Token> tokens = Tokenizer.Tokenize(Code[Line + 1]);
                    if (tokens.Count > 0 && tokens[0].type == Token.Type.Fin)
                    {
                        Line++;
                        parser = new(GetBody(), Field, Index, 0, Nesting) { Usings = Usings };

                        while (parser.TryInterpret()) ;

                        error = parser.ReturnValue;
                    }
                }
            }
        }
        else if (analyzedTokens[0].type == Token.Type.Throw)
        {
            Obj obj = Calculator.On(analyzedTokens[1..], Field);
            if (obj is not Error error)
                throw new ValueError("throw keyword is used only error");
            throw error.Value;
        }
        else if (analyzedTokens[0].type == Token.Type.Class)
        {
            var name = analyzedTokens[1].Value;

            Process.Class.Set(name, new(GetBody(includeHeader: true), Field));
        }
        else if (analyzedTokens[0].type == Token.Type.Enum)
        {
            var name = analyzedTokens[1].Value;

            Process.Class.Set(name, new Enu(GetBody(includeHeader: true)));
            Process.StaticClass.Set(name, Process.Class[name]);
        }
        else if (analyzedTokens[0].type == Token.Type.Func)
        {
            var token = analyzedTokens[1].Value;
            var name = token.Split(Literals.LParen)[0];

            Process.Field.Set(name, new LocalFun(name, GetBody(includeHeader: true)));
        }
        else if (analyzedTokens[0].type == Token.Type.Async)
        {
            var token = analyzedTokens[2].Value;
            var name = token.Split(Literals.LParen)[0];

            Process.Field.Set(name, new AsyncFun(name, GetBody(includeHeader: true)));
        }
        else if (analyzedTokens[0].type == Token.Type.Return)
        {
            ReturnValue = analyzedTokens.Count == 1 ? Obj.None : Calculator.On(analyzedTokens[1..], Field);
        }
        else if (analyzedTokens[0].type == Token.Type.Del)
        {
            Obj obj = Obj.None;
            obj = analyzedTokens[1].type switch
            {
                Token.Type.Function or Token.Type.Method => Fun.Invoke(obj, analyzedTokens[1].Value.Split('(')[0], new(analyzedTokens[1].Value.Split('(')[1], Field), Field),
                _ => Obj.Convert(analyzedTokens[1].Value, Field),
            };

            for (int i = 2; i < analyzedTokens.Count - 1; i++)
            {
                obj = analyzedTokens[i].type switch
                {
                    Token.Type.Property => obj.Get(analyzedTokens[i].Value),
                    Token.Type.Indexer => obj.GetItem(new(Obj.Convert(analyzedTokens[i].Value, field)), Field),
                    Token.Type.Method => Fun.Invoke(obj, analyzedTokens[i].Value.Split('(')[0], new(analyzedTokens[i].Value.Split('(')[1], Field), Field),
                    _ => throw new SyntaxError(),
                };
            }

            switch (analyzedTokens[^1].type)
            {
                case Token.Type.Property:
                    obj.field.Remove(analyzedTokens[^1].Value);
                    break;
                case Token.Type.Indexer:
                    Obj index = Obj.Convert(analyzedTokens[^1].Value, Field);
                    if (obj is List list && index is Int i)
                        list.RemoveAt(i);
                    else if (obj is Dict dict)
                        dict.Value.Remove(index);
                    break;
                case Token.Type.Variable:
                    Field.Remove(analyzedTokens[^1].Value);
                    break;
                default:                    
                    break;
            }
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
            Bool condition = analyzedTokens[0].type == Token.Type.Else ? Bool.True : Calculator.On(analyzedTokens[1..], Field).CBool();

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
                    obj = Calculator.On(analyzedTokens[3..], Field);

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

                            if (ReturnValue?.ClassName == Literals.Break)
                            {
                                ReturnValue = null;
                                break;
                            }
                            if (ReturnValue?.ClassName == Literals.Continue)
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
                else throw new SyntaxError();
            }
            else if (analyzedTokens[0].type == Token.Type.While)
            {
                Nesting++;

                while (true)
                {
                    Line = loop;

                    if (ReturnValue is not null) break;
                    if (!Calculator.On(analyzedTokens[1..], Field).CBool().Value)
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
            int i = 0;
            var values = new Collections.Tuple($"({Token.String(analyzedTokens[2..])})", Field);

            foreach (var value in values)
            {
                Obj var = Obj.None;                

                while (i < assign)
                {
                    var token = analyzedTokens[i];

                    if (IsEnd())
                    {
                        if (Token.IsLiteral(analyzedTokens[i].type)) throw new SyntaxError();

                        if (Obj.IsNone(var))
                        {
                            if (Field.Get(token.Value, out var local)) var = local;
                            else if (Process.TryGetGlobalProperty(token.Value, out var global)) var = global;
                            else Field.Set(token.Value, var);

                            Field[token.Value] = Assign(var, value, analyzedTokens[assign].type);
                        }
                        else if (token.type == Token.Type.Indexer)
                        {
                            Obj index = Obj.Convert(token.Value, Field);

                            var.SetItem(new(index, Assign(var.GetItem(new(index), Field.Null), value, analyzedTokens[assign].type).Copy()), Field.Null);
                        }
                        else if (token.type == Token.Type.Property)
                        {
                            var.Set(token.Value, Assign(var.Get(token.Value), value, analyzedTokens[assign].type).Copy());
                        }
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
                                var = var.GetItem(new(Obj.Convert(token.Value, Field)), Field.Null);
                            else if (token.type == Token.Type.Property)
                                var = var.Get(token.Value);
                            else throw new SyntaxError("unreachable");
                        }
                    }

                    i++;
                }

                i = Token.IndexOf(analyzedTokens, Token.Type.Comma, i) + 1;
            }

            bool IsEnd() => analyzedTokens[i + 1].type == Token.Type.Comma || analyzedTokens[i + 1].type == Token.Type.Colon || Token.IsAssigns(analyzedTokens[i + 1].type);
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
                    var = var.GetItem(new(index), field);
                }
                else if (analyzedTokens[next].type == Token.Type.Property)
                {
                    var name = analyzedTokens[next].Value;

                    var = var.Get(name);
                }
                else if (analyzedTokens[next].type == Token.Type.Method)
                {
                    var index = analyzedTokens[next].Value.IndexOf(Literals.LParen);
                    var name = analyzedTokens[next].Value[..index]; 
                    var args = new Collections.Tuple(analyzedTokens[next].Value[index..], field);

                    if (Fun.Method(var, name, args, Field.Self(var), out var result))
                        var = result;
                    else throw new SyntaxError();
                }
                else break;

                next++;
            }
        }
        else if (analyzedTokens[0].type == Token.Type.Function)
        {
            Calculator.On(analyzedTokens, Field);
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

    private void SkipBody()
    {
        while (Line < Code.Length && IsBody())
            Line++;
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

            if (tokens.Count > 0 && !Token.IsControl(tokens[0].type))
                break;

            Nesting++;
            Line++;

            SkipBody();

            Nesting--;
        }
    }

    private static Obj Assign(Obj a, Obj b, Token.Type type) => type switch
    {
        Token.Type.PlusAssign => a.Add(b, Field.Null),
        Token.Type.MinusAssign => a.Sub(b, Field.Null),
        Token.Type.AsteriskAssign => a.Mul(b, Field.Null),
        Token.Type.SlashAssign => a.Div(b, Field.Null),
        Token.Type.DoubleSlashAssign => a.IDiv(b, Field.Null),
        Token.Type.PercentAssign => a.Mod(b, Field.Null),
        Token.Type.DoubleAsteriskAssign => a.Pow(b, Field.Null),
        Token.Type.BOrAssign => a.BOr(b, Field.Null),
        Token.Type.BAndAssign => a.BAnd(b, Field.Null),
        Token.Type.BXorAssign => a.BXor(b, Field.Null),
        Token.Type.LeftShiftAssign => a.LSh(b, Field.Null),
        Token.Type.RightShiftAssign => a.RSh(b, Field.Null),
        _ => b
    };

}
