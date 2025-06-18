using Un.Object.Collections;

namespace Un.Object.Function;

public class PFn : Fn
{
    public List<Node> Nodes { get; set; }

    public override Obj Call(Tup args)
    {
        var scope = new Map(Closure?? new Map());
        Bind(scope, args);
        
        var parser = new Parser(new(scope, new("", [])));

        return parser.Parse(Nodes);
    }
}