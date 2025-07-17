using Un.Object.Collections;
using Un.Object.Primitive;

namespace Un.Object.Function;

public class Future() : Obj("future")
{
    public Task<Obj> State { get; set; }

    public void Run()
    {
        State.Start();
    }

    public Obj Wait() => State.Result;
}