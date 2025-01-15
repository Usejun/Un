namespace Un.Data
{
    public class AsyncNativeFun<T>(string name, int length, Func<Collections.Tuple, Field, T> func) : Fun(name)
        where T : Obj
    {
        public Func<Collections.Tuple, Field, T> function = func;

        public override Obj Call(Collections.Tuple args, Field field)
        {
            if (length != -1 && args.Count != length)
                throw new ValueError("arguments length is over");

            return new Task(new Task<Obj>(() => function(args, field)));
        }

        public override NativeFun Clone() => new(name, length, function);
    }
}
