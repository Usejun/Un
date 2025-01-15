namespace Un.Collections;

public class Dict : Ref<Dictionary<Obj, Obj>>
{
    public Dict() : base("dict", []) { }

    public Dict(string str, Field field) : base("dict", [])
    {
        var pairs = str[1..^1].Split(',');

        for (int i = 0; i < pairs.Length; i++)
        {
            if (string.IsNullOrEmpty(pairs[i])) continue;

            var pair = pairs[i].Trim().Split(':');
            var key = Convert(pair[0], field);
            var value = Convert(pair[1], field);

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
        field.Set("add", new NativeFun("add", -1, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");

            self.Value.Add(args[0], args[1]);

            return self;
        }));
        field.Set("remove", new NativeFun("remove", 1, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Remove(args[0]));
        }));
        field.Set("contains_key", new NativeFun("contains_key", 1, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");
            
            return new Bool(self.Value.ContainsKey(args[0]));
        }));
        field.Set("contains_value", new NativeFun("contains_value", 1, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.ContainsValue(args[0]));
        }));
        field.Set("clear", new NativeFun("clear", 0, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");

            self.Value.Clear();

            return None;
        }));
        field.Set("keys", new NativeFun("keys", 0, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");

            return new List([..self.Value.Keys]);
        }));
        field.Set("values", new NativeFun("values", 0, (args, field) =>
        {
            if (field[Literals.Self] is not Dict self)
                throw new ValueError("invalid argument");

            return new List([.. self.Value.Values]);
        }));           
    }

    public override Obj GetItem(Tuple args, Field field) => Value[args[0]];

    public override Obj SetItem(Tuple args, Field field) => Value[args[0]] = args[1];

    public override Int Len() => new(Value.Count);

    public override Str CStr() => new($"{{ {string.Join(", ", Value.Select(i => $"{i.Key.CStr().Value}:{i.Value.CStr().Value}"))} }}");

    public override Obj Clone() => new Dict() 
    { 
        Value = Value 
    };

    public override Obj Copy() => this;

    public static bool IsDict(string str) => str[0] == '{' && str[^1] == '}' && (str.Length == 2 || str.Contains(':'));
}
