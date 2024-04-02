using Un.Function;
using Un.Object;

namespace Un.Class
{
    public class Cla : Obj
    {
        public string className = "";

        public Dictionary<string, Obj> Properties = [];
        public Dictionary<string, ClaFun> Methodes = [];

        public Cla() { }

        public Cla(string className)
        {
            this.className = className;
        }

        public Cla(string[] code, Dictionary<string, Obj> variable, Dictionary<string, Fun> method)
        {
            List<Token> tokens = Tokenizer.All(code[0], variable, method);
            className = tokens[1].value;

            int line = 0, assign = 0, nesting = 1;

            while (code.Length - 1 > line)
            {
                line++;

                if (string.IsNullOrWhiteSpace(code[line]))
                    continue;

                assign = -1;
                tokens = Tokenizer.All(code[line], variable, method);

                for (int i = 0; assign == -1 && i < tokens.Count; i++)
                    if (tokens[i].tokenType == Token.Type.Assign)
                        assign = i;

                if (assign > 0)
                {
                    Token token = tokens[0];

                    if (!Properties.ContainsKey(token.value))
                    {
                        if (assign == 1) Properties.Add(token.value, None);
                        else throw new ObjException("Class Error");
                    }

                    Obj var = Properties[token.value];
                    Obj value = Tokenizer.Calculator.Calculate(tokens[(assign + 1)..], variable, method);

                    for (int i = 1; i < assign - 1; i++)
                    {
                        if (var is Iter iter1 && Convert(tokens[i].value, variable, method) is Int index1)
                            var = iter1[index1];
                        else throw new ObjException("Class Error");
                    }

                    if (assign == 1)
                        Properties[token.value] = value;
                    if (var is Iter iter2 && Convert(tokens[assign - 1].value, variable, method) is Int index2)
                        iter2[index2] = value;
                }
                else if (tokens[0].tokenType == Token.Type.Func)
                {
                    int start = line;

                    nesting++;
                    line++;

                    while (line < code.Length && Tokenizer.IsBody(code[line], nesting))
                        line++;

                    Methodes.Add(tokens[1].value, new(className, code[start..line]));

                    line--;
                    nesting--;
                }
                else throw new ObjException("Class Error");
            }
        }

        public virtual Obj Get(string str)
        {
            if (Properties.TryGetValue(str, out var property))
                return property;
            else if (Methodes.TryGetValue(str, out var method))
                return method;
            else throw new ObjException("Class Error");
        }

        public override Obj Add(Obj obj)
        {
            if (Methodes.TryGetValue("__add__", out var fun))
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Add Error");
        }

        public override Obj Sub(Obj obj)
        {
            if (Methodes.TryGetValue("__sub__", out var fun))
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Sub Error");
        }

        public override Obj Mul(Obj obj)
        {
            if (Methodes.TryGetValue("__mul__", out var fun))
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Mul Error");
        }

        public override Obj Mod(Obj obj)
        {
            if (Methodes.TryGetValue("__mod__", out var fun))
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Mod Error");
        }

        public override Obj Div(Obj obj)
        {
            if (Methodes.TryGetValue("__div__", out var fun))
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Div Error");
        }

        public override Obj IDiv(Obj obj)
        {
            if (Methodes.TryGetValue("__idiv__", out var fun))
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("iDiv Error");
        }

        public override Int Len()
        {
            if (Methodes.TryGetValue("__len__", out var fun))
                return fun.Call(new Iter([this])) as Int;
            else throw new ObjException("Len Error");
        }

        public override Cla Clone() => new(className)
        {
            Properties = Properties,
            Methodes = Methodes,
        };       

        public override string ToString() => className;
    }
}
