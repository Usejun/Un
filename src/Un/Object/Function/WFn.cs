using Un.Object.Collections;

namespace Un.Object.Function;

public class WFn : Fn
{
    public Fn Func { get; set; }

    public override Obj Call(Tup args)
    {
        var task = Task.Run(() => Func.Call(args));        

        return task.Result;
    }
}