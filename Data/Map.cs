namespace Un.Data;

public class Map : Ref<Obj[]>
{
    public Map() : base("map", []) { }

    public Map(string type, Obj[] value) : base(type, value) {}

    public override Obj Init(Iter args)
    {
        if (args.Count != 2) throw new ValueError("invalid argument count");
        if (args[0] is not Fun fun) throw new ValueError("first argument is not convert function");
        if (args[1] is not Iter iter) throw new ValueError("second argument is not iterator");

        value = new Obj[iter.Count];

        for (int i = 0; i < iter.Count; i++)
            value[i] = fun.Call(iter[i].CIter()).Clone();

        return this;
    }

    public override Int Len() => new(value.Length);

    public override Iter CIter() => new(value);

    public override Str CStr() => new($"[{string.Join(", ", value.Select(i => i.CStr().value))}]");

    public override Obj Clone() => new Map()
    {
        value = value,
    };
}
