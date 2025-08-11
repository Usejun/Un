using Un.Object.Collections;

namespace Un.Object.Function;

public class PFn(List<Node> nodes) : Fn
{
    private List<Node> nodes = nodes;

    public override Obj Call(Tup args)
    {
        if (Depth == Global.MAXRECURSIONDEPTH)
            return new Err("maximum recursion depth");

        var scope = new Scope(new Map(), Closure ?? Scope.Empty);
        Bind(scope, args);
        lock (Global.SyncRoot) { Depth++; }

        var parser = new Parser(new(scope, new("", []), []));
        parser.Parse(nodes);

        lock (Global.SyncRoot) { Depth--; }

        return parser.ReturnValue ?? None;
    }
    
    public override Obj Clone() => new PFn([..nodes])
    {
        Name = Name,
        Args = [..Args],
        ReturnType = ReturnType,
        Closure = Closure,
        Self = Self,
        Super = Super?.Clone()!,
    };
}