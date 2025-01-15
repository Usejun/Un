using System.Collections;
using Un.Data;

namespace Un.Collections;

public class Tuple : Ref<Obj[]>, IEnumerable<Obj>
{
    public static Tuple Empty => [];

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
        Count = 0;
    }

    public Tuple(string str, Field field) : base("tuple", [])
    {
        str = str[1..^1];

        List data = [];

        int index = 0, depth = 0;
        string buffer = "", name = "";
        var isString = false;

        while (index < str.Length)
        {
            if (Token.IsString(str[index])) isString = !isString;
            else if (str[index] == '[' || str[index] == '(' || str[index] == '{') ++depth;
            else if (str[index] == ']' || str[index] == ')' || str[index] == '}') --depth;
            else if (!isString && depth == 0 && str[index] == '=' && (str[index + 1] != '>' && (str[index + 1] != '=' && str[index - 1] != '=')))
            {                
                name = buffer.Split(':')[0];
                buffer = "";
                index++;
                continue;
            }

            if (str[index] == ',' && depth == 0 && !isString)
            {
                if (List.IsList(buffer)) data.Append(new List(new Tuple(buffer, field)));
                else if (IsTuple(buffer)) data.Append(new Tuple(buffer, field));
                else data.Append(Calculator.All(buffer, field));

                if (!string.IsNullOrEmpty(name))
                    this.field.Set(name, data[^1]);

                name = "";
                buffer = "";
            }
            else buffer += str[index];

            index++;
        }

        if (!string.IsNullOrWhiteSpace(buffer))
            data.Append(Calculator.All(buffer, field));

        if (!string.IsNullOrEmpty(name))
            this.field.Set(name, data[^1]);

        Value = new Obj[data.Count];
        Count = data.Count;

        for (int i = 0; i < data.Count; i++)
            Value[i] = data.Value[i];
    }

    public Tuple(params Obj[] values) : base("tuple", values)
    {
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

    public override Obj Get(string str)
    {
        if (field.Get(str, out var property))
            return property;
        if (Super != null)
            return Super.Get(str);
        throw new TypeError("A property that doesn't exist.");
    }

    public override void Set(string str, Obj value) => throw new ValueError("tuple is immutable type");

    public override Obj GetItem(Tuple args, Field field)
    {
        if (args[0] is not Int i) throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Count + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) throw new IndexError("out of range");

        return Value[index];
    }

    public override Obj SetItem(Tuple args, Field field) => throw new ValueError("tuple is immutable type");


    public override List CList() => new([.. Value]);

    public override Str CStr() => new($"({string.Join(", ", Value.Take(Count).Select(o => o.CStr().Value))})");

    public override Bool CBool() => new(Count != 0);


    public override Int Len() => new(Count);

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg is Tuple tuple)
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
        if (arg is Tuple tuple)
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

    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    private bool OutOfRange(int index) => index < 0 || index >= Count;


    public static bool IsTuple(string str) => str[0] == '(' && str[^1] == ')';

    public static Obj Split(Obj obj) => obj is Tuple tuple && tuple.Count == 1 ? tuple[0] : obj;
}
