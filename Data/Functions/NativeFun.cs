using Un.Collections;

namespace Un.Data
{
    public class NativeFun : Fun
    {
        public int paraLen;
        public Func<Iter, Obj> function;

        public NativeFun(string name, int paraLen, Func<Iter, Obj> func) : base()
        {
            this.name = name;
            this.paraLen = paraLen; 
            function = func;
        }

        public override Obj Call(Iter paras)
        {
            if (paraLen != -1 && paras.Count != paraLen)
                throw new ArgumentException("parameter count is over");

            return function(paras);
        }

        public override Str CStr() => new(name);

        public override Fun Clone() => new NativeFun(name, paraLen, function);
    }
}
