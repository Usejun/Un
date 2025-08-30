using Un.Object.Primitive;
using Un.Object.Collections;
using System.Collections;

namespace Un.Object.Iter;

public class Counter : Iters
{
    public Counter() : base(Default())
    {
        Type = "counter";
    }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Counter(),
        _ => new Err($"invaild '{Type}' initialize"),
    };

    public override Obj Len() => new Int(long.MaxValue);

    public override Obj Iter() => this;

    public override List ToList() => throw new Panic("counter is infinite");

    public override Tup ToTuple() => throw new Panic("counter is infinite");

    public override Str ToStr() => throw new Panic("counter is infinite");

    public override Spreads Spread() => throw new Panic("counter is infinite");

    public override Obj Clone() => new Counter()
    {
        Annotations = Annotations
    };

    protected static IEnumerable<Obj> Default()
    {
        long i = 0;
        while (true)
            yield return new Int(i++);
    }
}