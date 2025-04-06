namespace Un.Data;

public class NativeFun : Fun
{
    protected Func<Field, Obj> Function { get; set; }

    public NativeFun(string name, int length, Func<Field, Obj> func, Params args, bool isDynamic = false) : base(name)
    {
        Args = [..args];
        Length = length;
        Function = func;
        IsDynamic = isDynamic;
    }

    public override Obj Call(Field field) => Function(field);

    public override NativeFun Clone() => new(Name, Length, Function, Args, IsDynamic);
}
