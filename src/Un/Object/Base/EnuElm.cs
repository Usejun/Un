using Un.Object.Primitive;

namespace Un.Object;

public class EnuElm(string type, int n) : Obj(type)
{
    public int N => n;

    public override Str ToStr() => new(Type);

    public override Int ToInt() => new(N);

    public override Bool Eq(Obj other) => other switch
    {
        Int i => new(N == i.Value),
        EnuElm e => Type == other.Type ? new(N == e.N) : base.Eq(e),
        _ => throw new Error($"unsupported operand type(s) for ==: '{Type}' and '{other.Type}'")
    };

    public override Bool Lt(Obj other) => other switch
    {
        Int i => new(N < i.Value),
        EnuElm e => Type == other.Type ? new(N < e.N) : base.Eq(e),
        _ => throw new Error($"unsupported operand type(s) for <: '{Type}' and '{other.Type}'")
    };
}
