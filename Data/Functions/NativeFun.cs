namespace Un.Data;

public class NativeFun : Fun
{
    public int argsLen;
    public Func<Iter, Obj> function;

    public NativeFun(string name, int argsLen, Func<Iter, Obj> func) : base()
    {
        this.name = name;
        this.argsLen = argsLen; 
        function = func;
    }

    public override Obj Call(Iter args)
    {
        if (argsLen != -1 && args.Count != argsLen)
            throw new ValueError("arguments count is over");

        return function(args);
    }

    public override Str CStr() => new(name);

    public override Fun Clone() => new NativeFun(name, argsLen, function);
}
