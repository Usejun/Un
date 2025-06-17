using Un.Object.Collections;

namespace Un.Object.Function;

public class GFn : Fn
{
    public Fn Func { get; set; }

    public override Obj Call(Tup args) => new Future()
    {
        State = Task.Run(() => Func.Call(args))
    };
}