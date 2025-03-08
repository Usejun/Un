namespace Un.Data;

public abstract class Fun(string name) : Obj("func")
{
    public List<(string name, Obj obj)> Args { get; protected set; }
    public string Name { get; protected set; } = name;
    public int Length { get; protected set; } = 0;
    public bool IsDynamic { get; protected set; } = false;  

    public abstract Obj Call(Field field);

    public virtual Obj Call(Collections.Tuple args, Field field) 
    {
        if (args.Count < Length)
            throw new ArgumentError($"expeced {Length} arguments, got {args.Count}");

        field.Merge(args, Args, Length, IsDynamic);
        return Call(field);
    }

    public override Str CStr() => new(Name);

    public override abstract Fun Clone();

    public override int GetHashCode() => Name.GetHashCode();

    public static bool Invoke(Obj obj, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.As<Fun>(out var function))
        {
            field.Merge(args, function.Args, function.Args.Count);
            result = function.Call(field);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Invoke(Obj obj, Collections.Tuple args, Field field)
    {
        if (obj.As<Fun>(out var function))
        {
            field.Merge(args, function.Args, function.Args.Count);
            return function.Call(field);
        }

        return None;
    }

    public static bool Invoke(Obj obj, string name, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.field.Key(name) && obj.Get(name).As<Fun>(out var function))
        {
            field.Merge(args, function.Args, function.Args.Count);
            result = function.Call(field);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Invoke(Obj obj, string name, Collections.Tuple args, Field field)
    {
        if (obj.field.Key(name) && obj.Get(name).As<Fun>(out var function))
        {
            field.Merge(args, function.Args, function.Args.Count);
            return function.Call(field);
        }
        return None;
    }

    public static bool Method(Obj obj, string name, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.HasProperty(name) && obj.Get(name).As<Fun>(out var function))
        {
            field.Set(Literals.Self, obj);
            field.Merge(args, function.Args, function.Args.Count);
            result = function.Call(field);
            return true;
        }

        result = None;
        return false;
    }

    public static Obj Method(Obj obj, string name, Collections.Tuple args, Field field)
    {
        if (obj.HasProperty(name) && obj.Get(name).As<Fun>(out var function))
        {
            field.Set(Literals.Self, obj);
            field.Merge(args, function.Args, function.Args.Count);
            return function.Call(field);
        }
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

    public static (string name, Collections.Tuple args, bool call) Split(StringBuffer text, Field field)
    {
        var index = text.IndexOf(Literals.LParen);
        var name = index == -1 ? text.ToString() : text[..index].ToString();
        var args = index == -1 ? Collections.Tuple.Empty : new Collections.Tuple(text[index..].ToString(), field);
        var call = index != -1 && args.Count >= 0;

        return (name, args, call);
    }
}
