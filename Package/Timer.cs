namespace Un.Package;

public class Timer : Ref<System.Threading.Timer>
{
    public Timer() : base("timer", null!) { }

    public override void Init()
    {
        field.Set("start", new NativeFun("start", 2, field =>
        {
            if (!field[Literals.Self].As<Timer>(out var self))
                throw new ValueError("invalid argument");
            if (!field["due_time"].As<Int>(out var dueTime))
                throw new ValueError("invalid argument");
            if (!field["period"].As<Int>(out var period))
                throw new ValueError("invalid argument");

            self.Value.Change(dueTime.Value , period.Value);
            return None;
        }, [("due_time", null!), ("period", null!)]));
    }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        return this;
    }
}