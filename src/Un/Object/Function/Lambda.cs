using Un.Object.Collections;

namespace Un.Object.Function;

public class Lambda : Fn
{
    public List<Node> Nodes { get; set; }
    public Scope Closure { get; set; }

    public override Obj Call(Tup args)
    {
        var scope = new Scope([..Closure]);
        Bind(scope, args);
        //var parser = new Parser(scope);
        return Obj.None;//parser.Parse([new Node("->", TokenType.Return), .. Nodes]);
    }
}
