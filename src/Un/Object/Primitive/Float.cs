using Un.Object.Collections;

namespace Un.Object.Primitive;

public class Float(double value) : Val<double>(value, "float")
{
    public Float() : this(0) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Float(),
        { Count: 1 } => args[0].ToFloat(),
        _ => throw new Error($"cannot convert to '{Type}'"),
    };

    public override Obj Add(Obj other) => other switch
    {
        Int i => new Float(Value + i.Value),
        Float f => new Float(Value + f.Value),
        Str s => new Str(Value.ToString() + s.Value),
        _ => throw new Error($"unsupported operand type(s) for +: 'float' and '{other.Type}'")
    };

    public override Obj Sub(Obj other) => other switch
    {
        Int i => new Float(Value - i.Value),
        Float f => new Float(Value - f.Value),
        _ => throw new Error($"unsupported operand type(s) for -: 'float' and '{other.Type}'")
    };

    public override Obj Mul(Obj other) => other switch
    {
        Int i => new Float(Value * i.Value),
        Float f => new Float(Value * f.Value),
        _ => throw new Error($"unsupported operand type(s) for *: 'float' and '{other.Type}'")
    };

    public override Obj Div(Obj other) => other switch
    {
        Int i => i.Value == 0 ? throw new Error($"{Type} division by zero") : new Float(Value / i.Value),
        Float f => f.Value == 0 ? throw new Error($"{Type} division by zero") : new Float(Value / f.Value),
        _ => throw new Error($"unsupported operand type(s) for /: 'float' and '{other.Type}'")
    };

    public override Obj Mod(Obj other) => other switch
    {
        Int i => new Float(Value % i.Value),
        Float f => new Float(Value % f.Value),
        _ => throw new Error($"unsupported operand type(s) for %: 'float' and '{other.Type}'")
    };

    public override Obj IDiv(Obj other) => other switch
    {
        Int i => i.Value == 0 ? throw new Error($"{Type} division by zero") : new Int((long)Value / i.Value),
        Float f => f.Value == 0 ? throw new Error($"{Type} division by zero") : new Int((long)(Value / f.Value)),
        _ => throw new Error($"unsupported operand type(s) for //: 'float' and '{other.Type}'")
    };

    public override Obj Pow(Obj other) => other switch
    {
        Int i => new Float(Math.Pow(Value, i.Value)),
        Float f => new Float(Math.Pow(Value, f.Value)),
        _ => throw new Error($"unsupported operand type(s) for **: 'float' and '{other.Type}'")
    };

    public override Obj Neg() => new Float(-Value);

    public override Obj Pos() => new Float(+Value);

    public override Bool Eq(Obj other) => other switch
    {
        Int i => new Bool(Value == i.Value),
        Float f => new Bool(Value == f.Value),
        _ => throw new Error($"unsupported operand type(s) for ==: 'float' and '{other.Type}'")
    };

    public override Bool NEq(Obj other) => other switch
    {
        Int i => new Bool(Value != i.Value),
        Float f => new Bool(Value != f.Value),
        _ => throw new Error($"unsupported operand type(s) for !=: 'float' and '{other.Type}'")
    };

    public override Bool Lt(Obj other) => other switch
    {
        Int i => new Bool(Value < i.Value),
        Float f => new Bool(Value < f.Value),
        _ => throw new Error($"unsupported operand type(s) for <: 'float' and '{other.Type}'")
    };

    public override Bool Gt(Obj other) => other switch
    {
        Int i => new Bool(Value > i.Value),
        Float f => new Bool(Value > f.Value),
        _ => throw new Error($"unsupported operand type(s) for >: 'float' and '{other.Type}'")
    };

    public override Bool LtOrEq(Obj other) => other switch
    {
        Int i => new Bool(Value <= i.Value),
        Float f => new Bool(Value <= f.Value),
        _ => throw new Error($"unsupported operand type(s) for <=: 'float' and '{other.Type}'")
    };

    public override Bool GtOrEq(Obj other) => other switch
    {
        Int i => new Bool(Value >= i.Value),
        Float f => new Bool(Value >= f.Value),
        _ => throw new Error($"unsupported operand type(s) for >=: 'float' and '{other.Type}'")
    };

    public override Int ToInt() => new((long)Value);
    public override Float ToFloat() => new(Value);
    public override Str ToStr() => new(Value.ToString());
    public override Bool ToBool() => new(Value != 0);

    public override Obj Copy() => new Float(Value);
    public override Obj Clone() => new Float(Value);
}