namespace Un.Collections;

public class Dict : Ref<Dictionary<Obj, Obj>>
{
    public Dict() : base("dict", []) { }

    public Dict(string str, Dictionary<string, Obj> properties) : base("dict", [])
    {
        var pairs = str.Trim('{', '}').Split(',');

        for (int i = 0; i < pairs.Length; i++)
        {
            if (string.IsNullOrEmpty(pairs[i])) continue;

            var pair = pairs[i].Trim().Split(':');
            var key = Convert(pair[0], properties);
            var value = Convert(pair[1], properties);

            this.value.Add(key, value);
        }          
    }

    public override Obj Init(Iter args)
    {
        value.Clear();

        return this;
    }

    public override void Init()
    {
        properties.Add("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            self.value.Add(args[1], args[2]);

            return self;
        }));
        properties.Add("remove", new NativeFun("remove", 2, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            return new Bool(self.value.Remove(args[1]));
        }));
        properties.Add("contains_key", new NativeFun("contains_key", 2, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            return new Bool(self.value.ContainsKey(args[1]));
        }));
        properties.Add("contains_value", new NativeFun("contains_value", 2, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            return new Bool(self.value.ContainsValue(args[1]));
        }));
        properties.Add("clear", new NativeFun("clear", 1, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            self.value.Clear();

            return None;
        }));
        properties.Add("keys", new NativeFun("keys", 1, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            return new Iter([.. self.value.Keys]);
        }));
        properties.Add("values", new NativeFun("values", 1, args =>
        {
            if (args[0] is not Dict self)
                throw new ValueError("invalid argument");

            return new Iter([.. self.value.Values]);
        }));           
    }

    public override Obj GetItem(Iter args) => value[args[0]];

    public override Obj SetItem(Iter args) => value[args[0]] = args[1];

    public override Int Len() => new(value.Count);

    public override Str CStr() => new($"{{ {string.Join(", ", value.Select(i => $"{i.Key.CStr().value}:{i.Value.CStr().value}"))} }}");

    public override Obj Clone() => new Dict() 
    { 
        value = value 
    };

    public override Obj Copy() => this;

    public static bool IsDict(string str) => str[0] == '{' && str[^1] == '}' && (str.Length == 2 || str.Contains(':'));
}
