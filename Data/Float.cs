namespace Un.Data;

public class Float : Val<double>
{
    public Float() : base("float", 0) { }

    public Float(string value) : base("float", double.Parse(value)) { }

    public Float(double value) : base("float", value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", Int.Zero)], 0);
        Value = field["value"].CFloat().Value;

        return this;
    }

    public override Obj Add(Obj arg, Field field) => arg switch
    {
        Int i => new Float(Value + i.Value),
        Float f => new Float(Value + f.Value),
        Str => CStr().Add(arg, field),
        _ => base.Add(arg, field),
    };

    public override Obj Sub(Obj arg, Field field) => arg switch
    {
        Int i => new Float(Value - i.Value),
        Float f => new Float(Value - f.Value),
        _ => base.Sub(arg, field),
    };

    public override Obj Mul(Obj arg, Field field) => arg switch
    {
        Int i => new Float(Value * i.Value),
        Float f => new Float(Value * f.Value),
        _ => base.Mul(arg, field),
    };

    public override Obj Div(Obj arg, Field field) => arg switch
    {
        Int i => new Float(Value / i.Value),
        Float f => new Float(Value / f.Value),
        _ => base.Div(arg, field),
    };

    public override Obj IDiv(Obj arg, Field field) => arg switch
    {
        Int i => i.Value == 0 ? throw new DivideByZeroError() : new Int((long)(Value / i.Value)),
        Float f => f.Value == 0 ? throw new DivideByZeroError() : new Int((long)(Value / f.Value)),
        _ => base.IDiv(arg, field),
    };

    public override Obj Mod(Obj arg, Field field) => arg switch
    {
        Int i => new Float(Value % i.Value),
        Float f => new Float(Value % f.Value),
        _ => base.Mod(arg, field),
    };


    public override Obj Pow(Obj arg, Field field) => arg switch
    {
        Int i => new Float(Math.Pow(Value, i.Value)),
        Float f => new Float(Math.Pow(Value, f.Value)),
        _ => base.Pow(arg, field),
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
        _ => base.Eq(arg, field),
    };

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
