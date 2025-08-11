using Un.Object.Collections;

namespace Un.Object.Function;

public class LFn(string[] body) : Fn
{
    private string[] body = body;

    public override Obj Call(Tup args)
    {
        if (Depth == Global.MAXRECURSIONDEPTH)
            return new Err("maximum recursion depth");

        var scope = new Scope(new Map(), Closure ?? Scope.Empty);
        Bind(scope, args);

        var context = new Context(scope, new(Name ?? "fn", body), []);
        Obj? returned = null;

        lock (Global.SyncRoot) { Depth++; }

        try
        {
            returned = Runner.Load(context, this.context!).Run();
        }
        catch
        {
            throw;
        }
        finally
        {
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
        }

        return returned ?? None;
    }

    public override Obj Clone() => new LFn(body)
    {
        Name = Name,
        Args = [..Args],
        ReturnType = ReturnType,
        Closure = Closure,
        Self = Self,
        Super = Super?.Clone()!,
    };
}