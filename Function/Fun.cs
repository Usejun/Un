﻿using Un.Object;

namespace Un.Function
{
    public class Fun : Obj
    {
        public string name = "";
        public string argName = "";
        public string[] code = [];
        public Obj arg = None;
        public Dictionary<string, Obj> variable = [];

        public Fun() { }               

        public Fun(string[] code)
        {
            this.code = code;
            List<Token> tokens = Tokenizer.Tokenization(code[0]);
            name = tokens[1].value;        
            argName = tokens[3].value;
        }

        public virtual Obj Call(Obj arg)
        {
            this.arg = arg;
            Interpreter interpreter = new(code, variable: variable)
            {
                line = 1,
                nesting = 1
            };

            variable.Add(argName, arg);

            while (interpreter.TryInterpret());

            variable.Clear();

            return interpreter.ReturnValue;
        }

        public override Fun Clone()
        {
            return new() {
                name = name,
                argName = argName,
                code = code,
            };
        }
    }
}