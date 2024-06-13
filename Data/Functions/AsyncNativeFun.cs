namespace Un.Data
{
    public class AsyncNativeFun<T>(string name, int argsLen, Func<List, T> func) : Fun(name)
        where T : Obj
    {
        public int argsLen = argsLen;
        public Func<List, T> function = func;

        public override Obj Call(List args)
        {
            if (argsLen != -1 && args.Count != argsLen)
                throw new ValueError("arguments length is over");

            return new Task(new Task<Obj>(() => function(args)));
        }

        public override NativeFun Clone() => new(name, argsLen, function);
    }
}
