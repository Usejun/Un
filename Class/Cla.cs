using Un.Object;

namespace Un.Class
{
    public class Cla : Obj
    {
        public string className = "";

        public static Dictionary<string, Obj> Properties = [];
        public static Dictionary<string, ClaFun> Methodes = [];

        public Cla() { }

        public Cla(string className)
        {
            this.className = className;
        }

        public Cla(string[] code, Dictionary<string, Obj> variable)
        {
            List<Token> tokens = Tokenizer.All(code[0], variable);
            className = tokens[1].value;

            int line = 0, assign = 0, nesting = 1;

            while (code.Length - 1 > line)
            {
                line++;

                if (string.IsNullOrWhiteSpace(code[line]))
                    continue;

                assign = -1;
                tokens = Tokenizer.All(code[line], variable);

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
                    Obj value = Tokenizer.Calculator.Calculate(tokens[(assign + 1)..], variable);

                    for (int i = 1; i < assign - 1; i++)
                    {
                        if (var is Iter iter1 && Convert(tokens[i].value, variable) is Int index1)
                            var = iter1[index1];
                        else throw new ObjException("Class Error");
                    }

                    if (assign == 1)
                        Properties[token.value] = value;
                    if (var is Iter iter2 && Convert(tokens[assign - 1].value, variable) is Int index2)
                        iter2[index2] = value;
                }
                else if (tokens[0].tokenType == Token.Type.Func)
                {
                    int start = line;

                    nesting++;
                    line++;

                    while (line < code.Length && Tokenizer.IsBody(code[line], nesting))
                        line++;

                    Methodes.Add(tokens[1].value, new(code[start..line]));

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

        public override Cla Clone() => new(className);       

        public override string ToString() => className;
    }
}
