namespace Un.Data;

public class Int : Val<long>
{
    public static Int Zero => new(0);

    public static Int One => new(1);

    public static Int MinusOne => new(-1);

    public Int() : base("int", 0) { }

    public Int(string value) : base("int", long.Parse(value)) { }

    public Int(long value) : base("int", value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", Zero)], 0);
        Value = field["value"].CInt().Value;

        return this;
    }

    public override Obj Add(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value + i.Value),
        Float f => new Float(Value + f.Value),
        Str => CStr().Add(arg, field),
        _ => base.Add(arg, field),
    };

    public override Obj Sub(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value - i.Value),
        Float f => new Float(Value - f.Value),
        _ => base.Sub(arg, field),
    };

    public override Obj Mul(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value * i.Value),
        Float f => new Float(Value * f.Value),
        _ => base.Mul(arg, field),
    };

    public override Obj Div(Obj arg, Field field) => arg switch
    {
        Int i => new Float((double)Value / i.Value),
        Float f => new Float(Value / f.Value),
        _ => base.Div(arg, field),
    };

    public override Obj IDiv(Obj arg, Field field) => arg switch
    {
        Int i => i.Value == 0 ? throw new DivideByZeroError() : new Int(Value / i.Value),
        Float f => f.Value == 0 ? throw new DivideByZeroError() : new Int((long)(Value / f.Value)),
        _ => base.IDiv(arg, field),
    };

    public override Obj Mod(Obj arg, Field field) => arg switch
    {
        Int i => new Float((double)Value % i.Value),
        Float f => new Float(Value % f.Value),
        _ => base.Mod(arg, field),
    };

    public override Obj Pow(Obj arg, Field field) => arg switch
    {
        Int i => i.Value < 0 ? new Float(Math.Pow(Value, i.Value)) : new Int((long)Math.Pow(Value, i.Value)),
        Float f => new Float(Math.Pow(Value, f.Value)),
        _ => base.Pow(arg, field),
    };

    public override Obj BAnd(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value & i.Value),
        _ => base.BAnd(arg, field),
    };

    public override Obj BOr(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value | i.Value),
        _ => base.BOr(arg, field),
    };

    public override Obj BXor(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value ^ i.Value),
        _ => base.BXor(arg, field),
    };

    public override Obj BNot() => new Int(~Value);

    public override Obj LSh(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value << (int)i.Value),
        _ => base.LSh(arg, field),
    };

    public override Obj RSh(Obj arg, Field field) => arg switch
    {
        Int i => new Int(Value >> (int)i.Value),
        _ => base.RSh(arg, field),
    };

    public override Bool Eq(Obj arg, Field field) => arg switch
    {
        Int i => new(Value == i.Value),
        Float f => new(Value == f.Value),
        _ => base.Eq(arg, field),
    };

    public override Bool Lt(Obj arg, Field field) => arg switch
    {
        Int i => new(Value < i.Value),
        Float f => new(Value < f.Value),
        _ => base.Lt(arg, field),
    };

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
