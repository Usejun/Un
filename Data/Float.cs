namespace Un.Data;

public class Float : Val<double>
{
    public Float() : base("float", 0) { }

    public Float(double Value) : base("float", Value) { }

    public override Obj Init(List args)
    {
        if (args.Count == 0)
            Value = 0;
        else if (args.Count == 1)
            Value = args[0].CFloat().Value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Int i) return new Float(Value + i.Value);
        if (arg is Float f) return new Float(Value + f.Value);
        if (arg is Str) return CStr().Add(arg);

        return base.Add(arg);
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Int i) return new Float(Value - i.Value);
        if (arg is Float f) return new Float(Value * f.Value);

        return base.Sub(arg);
    }

    public override Obj Mul(Obj arg)
    {
        if (arg is Int i) return new Float(Value * i.Value);
        if (arg is Float f) return new Float(Value * f.Value);
        if (arg is Matrix m) return m.Mul(this);

        return base.Mul(arg);
    }

    public override Obj Div(Obj arg)
    {
        if (arg is Int i) return new Float(Value / i.Value);
        if (arg is Float f) return new Float(Value / f.Value);

        return base.Div(arg);
    }

    public override Obj IDiv(Obj arg)
    {
        if (arg is Int i) return new Int((long)Value / i.Value);
        if (arg is Float f) return new Int((long)Value / (long)f.Value);

        return base.IDiv(arg);
    }

    public override Obj Mod(Obj arg)
    {
        if (arg is Int i) return new Float(Value % i.Value);
        if (arg is Float f) return new Float(Value % f.Value);

        return base.Mod(arg);
    }

    public override Obj Pow(Obj arg)
    {
        if (arg is Int i) return new Float(Math.Pow(Value, i.Value));
        if (arg is Float f) return new Float(Math.Pow(Value, f.Value));

        return base.Pow(arg);
    }

    public override Obj Xor(Obj arg) => new Bool(CBool().Value ^ arg.CBool().Value);

    public override Bool Equals(Obj arg)
    {
        if (arg is Int i) return new(Value == i.Value);
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Int i) return new(Value < i.Value);
        return base.LessThen(arg);
    }

    public override Int CInt() => new((long)Value);

    public override Float CFloat() => new(Value);

    public override Bool CBool()
    {
        if (Value == 0)
            return new(false);
        return new(true);
    }

    public override Obj Clone() => new Float(Value);

    public override Obj Copy() => new Float(Value);
}
