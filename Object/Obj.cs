using Un.Function;
using Un.Supporter;

namespace Un.Object
{
    public class Obj : IIndexable
    {
        public static Obj None => new();

        public string ClassName { get; protected set; } = "None";

        public Dictionary<string, Obj> properties = [];

        public Obj() { }

        public Obj(string className)
        {
            ClassName = className;            
            if (Process.Class is not null && Process.TryGetClass(className, out var cla))
                properties = cla.properties;
        }

        public Obj(string[] code, Dictionary<string, Obj> local)
        {            
            List<Token> tokens = Interpreter.All(code[0], local);
            ClassName = tokens[1].value;

            int line = 0, nesting = 1, assign;

            while (code.Length - 1 > line)
            {
                line++;

                if (string.IsNullOrWhiteSpace(code[line]))
                    continue;

                assign = -1;
                tokens = Interpreter.All(code[line], local);

                if (tokens.Count == 0)
                    continue;

                for (int i = 0; assign == -1 && i < tokens.Count; i++)
                    if (tokens[i].type == Token.Type.Assign)
                        assign = i;

                if (assign > 0)
                {
                    Token token = tokens[0];

                    properties.TryAdd(token.value, None);
                    properties[token.value] = Calculator.Calculate(tokens[(assign + 1)..], properties);
                }
                else if (tokens[0].type == Token.Type.Func)
                {
                    int start = line;

                    nesting++;
                    line++;

                    while (line < code.Length && (string.IsNullOrWhiteSpace(code[line]) || Tokenizer.IsBody(code[line], nesting)))
                        line++;

                    properties.Add(tokens[1].value, new Fun(code[start..line]));

                    line--;
                    nesting--;
                }
                else throw new SyntaxException();
            }
        }

        public virtual void Ass(string value, Dictionary<string, Obj> properties) => throw new InvalidOperationException("This is a type that can't be assigned.");

        public virtual void Ass(Obj value, Dictionary<string, Obj> properties) => throw new InvalidOperationException("This is a type that can't be assigned.");

        public virtual void Init()
        {

        }

        public virtual Obj Init(Iter args)
        {
            if (properties.TryGetValue("__init__", out var value) && value is Fun fun)
            {
                Iter paras = [];
                paras.Append(this, false);
                paras.Append(args);
                fun.Call(paras);            
            }
            return this;
        }

        public virtual Obj Get(string str)
        {
            if (properties.TryGetValue(str, out var property))
                return property;
            else throw new PropertyException("A property that doesn't exist.");
        }

        public virtual void Set(string str, Obj value)
        {
            properties[str] = value;
        }

        public virtual Obj Add(Obj obj)
        {
            if (properties.TryGetValue("__add__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be added to each other.");
        }

        public virtual Obj Sub(Obj obj)
        {
            if (properties.TryGetValue("__sub__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be subtracted to each other.");
        }

        public virtual Obj Mul(Obj obj)
        {
            if (properties.TryGetValue("__mul__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be multiplied to each other.");
        }

        public virtual Obj Mod(Obj obj)
        {
            if (properties.TryGetValue("__mod__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform the remaining operations on each other.");
        }

        public virtual Obj Div(Obj obj)
        {
            if (properties.TryGetValue("__div__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be divided to each other.");
        }

        public virtual Obj IDiv(Obj obj)
        {
            if (properties.TryGetValue("__idiv__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be divided to each other.");
        }

        public virtual Obj And(Obj obj)
        {
            if (properties.TryGetValue("__and__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be divided to each other.");
        }

        public virtual Obj Or(Obj obj)
        {
            if (properties.TryGetValue("__or__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be divided to each other.");
        }

        public virtual Obj Xor(Obj obj)
        {
            if (properties.TryGetValue("__xor__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be divided to each other.");
        }

        public virtual Bool Equals(Obj obj)
        {
            if (obj.ClassName == "None" && ClassName == "None")
                return new(true);
            if (properties.TryGetValue("__eq__", out var value) && value is Fun fun && fun.Call(new Iter([this, obj])) is Bool b)
                return b;
            throw new InvalidOperationException("Types that are not comparable to each other.");
        }

        public virtual Bool Unequals(Obj obj) => new(!Equals(obj).value);

        public virtual Bool LessThen(Obj obj)
        {
            if (properties.TryGetValue("__lt__", out var value) && value is Fun fun && fun.Call(new Iter([this, obj])) is Bool b)
                return b;
            throw new InvalidOperationException("Types that are not comparable to each other.");
        }

        public virtual Bool GreaterThen(Obj obj) => new(!LessThen(obj).value);

        public virtual Bool LessOrEquals(Obj obj) => new(LessThen(obj).value || Equals(obj).value);

        public virtual Bool GreaterOrEquals(Obj obj) => new(!LessThen(obj).value || !Equals(obj).value);

        public virtual Int Len()
        {
            if (properties.TryGetValue("__len__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            return new(1);
        }

        public virtual Int Hash()
        {
            if (properties.TryGetValue("__hash__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            return new(GetHashCode());
        }

        public virtual Str Type()
        {
            if (properties.TryGetValue("__type__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
                return s;
            return new(ClassName);
        }

        public virtual Str CStr()
        {
            if (properties.TryGetValue("__str__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Str s)
                return s;
            return new(ClassName);
        }

        public virtual Bool CBool()
        {
            if (properties.TryGetValue("__bool__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Bool b)
                return b;
            throw new InvalidCastException("This type cannot cast bool.");
        }

        public virtual Float CFloat()
        {
            if (properties.TryGetValue("__float__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Float f)
                return f;
            throw new InvalidCastException("This type cannot cast float.");
        }

        public virtual Int CInt()
        {
            if (properties.TryGetValue("__int__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Int i)
                return i;
            throw new InvalidCastException("This type cannot cast int.");
        }

        public virtual Iter CIter()
        {
            if (properties.TryGetValue("__iter__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Iter it)
                return it;
            throw new InvalidCastException("This type cannot cast iter.");
        }

        public virtual Obj GetByIndex(Obj obj)
        {
            if (properties.TryGetValue("__getitem__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new IndexerException("It is not Indexable type");
        }

        public virtual Obj SetByIndex(Iter obj)
        {
            if (properties.TryGetValue("__setitem__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj[0], obj[1]]));
            throw new IndexerException("It is not Indexable type");
        }

        public virtual Obj Clone() => new(ClassName);

        public bool HasProperty(string key) => properties.ContainsKey(key);

        public static Obj Convert(string str, Dictionary<string, Obj> properties)
        {
            if (string.IsNullOrEmpty(str)) return None;
            if (properties.TryGetValue(str, out var value)) return value;
            if (Process.TryGetStaticClass(str, out var staticCla)) return staticCla;
            if (Process.TryGetProperty(str, out var property)) return property;
            if (str[0] == '\"' && str[^1] == '\"') return new Str(str.Trim('\"'));
            if (str[0] == '\'' && str[^1] == '\'') return new Str(str.Trim('\''));
            if (str[0] == '[' && str[^1] == ']') return new Iter(str, properties);
            if (str == "True") return new Bool(true);
            if (str == "False") return new Bool(false);
            if (str == "None") return None;
            if (long.TryParse(str, out var l)) return new Int(l);
            if (double.TryParse(str, out var d)) return new Float(d);

            throw new InvalidConvertException(str);
        }
        
        public static bool IsNone(Obj obj) => obj.ClassName == "None";
    }
}
