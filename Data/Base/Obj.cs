using Un.Interpreter;

namespace Un.Data;

public class Obj : IComparable<Obj>
{
    public static Obj None => new();

    public string ClassName { get; set; } = Literals.None;

    public Obj? Super { get; protected set; }

    public Field field = new();

    public Obj() { }

    public Obj(string className)
    {
        ClassName = className;

        Init();
    }

    public Obj(string className, Field field)
    {
        ClassName = className;
        Super = Process.Class[ClassName].Super;

        this.field.Copy(field);
    }

    public Obj(string[] code, Field field)
    {
        List<Token> tokens = Lexer.Lex(Tokenizer.Tokenize(code[0]), field);

        ClassName = tokens[1].Value;

        if (tokens.Count == 4 && tokens[2].type == Token.Type.Colon)
        {
            if (Process.TryGetClass(tokens[3], out var c))
                Super = c;
            else throw new ClassError("inheriitance error");
        }
        else if (tokens.Count != 2)
            throw new SyntaxError();
        

        int line = 0, nesting = 1, assign, comment;

        while (code.Length - 1 > line)
        {
            line++;

            if (string.IsNullOrWhiteSpace(code[line]))
                continue;

            tokens = Lexer.Lex(Tokenizer.Tokenize(code[line]), field);
            (assign, comment) = (-1, tokens.Count);

            if (tokens.Count == 0)
                continue;

            for (int i = 0; comment == tokens.Count && i < tokens.Count; i++)
                if (tokens[i].type == Token.Type.Comment)
                    comment = i;

            tokens = tokens[..comment];

            for (int i = 0; assign == -1 && i < tokens.Count; i++)
                if (tokens[i].type == Token.Type.Assign)
                    assign = i;

            if (assign > 0)
            {
                Token token = tokens[0];

                this.field.Set(token.Value, None);
                this.field[token.Value] = Calculator.On(tokens[(assign + 1)..], field);
            }
            else if (tokens[0].type == Token.Type.Func)
            {
                int start = line;

                nesting++;
                line++;


                while (line < code.Length && (string.IsNullOrWhiteSpace(code[line]) || Tokenizer.IsBody(code[line], nesting)))
                    line++;

                var name = tokens[1].Value.Split('(')[0];

                this.field.Set(name, new LocalFun(name, code[start..line]));

                line--;
                nesting--;
            }
            else throw new SyntaxError();

        }
    }

    public virtual void Init()
    {

    }

    public virtual Obj Init(Collections.Tuple args, Field field)
    {
        if (Fun.Method(this, Literals.Init, args, Field.Self(this), out _)) { }
        else Super?.Init(args, field);
            
        return this;
    }


    public virtual Obj Get(string str)
    {
        if (field.Get(str, out var property))
            return property;
        if (Super != null)
            return Super.Get(str);
        throw new TypeError("A property that doesn't exist.");
    }

    public virtual void Set(string str, Obj value)
    {
        if (field.Key(str))
            field[str] = value;      
        else if (Super != null)
            Super.Get(str);
        else
            throw new TypeError("A property that doesn't exist.");
    }


    public virtual Obj Add(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Add, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Add(arg, field);
        throw new TypeError("Types that cannot be added to each other.");
    }

    public virtual Obj Sub(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Sub, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Sub(arg, field);
        throw new TypeError("Types that cannot be subtracted to each other.");
    }

    public virtual Obj Mul(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Mul, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Mul(arg, field);
        throw new TypeError("Types that cannot be multiplied to each other.");
    }

    public virtual Obj Mod(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Mod, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Mod(arg, field);
        throw new TypeError("Types that cannot perform the remaining operations on each other.");
    }

