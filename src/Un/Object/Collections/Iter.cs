using Un.Object.Primitive;

namespace Un.Object.Collections;

public class Iters(IEnumerable<Obj> value) : Ref<IEnumerable<Obj>>(value, "iter")
{
    public Iters() : this([]) {}

    public override Obj Init(Tup args) => args switch
    {
        { Count: 1 } => args[0].Iter(),
        _ => new Err($"invaild '{Type}' initialize"),
    };

    public IEnumerator<Obj> Enumerator { get; private set; } = null!;

    public override Obj Len() => new Int(value.Count());

    public override Obj Iter() => this;

    public override List ToList() => new([..Value]);

    public override Tup ToTuple() => new([..Value], new string[Value.Count()]);
    
    public override Str ToStr() => new(string.Join(", ", Value.Select(x => x.ToStr().As<Str>().Value)));

    public override Obj Next()
    {
        Enumerator ??= Value.GetEnumerator();

        if (Enumerator.MoveNext())
            return Enumerator.Current;
        return new Err("iteration stopped");
    }

    public override Spread Spread() => new([.. Value]);

    public override Obj Copy() => this;

    public override Obj Clone() => new Iters(Value);
}