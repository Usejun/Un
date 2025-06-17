using Un.Object.Collections;

namespace Un.Object.Function;

public class Future : Obj
{
    public Task<Obj> State { get; set; }

    public void Run()
    {
        State.Start();
    }

    public Obj Wait() => State.Result;
}