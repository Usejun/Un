﻿using Un.Collections;

namespace Un.Data
{
    public class Obj : IComparable<Obj>
    {
        public static Obj None => new();

        public string ClassName { get; protected set; } = "None";

        public Dictionary<string, Obj> properties = [];

        public Obj() { }

        public Obj(string className)
        {
            ClassName = className;

            if (Process.Class is not null && Process.TryGetClass(ClassName, out Obj cla))
                foreach ((string key, Obj obj) in cla.properties)
                    properties.Add(key, obj);
        }

        public Obj(string[] code, Dictionary<string, Obj> local)
        {
            List<Token> tokens = Lexer.Lex(Tokenizer.Tokenize(code[0]), local);

            ClassName = tokens[1].value;

            int line = 0, nesting = 1, assign;

            while (code.Length - 1 > line)
            {
                line++;

                if (string.IsNullOrWhiteSpace(code[line]))
                    continue;

                assign = -1;
                tokens = Lexer.Lex(Tokenizer.Tokenize(code[line]), local);

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

        public virtual void Init()
        {

        }

        public virtual Obj Init(Iter args)
        {
            if (properties.TryGetValue("__init__", out var value) && value is Fun fun)
            {
                Iter paras = new([this]);
                fun.Call(paras.Extend(args));
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

        public virtual Obj Pow(Obj obj)
        {
            if (properties.TryGetValue("__pow__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot be raised to a power");
        }

        public virtual Obj LSh(Obj obj)
        {
            if (properties.TryGetValue("__lsh__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that can't be left-shifted");
        }

        public virtual Obj RSh(Obj obj)
        {
            if (properties.TryGetValue("__rsh__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that can't be right-shifted");
        }

        public virtual Obj BAnd(Obj obj)
        {
            if (properties.TryGetValue("__band__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform bitwise AND operations");
        }

        public virtual Obj BOr(Obj obj)
        {
            if (properties.TryGetValue("__bor__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform bitwise OR operations");
        }

        public virtual Obj BXor(Obj obj)
        {
            if (properties.TryGetValue("__bxor__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform bitwise XOR operations");
        }

        public virtual Obj BNot()
        {
            if (properties.TryGetValue("__bnot__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this]));
            throw new InvalidOperationException("Types that cannot perform bitwise Not operations");
        }

        public virtual Obj And(Obj obj)
        {
            if (properties.TryGetValue("__and__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform the boolean AND operation.");
        }

        public virtual Obj Or(Obj obj)
        {
            if (properties.TryGetValue("__or__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform the boolean OR operation.");
        }

        public virtual Obj Xor(Obj obj)
        {
            if (properties.TryGetValue("__xor__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this, obj]));
            throw new InvalidOperationException("Types that cannot perform the boolean XOR operation.");
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

        public Int Hash() => new(GetHashCode());

        public Str Type()
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

        public virtual Obj Clone() => new(ClassName);

        public virtual Obj Copy()
        {
            if (properties.TryGetValue("__copy__", out var value) && value is Fun fun && fun.Call(new Iter([this])) is Iter it)
                return it;
            return this;
        }


        public virtual Obj GetItem(Iter para)
        {
            if (properties.TryGetValue("__getitem__", out var value) && value is Fun fun)
                return fun.Call(para.ExtendInsert(this, 0));
            throw new IndexerException("It is not Indexable type");
        }

        public virtual Obj SetItem(Iter para)
        {
            if (properties.TryGetValue("__setitem__", out var value) && value is Fun fun)
                return fun.Call(para.ExtendInsert(this, 0));
            throw new IndexerException("It is not Indexable type");
        }

        public virtual Obj Slice(Iter para)
        {
            if (para[0] is not Int start || para[1] is not Int end)
                throw new SyntaxException();

            Iter sliced = [];

            while (start.value < end.value)
            {
                sliced.Append(GetItem([start]));
                start.value++;
            }

            return sliced;
        }


        public virtual Obj Entry()
        {
            if (properties.TryGetValue("__entry__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this]));
            throw new UsingException("Types with undefined entry functions");
        }

        public virtual Obj Exit()
        {
            if (properties.TryGetValue("__exit__", out var value) && value is Fun fun)
                return fun.Call(new Iter([this]));
            throw new IndexerException("Types with undefined exit functions");
        }



        public bool HasProperty(string key) => properties.ContainsKey(key);

        public override bool Equals(object? obj)
        {
            if (obj is Obj o) return Equals(o).value;
            return false;
        }

        public int CompareTo(Obj? other)
        {
            if (Equals(other).value) return 0;
            if (LessThen(other).value) return -1;
            if (GreaterThen(other).value) return 1;
            throw new InvalidOperationException("Types that are not comparable to each other.");
        }


        public static Obj Convert(string str, Dictionary<string, Obj> properties)
        {           
            if (string.IsNullOrEmpty(str)) return None;
            if (properties.TryGetValue(str, out var value)) return value;
            if (Process.TryGetStaticClass(str, out var staticCla)) return staticCla;
            if (Process.TryGetProperty(str, out var property)) return property;
            if (str[0] == '\"' && str[^1] == '\"') return new Str(str.Trim('\"'));
            if (str[0] == '\'' && str[^1] == '\'') return new Str(str.Trim('\''));
            if (str[0] == '[' && str[^1] == ']') return new Iter(str, properties);
            if (str == "true") return new Bool(true);
            if (str == "false") return new Bool(false);
            if (str == "None") return None;
            if (long.TryParse(str, out var l)) return new Int(l);
            if (int.TryParse(str, out var i)) return new Int(i);
            if (double.TryParse(str, out var d)) return new Float(d);
            if (str.Length >= 6 && str[0..6] == "lambda") return new Lambda(str);

            throw new InvalidConvertException(str);
        }

        public static bool IsNone(Obj obj) => obj.ClassName == "None";

    }
}
