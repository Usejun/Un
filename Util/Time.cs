namespace Un.Util;

public class Time : Obj, IPackage, IStatic
{
    public string Name => "time";

    Obj Sleep(Collections.Tuple args)
    {
        if (args[0] is Int i)
            Thread.Sleep((int)i.Value);
        return None;
    }

    Date Now(Collections.Tuple args) => new(DateTime.Now);

    Date Today(Collections.Tuple args) => new(DateTime.Today);

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("sleep", 1, Sleep),
    ];

    public Obj Static()
    {
        Time time = new();
        time.field.Set("now", new NativeFun("now", 1, Now));
        time.field.Set("today", new NativeFun("today", 1, Today));
        return time;
    }
}

