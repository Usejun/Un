namespace Un.Collections;

public class Set : Ref<HashSet<Obj>>
{
    public Set() : base("set", []) { }

    public Set(string str, Field field) : base("set", [])
    {
        var values = str[1..^1].Split(',');

        for (int i = 0; i < values.Length; i++)
        {
            var value = Convert(values[i].Trim(), field);
            Value.Add(value);
        }
    }

    public override Obj Init(Tuple args, Field field)
    {
        Value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CList())
                Value.Add(item);

        return this;
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", -1, (args, field) =>
        {
            if (field[Literals.Self] is not Set self)
                throw new ValueError("invalid argument");

            for (int i = 0; i < args.Count; i++)                
                self.Value.Add(args[i]);

            return None;
        }));
        field.Set("extend", new NativeFun("extend", -1, (args, field) =>
        {
            if (field[Literals.Self] is not Set self)
                throw new ValueError("invalid argument");

            for (int i = 0; i < args.Count; i++)
                foreach (var item in args[1].CList())
                    self.Value.Add(item);

            return None;
        }));
        field.Set("remove", new NativeFun("remove", 1, (args, field) =>
        {                
            if (field[Literals.Self] is not Set self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Remove(args[0]));
        }));
        field.Set("contains", new NativeFun("contains", 1, (args, field) =>
        {
            if (field[Literals.Self] is not Set self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Contains(args[0]));
        }));
        field.Set("clear", new NativeFun("clear", 0, (args, field) =>
        {
            if (field[Literals.Self] is not Set self)
                throw new ValueError("invalid argument");

            self.Value.Clear();

            return None;
        }));
        field.Set("values", new NativeFun("values", 0, (args, field) =>
        {
            if (field[Literals.Self] is not Set self)
                throw new ValueError("invalid argument");

            return new List([.. self.Value]);
        }));
    }

    public override Obj GetItem(Tuple args, Field field) => new Bool(Value.Contains(args[0]));

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
