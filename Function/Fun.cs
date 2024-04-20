using Un.Object;
using Un.Supporter;

namespace Un.Function
{
    public class Fun : Obj
    {
        public string name = "";
        public List<string> args = [];
        public string[] code = [];
        public int nesting = 0;

        public Fun() { }

        public Fun(string[] code) 
        {
            this.code = code;
            List<Token> tokens = Tokenizer.Tokenize(code[0]);
            int i = 3;
            name = tokens[1].value;

            while (tokens[i].type != Token.Type.RParen)
            { 
                if (tokens[i].type != Token.Type.Comma)
                    args.Add(tokens[i].value);
                i++;
            }

            foreach (var chr in code[0])
                if (chr == '\t')
                    nesting++;            
        }

        public virtual Obj Call(Iter paras)
        {                    
            Parser interpreter = new(code, properties, line: 1, nesting: nesting + 1);

            for (int i = 0; i < args.Count; i++)
                properties.Add(args[i], paras[i]);            

            while (interpreter.TryInterpret());

            properties.Clear();

            return interpreter.ReturnValue;
        }

        public override Fun Clone()
        {
            return new() {
                name = name,
                code = code,
                nesting = nesting,
                args = args,
                properties = []
            };
        }
    }
}
