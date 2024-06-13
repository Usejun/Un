namespace Un.Data;

public abstract class Fun(string name) : Obj("func")
{
    public string name = name;

    public abstract Obj Call(List args);

    public override Str CStr() => new(name);

    public override abstract Fun Clone();

    public override int GetHashCode() => name.GetHashCode();
}
