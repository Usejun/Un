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
        if (Depth == Global.MAXRECURSIONDEPTH)
            return new Err("maximum recursion depth");

        var scope = new Scope(new Map(), Closure ?? Scope.Empty);
        Bind(scope, args);
        lock (Global.SyncRoot) { Depth++; }

        var returned = Func(scope);

        lock (Global.SyncRoot) { Depth--; }

        return returned ?? None;
    }
    
    public override Obj Clone() => new NFn()
    {
        Name = Name,
        Args = [..Args],
        ReturnType = ReturnType,
        Closure = Closure,
        Func = Func,
        Self = Self,
        Super = Super?.Clone()!,
    };
}