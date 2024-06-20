namespace Un.Data
{
    public class AsyncNativeFun<T>(string name, int argsLen, Func<Collections.Tuple, T> func) : Fun(name)
        where T : Obj
    {
        public int argsLen = argsLen;
        public Func<Collections.Tuple, T> function = func;

        public override Obj Call(Collections.Tuple args)
        {
            if (argsLen != -1 && args.Count != argsLen)
                throw new ValueError("arguments length is over");

            return new Task(new Task<Obj>(() => function(args)));
        }

        public override NativeFun Clone() => new(name, argsLen, function);
    }
}
