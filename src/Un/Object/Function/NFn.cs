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
        if (Depth == Global.MaxDepth)
            return new Err("maximum recursion depth");

        var scope = new Scope(new Map(), Closure); 
        Bind(scope, args);        
        lock (Global.SyncRoot) { Depth++; }

        var returned = Func(scope);
        
        lock (Global.SyncRoot) { Depth--; }

        return returned ?? None;
    }
}