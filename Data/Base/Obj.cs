using System.Diagnostics;

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
                this.field[token.Value] = Calculator.Calculate(tokens[(assign + 1)..], field);
            }
            else if (tokens[0].type == Token.Type.Func)
            {
                int start = line;

                nesting++;
                line++;


                while (line < code.Length && (string.IsNullOrWhiteSpace(code[line]) || Tokenizer.IsBody(code[line], nesting)))
                    line++;

                this.field.Set(tokens[1].Value, new LocalFun(tokens[1].Value, code[start..line]));

                line--;
                nesting--;
            }
            else throw new SyntaxError();

        }
    }

    public virtual void Init()
    {

    }

    public virtual Obj Init(List args)
    {
        if (field.Get("__init__", out var Value) && Value is Fun fun)
        {
            List argss = new([this]);
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

    public virtual void Set(string str, Obj Value)
    {
        if (!field.Key(str))
            throw new TypeError("A property that doesn't exist.");

        field[str] = Value;
    }


    public virtual Obj Add(Obj arg)
    {
        if (field.Get("__add__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot be added to each other.");
    }

    public virtual Obj Sub(Obj arg)
    {
        if (field.Get("__sub__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot be subtracted to each other.");
    }

    public virtual Obj Mul(Obj arg)
    {
        if (field.Get("__mul__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot be multiplied to each other.");
    }

    public virtual Obj Mod(Obj arg)
    {
        if (field.Get("__mod__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot perform the remaining operations on each other.");
    }

    public virtual Obj Div(Obj arg)
    {
        if (field.Get("__div__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj IDiv(Obj arg)
    {
        if (field.Get("__idiv__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj Pow(Obj arg)
    {
        if (field.Get("__pow__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot be raised to a power");
    }

    public virtual Obj LSh(Obj arg)
    {
        if (field.Get("__lsh__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that can't be left-shifted");
    }

    public virtual Obj RSh(Obj arg)
    {
        if (field.Get("__rsh__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that can't be right-shifted");
    }

    public virtual Obj BAnd(Obj arg)
    {
        if (field.Get("__band__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot perform bitwise AND operations");
    }

    public virtual Obj BOr(Obj arg)
    {
        if (field.Get("__bor__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot perform bitwise OR operations");
    }

    public virtual Obj BXor(Obj arg)
    {
        if (field.Get("__bxor__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot perform bitwise XOR operations");
    }

    public virtual Obj BNot()
    {
        if (field.Get("__bnot__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this]));
        throw new TypeError("Types that cannot perform bitwise Not operations");
    }

    public virtual Obj Xor(Obj arg)
    {
        if (field.Get("__xor__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this, arg]));
        throw new TypeError("Types that cannot perform the boolean XOR operation.");
    }


    public virtual Bool Equals(Obj arg)
    {
        if (arg.ClassName == "None" && ClassName == "None")
            return new(true);
        if (field.Get("__eq__", out var Value) && Value is Fun fun && fun.Call(new List([this, arg])) is Bool b)
            return b;
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool Unequals(Obj arg) => new(!Equals(arg).Value);

    public virtual Bool LessThen(Obj arg)
    {
        if (field.Get("__lt__", out var Value) && Value is Fun fun && fun.Call(new List([this, arg])) is Bool b)
            return b;
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool GreaterThen(Obj arg) => new(!LessThen(arg).Value);

    public virtual Bool LessOrEquals(Obj arg) => new(LessThen(arg).Value || Equals(arg).Value);

    public virtual Bool GreaterOrEquals(Obj arg) => new(!LessThen(arg).Value || !Equals(arg).Value);


    public virtual Int Len()
    {
        if (field.Get("__len__", out var Value) && Value is Fun fun && fun.Call(new List([this])) is Int i)
            return i;
        return new(1);
    }

    public Int Hash() => new(GetHashCode());

    public Str Type()
    {
        if (field.Get("__type__", out var Value) && Value is Fun fun && fun.Call(new List([this])) is Str s)
            return s;
        return new(ClassName);
    }


    public virtual Str CStr()
    {
        if (field.Get("__str__", out var v) && v is Fun fun && fun.Call(new List([this])) is Str s)
            return s;
        return new(ClassName);
    }

    public virtual Bool CBool()
    {
        if (field.Get("__bool__", out var v) && v is Fun fun && fun.Call(new List([this])) is Bool b)
            return b;
        throw new TypeError("This type cannot cast bool.");
    }

    public virtual Float CFloat()
    {
        if (field.Get("__float__", out var v) && v is Fun fun && fun.Call(new List([this])) is Float f)
            return f;
        throw new TypeError("This type cannot cast float.");
    }

    public virtual Int CInt()
    {
        if (field.Get("__int__", out var v) && v is Fun fun && fun.Call(new List([this])) is Int i)
            return i;
        throw new TypeError("This type cannot cast int.");
    }

    public virtual List CList()
    {
        if (field.Get("__list__", out var v) && v is Fun fun && fun.Call(new List([this])) is List l)
            return l;
        throw new TypeError("This type cannot cast list.");
    }

    public virtual Obj Clone() => new(ClassName, field);

    public virtual Obj Copy()
    {
        if (field.Get("__copy__", out var Value) && Value is Fun fun && fun.Call(new List([this])) is List it)
            return it;
        return this;
    }


    public virtual Obj GetItem(List args)
    {
        if (field.Get("__getitem__", out var Value) && Value is Fun fun)
            return fun.Call(args.ExtendInsert(this, 0));
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj SetItem(List args)
    {
        if (field.Get("__setitem__", out var Value) && Value is Fun fun)
            return fun.Call(args.ExtendInsert(this, 0));
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj Slice(List args)
    {
        if (args[0] is not Int start || args[1] is not Int end)
            throw new SyntaxError();

        long s = start.Value < 0 ? Len().Value + start.Value + 1 : start.Value;
        long e = end.Value < 0 ? Len().Value + end.Value + 1 : end.Value;

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
        if (field.Get("__entry__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this]));
        throw new FileError("Types with undefined entry functions");
    }

    public virtual Obj Exit()
    {
        if (field.Get("__exit__", out var Value) && Value is Fun fun)
            return fun.Call(new List([this]));
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
        if (field.Get(str, out var Value)) return Value;
        if (Process.TryGetStaticClass(str, out var staticCla)) return staticCla;
        if (Process.TryGetGlobalProperty(str, out var property)) return property;
        if (Str.IsDoubleStr(str)) return new Str(str.Trim('\"'));
        if (Str.IsSingleStr(str)) return new Str(str.Trim('\''));
        if (Str.IsFStr(str)) return Str.FStr(str.Trim('`'), field);
        if (List.IsList(str)) return new List(str, field);
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
