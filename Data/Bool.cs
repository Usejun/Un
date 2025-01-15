namespace Un.Data;

public class Bool : Val<bool>
{
    public static readonly Bool True = new(true);
    public static readonly Bool False = new(false);

    public Bool() : base("bool", false) { }

    public Bool(bool value) : base("bool", value) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        if (args.Count == 0)
            Value = false;
        else if (args.Count == 1)
            Value = args[0].CBool().Value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg, Field field)
    {
        if (arg is Str) return CStr().Add(arg, field);
        return base.Add(arg, field);
    }   

    public override Str CStr() => new(Value ? Literals.True : Literals.False);

    public override Bool CBool() => new(Value);

    public override Obj Clone() => new Bool(Value);

    public override Obj Copy() => new Bool(Value);  

    public static bool IsBool(string str) => str == Literals.True || str == Literals.False;
}