    public virtual Obj Div(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Div, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Div(arg, field);
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj IDiv(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.IDiv, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.IDiv(arg, field);
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj Pow(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Pow, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Pow(arg, field);
        throw new TypeError("Types that cannot be raised to a power");
    }

    public virtual Obj LSh(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.LSh, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.LSh(arg, field);
        throw new TypeError("Types that can't be left-shifted");
    }

    public virtual Obj RSh(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.RSh, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.RSh(arg, field);
        throw new TypeError("Types that can't be right-shifted");
    }

    public virtual Obj BAnd(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.BAnd, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.BAnd(arg, field);
        throw new TypeError("Types that cannot perform bitwise AND operations");
    }

    public virtual Obj BOr(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.BOr, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.BOr(arg, field);
        throw new TypeError("Types that cannot perform bitwise OR operations");
    }

    public virtual Obj BXor(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.BXor, new(arg), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.BXor(arg, field);
        throw new TypeError("Types that cannot perform bitwise XOR operations");
    }

    public virtual Obj BNot()
    {
        if (Fun.Method(this, Literals.BNot, [], Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.BNot();
        throw new TypeError("Types that cannot perform bitwise Not operations");
    }

    public virtual Obj At(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.At, new(), Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.At(arg, field);
        throw new TypeError("Types that cannot perform the At operation.");
    }


    public virtual Bool Eq(Obj arg, Field field)
    {
        if (arg.ClassName == Literals.None && ClassName == Literals.None)
            return new(true);
        else if (Fun.Method(this, Literals.Eq, new(), Field.Self(this), out var result) && result is Bool value)
            return value;
        if (Super != null)
            return Super.Eq(arg, field);
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool Ueq(Obj arg, Field field) => new(!Eq(arg, Field.Null).Value);

    public virtual Bool Lt(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Lt, new(), Field.Self(this), out var result) && result is Bool value)
            return value;
        if (Super != null)
            return Super.Lt(arg, field);
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool Gt(Obj arg, Field field) => new(!Lt(arg, Field.Null).Value);

    public virtual Bool Loe(Obj arg, Field field) => new(Lt(arg, Field.Null).Value || Eq(arg, Field.Null).Value);

    public virtual Bool Goe(Obj arg, Field field) => new(!Lt(arg, Field.Null).Value || !Eq(arg, Field.Null).Value);


    public virtual Int Len()
    {
        if (Fun.Method(this, Literals.Len, new(), Field.Self(this), out var result) && result is Int value)
            return value;
        if (Super != null)
            return Super.Len();
        return new(1);
    }

    public Int Hash() => new(GetHashCode());

    public Str Type()
    {
        if (Fun.Method(this, Literals.Type, new(), Field.Self(this), out var result) && result is Str value)
            return value;
        if (Super != null)
            return Super.Type();
        return new(ClassName);
    }


    public virtual Str CStr()
    {
        if (Fun.Method(this, Literals.CStr, Collections.Tuple.Empty, Field.Self(this), out var result) && result is Str value)
            return value;
        if (Super != null)
            return Super.CStr();
        return new(ClassName);
    }

    public virtual Bool CBool()
    {
        if (Fun.Method(this, Literals.CBool, [], Field.Self(this), out var result) && result is Bool value)
            return value;
        if (Super != null)
            return Super.CBool();
        throw new TypeError("This type cannot cast bool.");
    }

    public virtual Float CFloat()
    {
        if (Fun.Method(this, Literals.CFloat, [], Field.Self(this), out var result) && result is Float value)
            return value;
        if (Super != null)
            return Super.CFloat();
        throw new TypeError("This type cannot cast float.");
    }

    public virtual Int CInt()
    {
        if (Fun.Method(this, Literals.CInt, [], Field.Self(this), out var result) && result is Int value)
            return value;
        if (Super != null)
            return Super.CInt();
        throw new TypeError("This type cannot cast int.");
    }

    public virtual List CList()
    {
        if (Fun.Method(this, Literals.CList, [], Field.Self(this), out var result) && result is List value)
            return value;
        if (Super != null)
            return Super.CList();
        throw new TypeError("This type cannot cast list.");
    }

    public virtual Obj Clone() => new(ClassName, field);

    public virtual Obj Copy()
    {
        if (Fun.Method(this, Literals.Copy, [], Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Copy();
        return this;
    }


    public virtual Obj GetItem(Collections.Tuple args, Field field)
    {
        if (Fun.Method(this, Literals.GetItem, args, Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.GetItem(args, field);
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj SetItem(Collections.Tuple args, Field field)
    {
        if (Fun.Method(this, Literals.SetItem, args, Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.SetItem(args, field);
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj Slice(Collections.Tuple args, Field field)
    {
        if (args[0] is not Int start || args[1] is not Int end)
            throw new SyntaxError();

        long len = Len().Value;
        long s = start.Value < 0 ? len + start.Value + 1 : start.Value;
        long e = end.Value < 0 ? len + end.Value + 1 : end.Value;

        if (s > e)
            throw new SyntaxError();

        List sliced = [];

        while (s < e)
        {
            sliced.Append(GetItem(new(new Int(s)), Field.Null));
            s++;
        }

        return sliced;
    }


    public virtual Obj Entry()
    {
        if (Fun.Method(this, Literals.Entry, [], Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Entry();
        throw new FileError("Types with undefined entry functions");
    }

    public virtual Obj Exit()
    {
        if (Fun.Method(this, Literals.Exit, [], Field.Self(this), out var value))
            return value;
        if (Super != null)
            return Super.Exit();
        throw new FileError("Types with undefined exit functions");
    }
    

    public bool HasProperty(string key)
    {
        if (field.Key(key))
            return true;
        if (Super is not null)
            return Super.HasProperty(key);
        return false;
    }

    public bool Is(string name)
    {
        if (ClassName == name)
            return true;
        if (Super != null)
            return Super.Is(name);
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Obj o) return Eq(o, Field.Null).Value;
        return false;
    }

    public override int GetHashCode() => (int)Hash().Value;  

    public int CompareTo(Obj? other)
    {
        if (other == null) return 0;
        if (Eq(other, Field.Null).Value) return 0;
        if (Lt(other, Field.Null).Value) return -1;
        if (Gt(other, Field.Null).Value) return 1;
        throw new TypeError("Types that are not comparable to each other.");
    }



    public static Obj Convert(string str, Field field)
    {
        if (string.IsNullOrEmpty(str)) throw new SyntaxError();

        if (field.Get(str, out var value)) return value;
        if (Process.TryGetStaticClass(str, out var staticClass)) return staticClass;
        if (Process.TryGetGlobalProperty(str, out var property)) return property;
        if (Collections.Tuple.IsTuple(str)) return new Collections.Tuple(str, field);
        if (List.IsList(str)) return new List(new Collections.Tuple(str, field));
        if (Dict.IsDict(str)) return new Dict(str, field);
        if (Collections.Set.IsSet(str)) return new Set(str, field);        
        if (Str.IsFStr(str)) return Str.FStr(str.Trim(Literals.Backtick), field);

        return Literal(str);
    }

    public static Obj Literal(string str)
    {
        if (IsNone(str)) return None;
        if (Bool.IsBool(str)) return new Bool(str == Literals.True);
        if (long.TryParse(str, out var l)) return new Int(l);
        if (int.TryParse(str, out var i)) return new Int(i);
        if (double.TryParse(str, out var d)) return new Float(d);
        if (Lambda.IsLambda(str)) return new Lambda(str);
        if (Str.IsDoubleStr(str)) return new Str(str.Trim(Literals.Double));
        if (Str.IsSingleStr(str)) return new Str(str.Trim(Literals.Single));

        throw new SyntaxError();
    }

    public static bool IsNone(Obj obj) => obj.ClassName == Literals.None;

    public static bool IsNone(string str) => str == Literals.None;    
}
