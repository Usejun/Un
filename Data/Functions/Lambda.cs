namespace Un.Data;

public class Lambda : LocalFun
{    
    public Lambda() : base("lambda") 
    {
        ClassName = "lambda";
    }

    public Lambda(string str) : base("lambda")
    {
        ClassName = "lambda";

        var line = str.Split("=>");
        var args = line[0][1..^1];
        var code = line[1];

        foreach (var arg in args.Split(",").Reverse())
            this.args.Add(arg);

        Length = this.args.Count;
        this.code = [$"{Literals.Return} {code}"];
    }

    public override Obj Call(Collections.Tuple args, Field field)
    {
        if (args.Count != Length)
            throw new ValueError("invalid arguments");

        Field local = new(this.field);
        local.Add(field);

        for (int i = 0; i < this.args.Count; i++)
            local.Set(this.args[i], args[i]);

        return Process.Interpret(Name, code, local, []);
    }

    public override LocalFun Clone() => new Lambda() { args = args[..], code = code, Length = Length };

    public static bool IsLambda(string str) => str.Contains(Literals.Arrow);
}
