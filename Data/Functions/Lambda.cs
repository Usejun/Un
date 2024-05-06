using Un.Collections;

namespace Un.Data
{
    public class Lambda : Fun
    {
        public Lambda() : base() { }

        public Lambda(string str) : base()
        {
            var v = str[7..].Split("=>");
            name = "lambda";

            foreach (var arg in v[0].Split(","))
                if (arg != "_") args.Add(arg);

            code = [("return " + v[1])];
        }

        public override Obj Call(Iter paras)
        {
            if (paras.Count != args.Count) throw new ArgumentException();

            Parser interpreter = new(code, properties);

            for (int i = 0; i < args.Count; i++)
                properties.Add(args[i], paras[i]);

            while (interpreter.TryInterpret()) ;

            properties.Clear();

            return interpreter.ReturnValue;
        }

        public override Str CStr() => new("lambda");

        public override Fun Clone() => new Lambda() { args = args[..], code = code };
    }
}
