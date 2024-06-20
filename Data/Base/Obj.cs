namespace Un.Data;

public class Obj : IComparable<Obj>
{
    public static Obj None => new();

    public string ClassName { get; protected set; } = Literals.None;

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
        this.field.Copy(field);
    }

    public Obj(string[] code, Field field)
    {
        List<Token> tokens = Lexer.Lex(Tokenizer.Tokenize(code[0]), field);

        ClassName = tokens[1].Value;

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

                var name = tokens[1].Value.Split(Literals.FunctionSep)[0];

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

    public virtual Obj Init(Collections.Tuple args)
    {
        if (Fun.Invoke(this, Literals.Init, new([this, ..args]), out _ )) { }

        return this;
    }


    public virtual Obj Get(string str)
    {
        if (field.Get(str, out var property))
            return property;
        throw new TypeError("A property that doesn't exist.");
    }

    public virtual void Set(string str, Obj value)
    {
        if (!field.Key(str))
            throw new TypeError("A property that doesn't exist.");

        field[str] = value;
    }


    public virtual Obj Add(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Add, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot be added to each other.");
    }

    public virtual Obj Sub(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Sub, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot be subtracted to each other.");
    }

    public virtual Obj Mul(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Mul, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot be multiplied to each other.");
    }

    public virtual Obj Mod(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Mod, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot perform the remaining operations on each other.");
    }

    public virtual Obj Div(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Div, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj IDiv(Obj arg)
    {
        if (Fun.Invoke(this, Literals.IDiv, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj Pow(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Pow, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot be raised to a power");
    }

    public virtual Obj LSh(Obj arg)
    {
        if (Fun.Invoke(this, Literals.LSh, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that can't be left-shifted");
    }

    public virtual Obj RSh(Obj arg)
    {
        if (Fun.Invoke(this, Literals.RSh, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that can't be right-shifted");
    }

    public virtual Obj BAnd(Obj arg)
    {
        if (Fun.Invoke(this, Literals.BAnd, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot perform bitwise AND operations");
    }

    public virtual Obj BOr(Obj arg)
    {
        if (Fun.Invoke(this, Literals.BOr, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot perform bitwise OR operations");
    }

    public virtual Obj BXor(Obj arg)
    {
        if (Fun.Invoke(this, Literals.BXor, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot perform bitwise XOR operations");
    }

    public virtual Obj BNot()
    {
        if (Fun.Invoke(this, Literals.BNot, new(this), out var value))
            return value;
        throw new TypeError("Types that cannot perform bitwise Not operations");
    }

    public virtual Obj Xor(Obj arg)
    {
        if (Fun.Invoke(this, Literals.Xor, new(this, arg), out var value))
            return value;
        throw new TypeError("Types that cannot perform the boolean XOR operation.");
    }


    public virtual Bool Equals(Obj arg)
    {
        if (arg.ClassName == Literals.None && ClassName == Literals.None)
            return new(true);
        else if (Fun.Invoke(this, Literals.Eqaul, new(this, arg), out var result) && result is Bool value)
            return value;
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool Unequals(Obj arg) => new(!Equals(arg).Value);

    public virtual Bool LessThen(Obj arg)
    {
        if (Fun.Invoke(this, Literals.LessThen, new(this, arg), out var result) && result is Bool value)
            return value;
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool GreaterThen(Obj arg) => new(!LessThen(arg).Value);

    public virtual Bool LessOrEquals(Obj arg) => new(LessThen(arg).Value || Equals(arg).Value);

    public virtual Bool GreaterOrEquals(Obj arg) => new(!LessThen(arg).Value || !Equals(arg).Value);


    public virtual Int Len()
    {
        if (Fun.Invoke(this, Literals.Len, new(this), out var result) && result is Int value)
            return value;
        return new(1);
    }

    public Int Hash() => new(GetHashCode());

    public Str Type()
    {
        if (Fun.Invoke(this, Literals.Type, new(this), out var result) && result is Str value)
            return value;
        return new(ClassName);
    }


    public virtual Str CStr()
    {
        if (Fun.Invoke(this, Literals.CStr, new(this), out var result) && result is Str value)
            return value;
        return new(ClassName);
    }

    public virtual Bool CBool()
    {
        if (Fun.Invoke(this, Literals.CBool, new(this), out var result) && result is Bool value)
            return value;
        throw new TypeError("This type cannot cast bool.");
    }

    public virtual Float CFloat()
    {
        if (Fun.Invoke(this, Literals.CFloat, new(this), out var result) && result is Float value)
            return value;
        throw new TypeError("This type cannot cast float.");
    }

    public virtual Int CInt()
    {
        if (Fun.Invoke(this, Literals.CInt, new(this), out var result) && result is Int value)
            return value;
        throw new TypeError("This type cannot cast int.");
    }

    public virtual List CList()
    {
        if (Fun.Invoke(this, Literals.CList, new(this), out var result) && result is List value)
            return value;
        throw new TypeError("This type cannot cast list.");
    }

    public virtual Obj Clone() => new(ClassName, field);

    public virtual Obj Copy()
    {
        if (Fun.Invoke(this, Literals.Copy, new(this), out var value))
            return value;
        return this;
    }


    public virtual Obj GetItem(List args)
    {
        if (Fun.Invoke(this, Literals.GetItem, new([this, ..args]), out var value))
            return value;
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj SetItem(List args)
    {
        if (Fun.Invoke(this, Literals.SetItem, new([this, .. args]), out var value))
            return value;
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj Slice(List args)
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
            sliced.Append(GetItem([new Int(s)]));
            s++;
        }

        return sliced;
    }


    public virtual Obj Entry()
    {
        if (Fun.Invoke(this, Literals.Entry, new(this), out var value))
            return value;
        throw new FileError("Types with undefined entry functions");
    }

    public virtual Obj Exit()
    {
        if (Fun.Invoke(this, Literals.Exit, new(this), out var value))
            return value;
        throw new FileError("Types with undefined exit functions");
    }


    public bool HasProperty(string key) => field.Key(key);

    public override bool Equals(object? obj)
    {
        if (obj is Obj o) return Equals(o).Value;
        return false;
    }

    public int CompareTo(Obj? other)
    {
        NoneError.IsNull(other);

        if (Equals(other).Value) return 0;
        if (LessThen(other).Value) return -1;
        if (GreaterThen(other).Value) return 1;
        throw new TypeError("Types that are not comargsble to each other.");
    }


    public static Obj Convert(string str, Field field)
    {
        if (string.IsNullOrEmpty(str)) return None;
        if (field.Get(str, out var value)) return value;
        if (Process.TryGetStaticClass(str, out var staticCla)) return staticCla;
        if (Process.TryGetGlobalProperty(str, out var property)) return property;        
        if (List.IsList(str)) return new List(new Map(str, field));
        if (Collections.Tuple.IsTuple(str)) return new Collections.Tuple(new Map(str, field));
        if (Dict.IsDict(str)) return new Dict(str, field);
        if (Collections.Set.IsSet(str)) return new Set(str, field);        
        if (Str.IsFStr(str)) return Str.FStr(str.Trim(Literals.Grave), field);

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
