using Un.Object.Collections;

namespace Un.Object.Function;

public class LFn : Fn
{
    public string[] Body { get; set; }

    public override Obj Call(Tup args)
    {
        var scope = new Map(Closure?? new Map());
        Bind(scope, args);

        var returned = Runner.Load(Name, Body, scope).Run();

        if (scope.TryGetValue("__using__", out var usings))
            foreach (var obj in usings.As<List>())
                obj.Exit();

        return returned;
    }
}