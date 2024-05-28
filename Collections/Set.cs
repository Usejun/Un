namespace Un.Collections;

public class Set : Ref<HashSet<Obj>>
{
    public Set() : base("set", []) { }

    public Set(string str, Field field) : base("set", [])
    {
        var values = str.Trim('{', '}').Split(',');

        for (int i = 0; i < values.Length; i++)
        {
            var value = Convert(values[i].Trim(), field);
            this.value.Add(value);
        }
    }

    public override Obj Init(Iter args)
    {
        value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CIter())
                value.Add(item);

        return this;
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)                
                self.value.Add(args[i]);

            return None;
        }));
        field.Set("extend", new NativeFun("extend", -1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[1].CIter())
                    self.value.Add(item);

            return None;
        }));
        field.Set("remove", new NativeFun("remove", 2, args =>
        {                
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            return new Bool(self.value.Remove(args[1]));
        }));
        field.Set("contains", new NativeFun("contains", 2, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            return new Bool(self.value.Contains(args[1]));
        }));
        field.Set("clear", new NativeFun("clear", 1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            self.value.Clear();

            return None;
        }));
        field.Set("values", new NativeFun("values", 1, args =>
        {
            if (args[0] is not Set self)
                throw new ValueError("invalid argument");

            return new Iter([.. self.value]);
        }));
    }

    public override Obj GetItem(Iter args) => new Bool(value.Contains(args[0]));

    public override Obj Add(Obj arg)
    {
        foreach (var item in arg.CIter())
            value.Add(item);            

        return this;
    }

    public override Obj Sub(Obj arg)
    {
        foreach (var item in arg.CIter())
            value.Remove(item);

        return this;
    }

    public override Int Len() => new(value.Count);

    public override Str CStr() => new($"{{ {string.Join(", ", value.Select(i => i.CStr().value))} }}");

    public override Obj Clone() => new Set()
    {
        value = value
    };

    public override Obj Copy() => this;

    public static bool IsSet(string str) => str[0] == '{' && str[^1] == '}';
}
