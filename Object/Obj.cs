﻿namespace Un.Object
{
    public class Obj : IComparable<Obj>
    {
        public static Obj None => new Str("None");

        public virtual void Ass(string value, Dictionary<string, Obj> properties) => throw new ObjException("Ass Error");

        public virtual void Ass(Obj value, Dictionary<string, Obj> properties) => throw new ObjException("Ass Error");

        public virtual Obj Add(Obj obj) => throw new ObjException("Add Error");

        public virtual Obj Sub(Obj obj) => throw new ObjException("Sub Error");

        public virtual Obj Mul(Obj obj) => throw new ObjException("Mul Error");

        public virtual Obj Div(Obj obj) => throw new ObjException("Div Error");

        public virtual Obj IDiv(Obj obj) => throw new ObjException("IDiv Error");

        public virtual Obj Mod(Obj obj) => throw new ObjException("Mod Error");

        public virtual Int Len() => new(1);

        public virtual Int Hash() => new(GetHashCode());

        public virtual Str Type() => new ("obj");

        public virtual Int CInt() => throw new ObjException("Int Error");

        public virtual Float CFloat() => throw new ObjException("Float Error");

        public virtual Str CStr() => new("obj");

        public virtual Bool CBool() => new(false);

        public virtual Iter CIter() => throw new ObjException("Iter Error");

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

        public virtual int CompareTo(Obj? obj) => throw new ObjException("compare Error");

        public virtual Obj Clone() => throw new ObjException("Clone Error");
    }

    public class ObjException : Exception
    {
        public ObjException() { }
        public ObjException(string message) : base(message) { }
        public ObjException(string message, Exception inner) : base(message, inner) { }
    }
}
