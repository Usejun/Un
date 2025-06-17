using System.Numerics;
using Un.Object.Collections;

namespace Un.Object.Primitive;

public class Long(BigInteger value) : Val<BigInteger>(value, "long")
{
    public Long() : this(BigInteger.Zero) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Long(),
        { Count: 1 } => args[0] switch
        {
            Str s => BigInteger.TryParse(s.Value, out var result) ? new Long(result) : throw new Error($"cannot convert '{args[0].Type}' to '{Type}'"),
            Int i => new Long(i.Value),
            Float f => new Long((long)f.Value),
            Long l => new Long(l.Value),
            _ => throw new Error($"cannot convert '{args[0].Type}' to '{Type}'"),
        },
        _ => throw new Error($"cannot convert '{args[0].Type}' to '{Type}'"),
    };

    public override Obj Add(Obj other) => other switch
    {
        Int i => new Long(Value + i.Value),
        Float f => new Long(Value + (long)f.Value),
        Str s => new Str(Value.ToString() + s.Value),
        _ => throw new Error($"unsupported operand type(s) for +: 'long' and '{other.Type}'")
    };

    public override Obj Sub(Obj other) => other switch
    {
        Int i => new Long(Value - i.Value),
        Float f => new Long(Value - (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for -: 'long' and '{other.Type}'")
    };

    public override Obj Mul(Obj other) => other switch
    {
        Int i => new Long(Value * i.Value),
        Float f => new Long(Value * (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for *: 'long' and '{other.Type}'")
    };

    public override Obj Div(Obj other) => other switch
    {
        Int i => new Long(Value / i.Value),
        Float f => new Long(Value / (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for /: 'long' and '{other.Type}'")
    };

    public override Obj Mod(Obj other) => other switch
    {
        Int i => new Long(Value % i.Value),
        Float f => new Long(Value % (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for %: 'long' and '{other.Type}'")
    };

    public override Obj IDiv(Obj other) => other switch
    {
        Int i => new Long(Value / i.Value),
        Float f => new Long(Value / (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for //: 'long' and '{other.Type}'")
    };

    public override Obj Pow(Obj other) => other switch
    {
        Int i => new Long(BigInteger.Pow(Value, (int)i.Value)),
        Float f => new Long(BigInteger.Pow(Value, (int)f.Value)),
        _ => throw new Error($"unsupported operand type(s) for **: 'long' and '{other.Type}'")
    };

    public override Obj Neg() => new Long(-Value);

    public override Obj Pos() => new Long(+Value);

    public override Obj BAnd(Obj other) => other switch
    {
        Int i => new Long(Value & i.Value),
        _ => throw new Error($"unsupported operand type(s) for &: 'long' and '{other.Type}'")
    };

    public override Obj BOr(Obj other) => other switch
    {
        Int i => new Long(Value | i.Value),
        _ => throw new Error($"unsupported operand type(s) for |: 'long' and '{other.Type}'")
    };

    public override Obj BXor(Obj other) => other switch
    {
        Int i => new Long(Value ^ i.Value),
        _ => throw new Error($"unsupported operand type(s) for ^: 'long' and '{other.Type}'")
    };

    public override Obj BNot() => new Long(~Value);

    public override Obj LShift(Obj other) => other switch
    {
        Int i => new Long(Value << (int)i.Value),
        _ => throw new Error($"unsupported operand type(s) for <<: 'long' and '{other.Type}'")
    };

    public override Obj RShift(Obj other) => other switch
    {
        Int i => new Long(Value >> (int)i.Value),
        _ => throw new Error($"unsupported operand type(s) for >>: 'long' and '{other.Type}'")
    };

    public override Bool Eq(Obj other) => other switch
    {
        Int i => new Bool(Value == i.Value),
        Float f => new Bool(Value == (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for ==: 'long' and '{other.Type}'")
    };

    public override Bool Lt(Obj other) => other switch
    {
        Int i => new Bool(Value < i.Value),
        Float f => new Bool(Value < (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for <: 'long' and '{other.Type}'")
    };

    public override Bool Gt(Obj other) => other switch
    {
        Int i => new Bool(Value > i.Value),
        Float f => new Bool(Value > (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for >: 'long' and '{other.Type}'")
    };

    public override Bool LtOrEq(Obj other) => other switch
    {
        Int i => new Bool(Value <= i.Value),
        Float f => new Bool(Value <= (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for <=: 'long' and '{other.Type}'")
    };

    public override Bool GtOrEq(Obj other) => other switch
    {
        Int i => new Bool(Value >= i.Value),
        Float f => new Bool(Value >= (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for >=: 'long' and '{other.Type}'")
    };

    public override Bool NEq(Obj other) => other switch
    {
        Int i => new Bool(Value != i.Value),
        Float f => new Bool(Value != (long)f.Value),
        _ => throw new Error($"unsupported operand type(s) for !=: 'long' and '{other.Type}'")
    };

    public override Obj Len() => new Int(1);



    public override Int ToInt() => new((long)Value);

    public override Float ToFloat() => new((double)Value);

    public override Str ToStr() => new(Value.ToString());

    public override Bool ToBool() => new(Value != 0);

    public override Obj Copy() => new Long(Value);

    public override Obj Clone() => new Long(Value);
}