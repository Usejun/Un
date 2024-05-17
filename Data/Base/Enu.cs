namespace Un.Data;

public class Enu : Obj
{
    public Dictionary<int, string> convert = [];

    public Enu(string className) : base(className) { }

    public Enu(string[] code)
    {
        int value = 0;
        ClassName = code[0].Split()[1];

        for (int i = 1; i < code.Length; i++)
        {
            var line = code[i].Split(',');
            foreach (var var in line)
            {
                properties.Add(var.Trim(), new EnuElm($"{ClassName}.{var.Trim()}", value));
                convert.Add(value, var.Trim());
                value++;
            }
        }
    }

    public override Obj Init(Iter args)
    {
        if (args[0] is Int i) return properties[convert[(int)i.value]];
        if (args[0] is Str s) return properties[s.value];

        return base.Init(args);
    }
}
