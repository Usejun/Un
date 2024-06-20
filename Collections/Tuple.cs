using System.Collections;

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
        Count = -1;
    }

    public Tuple(Map map) : base("tuple", map.Value)
    {
        Count = map.Value.Length;
    }

    public Tuple(params Obj[] value) : base("tuple", value)
    {
        Count = value.Length;
    }

    public Obj this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError();
            return Value[index];
        }   
    }

    public override List CList() => new(Value);   

    public override Int Len() => new(Count);

    public override Str CStr() => new($"({string.Join(", ", Value.Take(Count).Select(o => o.CStr().Value))})");

    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    private bool OutOfRange(int index) => index < 0 || index >= Count;

    public static bool IsTuple(string str) => str[0] == '(' && str[^1] == ')';
}
