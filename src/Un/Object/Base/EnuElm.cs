using Un.Object.Primitive;

namespace Un.Object;

public class EnuElm(string type, int n) : Obj(type)
{
    public int N => n;

    public override Str ToStr() => new(Type);

    public override Int ToInt() => new(N);

    public override Obj Eq(Obj other) => other switch
    {
        Int i => new Bool(N == i.Value),
        EnuElm e => Type == other.Type ? new Bool(N == e.N) : base.Eq(e),
        _ => new Err($"unsupported operand type(s) for ==: '{Type}' and '{other.Type}'")
    };

    public override Obj Lt(Obj other) => other switch
    {
        Int i => new Bool(N < i.Value),
        EnuElm e => Type == other.Type ? new Bool(N < e.N) : base.Eq(e),
        _ => new Err($"unsupported operand type(s) for <: '{Type}' and '{other.Type}'")
    };
}
