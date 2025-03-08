namespace Un.Data;

public class Bool : Val<bool>
{
    public static readonly Bool True = new(true);
    public static readonly Bool False = new(false);

    public Bool() : base("bool", false) { }

    public Bool(bool value) : base("bool", value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", False)], 0);
        Value = field["value"].CBool().Value;

        return this;
    }

    public override Obj Add(Obj arg, Field field) => arg switch
    {
        Str s => s.Add(arg, field),
        _ => base.Add(arg, field),
    };

    public override Str CStr() => new(Value ? Literals.True : Literals.False);

    public override Bool CBool() => new(Value);

    public override Obj Clone() => new Bool(Value);

    public override Obj Copy() => new Bool(Value);  

    public static bool IsBool(string str) => str == Literals.True || str == Literals.False;
}
