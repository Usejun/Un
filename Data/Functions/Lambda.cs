namespace Un.Data;

public class Lambda : LocalFun
{
    public Lambda() : base("lambda") { }

    public Lambda(string str) : base("lambda")
    {
        var v = str[7..].Split("=>");

        foreach (var arg in v[0].Split(",").Reverse())
            if (arg != "_") args.Add(arg);

        code = [("return " + v[1])];
    }

    public override Obj Call(List args)
    {
        Field field = new(this.field);

        for (int i = 0; i < this.args.Count; i++)
            field.Set(this.args[i], args[i]);

        return Process.Interpret(name, code, field, []);
    }

    public override LocalFun Clone() => new Lambda() { args = args[..], code = code };

    public static bool IsLambda(string str) => str.Length >= 6 && str[0..6] == "lambda";
}
