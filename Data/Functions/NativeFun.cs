namespace Un.Data;

public class NativeFun(string name, int length, Func<Collections.Tuple, Field, Obj> func) : Fun(name) 
{
    public Func<Collections.Tuple, Field, Obj> function = func;

    public override Obj Call(Collections.Tuple args, Field field)
    {
        if (length != -1 && args.Count != length)
            throw new ValueError("arguments length is over");

        return function(args, field);
    }

    public override NativeFun Clone() => new(name, length, function);
}
