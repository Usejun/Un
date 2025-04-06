namespace Un.Collections;

public class Dict : Ref<Dictionary<Obj, Obj>>
{
    public Dict() : base("dict", []) { }

    public Dict(string str, Field field) : base("dict", [])
    {
        var pairs = str[1..^1].Split(Literals.CComma);

        for (int i = 0; i < pairs.Length; i++)
        {
            if (string.IsNullOrEmpty(pairs[i])) continue;

            var pair = pairs[i].Trim().Split(Literals.CColon);
            var key = Parse(pair[0], field);
            var value = Parse(pair[1], field);

            Value.Add(key, value);
        }          
    }

    public override Obj Init(Tuple args, Field field)
    {
        Value.Clear();

        return this;
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", 2, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            self.Value.Add(field["key"], field["value"]);

            return self;
        }, [("key", null!), ("value", null!)]));
        field.Set("remove", new NativeFun("remove", 1, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Remove(field["key"]));
        }, [("key", null!)]));
        field.Set("get", new NativeFun("get", 1, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            return self.Value.TryGetValue(field["key"], out var value) ? value : field["default"] ;
        }, [("key", null!), ("default", None)]));
        field.Set("contains_key", new NativeFun("contains_key", 1, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");
            
            return new Bool(self.Value.ContainsKey(field["key"]));
        }, [("key", null!)]));
        field.Set("contains_value", new NativeFun("contains_value", 1, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            return new Bool(self.Value.ContainsValue(field["value"]));
        }, [("value", null!)]));
        field.Set("clear", new NativeFun("clear", 0, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            self.Value.Clear();

            return None;
        }, []));
        field.Set("keys", new NativeFun("keys", 0, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            return new List([..self.Value.Keys]);
        }, []));
        field.Set("values", new NativeFun("values", 0, field =>
        {
            if (!field[Literals.Self].As<Dict>(out var self))
                throw new ValueError("invalid argument");

            return new List([.. self.Value.Values]);
        }, []));           
    }

    public override Obj GetItem(Obj arg, Field field) => Value[arg];

    public override Obj SetItem(Obj arg, Obj index, Field field) => Value[index] = arg;

    public override Int Len() => new(Value.Count);

    public override Str CStr() => new($"{{ {string.Join(", ", Value.Select(i => $"{i.Key.CStr().Value}:{i.Value.CStr().Value}"))} }}");

    public override Obj Clone() => new Dict() 
    { 
        Value = Value 
    };

    public override Obj Copy() => this;

    public static bool IsDict(string str) => str[0] == Literals.CLBrace && str[^1] == Literals.CRBrace && (str.Length == 2 || str.Contains(Literals.CColon));
}
