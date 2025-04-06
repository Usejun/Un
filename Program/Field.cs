namespace Un;

public class Field 
{
    public readonly static Field Null = new();

    public int Count => field.Count;
    public string[] Keys => [..field.Keys];
    public Obj[] Values => [..field.Values];

    private readonly Dictionary<string, Obj> field = [];

    public Field() { }

    public Field(Field other)
    {
        Copy(other);
    }

    public Field(params (string key, Obj value)[] pairs)
    {
        foreach (var (key, value) in pairs)
            Set(key, value);            
    }

    public bool Get(string name, out Obj value) => field.TryGetValue(name, out value!);

    public bool Get(StringBuffer name, out Obj value) => field.TryGetValue(name.ToString(), out value!);

    public void Set(string name, Obj value) => field[name] = value;

    public void Set(StringBuffer name, Obj value) => Set(name.ToString(), value);

    public void Merge(Collections.Tuple tuple, bool isForced = true)
    {
        for (int i = 0; i < tuple.Count; i++)
            if (isForced || !Key(tuple.Names[i]))
                Set(tuple.Names[i], tuple[i]);            
    }

    public void Merge(Params args, bool isForced = true)
    {
        foreach ((var key, var value) in args)
            if (isForced || !Key(key))
                Set(key, value);
    }

    public void Merge(Field field, bool isForced = true)
    {
        foreach ((var key, var value) in field.field)
            if (isForced || !Key(key))
                Set(key, value);            
    }

    public void Merge(Collections.Tuple parameters, Params args, bool isDynamic = false)
    {        
        if (!parameters.IsArgument())
            throw new ArgumentError("invlid arguments positions.");
        if (!args.Match(parameters))
            throw new ArgumentError("not matched arguments positions.");

        Merge(args);

        int positions = args.Positional();       

        for (int i = 0; i < positions; i++)
            this[args.Names[i]] = parameters[i];

        Merge(parameters, false);

        if (isDynamic)
        {
            if (!Get(args.Names[^1], out var val))
                throw new ArgumentError("invalid argument name.");
                
            List l = val switch
            {
                List => val.As<List>(),
                null => [],
                _ => new List().Append(val),
            };

            for (int i = positions; i < parameters.Count; i++)
                if (string.IsNullOrEmpty(parameters.Names[i]))
                    l.Append(parameters[i]);

            Set(args.Names[^1], l);
        }
    }
    
    public void Remove(string name)
    {
        field.Remove(name);
    }

    public void Remove(StringBuffer name) => Remove(name.ToString());

    public bool Key(string name) => field.ContainsKey(name);

    public bool Key(StringBuffer name) => Key(name.ToString());

    public bool Value(Obj obj) => field.ContainsValue(obj);

    public void Copy(Field other)
    {
        foreach (var (key, value) in other.field)
            Set(key, value.Clone());
    }

    public void Clear() => field.Clear();   

    public Obj this[string name]
    {
        get => field[name];
        set => field[name] = value;
    }

    public Obj this[StringBuffer name]
    {
        get => field[name.ToString()];
        set => field[name.ToString()] = value;
    }    
}
