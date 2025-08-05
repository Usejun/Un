using Un.Object.Collections;

namespace Un.Object.Function;

public class LFn : Fn
{
    public string[] Body { get; set; }

    public override Obj Call(Tup args)
    {
        if (Depth == Global.MAXRECURSIONDEPTH)
            return new Err("maximum recursion depth");

        var scope = new Scope(new Map(), Closure);
        Bind(scope, args);
        var context = new Context(scope, new(Name, Body), []);

        lock (Global.SyncRoot) { Depth++; }
        var returned = Runner.Load(context).Run();

        foreach (var nodes in context.Defers)
        {
            var parser = new Parser(new Context(context.Scope, new("defer", []), []));
            parser.Parse(nodes);
        }

        foreach (var obj in context.Usings)
        {
            obj.Exit();
        }

        lock (Global.SyncRoot) { Depth--; }

        return returned ?? None;
    }

    public override Obj Clone() => new LFn()
    {
        Name = Name,
        Args = [..Args],
        ReturnType = ReturnType,
        Closure = Closure,
        Body = Body,
        Self = Self,
        Super = Super?.Clone(),
    };
}