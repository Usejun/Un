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
    }

    public Obj(string className, Field field)
    {        
        ClassName = className;
        Super = Process.TryGetClass(className, out var original) ? original.Super : null;

        this.field.Copy(field);
    }

    public Obj(string[] code, Field field)
    {
        List<Token> tokens = Lexer.Lex(Tokenizer.Tokenize(code[0]), field);

        ClassName = tokens[1].value.ToString();

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

            int indexOfComment = Token.IndexOf(tokens, Token.Type.Comment);
            comment = indexOfComment > 0 ? indexOfComment : comment;

            tokens = tokens[..comment];

            assign = Token.IndexOf(tokens, Token.Type.Assign);

            if (assign > 0)
            {
                Token token = tokens[0];

                this.field.Set(token.value, None);
                this.field[token.value] = Calculator.On(tokens[(assign + 1)..], field);
            }
            else if (tokens[0].type == Token.Type.Func)
            {
                int start = line;

                nesting++;
                line++;

                while (line < code.Length && (string.IsNullOrWhiteSpace(code[line]) || Tokenizer.IsBody(code[line], nesting)))
                    line++;

                var name = tokens[1].value.Split(Literals.CLParen, 1)[0];

                this.field.Set(name, new LocalFun(name.ToString(), code[start..line]));

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
        if (Fun.Method(this, Literals.Init, args, new(), out _)) { }
        else Super?.Init(args, field);
            
        return this;
    }

    public Obj Get(StringBuffer sb) => Get(sb.ToString());

    public virtual Obj Get(string str, bool isOriginal = false)
    {
        if (field.Get(str, out var property))
            return property;
        if (!isOriginal && Process.TryGetClass(ClassName, out var original))
            return original.Get(str, true);
        if (Super is not null)
            return Super.Get(str);

        throw new TypeError("A property that doesn't exist.");
    }

    public virtual void Set(string str, Obj value)
    {
        if (IsNone(this))
            throw new TypeError("A property that doesn't exist.");
        else if (field.Key(str))
            field[str] = value;      
        else if (Super != null)
            Super.Get(str);
        else
            throw new TypeError("A property that doesn't exist.");
    }


    public virtual Obj Add(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Add, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.Add(arg, field);
        throw new TypeError("Types that cannot be added to each other.");
    }

    public virtual Obj Sub(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Sub, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.Sub(arg, field);
        throw new TypeError("Types that cannot be subtracted to each other.");
    }

    public virtual Obj Mul(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Mul, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.Mul(arg, field);
        throw new TypeError("Types that cannot be multiplied to each other.");
    }

    public virtual Obj Mod(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Mod, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.Mod(arg, field);
        throw new TypeError("Types that cannot perform the remaining operations on each other.");
    }

    public virtual Obj Div(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Div, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.Div(arg, field);
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj IDiv(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.IDiv, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.IDiv(arg, field);
        throw new TypeError("Types that cannot be divided to each other.");
    }

    public virtual Obj Pow(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Pow, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.Pow(arg, field);
        throw new TypeError("Types that cannot be raised to a power");
    }

    public virtual Obj LSh(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.LSh, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.LSh(arg, field);
        throw new TypeError("Types that can't be left-shifted");
    }

    public virtual Obj RSh(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.RSh, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.RSh(arg, field);
        throw new TypeError("Types that can't be right-shifted");
    }

    public virtual Obj BAnd(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.BAnd, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.BAnd(arg, field);
        throw new TypeError("Types that cannot perform bitwise AND operations");
    }

    public virtual Obj BOr(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.BOr, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.BOr(arg, field);
        throw new TypeError("Types that cannot perform bitwise OR operations");
    }

    public virtual Obj BXor(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.BXor, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.BXor(arg, field);
        throw new TypeError("Types that cannot perform bitwise XOR operations");
    }

    public virtual Obj BNot()
    {
        if (Fun.Method(this, Literals.BNot, [], new(), out var value))
            return value;
        if (Super != null)
            return Super.BNot();
        throw new TypeError("Types that cannot perform bitwise Not operations");
    }

    public virtual Obj At(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.At, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.At(arg, field);
        throw new TypeError("Types that cannot perform the At operation.");
    }


    public virtual Bool Eq(Obj arg, Field field)
    {
        if (arg.ClassName == Literals.None && ClassName == Literals.None)
            return new(true);
        else if (Fun.Method(this, Literals.Eq, new(arg), new(), out var result) && result is Bool value)
            return value;
        if (Super != null)
            return Super.Eq(arg, field);
        throw new TypeError("Types that are not comargsble to each other.");
    }

    public virtual Bool Ueq(Obj arg, Field field) => new(!Eq(arg, Field.Null).Value);

    public virtual Bool Lt(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.Lt, new(arg), new(), out var result) && result is Bool value)
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
        if (Fun.Method(this, Literals.Len, new(), new(), out var result) && result is Int value)
            return value;
        if (Super != null)
            return Super.Len();
        return new(1);
    }

    public virtual Int Hash() => new(GetHashCode());

    public virtual Str Type()
    {
        if (Fun.Method(this, Literals.Type, new(), new(), out var result) && result is Str value)
            return value;
        if (Super != null)
            return Super.Type();
        return new(ClassName);
    }


    public virtual Str CStr()
    {
        if (Fun.Method(this, Literals.CStr, Collections.Tuple.Empty, new(), out var result) && result is Str value)
            return value;
        if (Super != null)
            return Super.CStr();
        return new(ClassName);
    }

    public virtual Bool CBool()
    {
        if (Fun.Method(this, Literals.CBool, [], new(), out var result) && result is Bool value)
            return value;
        if (Super != null)
            return Super.CBool();
        throw new TypeError("This type cannot cast bool.");
    }

    public virtual Float CFloat()
    {
        if (Fun.Method(this, Literals.CFloat, [], new(), out var result) && result is Float value)
            return value;
        if (Super != null)
            return Super.CFloat();
        throw new TypeError("This type cannot cast float.");
    }

    public virtual Int CInt()
    {
        if (Fun.Method(this, Literals.CInt, [], new(), out var result) && result is Int value)
            return value;
        if (Super != null)
            return Super.CInt();
        throw new TypeError("This type cannot cast int.");
    }

    public virtual List CList()
    {
        if (Fun.Method(this, Literals.CList, [], new(), out var result) && result is List value)
            return value;
        if (Super != null)
            return Super.CList();
        throw new TypeError("This type cannot cast list.");
    }

    public virtual Obj Clone() => new(ClassName, field);

    public virtual Obj Copy()
    {
        if (Fun.Method(this, Literals.Copy, [], new(), out var value))
            return value;
        if (Super != null)
            return Super.Copy();
        return this;
    }


    public virtual Obj GetItem(Obj arg, Field field)
    {
        if (Fun.Method(this, Literals.GetItem, new(arg), new(), out var value))
            return value;
        if (Super != null)
            return Super.GetItem(arg, field);
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj SetItem(Obj arg, Obj index, Field field)
    {
        if (Fun.Method(this, Literals.SetItem, new(arg, index), new(), out var value))
            return value;
        if (Super != null)
            return Super.SetItem(arg, index, field);
        throw new IndexError("It is not Indexable type");
    }

    public virtual Obj Slice(Obj a, Obj b, Field field)
    {
        if (!a.As<Int>(out var start) || 
            !b.As<Int>(out var end))
            throw new SyntaxError();

        long len = Len().Value;
        long s = start.Value < 0 ? len + start.Value + 1 : start.Value;
        long e = end.Value < 0 ? len + end.Value + 1 : end.Value;

        if (s > e)
            throw new SyntaxError();

        List sliced = [];

        while (s < e)
        {
            sliced.Append(GetItem(new Int(s), Field.Null));
            s++;
        }

        return sliced;
    }


    public virtual Obj Entry()
    {
        if (Fun.Method(this, Literals.Entry, [], new(), out var value))
            return value;
        if (Super != null)
            return Super.Entry();
        throw new FileError("This type is with undefined entry functions");
    }

    public virtual Obj Exit()
    {
        if (Fun.Method(this, Literals.Exit, [], new(), out var value))
            return value;
        if (Super != null)
            return Super.Exit();
        throw new FileError("This type is with undefined exit functions");
    }

    public bool HasProperty(StringBuffer sb) => HasProperty(sb.ToString());

    public bool HasProperty(string key, bool isOriginal = false)
    {
        if (field.Key(key))
            return true;
        if (!isOriginal && Process.TryGetClass(ClassName, out var original))
            return original.HasProperty(key, true);
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

    public bool As<T>(out T value)
        where T : Obj
    {
        if (this is T t)
        {
            value = t;
            return true;
        }
        value = null!;
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

    public static Obj Parse(StringBuffer sb, Field field) => Parse(sb.ToString(), field);

    public static Obj Parse(string str, Field field)
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

    public static bool IsNone(Obj obj) => obj.Type().Value == Literals.None;

    public static bool IsNone(string str) => str == Literals.None;    
}
