using Un.Function;
using Un.Object;

namespace Un.Class
{
    public class ClaFun : Fun
    {
        public ClaFun() { }

        public ClaFun(string[] code) : base(code) { }

        public override Obj Call(Obj arg)
        {
            this.arg = arg;
            Interpreter interpreter = new(code, variable)
            {
                line = 1,
                nesting = 1
            };

            variable.Add(argName, arg);

            while (interpreter.TryInterpret()) ;

            if (arg is Iter args)
                args[0] = variable["this"];

            variable.Clear();

            return interpreter.ReturnValue;
        }
    }
}
