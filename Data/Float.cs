namespace Un.Data;

public class Float : Val<double>
{
    public Float() : base("float", 0) { }

    public Float(double value) : base("float", value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        if (args.Count == 0)
            Value = 0;
        else if (args.Count == 1)
            Value = args[0].CFloat().Value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg, Field field)
    {
        if (arg is Int i) return new Float(Value + i.Value);
        if (arg is Float f) return new Float(Value + f.Value);
        if (arg is Str) return CStr().Add(arg, field);

        return base.Add(arg, field);
    }

    public override Obj Sub(Obj arg, Field field)
    {
        if (arg is Int i) return new Float(Value - i.Value);
        if (arg is Float f) return new Float(Value - f.Value);

        return base.Sub(arg, field);
    }

    public override Obj Mul(Obj arg, Field field)
    {
        if (arg is Int i) return new Float(Value * i.Value);
        if (arg is Float f) return new Float(Value * f.Value);

        return base.Mul(arg, field);
    }

    public override Obj Div(Obj arg, Field field)
    {
        if (arg is Int i) return new Float(Value / i.Value);
        if (arg is Float f) return new Float(Value / f.Value);

        return base.Div(arg, field);
    }

    public override Obj IDiv(Obj arg, Field field)
    {
        if (arg is Int i)
        {
            if (i.Value == 0) throw new DivideByZeroError();
            return new Int((long)(Value / i.Value));
        }
        if (arg is Float f)
        {
            if ((long)f.Value == 0) throw new DivideByZeroError();
            return new Int((long)(Value / (long)f.Value));
        }


        return base.IDiv(arg, field);
    }

    public override Obj Mod(Obj arg, Field field)
    {
        if (arg is Int i) return new Float(Value % i.Value);
        if (arg is Float f) return new Float(Value % f.Value);

        return base.Mod(arg, field);
    }

    public override Obj Pow(Obj arg, Field field)
    {
        if (arg is Int i) return new Float(Math.Pow(Value, i.Value));
        if (arg is Float f) return new Float(Math.Pow(Value, f.Value));

        return base.Pow(arg, field);
    }

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg is Int i) return new(Value == i.Value);

        return base.Eq(arg, field);
    }

    public override Bool Lt(Obj arg, Field field)
    {
        if (arg is Int i) return new(Value < i.Value);
        return base.Lt(arg, field);
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
