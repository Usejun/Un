namespace Un.Collections;

public class Set : Ref<HashSet<Obj>>
{
    public Set() : base("set", []) { }

    public Set(string str, Field field) : base("set", [])
    {
        var values = str[1..^1].Split(Literals.CComma);

        for (int i = 0; i < values.Length; i++)
        {
            var value = Parse(values[i].Trim(), field);
            Value.Add(value);
        }
    }

    public override Obj Init(Tuple args, Field field)
    {
        field.Merge(args, [("values", new List())], true);
        Value.Clear();

        if (!field["values"].As<List>(out var list))
            throw new ClassError();

        foreach (var arg in list)
            Value.Add(arg);

        return this;
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", 1, field =>
        {
            if (!field[Literals.Self].As<Set>(out var self))
                throw new ValueError("invalid argument");  

            self.Value.Add(field["value"]);          
            return None;
        }, [("value", null!)]));
        field.Set("extend", new NativeFun("extend", 1, field =>
        {
            if (!field[Literals.Self].As<Set>(out var self))
                throw new ValueError("invalid argument");
            
            var value = field["value"];

            if (value.As<List>(out _) || value.As<Tuple>(out _))
            {   
                foreach (var item in value.CList())
                    self.Value.Add(value);
            }
            else
                self.Value.Add(value);

            return None;
        }, [("value", null!)]));
        field.Set("remove", new NativeFun("remove", 1, field =>
        {                
            if (!field[Literals.Self].As<Set>(out var self))
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Remove(field["value"]));
        }, [("value", null!)]));
        field.Set("contains", new NativeFun("contains", 1, field =>
        {
            if (!field[Literals.Self].As<Set>(out var self))
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Contains(field["value"]));
        }, [("value", null!)]));
        field.Set("clear", new NativeFun("clear", 0, field =>
        {
            if (!field[Literals.Self].As<Set>(out var self))
                throw new ValueError("invalid argument");

            self.Value.Clear();

            return None;
        }, []));
        field.Set("values", new NativeFun("values", 0, field =>
        {
            if (!field[Literals.Self].As<Set>(out var self))
                throw new ValueError("invalid argument");

            return new List([.. self.Value]);
        }, []));
    }

    public override Obj GetItem(Obj arg, Field field) => new Bool(Value.Contains(arg));

    public override Obj Add(Obj arg, Field field)
    {
        foreach (var item in arg.CList())
            Value.Add(item);            

        return this;
    }

    public override Obj Sub(Obj arg, Field field)
    {
        foreach (var item in arg.CList())
            Value.Remove(item);

        return this;
    }

    public override Int Len() => new(Value.Count);

    public override List CList() => new([.. Value]);

    public override Str CStr() => new($"{{ {string.Join(", ", Value.Select(i => i.CStr().Value))} }}");

    public override Obj Clone() => new Set()
    {
        Value = Value
    };

    public override Obj Copy() => this;

    public static bool IsSet(string str) => str[0] == '{' && str[^1] == '}';
}
