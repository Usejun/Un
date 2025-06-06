using Un.Object.Collections;

namespace Un.Object.Function;

public class PFn : Fn
{
    public List<Node> Nodes { get; set; }

    public override Obj Call(Tup args)
    {
        var scope = new Scope()
        {
            ["self"] = Self
        };
        var parser = new Parser(scope);
        Bind(scope, args);

        return parser.Parse(Nodes);
    }
}