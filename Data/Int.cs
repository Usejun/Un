namespace Un.Data;

public class Int : Val<long>
{
    public Int() : base("int", 0) { }

    public Int(long value) : base("int", value) { }

    public override Obj Init(Collections.Tuple args)
    {
        if (args.Count == 0)
            Value = 0;
        else if (args.Count == 1)
            Value = args[0].CInt().Value;
        else 
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Int i) return new Int(Value + i.Value);
        if (arg is Float f) return new Float(Value + f.Value);
        if (arg is Str) return CStr().Add(arg);

        return base.Add(arg);
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Int i) return new Int(Value - i.Value);
        if (arg is Float f) return new Float(Value - f.Value);

        return base.Sub(arg);
    }

    public override Obj Mul(Obj arg)
    {
        if (arg is Int i) return new Int(Value * i.Value);
        if (arg is Float f) return new Float(Value * f.Value);
        if (arg is Matrix m) return m.Mul(this);

        return base.Mul(arg);
    }

    public override Obj Div(Obj arg)
    {
        if (arg is Int i) return new Float((double)Value / i.Value);
        if (arg is Float f) return new Float(Value / f.Value);

        return base.Div(arg);
    }

    public override Obj IDiv(Obj arg)
    {
        if (arg is Int i) return new Int(Value / i.Value);
        if (arg is Float f) return new Int(Value / (long)f.Value);

        return base.IDiv(arg);
    }

    public override Obj Mod(Obj arg)
    {
        if (arg is Int i) return new Float((double)Value % i.Value);
        if (arg is Float f) return new Float(Value % f.Value);

        return base.Mod(arg);
    }

    public override Obj Pow(Obj arg)
    {
        if (arg is Int i)
        {
            if (i.Value < 0) return new Float(Math.Pow(Value, i.Value));
            return new Int((long)Math.Pow(Value, i.Value));
        }
        if (arg is Float f) return new Float(Math.Pow(Value, f.Value));

        return base.Pow(arg);
    }

    public override Obj BAnd(Obj arg)
    {
        if (arg is Int i) return new Int(Value & i.Value);

        return base.BAnd(arg);
    }

    public override Obj BOr(Obj arg)
    {
        if (arg is Int i) return new Int(Value | i.Value);

        return base.BOr(arg);
    }

    public override Obj BXor(Obj arg)
    {
        if (arg is Int i) return new Int(Value ^ i.Value);

        return base.BXor(arg);
    }

    public override Obj BNot() => new Int(~Value);

    public override Obj LSh(Obj arg)
    {
        if (arg is Int i) return new Int(Value << (int)i.Value);

        return base.LSh(arg);
    }

    public override Obj RSh(Obj arg)
    {
        if (arg is Int i) return new Int(Value >> (int)i.Value);

        return base.RSh(arg);
    }

    public override Obj Xor(Obj arg) => new Bool(CBool().Value ^ arg.CBool().Value);

    public override Bool Equals(Obj arg)
    {
        if (arg is Float f) return new(Value == f.Value);
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Float f) return new(Value < f.Value);
        return base.LessThen(arg);
    }

    public override Int CInt() => new(Value);

    public override Float CFloat() => new(Value);

    public override Bool CBool()
    {
        if (Value == 0) return new(false);
        return new(true);
    }

    public override Obj Clone() => new Int(Value);

    public override Obj Copy() => new Int(Value);
}
