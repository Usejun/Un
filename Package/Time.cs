namespace Un.Package;

public class Time : Obj, IPackage, IStatic
{
    public string Name => "time";

    Obj Sleep(Collections.Tuple args, Field field)
    {
        if (args[0] is not Int i)
           throw new ValueError("invalid argument");
        System.Threading.Thread.Sleep((int)i.Value);
        return None;
    }

    Date Now(Collections.Tuple args, Field field) => new(DateTime.Now);

    Date Today(Collections.Tuple args, Field field) => new(DateTime.Today);

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("sleep", 1, Sleep),
    ];

    public Obj Static()
    {
        Time time = new();
        time.field.Set("now", new NativeFun("now", 0, Now));
        time.field.Set("today", new NativeFun("today", 0, Today));
        return time;
    }
}

