using System.Numerics;

namespace Un.Data;

public class Long : Val<BigInteger>
{
    public Long() : base("long", 0) { }

    public Long(BigInteger Value) : base("long", Value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", null!)], 1);

        var value = field["value"];

        Value = value switch
        {
            Str s => Value = BigInteger.Parse(s.Value),
            Int i => Value = i.Value,
            Float f => Value = (long)f.Value,
            Long l => Value = l.Value,
            _ => throw new ClassError()
        };

        return this;
    }

    public override Obj Add(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value + i.Value),
        Long l => new Long(Value + l.Value),
        Float f => new Long(Value + (long)f.Value),
        Str s => new Str(Value + s.Value),
        _ => base.Add(arg, field),
    };

    public override Obj Sub(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value - i.Value),
        Long l => new Long(Value - l.Value),
        Float f => new Long(Value - (long)f.Value),
        _ => base.Sub(arg, field),
    };

    public override Obj Mul(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value * i.Value),
        Long l => new Long(Value * l.Value),
        Float f => new Long(Value * (long)f.Value),
        _ => base.Mul(arg, field),
    };

    public override Obj Div(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value / i.Value),
        Long l => new Long(Value / l.Value),
        Float f => new Long(Value / (long)f.Value),
        _ => base.Div(arg, field),
    };

    public override Obj IDiv(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value / i.Value),
        Long l => new Long(Value / l.Value),
        Float f => new Long(Value / (long)f.Value),
        _ => base.IDiv(arg, field),
    };

    public override Obj Mod(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value & i.Value),
        Long l => new Long(Value & l.Value),
        Float f => new Long(Value & (long)f.Value),
        _ => base.Mod(arg, field),
    };

    public override Obj Pow(Obj arg, Field field) => arg switch
    {
        Int i => new Long(BigInteger.Pow(Value, (int)i.Value)),
        Long l => new Long(BigInteger.Pow(Value, (int)l.Value)),
        Float f => new Long(BigInteger.Pow(Value, (int)f.Value)),
        _ => base.Pow(arg, field),
    };

    public override Obj BAnd(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value & i.Value),
        Long l => new Long(Value & l.Value),
        Float f => new Long(Value & (long)f.Value),
        _ => base.BAnd(arg, field),
    };

    public override Obj BOr(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value | i.Value),
        Long l => new Long(Value | l.Value),
        Float f => new Long(Value | (long)f.Value),
        _ => base.BOr(arg, field),
    };

    public override Obj BXor(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value ^ i.Value),
        Long l => new Long(Value ^ l.Value),
        Float f => new Long(Value ^ (long)f.Value),
        _ => base.BXor(arg, field),
    };

    public override Obj BNot() => new Long(~Value);

    public override Obj LSh(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value << (int)i.Value),
        Long l => new Long(Value << (int)l.Value),
        Float f => new Long(Value << (int)f.Value),
        _ => base.LSh(arg, field),
    };

    public override Obj RSh(Obj arg, Field field) => arg switch
    {
        Int i => new Long(Value >> (int)i.Value),
        Long l => new Long(Value >> (int)l.Value),
        Float f => new Long(Value >> (int)f.Value),
        _ => base.RSh(arg, field),
    };

    public override Bool Eq(Obj arg, Field field) => arg switch
    {
        Int i => new Bool(Value == i.Value),
        Long l => new Bool(Value == l.Value),
        Float f => new Bool(Value == (long)f.Value),
        _ => base.Eq(arg, field),
    };


    public override Bool Lt(Obj arg, Field field) => arg switch
    {
        Int i => new Bool(Value < i.Value),
        Long l => new Bool(Value < l.Value),
        Float f => new Bool(Value < (long)f.Value),
        _ => base.Eq(arg, field),
    };

    public override Int CInt() => new((long)Value);

    public override Float CFloat() => new((double)Value);

    public override Bool CBool()
    {
        if (Value == 0) return new(false);
        return new(true);
    }

    public override Obj Clone() => new Long(Value);

    public override Obj Copy() => new Long(Value);
}
