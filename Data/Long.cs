using System.Numerics;

namespace Un.Data;

public class Long : Val<BigInteger>
{
    public Long() : base("long", 0) { }

    public Long(BigInteger value) : base("long", value) { }

    public override Obj Init(Iter args)
    {
        if (args.Count == 0)
            value = BigInteger.Zero;
        else if (args.Count == 1)
        {
            if (args[0] is Str s) value = BigInteger.Parse(s.value);
            else if (args[0] is Int i) value = i.value;
            else if (args[0] is Float f) value = (long)f.value;
            else throw new ClassError("initialize error");
        }
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Int i) return new Long(value + i.value);
        if (arg is Long l) return new Long(value + l.value);
        if (arg is Str) return CStr().Add(arg);

        return base.Add(arg);
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Int i) return new Long(value - i.value);
        if (arg is Long l) return new Long(value - l.value);

        return base.Sub(arg);
    }

    public override Obj Mul(Obj arg)
    {
        if (arg is Int i) return new Long(value * i.value);
        if (arg is Long l) return new Long(value * l.value);

        return base.Mul(arg);
    }

    public override Obj Div(Obj arg)
    {
        if (arg is Int i) return new Long(value / i.value);
        if (arg is Long l) return new Long(value / l.value);

        return base.Div(arg);
    }

    public override Obj IDiv(Obj arg)
    {
        if (arg is Int i) return new Long(value / i.value);
        if (arg is Long l) return new Long(value / l.value);

        return base.IDiv(arg);
    }

    public override Obj Mod(Obj arg)
    {
        if (arg is Int i) return new Long(value % i.value);
        if (arg is Long l) return new Long(value % l.value);

        return base.Mod(arg);
    }

    public override Obj Pow(Obj arg)
    {
        if (arg is Int i) return new Long(BigInteger.Pow(value, (int)i.value));
        if (arg is Long l) return new Long(BigInteger.Pow(value, (int)l.value));

        return base.Pow(arg);
    }

    public override Obj BAnd(Obj arg)
    {
        if (arg is Int i) return new Long(value & i.value);
        if (arg is Long l) return new Long(value & l.value);

        return base.BAnd(arg);
    }

    public override Obj BOr(Obj arg)
    {
        if (arg is Int i) return new Long(value | i.value);
        if (arg is Long l) return new Long(value | l.value);

        return base.BOr(arg);
    }

    public override Obj BXor(Obj arg)
    {
        if (arg is Int i) return new Long(value ^ i.value);
        if (arg is Long l) return new Long(value ^ l.value);

        return base.BXor(arg);
    }

    public override Obj BNot() => new Long(~value);

    public override Obj LSh(Obj arg)
    {
        if (arg is Int i) return new Long(value << (int)i.value);
        if (arg is Long l) return new Long(value << (int)l.value);

        return base.LSh(arg);
    }

    public override Obj RSh(Obj arg)
    {
        if (arg is Int i) return new Long(value >> (int)i.value);
        if (arg is Long l) return new Long(value >> (int)l.value);

        return base.RSh(arg);
    }

    public override Obj Xor(Obj arg) => new Bool(CBool().value || arg.CBool().value);

    public override Bool Equals(Obj arg)
    {
        if (arg is Int i) return new(value == i.value);
        if (arg is Float f) return new(value == (long)f.value);

        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Int i) return new(value < i.value);
        if (arg is Float f) return new(value < (long)f.value);
        return base.LessThen(arg);
    }

    public override Int CInt() => new((long)value);

    public override Float CFloat() => new((long)value);

    public override Bool CBool()
    {
        if (value == 0) return new(false);
        return new(true);
    }

    public override Obj Clone() => new Long(value);

    public override Obj Copy() => new Long(value);
}
