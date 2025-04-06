namespace Un.Data;

public abstract class Fun(string name) : Obj("func")
{
    public Params Args { get; protected set; }
    public string Name { get; protected set; } = name;
    public int Length { get; protected set; } = 0;
    public bool IsDynamic { get; protected set; } = false;  

    public abstract Obj Call(Field field);

    public virtual Obj Call(Collections.Tuple args, Field field) 
    {
        if (args.Count < Length)
            throw new ArgumentError($"expeced {Length} arguments, got {args.Count}");

        field.Merge(args, Args, IsDynamic);
        return Call(field);
    }

    public override Str CStr() => new(Name);

    public override abstract Fun Clone();

    public override int GetHashCode() => Name.GetHashCode();

    public static bool Invoke(Obj obj, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.As<Fun>(out var function))
        {
            field.Merge(args, function.Args, function.IsDynamic);
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
            field.Merge(args, function.Args, function.IsDynamic);
            return function.Call(field);
        }

        return None;
    }

    public static bool Invoke(Obj obj, string name, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.field.Key(name) && obj.Get(name).As<Fun>(out var function))
        {
            field.Merge(args, function.Args, function.IsDynamic);
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
            field.Merge(args, function.Args, function.IsDynamic);
            return function.Call(field);
        }
        return None;
    }

    public static bool Method(Obj obj, string name, Collections.Tuple args, Field field, out Obj result)
    {
        if (obj.HasProperty(name) && obj.Get(name).As<Fun>(out var function))
        {
            field.Set(Literals.Self, obj);
            field.Merge(args, function.Args, function.IsDynamic);
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
            field.Merge(args, function.Args, function.IsDynamic);
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

    public static Collections.Tuple Parameter(string header)
    {
        int index = header.IndexOf(Literals.LParen);
        int arrow = header.IndexOf(Literals.Arrow);
        var parameters = header[index..(arrow == -1 ? ^0 : arrow)].Trim()[1..^1];        

        List data = [];
        List<string> names = [];

        index = 0;
        int depth = 0;
        StringBuffer buffer = new();
        var name = ""; 
        var value = "";
        var isString = false;

        while (index < parameters.Length)
        {
            if (Token.IsString(parameters[index])) isString = !isString;
            else if (Up()) ++depth;
            else if (Down()) --depth;
            else if (IsName())
            {                
                name = buffer.Split(Literals.Colon, 1)[0].ToString().Trim();
                buffer.Clear();
                index++;
                continue;
            }

            if (IsNext())
            {
                value = buffer.ToString().Trim();

                if (string.IsNullOrEmpty(name))
                {
                    data.Append(obj:null);
                    names.Add(value);
                }
                else
                {
                    if (List.IsList(value)) 
                        data.Append(new List(new Collections.Tuple(value, new())));
                    else if (Collections.Tuple.IsTuple(value)) 
                        data.Append(new Collections.Tuple(value, new()));
                    else 
                        data.Append(Calculator.All(value, new()));
                    names.Add(name);
                }
                
                name = "";
                buffer.Clear();
            }
            else buffer.Append(parameters[index]);

            index++;
        }

        value = buffer.ToString().Trim(); 

        if (!string.IsNullOrWhiteSpace(name))
        {
            names.Add(name);
            data.Append(Calculator.All(value, new()));
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                names.Add(value);
                data.Append(obj:null);    
            }
        }

        return new([..names], [..data]);

        bool Up() => parameters[index] == Literals.CLBrack || parameters[index] == Literals.CLParen || parameters[index] == Literals.CLBrace;

        bool Down() => parameters[index] == Literals.CRBrack || parameters[index] == Literals.CRParen || parameters[index] == Literals.CRBrace;

        bool IsNext() => parameters[index] == Literals.CComma && depth == 0 && !isString;

        bool IsName() => !isString && depth == 0 && parameters[index] == Literals.CAssign;
    }
}

