namespace Un.Data;

public class Int : Val<long>
{
    public static Int Zero => new(0);

    public static Int One => new(1);

    public static Int MinusOne => new(-1);

    public Int() : base("int", 0) { }

    public Int(long value) : base("int", value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        if (args.Count == 0)
            Value = 0;
        else if (args.Count == 1)
            Value = args[0].CInt().Value;
        else 
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value + i.Value);
        if (arg is Float f) return new Float(Value + f.Value);
        if (arg is Str) return CStr().Add(arg, field);

        return base.Add(arg, field);
    }

    public override Obj Sub(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value - i.Value);
        if (arg is Float f) return new Float(Value - f.Value);

        return base.Sub(arg, field);
    }

    public override Obj Mul(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value * i.Value);
        if (arg is Float f) return new Float(Value * f.Value);

        return base.Mul(arg, field);
    }

    public override Obj Div(Obj arg, Field field)
    {
        if (arg is Int i) return new Float((double)Value / i.Value);
        if (arg is Float f) return new Float(Value / f.Value);

        return base.Div(arg, field);
    }

    public override Obj IDiv(Obj arg, Field field)
    {
        if (arg is Int i)
        {
            if (i.Value == 0) throw new DivideByZeroError();
            return new Int(Value / i.Value);
        }
        if (arg is Float f)
        {
            if ((long)f.Value == 0) throw new DivideByZeroError();
            return new Int(Value / (long)f.Value);
        }

        return base.IDiv(arg, field);
    }

    public override Obj Mod(Obj arg, Field field)
    {
        if (arg is Int i) return new Float((double)Value % i.Value);
        if (arg is Float f) return new Float(Value % f.Value);

        return base.Mod(arg, field);
    }

    public override Obj Pow(Obj arg, Field field)
    {
        if (arg is Int i)
        {
            if (i.Value < 0) return new Float(Math.Pow(Value, i.Value));
            return new Int((long)Math.Pow(Value, i.Value));
        }
        if (arg is Float f) return new Float(Math.Pow(Value, f.Value));

        return base.Pow(arg, field);
    }

    public override Obj BAnd(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value & i.Value);

        return base.BAnd(arg, field);
    }

    public override Obj BOr(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value | i.Value);

        return base.BOr(arg, field);
    }

    public override Obj BXor(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value ^ i.Value);

        return base.BXor(arg, field);
    }

    public override Obj BNot() => new Int(~Value);

    public override Obj LSh(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value << (int)i.Value);

        return base.LSh(arg, field);
    }

    public override Obj RSh(Obj arg, Field field)
    {
        if (arg is Int i) return new Int(Value >> (int)i.Value);

        return base.RSh(arg, field);
    }

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg is Float f) return new(Value == f.Value);

        return base.Eq(arg, field);
    }

    public override Bool Lt(Obj arg, Field field)
    {
        if (arg is Float f) return new(Value < f.Value);

        return base.Lt(arg, field);
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
