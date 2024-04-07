using Un.Function;

namespace Un.Object
{
    public class Obj : IIndexable
    {
        public static Obj None => new("None");

        public string className = "";

        public Dictionary<string, Obj> Properties = [];

        public Obj()
        {
            Init();
        }

        public Obj(string className)
        {
            this.className = className;
            Init();
        }

        public Obj(string[] code, Dictionary<string, Obj> properties)
        {
            
            List<Token> tokens = Tokenizer.All(code[0], properties);
            className = tokens[1].value;
            Init();

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

                    while (line < code.Length && (string.IsNullOrWhiteSpace(code[line]) || Tokenizer.IsBody(code[line], nesting)))
                        line++;

                    Properties.Add(tokens[1].value, new Fun(code[start..line]));

                    line--;
                    nesting--;
                }
                else throw new ObjException("Class Error");
            }
        }

        public virtual void Ass(string value, Dictionary<string, Obj> properties) => throw new ObjException("Ass Error");

        public virtual void Ass(Obj value, Dictionary<string, Obj> properties) => throw new ObjException("Ass Error");

        public virtual void Init()
        {

        }

        public virtual Obj Init(Obj obj)
        {
            if (Properties.TryGetValue("__init__", out var value) && value is Fun fun)
                fun.Call(new Iter([this, obj]));
            return this;
        }

        public virtual Obj Get(string str)
        {
            if (Properties.TryGetValue(str, out var property))
                return property;
            else throw new ObjException("Class Error");
        }

        public virtual Obj Add(Obj obj)
        {
            if (Properties.TryGetValue("__add__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Add Error");
        }

        public virtual Obj Sub(Obj obj)
        {
            if (Properties.TryGetValue("__sub__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Sub Error");
        }

        public virtual Obj Mul(Obj obj)
        {
            if (Properties.TryGetValue("__mul__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Mul Error");
        }

        public virtual Obj Mod(Obj obj)
        {
            if (Properties.TryGetValue("__mod__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Mod Error");
        }

        public virtual Obj Div(Obj obj)
        {
            if (Properties.TryGetValue("__div__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            else throw new ObjException("Div Error");
        }

        public virtual Obj IDiv(Obj obj)
        {
            if (Properties.TryGetValue("__idiv__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new ObjException("iDiv Error");
        }

        public virtual Int Comp(Obj obj)
        {
            if (obj.className == "None" && className == "None")
                return new(0);
            if (obj.className == "None" || className == "None")
                return new(1);
            if (Properties.TryGetValue("__comp__", out var value) && value is Fun fun && fun.Call(new Iter([this, obj])) is Int i)
                return i;
            throw new ObjException("Comp Error");
        }

        public virtual Int Len()
        {
            if (Properties.TryGetValue("__len__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            return new(1);
        }

        public virtual Int Hash()
        {
            if (Properties.TryGetValue("__hash__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            return new(GetHashCode());
        }

        public virtual Str Type()
        {
            if (Properties.TryGetValue("__type__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
                return s;
            return new(className);
        }

        public virtual Str CStr()
        {
            if (className == "None")
                return new("None");
            if (Properties.TryGetValue("__str__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
                return s;
            throw new ObjException("Str Error");
        }

        public virtual Bool CBool()
        {
            if (Properties.TryGetValue("__bool__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Bool b)
                return b;
            throw new ObjException("Bool Error");
        }

        public virtual Float CFloat()
        {
            if (Properties.TryGetValue("__float__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Float f)
                return f;
            throw new ObjException("Float Error");
        }

        public virtual Int CInt()
        {
            if (Properties.TryGetValue("__int__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            throw new ObjException("Int Error");
        }

        public virtual Iter CIter()
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

        public virtual Obj Clone()
        {
            Obj clone = new(className);

            foreach ((string key, Obj property) in Properties)
                clone.Properties.Add(key, property.Clone());

            return clone;
        }    

        public static Obj Convert(string str, Dictionary<string, Obj> properties)
        {
            if (string.IsNullOrEmpty(str)) return None;
            if (properties.TryGetValue(str, out var value)) return value;
            if (Process.IsStaticClass(str)) return Process.GetStaticClass(str);
            if (Process.IsGlobalVariable(str)) return Process.GetProperty(str);
            if (str[0] == '\"' && str[^1] == '\"') return new Str(str.Trim('\"'));
            if (str[0] == '[' && str[^1] == ']') return new Iter(str, properties);
            if (str == "True") return new Bool(true);
            if (str == "False") return new Bool(false);
            if (str == "None") return None;
            if (long.TryParse(str, out var l)) return new Int(l);
            if (double.TryParse(str, out var d)) return new Float(d);

            throw new ObjException("Convert Error");
        }
        
        public static bool IsNone(Obj obj) => obj.className == "None";
    }

    [Serializable]
    public class ObjException : Exception
    {
        public ObjException() { }
        public ObjException(string message) : base(message) { }
        public ObjException(string message, Exception inner) : base(message, inner) { }
    }
}
