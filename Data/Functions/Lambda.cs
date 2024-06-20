namespace Un.Data;

public class Lambda : LocalFun
{
    public Lambda() : base("lambda") { }

    public Lambda(string str) : base("lambda")
    {
        var line = str[(Literals.Lambda.Length + 1)..].Split("=>");
        var args = line[0];
        var code = line[1];

        foreach (var arg in args.Split(",").Reverse())
            if (arg != "_") this.args.Add(arg);

        this.code = [$"{Literals.Return} {code}"];
    }

    public override Obj Call(Collections.Tuple args)
    {
        Field field = new(this.field);

        for (int i = 0; i < this.args.Count; i++)
            field.Set(this.args[i], args[i]);

        return Process.Interpret(Name, code, field, []);
    }

    public override LocalFun Clone() => new Lambda() { args = args[..], code = code };

    public static bool IsLambda(string str) => str.Length >= 6 && str[0..6] == Literals.Lambda;
}
