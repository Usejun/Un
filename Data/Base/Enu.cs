namespace Un.Data;

public class Enu : Obj
{
    private readonly List<Str> number = [];

    public Enu(string className) : base(className) { }

    public Enu(string[] code)
    {
        int Value = 0;
        ClassName = code[0].Split()[1];

        for (int i = 1; i < code.Length; i++)
        {
            var line = code[i].Split(',').Where(s => !string.IsNullOrWhiteSpace(s));
            foreach (var var in line)
            {
                if (string.IsNullOrWhiteSpace(var)) continue;

                field.Set(var.Trim(), new EnuElm($"{ClassName}.{var.Trim()}", Value));
                number.Add(new($"{ClassName}.{var.Trim()}"));
            }
        }
    }

    public override Obj Init(Collections.Tuple args)
    {
        if (args[0] is Int i) return new EnuElm($"{number[(int)i.Value].Value}", (int)i.Value);
        if (args[0] is Str s) return field[s.Value];

        return base.Init(args);
    }

    public override Obj Clone() => this;

    public override Obj Copy() => this;

    public override Int Len() => new(number.Count);
}
