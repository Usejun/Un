namespace Un.Data;

public class Int : Val<long>
{
    public Int() : base("int", 0) { }

    public Int(long value) : base("int", value) { }

    public override Obj Init(Iter args)
    {
        if (args.Count == 0)
            value = 0;
        else if (args.Count == 1)
            value = args[0].CInt().value;
        else 
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Int i) return new Int(value + i.value);
        if (arg is Float f) return new Float(value + f.value);
        if (arg is Str) return CStr().Add(arg);

        return base.Add(arg);
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Int i) return new Int(value - i.value);
        if (arg is Float f) return new Float(value - f.value);

        return base.Sub(arg);
    }

    public override Obj Mul(Obj arg)
    {
        if (arg is Int i) return new Int(value * i.value);
        if (arg is Float f) return new Float(value * f.value);
        if (arg is Matrix m) return m.Mul(this);

        return base.Mul(arg);
    }

    public override Obj Div(Obj arg)
    {
        if (arg is Int i) return new Float((double)value / i.value);
        if (arg is Float f) return new Float(value / f.value);

        return base.Div(arg);
    }

    public override Obj IDiv(Obj arg)
    {
        if (arg is Int i) return new Int(value / i.value);
        if (arg is Float f) return new Int(value / (long)f.value);

        return base.IDiv(arg);
    }

    public override Obj Mod(Obj arg)
    {
        if (arg is Int i) return new Float((double)value % i.value);
        if (arg is Float f) return new Float(value % f.value);

        return base.Mod(arg);
    }

    public override Obj Pow(Obj arg)
    {
        if (arg is Int i)
        {
            if (i.value < 0) return new Float(Math.Pow(value, i.value));
            return new Int((long)Math.Pow(value, i.value));
        }
        if (arg is Float f) return new Float(Math.Pow(value, f.value));

        return base.Pow(arg);
    }

    public override Obj BAnd(Obj arg)
    {
        if (arg is Int i) return new Int(value & i.value);

        return base.BAnd(arg);
    }

    public override Obj BOr(Obj arg)
    {
        if (arg is Int i) return new Int(value | i.value);

        return base.BOr(arg);
    }

    public override Obj BXor(Obj arg)
    {
        if (arg is Int i) return new Int(value ^ i.value);

        return base.BXor(arg);
    }

    public override Obj BNot() => new Int(~value);

    public override Obj LSh(Obj arg)
    {
        if (arg is Int i) return new Int(value << (int)i.value);

        return base.LSh(arg);
    }

    public override Obj RSh(Obj arg)
    {
        if (arg is Int i) return new Int(value >> (int)i.value);

        return base.RSh(arg);
    }

    public override Obj Xor(Obj arg) => new Bool(CBool().value ^ arg.CBool().value);

    public override Bool Equals(Obj arg)
    {
        if (arg is Float f) return new(value == f.value);
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Float f) return new(value < f.value);
        return base.LessThen(arg);
    }

    public override Int CInt() => new(value);

    public override Float CFloat() => new(value);

    public override Bool CBool()
    {
        if (value == 0) return new(false);
        return new(true);
    }

    public override Obj Clone() => new Int(value);

    public override Obj Copy() => new Int(value);
}
