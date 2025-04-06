using System.Collections;

namespace Un.Collections;

public class Tuple : Ref<Obj[]>, IEnumerable<Obj>
{
    public static Tuple Empty => new();

    public string[] Names { get; protected set; }

    public struct Enumerator : IEnumerator<Obj>
    {
        private readonly Obj[] value;
        private int index;

        public readonly Obj Current => value[index];

        readonly object IEnumerator.Current => value[index];

        internal Enumerator(Tuple tuple)
        {
            value = tuple.Value;
            index = -1;
        }

        public bool MoveNext()
        {
            index++;
            return index < value.Length;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {

        }
    }

    public int Count { get; init; }

    public Tuple() : base("tuple", [])
    {
        Names = [];
        Count = 0;
    }

    public Tuple(int capacity) : base("tuple", new Obj[capacity])
    {
        Names = new string[capacity];
        Count = 0;
    }

    public Tuple(string str, Field field) : base("tuple", [])
    {
        str = str[1..^1];

        List data = [];
        List<string> names = [];

        int index = 0;
        int depth = 0;
        StringBuffer buffer = new();
        string name = "";
        string value;
        var isString = false;

        while (index < str.Length)
        {
            if (Token.IsString(str[index])) isString = !isString;
            else if (Up()) ++depth;
            else if (Down()) --depth;
            else if (IsName())
            {                
                name = buffer.Split(Literals.Colon, 1)[0].ToString();
                buffer.Clear();
                index++;
                continue;
            }

            if (IsNext())
            {
                value = buffer.ToString();

                if (List.IsList(value)) 
                    data.Append(new List(new Tuple(value, field)));
                else if (IsTuple(value)) 
                    data.Append(new Tuple(value, field));
                else 
                    data.Append(Calculator.All(value, field));
                
                names.Add(name);
                name = "";
                buffer.Clear();
            }
            else buffer.Append(str[index]);

            index++;
        }

        value = buffer.ToString(); 

        names.Add(name);

        if (!string.IsNullOrWhiteSpace(value))
            data.Append(Calculator.All(value, field));

        for (int i = 0; i < Count; i++)
            if (!string.IsNullOrEmpty(names[i]))
                field.Set(names[i], data[i]);

        Value = [.. data];
        Names = [.. names];
        Count = data.Count;

        bool Up() => str[index] == Literals.CLBrack || str[index] == Literals.CLParen || str[index] == Literals.CLBrace;

        bool Down() => str[index] == Literals.CRBrack || str[index] == Literals.CRParen || str[index] == Literals.CRBrace;

        bool IsNext() => str[index] == Literals.CComma && depth == 0 && !isString;

        bool IsName() => !isString && depth == 0 && str[index] == Literals.CAssign;

    }

    public Tuple(string[] names, Obj[] values) : base("tuple", values)
    {
        Names = names;
        
        for (int i = 0; i < Count; i++)
            if (!string.IsNullOrEmpty(names[i]))
                field.Set(names[i], values[i]);

        Count = names.Length;
    }

    public Tuple(params Obj[] values) : base("tuple", new Obj[values.Length])
    {
        for (int i = 0; i < values.Length; i++)
            Value[i] = values[i].Clone();

        Names = new string[values.Length];
        Count = values.Length;
    }

    public Obj this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError();
            return Value[index];
        }
    }

    public override void Set(string str, Obj value) => throw new ValueError("tuple is immutable type");

    public override Obj GetItem(Obj arg, Field field)
    {
        if (arg.As<Int>(out var i)) 
            throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Count + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) throw new IndexError("out of range");

        return Value[index];
    }

    public override Obj SetItem(Obj arg, Obj index, Field field) => throw new ValueError("tuple is immutable type");


    public override List CList() => new(Value);

    public override Str CStr() => new($"({string.Join(", ", Value.Take(Count).Select(o => o.CStr().Value))})");

    public override Bool CBool() => new(Count != 0);


    public override Int Len() => new(Count);

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg.As<Tuple>(out var tuple))
        {
            if (tuple.Count != Count) return Bool.False;

            for (int i = 0; i < Count; i++)
                if (tuple[i].Ueq(this[i], field).Value)
                    return Bool.False;
                        
            return Bool.True;
        }
        return base.Eq(arg, field);
    }

    public override Bool Lt(Obj arg, Field field)
    {
        if (arg.As<Tuple>(out var tuple))
        {
            if (tuple.Count != Count) return Bool.False;

            for (int i = 0; i < Count; i++)
                if (tuple[i].Lt(this[i], field).Value)
                    return Bool.False;

            return Bool.True;
        }
        return base.Lt(arg, field);
    }


    public override Obj Clone() => new Tuple()
    {
        Value = Value[..],
        Count = Count,
    };

    public override int GetHashCode()
    {
        int hash = 123456789;

        foreach (var item in this)
        {
            hash <<= 5;
            hash |= item.GetHashCode();
            hash += item.GetHashCode();
            hash = Math.Abs(hash);
        }

        return hash;
    }


    public bool IsArgument()
    {
        int start = 0, end = 0, len = Names.Length;

        while (start < len && string.IsNullOrEmpty(Names[start]))
            ++start;

        while (end >= 0 && Names.Length != 0 && string.IsNullOrEmpty(Names[end]))
            --end;

        for (int i = start + 1; i < end; i++)
            if (string.IsNullOrEmpty(Names[i]))        
                return false;
        return true;
    }

    public int Positional(int start = 0)
    {
        int count = 0;
        for (int i = start; i < Count; i++)
            if (string.IsNullOrEmpty(Names[i]))
                ++count;
        return count;
    }

    public int Named(int start = 0)
    {
        int count = 0;
        for (int i = start; i < Count; i++)
            if (!string.IsNullOrEmpty(Names[i]))
                ++count;
        return count;
    }

    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    private bool OutOfRange(int index) => index < 0 || index >= Count;


    public static bool IsTuple(string str) => str[0] == Literals.CLParen && str[^1] == Literals.CRParen;

    public static Obj Split(Obj obj) => obj.As<Tuple>(out var tuple) && tuple.Count == 1 ? tuple[0] : obj;
}
