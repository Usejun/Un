namespace Un.Package;

public class Thread : Obj, IPackage, IStatic
{
    public string Name => "thread";

    public Obj Static()
    {   
        Thread thread = new();
        thread.field.Set("foreach", new NativeFun("foreach", 1, field =>
        {
            if (field["func"].As<Fun>(out var func))
                throw new ArgumentError();

            var state = Parallel.ForEach(field["values"].CList(), i => func.Call(new(i), field));

            Obj obj = new("state");
            obj.field.Set("is_completed", new Bool(state.IsCompleted));           

            return None;
        }, [("func", null!), ("values", null!)], true));
        thread.field.Set("run", new NativeFun("run", 1, field =>
        {
            if (field["count"].As<Int>(out var count) || field["func"].As<Fun>(out var func))
                throw new ArgumentError();

            var state = Parallel.For(0, count.Value, _ => func.Call([], field));

            Obj obj = new("state");
            obj.field.Set("is_completed", new Bool(state.IsCompleted));           

            return None;
        }, [("func", null!), ("count", Int.One)]));
        return thread;
    }
}
