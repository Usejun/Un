namespace Un.Data;

public abstract class Fun(string name) : Obj("func")
{
    public string Name { get; protected set; } = name;

    public abstract Obj Call(Collections.Tuple args);

    public override Str CStr() => new(Name);

    public override abstract Fun Clone();

    public override int GetHashCode() => Name.GetHashCode();

    public static bool Invoke(Obj obj, Collections.Tuple args, out Obj result)
    {
        if (obj is Fun function)
        {
            result = function.Call(args);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Invoke(Obj obj, Collections.Tuple args)
    {
        if (obj is Fun function)
            return function.Call(args);

        return None;
    }

    public static bool Invoke(Obj obj, string name, Collections.Tuple args, out Obj result)
    {
        if (obj.field.Key(name) && obj.Get(name) is Fun function)
        {
            result = function.Call(args);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Invoke(Obj obj, string name, Collections.Tuple args)
    {
        if (obj.field.Key(name) && obj.Get(name) is Fun function)
            return function.Call(args);
        return None;
    }
}
