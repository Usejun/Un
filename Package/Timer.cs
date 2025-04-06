namespace Un.Package;

public class Timer : Ref<System.Threading.Timer>
{
    public Timer() : base("timer", null!) { }

    public override void Init()
    {
        field.Set("change", new NativeFun("change", 2, field =>
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
        field.Merge(args, [("func", null!), ("due_time", Int.MinusOne), ("period", Int.MinusOne)]);

        if (!field["func"].As<Fun>(out var fun) || fun.Length > 0 ||
            !field["due_time"].As<Int>(out var dueTime) ||
            !field["period"].As<Int>(out var period))
            throw new ArgumentError();        

        Value = (dueTime.Value, period.Value) switch 
        {
            (< 0, < 0) => new(state => { fun.Call([], new()); }),
            _ =>  new(state => { fun.Call([], new()); }, new Obj("state"), dueTime.Value, period.Value)
        };
        return this;
    }

    public override Obj Copy() => this;
}