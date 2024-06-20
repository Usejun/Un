namespace Un.Collections;

public class Map : Ref<Obj[]>
{
    public Map() : base("map", []) { }

    public Map(string str, Field field) : base("base", [])
    {
        str = str[1..^1];

        List data = [];

        int index = 0, depth = 0;
        var buffer = "";
        var isString = false;

        while (index < str.Length)
        {
            if (Token.IsString(str[index])) isString = !isString;
            if (str[index] == '[' || str[index] == '(') ++depth;
            if (str[index] == ']' || str[index] == ')') --depth;

            if (str[index] == ',' && depth == 0 && !isString)
            {
                if (List.IsList(buffer)) data.Append(new List(new Map(buffer, field)));
                else if (Tuple.IsTuple(buffer)) data.Append(new Tuple(new Map(buffer, field)));
                else data.Append(Calculator.All(buffer, field));

                buffer = "";
            }
            else buffer += str[index];

            index++;
        }

        if (!string.IsNullOrWhiteSpace(buffer))
            data.Append(Calculator.All(buffer, field));

        Value = new Obj[data.Count];

        for (int i = 0; i < data.Count; i++)
            Value[i] = data.Value[i];

    }

    public Map(Obj[] value) : base("map", value) { }

    public override Obj Init(Tuple args)
    {
        if (args.Count != 2) throw new ValueError("invalid argument count");
        if (args[0] is not Fun fun) throw new ValueError("first argument is not convert function");
        if (args[1] is not List list) throw new ValueError("second argument is not iterable");

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
