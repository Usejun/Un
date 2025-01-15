namespace Un.Package;

public class Thread : Obj, IPackage, IStatic
{
    public string Name => "thread";

    public Obj Static()
    {   
        Thread thread = new();
        thread.field.Set("foreach", new NativeFun("foreach", 2, (args, field) =>
        {
            if (args[1] is not Fun func)
                throw new ArgumentError();

            var state = Parallel.ForEach(args[0].CList(), i => func.Call(new(i), field));

            Obj obj = new("state");
            obj.field.Set("is_completed", new Bool(state.IsCompleted));           

            return None;
        }));
        return thread;
    }
}
