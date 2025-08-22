using Un.Object.Collections;

namespace Un.Object.Primitive;

public class Bool(bool value) : Val<bool>(value, "bool")
{
    public Bool() : this(false) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Bool(),
        { Count: 1 } => args[0].ToBool(),
        _ => new Bool(false)
    };

    public override Obj Not() => Value ? new Bool(false) : new(true);

    public override Obj Xor(Obj other) => Value ? other.Not() : this;

    public override Bool Eq(Obj other) => other is Bool b && Value == b.Value ? new Bool(true) : new(false);

    public override Str ToStr() => new(Value ? "true" : "false");

    public override Obj Copy() => new Bool(Value)
    {
        Annotations = Annotations
    };

    public override Obj Clone() => new Bool(Value)
    {
        Annotations = Annotations
    };

    public override Bool ToBool() => new(Value);
}