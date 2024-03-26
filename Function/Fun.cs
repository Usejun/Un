using Un.Object;

namespace Un.Function
{
    public class Fun 
    {
        public string name = "";
        public string argName = "";
        public string[] code = [];
        public Obj arg = Obj.None;
        public Interpreter interpreter = new([]);

        public Fun() { }               

        public Fun(string[] code)
        {
            this.code = code;
            interpreter = new(code);
            List<Token> tokens = interpreter.Scan(code[0]);
            name = tokens[1].value;        
            argName = tokens[3].value;
        }

        public virtual Obj Call(Obj arg)
        {
            this.arg = arg;
            interpreter.line = 1;
            interpreter.nesting = 1;
            Obj obj = Obj.None;

            if (Process.IsVariable(argName))
                obj = Process.Variable[argName];
            else
                Process.Variable.Add(argName, Obj.None);

            Process.Variable[argName] = arg;

            while (interpreter.TryInterpret());                  

            Process.Variable[argName] = obj;
            if (obj == Obj.None)
                Process.Variable.Remove(argName);

            Obj returnValue = interpreter.ReturnValue;
            interpreter.ReturnValue = Obj.None;

            return returnValue;
        }

        public virtual Fun Clone()
        {
            return new() {
                name = name,
                argName = argName,
                code = code,
                interpreter = new(code)
            };
        }
    }
}
