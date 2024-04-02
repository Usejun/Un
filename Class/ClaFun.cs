using Un.Function;
using Un.Object;

namespace Un.Class
{
    public class ClaFun : Fun
    {
        public string className;

        public ClaFun(string className) 
        {
            this.className = className;
        }

        public ClaFun(string className, string[] code) : base(code) 
        {
            this.className = className;
        }

        public override Obj Call(Obj arg)
        {
            this.arg = arg;
            Interpreter interpreter = new(code, variable, method)
            {
                line = 1,
                nesting = 2,
                className = className
            };

            variable.Add(argName, arg);

            while (interpreter.TryInterpret()) ;

            //if (arg is Iter args)
            //    args[0] = variable["this"];

            variable.Clear();

            return interpreter.ReturnValue;
        }
    }
}
