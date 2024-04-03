using Un.Function;
using Un.Object;

namespace Un.Class
{
    public class Cla : Obj, IIndexable
    {
        public string className = "";

        public Dictionary<string, Obj> Properties = [];

        public Cla() { }

        public Cla(string className)
        {
            this.className = className;
        }

        public Cla(string[] code, Dictionary<string, Obj> properties)
        {
            List<Token> tokens = Tokenizer.All(code[0], properties);
            className = tokens[1].value;

            int line = 0, assign = 0, nesting = 1;

            while (code.Length - 1 > line)
            {
                line++;

                if (string.IsNullOrWhiteSpace(code[line]))
                    continue;

                assign = -1;
                tokens = Tokenizer.All(code[line], properties);

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
                    Obj value = Tokenizer.Calculator.Calculate(tokens[(assign + 1)..], Properties);

                    for (int i = 1; i < assign - 1; i++)
                    {
                        if (var is IIndexable indexable1)
                            indexable1.SetByIndex(new Iter([Convert(tokens[i].value, Properties), value]));
                        else throw new ObjException("Class Error");
                    }

                    if (assign == 1)
                        Properties[token.value] = value;
                    else if (var is IIndexable indexable2)
                        indexable2.SetByIndex(new Iter([Convert(tokens[assign - 1].value, Properties), value]));
                }
                else if (tokens[0].tokenType == Token.Type.Func)
                {
                    int start = line;

                    nesting++;
                    line++;

                    while (line < code.Length && Tokenizer.IsBody(code[line], nesting))
                        line++;

                    Properties.Add(tokens[1].value, new ClaFun(className, code[start..line]));

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
            else throw new ObjException("Class Error");
        }

        public override Obj Add(Obj obj)
        {
            if (Properties.TryGetValue("__add__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Add Error");
        }

        public override Obj Sub(Obj obj)
        {
            if (Properties.TryGetValue("__sub__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Sub Error");
        }

        public override Obj Mul(Obj obj)
        {
            if (Properties.TryGetValue("__mul__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Mul Error");
        }

        public override Obj Mod(Obj obj)
        {
            if (Properties.TryGetValue("__mod__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Mod Error");
        }

        public override Obj Div(Obj obj)
        {
            if (Properties.TryGetValue("__div__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Div Error");
        }

        public override Obj IDiv(Obj obj)
        {
            if (Properties.TryGetValue("__idiv__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new ObjException("iDiv Error");
        }

        public override Int Len()
        {
            if (Properties.TryGetValue("__len__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            return base.Len();
        }

        public override Int Hash()
        {
            if (Properties.TryGetValue("__hash__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            return base.Hash();
        }

        public override Str Type()
        {
            if (Properties.TryGetValue("__type__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
                return s;
            throw new ObjException("Type Error");
        }

        public override Str CStr()
        {
            if (Properties.TryGetValue("__str__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
                return s;
            throw new ObjException("Str Error");
        }

        public override Bool CBool()
        {
            if (Properties.TryGetValue("__bool__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Bool b)
                return b;
            throw new ObjException("Bool Error");
        }

        public override Float CFloat()
        {
            if (Properties.TryGetValue("__float__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Float f)
                return f;
            throw new ObjException("Float Error");
        }

        public override Int CInt()
        {
            if (Properties.TryGetValue("__int__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            throw new ObjException("Int Error");
        }

        public override Iter CIter()
        {
            if (Properties.TryGetValue("__iter__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Iter it)
                return it;
            throw new ObjException("Iter Error");
        }

        public virtual Obj GetByIndex(Obj obj)
        {
            if (Properties.TryGetValue("__get__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Get Error");
        }

        public virtual Obj SetByIndex(Obj obj)
        {
            if (Properties.TryGetValue("__set__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Set Error");
        }

        public override Cla Clone() => new(className)
        {
            Properties = Properties,
        };       

        public override string ToString() => className;
    }
}
