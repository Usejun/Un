using Un.Object.Collections;

namespace Un.Object.Function;

public class NFn : Fn
{
    public static NFn My => new()
    {
        Name = "self",
        Args = [new Arg("x") { IsEssential = true }],
        Func = (args) => args["x"],
    };

    public Func<Scope, Obj> Func { get; set; } = null!;

    public override Obj Call(Tup args)
    {
        var scope = new Scope
        {
            ["self"] = Self
        };      
        Bind(scope, args);        
        var returned = Func(scope);
        return returned;
    }
}