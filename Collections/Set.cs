namespace Un.Collections;

public class Set : Ref<HashSet<Obj>>
{
    public Set() : base("set", []) { }

    public Set(string str, Field field) : base("set", [])
    {
        var Values = str.Trim('{', '}').Split(',');

        for (int i = 0; i < Values.Length; i++)
        {
            var Value = Convert(Values[i].Trim(), field);
            this.Value.Add(Value);
        }
    }

    public override Obj Init(Tuple args)
    {
        Value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CList())
                Value.Add(item);

        return this;
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)                
                self.Value.Add(args[i]);

            return None;
        }));
        field.Set("extend", new NativeFun("extend", -1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[1].CList())
                    self.Value.Add(item);

            return None;
        }));
        field.Set("remove", new NativeFun("remove", 2, args =>
        {                
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Remove(args[1]));
        }));
        field.Set("contains", new NativeFun("contains", 2, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Contains(args[1]));
        }));
        field.Set("clear", new NativeFun("clear", 1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            self.Value.Clear();

            return None;
        }));
        field.Set("values", new NativeFun("values", 1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            return new List([.. self.Value]);
        }));
    }

    public override Obj GetItem(List args) => new Bool(Value.Contains(args[0]));

    public override Obj Add(Obj arg)
    {
        foreach (var item in arg.CList())
            Value.Add(item);            

        return this;
    }

    public override Obj Sub(Obj arg)
    {
        foreach (var item in arg.CList())
            Value.Remove(item);

        return this;
    }

    public override Int Len() => new(Value.Count);

    public override Str CStr() => new($"{{ {string.Join(", ", Value.Select(i => i.CStr().Value))} }}");

    public override Obj Clone() => new Set()
    {
        Value = Value
    };

    public override Obj Copy() => this;

    public static bool IsSet(string str) => str[0] == '{' && str[^1] == '}';
}
