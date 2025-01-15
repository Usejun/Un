using System.Numerics;

namespace Un.Data;

public class Long : Val<BigInteger>
{
    public Long() : base("long", 0) { }

    public Long(BigInteger Value) : base("long", Value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        if (args.Count == 0)
            Value = BigInteger.Zero;
        else if (args.Count == 1)
        {
            if (args[0] is Str s) Value = BigInteger.Parse(s.Value);
            else if (args[0] is Int i) Value = i.Value;
            else if (args[0] is Float f) Value = (long)f.Value;
            else throw new ClassError("initialize error");
        }
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value + i.Value);
        if (arg is Long l) return new Long(Value + l.Value);
        if (arg is Str) return CStr().Add(arg, field);

        return base.Add(arg, field);
    }

    public override Obj Sub(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value - i.Value);
        if (arg is Long l) return new Long(Value - l.Value);

        return base.Sub(arg, field);
    }

    public override Obj Mul(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value * i.Value);
        if (arg is Long l) return new Long(Value * l.Value);

        return base.Mul(arg, field);
    }

    public override Obj Div(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value / i.Value);
        if (arg is Long l) return new Long(Value / l.Value);

        return base.Div(arg, field);
    }

    public override Obj IDiv(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value / i.Value);
        if (arg is Long l) return new Long(Value / l.Value);

        return base.IDiv(arg, field);
    }

    public override Obj Mod(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value % i.Value);
        if (arg is Long l) return new Long(Value % l.Value);

        return base.Mod(arg, field);
    }

    public override Obj Pow(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(BigInteger.Pow(Value, (int)i.Value));
        if (arg is Long l) return new Long(BigInteger.Pow(Value, (int)l.Value));

        return base.Pow(arg, field);
    }

    public override Obj BAnd(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value & i.Value);
        if (arg is Long l) return new Long(Value & l.Value);

        return base.BAnd(arg, field);
    }

    public override Obj BOr(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value | i.Value);
        if (arg is Long l) return new Long(Value | l.Value);

        return base.BOr(arg, field);
    }

    public override Obj BXor(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value ^ i.Value);
        if (arg is Long l) return new Long(Value ^ l.Value);

        return base.BXor(arg, field);
    }

    public override Obj BNot() => new Long(~Value);

    public override Obj LSh(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value << (int)i.Value);
        if (arg is Long l) return new Long(Value << (int)l.Value);

        return base.LSh(arg, field);
    }

    public override Obj RSh(Obj arg, Field field)
    {
        if (arg is Int i) return new Long(Value >> (int)i.Value);
        if (arg is Long l) return new Long(Value >> (int)l.Value);

        return base.RSh(arg, field);
    }

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg is Int i) return new(Value == i.Value);
        if (arg is Float f) return new(Value == (long)f.Value);

        return base.Eq(arg, field);
    }

    public override Bool Lt(Obj arg, Field field)
    {
        if (arg is Int i) return new(Value < i.Value);
        if (arg is Float f) return new(Value < (long)f.Value);
        return base.Lt(arg, field);
    }

    public override Int CInt() => new((long)Value);

    public override Float CFloat() => new((long)Value);

    public override Bool CBool()
    {
        if (Value == 0) return new(false);
        return new(true);
    }

    public override Obj Clone() => new Long(Value);

    public override Obj Copy() => new Long(Value);
}
