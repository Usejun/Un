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
        analyzedTokens = SkipComment(analyzedTokens);

        int assign = Token.IndexOf(analyzedTokens, Token.IsAssigns);

        if (analyzedTokens.Count == 0) return;

        var type = analyzedTokens[0].type;  
        
        switch (type)
        {
            case Token.Type.Using:
                HandleUsing();                
                break;
            case Token.Type.Await:
                HandleAwait();
                break;
            case Token.Type.Import:
                HandleImport();
                break;
            case Token.Type.Try:
                HandleTry();
                break;
            case Token.Type.Throw:
                HandleThrow();
                break;
            case Token.Type.Class:
                HandleClass();
                break;
            case Token.Type.Enum:
                HandleEnum();
                break;
            case Token.Type.Func:  
                HandleFunc();
                break; 
            case Token.Type.Async: 
                HandleAsync();
                break;
            case Token.Type.Return:
                HandleReturn(analyzedTokens.Count == 1 ? Obj.None : Calculator.On(analyzedTokens[1..], Field));
                break;
            case Token.Type.Del:
                HandleDel();
                break;
            case Token.Type.Break:
                HandleReturn(new Obj(Literals.Break));
                break;
            case Token.Type.Continue:
                HandleReturn(new Obj(Literals.Continue));
                break;
            case Token.Type.If:                                
            case Token.Type.ElIf:
            case Token.Type.Else:
                HandleCodition();
                break;
            case Token.Type.For:
                HandleFor();                    
                break;
            case Token.Type.While:
                HandleWhile();
                break;
            default:     
                {
                    if (assign != -1)
                        HandleAssign();                                            
                    else if (analyzedTokens[0].type == Token.Type.Variable)
                    {
                        int next = 1;
                        Obj var = Obj.Parse(analyzedTokens[0].value, Field);

                        while (analyzedTokens.Count > next)
                        {
                            if (analyzedTokens[next].type == Token.Type.Indexer)
                            {
                                Obj index = Obj.Parse(analyzedTokens[next].value, Field);
                                var = var.GetItem(index, field);
                            }
                            else if (analyzedTokens[next].type == Token.Type.Property)
                            {
                                var name = analyzedTokens[next].value;

                                var = var.Get(name);
                            }
                            else if (analyzedTokens[next].type == Token.Type.Method)
                            {
                                var index = analyzedTokens[next].value.IndexOf(Literals.LParen);
                                var name = analyzedTokens[next].value[..index].ToString(); 
                                var args = new Collections.Tuple(analyzedTokens[next].value[index..].ToString(), field);

                                if (Fun.Method(var, name, args, new(), out var result))
                                    var = result;
                                else 
                                    throw new TypeError("A property that doesn't exist.");
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
                break;
        }

        void HandleUsing()
        {
            if (assign < 0)
                throw new SyntaxError("use keyword must be followed by assigns");

            Usings.Add(analyzedTokens[1].value.ToString());            
            Field[analyzedTokens[1].value].Entry();       
        }

        void HandleAwait()
        {
            if (Token.IsAssigns(analyzedTokens[2].type))
            {
                var name = analyzedTokens[1].value;
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
    
        void HandleImport()
        {
            if (analyzedTokens.Count == 4 && analyzedTokens[2].type == Token.Type.As)
                Process.Import(analyzedTokens[1].value.ToString(), analyzedTokens[3].value.ToString());
            else            
                for (int i = 1; i < analyzedTokens.Count; i += 2)
                    Process.Import(analyzedTokens[i].value.ToString());
        }
    
        void HandleTry()
        {
            Obj error;
            Parser parser;

            try
            {
                parser = new(GetBody(), Field, Index, 0, Nesting) { Usings = Usings };

                while (parser.TryInterpret()) ;

                error = parser.ReturnValue!;
            }
            catch (Exception e)
            {
                List<Token> tokens = Tokenizer.Tokenize(Code[Line + 1]);
                if (tokens.Count > 0 && tokens[0].type == Token.Type.Catch)
                {
                    Line++;
                    if (tokens.Count == 2)
                        Field.Set(tokens[1].value, new Error(e));

                    parser = new(GetBody(), Field, Index, 0, Nesting) { Usings = Usings };

                    while (parser.TryInterpret()) ;

                    error = parser.ReturnValue!;

                    if (tokens.Count == 2)
                        Field.Remove(tokens[1].value);
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

                        error = parser.ReturnValue!;
                    }
                }
            }
        }
    
        void HandleThrow()
        {
            Obj obj = Calculator.On(analyzedTokens[1..], Field);

            if (obj.As<Error>(out var error))
                throw new ValueError("throw keyword is used only error");
            throw error.Value;
        }
    
        void HandleClass()
        {
            var name = analyzedTokens[1].value;

            Process.Class.Set(name, new(GetBody(includeHeader: true), Field));
        }
    
        void HandleEnum()
        {
            var name = analyzedTokens[1].value;

            Process.Class.Set(name, new Enu(GetBody(includeHeader: true)));
            Process.Class.Get(name, out var cls);
            Process.StaticClass.Set(name, cls);
        }
    
        void HandleFunc()
        {
            var token = analyzedTokens[1].value;
            var name = token.Split(Literals.LParen, 1)[0].ToString();

            Process.Field.Set(name, new LocalFun(name, GetBody(includeHeader: true)));
        }
    
        void HandleAsync()
        {
            var token = analyzedTokens[2].value;
            var name = token.Split(Literals.LParen, 1)[0].ToString();

            Process.Field.Set(name, new AsyncFun(name, GetBody(includeHeader: true)));
        } 
    
        void HandleDel()
        {
            Obj obj = Obj.None;
            obj = analyzedTokens[1].type switch
            {
                Token.Type.Function or Token.Type.Method => Fun.Invoke(obj, 
                                                                analyzedTokens[1].value.Split(Literals.CLParen, 1)[0].ToString(), 
                                                                new(analyzedTokens[1].value.Split(Literals.CLParen, 2)[1].ToString(), Field), Field),
                _ => Obj.Parse(analyzedTokens[1].value, Field),
            };

            for (int i = 2; i < analyzedTokens.Count - 1; i++)
            {
                obj = analyzedTokens[i].type switch
                {
                    Token.Type.Property => obj.Get(analyzedTokens[i].value),
                    Token.Type.Indexer => obj.GetItem(Obj.Parse(analyzedTokens[i].value, field), Field),
                    Token.Type.Method => Fun.Invoke(obj, 
                                            analyzedTokens[i].value.Split(Literals.CLParen)[0].ToString(), 
                                            new(analyzedTokens[i].value.Split(Literals.CLParen)[1].ToString(), Field), Field),
                    _ => throw new SyntaxError(),
                };
            }

            switch (analyzedTokens[^1].type)
            {
                case Token.Type.Property:
                    obj.field.Remove(analyzedTokens[^1].value);
                    break;
                case Token.Type.Indexer:
                    Obj index = Obj.Parse(analyzedTokens[^1].value, Field);
                    switch (obj)
                    {
                        case List list:
                            if (!index.As<Int>(out var i))
                                throw new ValueError("index must be integer");
                            list.RemoveAt(i);
                            break;
                        case Dict dict:
                            dict.Value.Remove(index);
                            break;
                        default:
                            break;
                    }                            
                    break;
                case Token.Type.Variable:
                    Field.Remove(analyzedTokens[^1].value);
                    break;
                default:                    
                    break;
            }
        }

        void HandleCodition()
        {
            Bool condition = analyzedTokens[0].type == Token.Type.Else ? Bool.True : Calculator.On(analyzedTokens[1..], Field).CBool();

            if (condition.Value)
            {
                Nesting++;
                Line++;

                while (Line < Code.Length && IsBody() && TryInterpret())
                {
                    if (ReturnValue?.ClassName == Literals.Break)
                    {
                        ReturnValue = null;
                        break;
                    }
                    else if (ReturnValue?.ClassName == Literals.Continue)
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
    
        void HandleFor()
        {
            if (analyzedTokens[2].type != Token.Type.In)
                throw new SyntaxError("for loop must be followed by in keyword");

            Obj obj = Calculator.On(analyzedTokens[3..], Field);
            int loop = Line;            
            var name = analyzedTokens[1].value;

            if (!obj.CList().As<List>(out var list))
                throw new ValueError("for loop must be iterable");

            if (Field.Get(name, out var prev)) Field[name] = Obj.None;
            else Field.Set(name, Obj.None);

            Nesting++;

            foreach (var item in list)
            {
                Line = loop + 1;

                Field[name] = item;

                while (Line < Code.Length && IsBody() && TryInterpret()) ;

                if (ReturnValue?.ClassName == Literals.Break)
                {
                    ReturnValue = null;
                    break;
                }
                if (ReturnValue?.ClassName == Literals.Continue)
                    ReturnValue = null;
            }

            Line = loop + 1; 
            SkipBody();

            if (prev == Obj.None)
                Field.Remove(name);
            else
                Field[name] = prev is null ? Obj.None : prev;

            Line--;
            Nesting--;
        }

        void HandleWhile()
        {
            Nesting++;
            int loop = Line;
            while (true)
            {
                Line = loop;

                if (ReturnValue is not null) break;
                if (!Calculator.On(analyzedTokens[1..], Field).CBool().Value)
                    break;
                Line++;

                while (Line < Code.Length && IsBody() && TryInterpret()) ;

                if (ReturnValue?.ClassName == Literals.Break)
                {
                    ReturnValue = null;
                    break;
                }
                if (ReturnValue?.ClassName == Literals.Continue)
                    ReturnValue = null;
            }

            Line++;
            SkipBody();
            Line--;
            Nesting--;
            
        }

        void HandleAssign()
        {
            int i = 0;
            var values = new Collections.Tuple($"({Token.String(analyzedTokens[(assign+1)..])})", Field);

            foreach (var value in values)
            {
                Obj var = Obj.None;                

                while (i < assign)
                {
                    var token = analyzedTokens[i];

                    if (IsEnd())
                    {
                        if (Token.IsLiteral(analyzedTokens[i].type)) 
                            throw new SyntaxError("invalid assign");

                        if (Obj.IsNone(var))
                        {
                            if (Field.Get(token.value, out var local)) var = local;
                            else if (Process.TryGetGlobalProperty(token.value.ToString(), out var global)) var = global;
                            else Field.Set(token.value, var);

                            Field[token.value] = Assign(var, value, analyzedTokens[assign].type);
                        }
                        else if (token.type == Token.Type.Indexer)
                        {
                            Obj index = Obj.Parse(token.value, Field);

                            var.SetItem(index, Assign(var.GetItem(index, Field.Null), value, analyzedTokens[assign].type).Copy(), Field.Null);
                        }
                        else if (token.type == Token.Type.Property)
                        {
                            if (var.Get(token.value).As<Prop>(out var prop))
                                prop.Setter.Call(new(Assign(prop.Getter.Call([], new()), value, analyzedTokens[assign].type)), new());                         
                            else 
                                var.Set(token.value.ToString(), Assign(var.Get(token.value), value, analyzedTokens[assign].type).Copy());
                        }
                        else throw new SyntaxError("invalid assign");

                        i += 2;

                        break;
                    }
                    else
                    {
                        if (Obj.IsNone(var))
                        {
                            if (Field.Get(token.value, out var local))
                                var = local;
                            else if (Process.TryGetGlobalProperty(token.value.ToString(), out var global))
                                var = global;
                            else if (Process.TryGetStaticClass(token.value.ToString(), out var staticCla))
                                var = staticCla;
                            else
                                Field.Set(token.value, var);
                        }
                        else
                        {
                            if (token.type == Token.Type.Indexer)
                                var = var.GetItem(Obj.Parse(token.value, Field), Field.Null);
                            else if (token.type == Token.Type.Property)
                                var = var.Get(token.value);
                            else throw new SyntaxError("unknown assign");
                        }
                    }

                    i++;
                }

                i = Token.IndexOf(analyzedTokens, Token.Type.Comma, i) + 1;
            }

            bool IsEnd() => analyzedTokens[i + 1].type == Token.Type.Comma || analyzedTokens[i + 1].type == Token.Type.Colon || Token.IsAssigns(analyzedTokens[i + 1].type);
        }

        void HandleReturn(Obj obj) => ReturnValue = obj;
    }

    private List<Token> SkipComment(List<Token> analyzedTokens)
    {
        int comment = Token.IndexOf(analyzedTokens, Token.Type.Comment);
        return analyzedTokens[..(comment < 0 ? analyzedTokens.Count : comment)];
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
            if (Code[Line].Length > i && Code[Line][i] != Literals.Tab)
                a = false;

        for (int i = 0; b && i < 4 * Nesting; i++)
            if (Code[Line].Length > i && Code[Line][i] != Literals.Space)
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
