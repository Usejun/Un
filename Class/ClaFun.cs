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
            Interpreter interpreter = new(code, properties)
            {
                line = 1,
                nesting = 2,
                className = className
            };

            properties.Add(argName, arg);

            while (interpreter.TryInterpret()) ;

            properties.Clear();

            return interpreter.ReturnValue;
        }
    }
}
