namespace Un;

public class Time : Obj, IPackage, IStatic
{
    public string Name => "time";

    public static long Tick => DateTime.Now.Ticks;

    Obj Sleep(Field field)
    {
        if (!field["millisec"].As<Int>(out var i))
           throw new ValueError("invalid argument");
        Thread.Sleep((int)i.Value);
        return None;
    }

    Date Now(Field field) => new(DateTime.Now);

    Date Today(Field field) => new(DateTime.Today);

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("sleep", 1, Sleep, [("millisec", null!)]),
    ];

    public Obj Static()
    {
        Time time = new();
        time.field.Set("now", new NativeFun("now", 0, Now, []));
        time.field.Set("today", new NativeFun("today", 0, Today, []));
        return time;
    }
}

