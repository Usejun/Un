namespace Un.Data;

public abstract class Fun(string name) : Obj("func")
{
    public string Name { get; protected set; } = name;

    public int Length { get; protected set; } = 0;

    public abstract Obj Call(Collections.Tuple args, Field field);

    public override Str CStr() => new(Name);

    public override abstract Fun Clone();

    public override int GetHashCode() => Name.GetHashCode();

    public static bool Invoke(Obj obj, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj is Fun function)
        {
            result = function.Call(args, field);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Invoke(Obj obj, Collections.Tuple args, Field field)
    {
        if (obj is Fun function)
            return function.Call(args, field);

        return None;
    }

    public static bool Invoke(Obj obj, string name, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.field.Key(name) && obj.Get(name) is Fun function)
        {
            result = function.Call(args, field);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Invoke(Obj obj, string name, Collections.Tuple args, Field field)
    {
        if (obj.field.Key(name) && obj.Get(name) is Fun function)
            return function.Call(args, field);
        return None;
    }

    public static bool Method(Obj obj, string name, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.HasProperty(name) && obj.Get(name) is Fun function)
        {
            result = function.Call(args, field);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Method(Obj obj, string name, Collections.Tuple args, Field field)
    {
        if (obj.HasProperty(name) && obj.Get(name) is Fun function)
            return function.Call(args, field);
        return None;
    }

    public static (string name, Collections.Tuple args, bool call) Split(string text, Field field)
    {
        var index = text.IndexOf(Literals.LParen);
        var name = index == -1 ? text : text[..index];
        var args = index == -1 ? Collections.Tuple.Empty : new Collections.Tuple(text[index..], field);
        var call = index != -1 && args.Count >= 0;

        return (name, args, call);
    }
}
