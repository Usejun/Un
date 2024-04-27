using Un.Collections;

namespace Un.Data
{
    public class NativeFun : Fun
    {
        public Func<Iter, Obj> function;

        public NativeFun(string name, Func<Iter, Obj> func) : base()
        {
            this.name = name;
            function = func;
        }

        public override Str CStr() => new(name);

        public override Obj Call(Iter paras)
        {
            return function(paras);
        }

        public override Fun Clone()
        {
            return new NativeFun(name, function);
        }
    }
}
