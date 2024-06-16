namespace Un.Data;

public abstract class Fun(string name) : Obj("func")
{
    public string name = name;

    public abstract Obj Call(List args);

    public override Str CStr() => new(name);

    public override abstract Fun Clone();

    public override int GetHashCode() => name.GetHashCode();

    public static bool Invoke(Obj obj, List args, out Obj result)
    {
        if (obj is Fun function)
        {
            result = function.Call(args);
            return true;
        }

        result = None;
        return false;
    }

    public static bool Invoke(Obj obj, string name, List args, out Obj result)
    {
        if (obj.field.Key(name) && obj.Get(name) is Fun function)
        {
            result = function.Call(args);
            return true;
        }

        result = None;
        return false;
    }
}
