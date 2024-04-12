using Un.Object;

namespace Un.Function
{
    public class NativeFun : Fun
    {
        public Func<Iter, Obj> function;

        public NativeFun(string name, Func<Iter, Obj> func)
        {
            code = [];
            this.name = name;         
            function = func;
        }

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
