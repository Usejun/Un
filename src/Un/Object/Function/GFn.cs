using Un.Object.Collections;

namespace Un.Object.Function;

public class GFn : Fn
{
    public Fn Func { get; set; }

    public override Obj Call(Tup args) => new Future()
    {
        State = Task.Run(() => Func.Call(args))
    };

    public override Obj Clone() => new GFn()
    {
        Name = Name,
        Args = Args.Select(arg => arg.Clone() as Arg).ToList(),
        ReturnType = ReturnType,
        Closure = Closure,
        Func = Func.Clone() as Fn,
        Self = Self,
        Super = Super?.Clone(),
    };
}