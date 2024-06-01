namespace Un.Data;

public class Bool : Val<bool>
{
    public Bool() : base("bool", false) { }

    public Bool(bool value) : base("bool", value) { }

    public override Obj Init(Iter args)
    {
        if (args.Count == 0)
            value = false;
        else if (args.Count == 1)
            value = args[0].CBool().value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Str) return CStr().Add(arg);
        return base.Add(arg);
    }

    public override Obj Xor(Obj arg) => new Bool(value ^ arg.CBool().value);

    public override Str CStr() => new(value ? "true" : "false");

    public override Bool CBool() => new(value);

    public override Obj Clone() => new Bool(value);

    public override Obj Copy() => new Bool(value);

    public static bool IsBool(string str) => str == "true" || str == "false";
}
