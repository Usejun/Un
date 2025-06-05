using Un.Object.Collections;

namespace Un.Object.Function;

public class LFn : Fn
{
    public string[] Body { get; set; }

    public override Obj Call(Tup args)
    {
        var scope = new Scope()
        {
            ["self"] = Self
        };
        Bind(scope, args);
        return Obj.None;//Global.Swap(Name, Body, scope);
    }
}