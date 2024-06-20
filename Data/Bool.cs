namespace Un.Data;

public class Bool : Val<bool>
{
    public Bool() : base("bool", false) { }

    public Bool(bool value) : base("bool", value) { }

    public override Obj Init(Collections.Tuple args)
    {
        if (args.Count == 0)
            Value = false;
        else if (args.Count == 1)
            Value = args[0].CBool().Value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Str) return CStr().Add(arg);
        return base.Add(arg);
    }   

    public override Obj Xor(Obj arg) => new Bool(Value ^ arg.CBool().Value);

    public override Str CStr() => new(Value ? Literals.True : Literals.False);

    public override Bool CBool() => new(Value);

    public override Obj Clone() => new Bool(Value);

    public override Obj Copy() => new Bool(Value);

    public static bool IsBool(string str) => str == Literals.True || str == Literals.False;
}
