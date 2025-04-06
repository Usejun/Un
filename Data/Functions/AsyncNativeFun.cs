namespace Un.Data;

public class AsyncNativeFun<T>(string name, int length, Func<Field, T> func, Params args, bool IsDynamic = false) : NativeFun(name, length, func, args, IsDynamic)
    where T : Obj
{
    public override Obj Call(Field field) => new Task(new Task<Obj>(() => Function(field)));
}

