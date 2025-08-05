using Un.Object.Collections;

namespace Un.Object.Function;

public class PFn : Fn
{
    public List<Node> Nodes { get; set; }

    public override Obj Call(Tup args)
    {
        if (Depth == Global.MAXRECURSIONDEPTH)
            return new Err("maximum recursion depth");

        var scope = new Scope(new Map(), Closure);
        Bind(scope, args);
        lock (Global.SyncRoot) { Depth++; }

        var parser = new Parser(new(scope, new("", []), []));

        lock (Global.SyncRoot) { Depth--; }

        return parser.ReturnValue ?? None;
    }
    
    public override Obj Clone() => new PFn()
    {
        Name = Name,
        Args = [..Args],
        ReturnType = ReturnType,
        Closure = Closure,
        Nodes = [..Nodes],
        Self = Self,
        Super = Super?.Clone(),
    };
}