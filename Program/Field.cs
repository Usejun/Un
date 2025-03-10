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

    public void Merge(Field field)
    {
        foreach ((var key, var value) in field.field)
            Set(key, value);            
    }

    public void Merge(Collections.Tuple tuple, List<(string name, Obj obj)> args, int len, bool isDynamic = false)
    {
        foreach ((var name, Obj obj) in args)
            Set(name, obj);
            
        Merge(tuple.field);

        int i = 0, j = 0;

        while (j < len)
        {
            if (string.IsNullOrEmpty(tuple.Names[i]))
            {
                Set(args[i].name, tuple[i]);                
                j++;
            }
            else if (Key(tuple.Names[i]))
                Set(tuple.Names[i], tuple[i]);
            else
                throw new ArgumentError();
            i++;
        }

        if (isDynamic)
        {
            var value = field[args[^1].name];
            List list = [];

            if (value is not null)
                list.Append(value);

            for (i = len; i < tuple.Count; i++)
                if (string.IsNullOrEmpty(tuple.Names[i]))
                    list.Append(tuple[i]);
                else if (Key(tuple.Names[i]))
                    Set(tuple.Names[i], tuple[i]);

            Set(args[^1].name, list);
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
