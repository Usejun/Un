namespace Un.Data;

public class NativeFun(string name, int argsLen, Func<Collections.Tuple, Obj> func) : Fun(name)
{
    public int argsLen = argsLen;
    public Func<Collections.Tuple, Obj> function = func;

    public override Obj Call(Collections.Tuple args)
    {
        if (argsLen != -1 && args.Count != argsLen)
            throw new ValueError("arguments length is over");

        return function(args);
    }

    public override NativeFun Clone() => new(name, argsLen, function);
}
