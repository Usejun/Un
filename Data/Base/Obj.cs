namespace Un.Data;

public class Obj : IComparable<Obj>
{
    public static Obj None => new();

    public string ClassName { get; protected set; } = "None";

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

        ClassName = tokens[1].value;

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

                this.field.Set(token.value, None);
                this.field[token.value] = Calculator.Calculate(tokens[(assign + 1)..], field);
            }
            else if (tokens[0].type == Token.Type.Func)
            {
                int start = line;

                nesting++;
                line++;


                while (line < code.Length && (string.IsNullOrWhiteSpace(code[line]) || Tokenizer.IsBody(code[line], nesting)))
                    line++;

                this.field.Set(tokens[1].value, new Fun(tokens[1].value, code[start..line]));

                line--;
                nesting--;
            }
            else throw new SyntaxError();

        }
    }

    public virtual void Init()
    {

    }

    public virtual Obj Init(Iter args)
    {
        if (field.Get("__init__", out var value) && value is Fun fun)
        {
            Iter argss = new([this]);
            fun.Call(argss.Extend(args));
        }
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
        if (field.Get("__add__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot be added to each other.");
    }

    public virtual Obj Sub(Obj arg)
    {
        if (field.Get("__sub__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot be subtracted to each other.");
    }

    public virtual Obj Mul(Obj arg)
    {
        if (field.Get("__mul__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot be multiplied to each other.");
    }

    public virtual Obj Mod(Obj arg)
    {
        if (field.Get("__mod__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform the remaining operations on each other.");
    }

    public virtual Obj Div(Obj arg)
    {
        if (field.Get("__div__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj IDiv(Obj arg)
    {
        if (field.Get("__idiv__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj Pow(Obj arg)
    {
        if (field.Get("__pow__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot be raised to a power");
    }

    public virtual Obj LSh(Obj arg)
    {
        if (field.Get("__lsh__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that can't be left-shifted");
    }

    public virtual Obj RSh(Obj arg)
    {
        if (field.Get("__rsh__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that can't be right-shifted");
    }

    public virtual Obj BAnd(Obj arg)
    {
        if (field.Get("__band__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform bitwise AND operations");
    }

    public virtual Obj BOr(Obj arg)
    {
        if (field.Get("__bor__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform bitwise OR operations");
    }

    public virtual Obj BXor(Obj arg)
    {
        if (field.Get("__bxor__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform bitwise XOR operations");
    }

    public virtual Obj BNot()
    {
        if (field.Get("__bnot__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this]));
        throw new TypeError("Types that cannot perform bitwise Not operations");
    }

    public virtual Obj And(Obj arg)
    {
        if (field.Get("__and__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform the boolean AND operation.");
    }

    public virtual Obj Or(Obj arg)
    {
        if (field.Get("__or__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform the boolean OR operation.");
    }

    public virtual Obj Xor(Obj arg)
    {
        if (field.Get("__xor__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this, arg]));
        throw new TypeError("Types that cannot perform the boolean XOR operation.");
    }


    public virtual Bool Equals(Obj arg)
    {
        if (arg.ClassName == "None" && ClassName == "None")
            return new(true);
        if (field.Get("__eq__", out var value) && value is Fun fun && fun.Call(new Iter([this, arg])) is Bool b)
            return b;
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool Unequals(Obj arg) => new(!Equals(arg).value);

    public virtual Bool LessThen(Obj arg)
    {
        if (field.Get("__lt__", out var value) && value is Fun fun && fun.Call(new Iter([this, arg])) is Bool b)
            return b;
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool GreaterThen(Obj arg) => new(!LessThen(arg).value);

    public virtual Bool LessOrEquals(Obj arg) => new(LessThen(arg).value || Equals(arg).value);

    public virtual Bool GreaterOrEquals(Obj arg) => new(!LessThen(arg).value || !Equals(arg).value);


    public virtual Int Len()
    {
        if (field.Get("__len__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
            return i;
        return new(1);
    }

    public Int Hash() => new(GetHashCode());

    public Str Type()
    {
        if (field.Get("__type__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
            return s;
        return new(ClassName);
    }


    public virtual Str CStr()
    {
        if (field.Get("__str__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
            return s;
        return new(ClassName);
    }

    public virtual Bool CBool()
    {
        if (field.Get("__bool__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Bool b)
            return b;
        throw new TypeError("This type cannot cast bool.");
    }

    public virtual Float CFloat()
    {
        if (field.Get("__float__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Float f)
            return f;
        throw new TypeError("This type cannot cast float.");
    }

    public virtual Int CInt()
    {
        if (field.Get("__int__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
            return i;
        throw new TypeError("This type cannot cast int.");
    }

    public virtual Iter CIter()
    {
        if (field.Get("__iter__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Iter it)
            return it;
        throw new TypeError("This type cannot cast iter.");
    }

    public virtual Obj Clone() => new(ClassName, field);

    public virtual Obj Copy()
    {
        if (field.Get("__copy__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Iter it)
            return it;
        return this;
    }


    public virtual Obj GetItem(Iter args)
    {
        if (field.Get("__getitem__", out var value) && value is Fun fun)
            return fun.Call(args.ExtendInsert(this, 0));
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj SetItem(Iter args)
    {
        if (field.Get("__setitem__", out var value) && value is Fun fun)
            return fun.Call(args.ExtendInsert(this, 0));
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj Slice(Iter args)
    {
        if (args[0] is not Int start || args[1] is not Int end)
            throw new SyntaxError();

        start.value = start.value < 0 ? Len().value + start.value + 1 : start.value;
        end.value = end.value < 0 ? Len().value + end.value + 1 : end.value;

        if (start.value > end.value)
            throw new SyntaxError();

        Iter sliced = [];

        while (start.value < end.value)
        {
            sliced.Append(GetItem([start]));
            start.value++;
        }

        return sliced;
    }


    public virtual Obj Entry()
    {
        if (field.Get("__entry__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this]));
        throw new FileError("Types with undefined entry functions");
    }

    public virtual Obj Exit()
    {
        if (field.Get("__exit__", out var value) && value is Fun fun)
            return fun.Call(new Iter([this]));
        throw new FileError("Types with undefined exit functions");
    }


    public bool HasProperty(string key) => field.Key(key);

    public override bool Equals(object? obj)
    {
        if (obj is Obj o) return Equals(o).value;
        return false;
    }

    public int CompareTo(Obj? other)
    {
        NoneError.IsNull(other);

        if (Equals(other).value) return 0;
        if (LessThen(other).value) return -1;
        if (GreaterThen(other).value) return 1;
        throw new TypeError("Types that are not comargsble to each other.");
    }


    public static Obj Convert(string str, Field field)
    {
        if (string.IsNullOrEmpty(str)) return None;
        if (field.Get(str, out var value)) return value;
        if (Process.TryGetStaticClass(str, out var staticCla)) return staticCla;
        if (Process.TryGetPublicProperty(str, out var property)) return property;
        if (Str.IsDoubleStr(str)) return new Str(str.Trim('\"'));
        if (Str.IsSingleStr(str)) return new Str(str.Trim('\''));
        if (Iter.IsIter(str)) return new Iter(str, field);
        if (Dict.IsDict(str)) return new Dict(str, field);
        if (Collections.Set.IsSet(str)) return new Set(str, field);
        if (Bool.IsBool(str)) return new Bool(str == "true");
        if (str == "None") return None;
        if (long.TryParse(str, out var l)) return new Int(l);
        if (int.TryParse(str, out var i)) return new Int(i);
        if (double.TryParse(str, out var d)) return new Float(d);
        if (Lambda.IsLambda(str)) return new Lambda(str);

        throw new SyntaxError();
    }

    public static bool IsNone(Obj obj) => obj.ClassName == "None";
}
