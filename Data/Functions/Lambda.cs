namespace Un.Data;

public class Lambda : LocalFun
{    
    public static readonly Lambda Empty = new();
    public static readonly Lambda Self = new("(i) => (i)");    

    public Lambda() : base("lambda") 
    {
        ClassName = "lambda";
    }

    public Lambda(string str) : base("lambda")
    {
        ClassName = "lambda";
        Args = [];
        
        var line = str.Split(Literals.Arrow);
        var args = line[0][1..^1];
        var code = line[1];

        foreach (var arg in args.Split(Literals.CComma).Reverse())
            Args.Add((arg, null!));
        

        Length = Args.Count;
        this.code = [$"{Literals.Return} {code}"];
    }

    public override LocalFun Clone() => new Lambda() { Args = Args, code = code, Length = Length };

    public static bool IsLambda(string str) => str.Contains(Literals.Arrow);
}
