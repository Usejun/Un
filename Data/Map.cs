namespace Un.Data;

public class Map : Ref<Obj[]>
{
    public Map() : base("map", []) { }

    public Map(string type, Obj[] Value) : base(type, Value) {}

    public override Obj Init(List args)
    {
        if (args.Count != 2) throw new ValueError("invalid argument count");
        if (args[0] is not Fun fun) throw new ValueError("first argument is not convert function");
        if (args[1] is not List list) throw new ValueError("second argument is not listator");

        Value = new Obj[list.Count];

        for (int i = 0; i < list.Count; i++)
            Value[i] = fun.Call([list[i]]).Clone();

        return this;
    }

    public override Int Len() => new(Value.Length);

    public override List CList() => new(Value);

    public override Str CStr() => new($"[{string.Join(", ", Value.Select(i => i.CStr().Value))}]");

    public override Obj Clone() => new Map()
    {
        Value = Value,
    };
}
