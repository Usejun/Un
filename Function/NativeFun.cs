﻿using Un.Object;

namespace Un.Function
{
    public class NativeFun : Fun
    {
        public Func<Obj, Obj> function;

        public NativeFun(string name, Func<Obj, Obj> func)
        {
            code = [];
            this.name = name;
            this.argName = argName;            
            function = func;
        }

        public override Obj Call(Obj arg)
        {
            return function(arg);
        }

        public override Fun Clone()
        {
            return new NativeFun(name, function);
        }
    }
}
