using Un.Object.Collections;

namespace Un.Object.Function;

public class LFn : Fn
{
    public string[] Body { get; set; }

    public override Obj Call(Tup args)
    {
        if (Depth == Global.MaxDepth)
            return new Err("maximum recursion depth");

        var scope = new Map(Closure ?? new Map());
        Bind(scope, args);
        var context = new Context(scope, new(Name, Body));

        lock (Global.SyncRoot) { Depth++; }
        context.EnterBlock("fn");
        var returned = Runner.Load(context).Run();
        context.ExitBlock();

        if (scope.TryGetValue("__using__", out var usings))
        {
            if (usings.As<List>(out var list))
                foreach (var obj in list)
                    obj.Exit();
            else
                return new Err("__usings__ must be list");

        }
        lock (Global.SyncRoot) { Depth--; }

        return returned ?? None;
    }
}