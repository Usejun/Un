namespace Un.Data;

public class Enu : Obj
{
    public List<Str> number = [];

    public Enu(string className) : base(className) { }

    public Enu(string[] code)
    {
        int value = 0;
        ClassName = code[0].Split()[1];

        for (int i = 1; i < code.Length; i++)
        {
            var line = code[i].Split(',').Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var var in line)
            {
                if (string.IsNullOrWhiteSpace(var)) continue;

                field.Set(var.Trim(), new EnuElm($"{ClassName}.{var.Trim()}", value));
                number.Add(new($"{ClassName}.{var.Trim()}"));
            }
        }
    }

    public override Obj Init(Iter args)
    {
        if (args[0] is Int i) return new EnuElm($"{number[(int)i.value].value}", (int)i.value);
        if (args[0] is Str s) return field[s.value];

        return base.Init(args);
    }

    public override Obj Clone() => this;

    public override Obj Copy() => this;

    public override Int Len() => new(number.Count);
}
