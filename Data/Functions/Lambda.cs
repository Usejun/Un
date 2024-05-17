namespace Un.Data;

public class Lambda : Fun
{
    public Lambda() : base() { }

    public Lambda(string str) : base()
    {
        var v = str[7..].Split("=>");
        name = "lambda";

        foreach (var arg in v[0].Split(","))
            if (arg != "_") args.Add(arg);

        code = [("return " + v[1])];
    }

    public override Obj Call(Iter args)
    {
        if (this.args.Count != args.Count) throw new ValueError("invalid argument");

        Parser interpreter = new(code, properties);

        for (int i = 0; i < this.args.Count; i++)
            properties.Add(this.args[i], args[i]);

        while (interpreter.TryInterpret()) ;

        properties.Clear();

        return interpreter.ReturnValue ?? None;
    }

    public override Str CStr() => new("lambda");

    public override Fun Clone() => new Lambda() { args = args[..], code = code };

    public static bool IsLambda(string str) => str.Length >= 6 && str[0..6] == "lambda";
}
