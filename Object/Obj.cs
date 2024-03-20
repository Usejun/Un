namespace Un.Object
{
    public class Obj : IComparable<Obj>
    {
        public static Obj None => null;

        public virtual Obj Add(Obj obj) => throw new ObjException("Add Error");

        public virtual Obj Sub(Obj obj) => throw new ObjException("Sub Error");

        public virtual Obj Mul(Obj obj) => throw new ObjException("Mul Error");

        public virtual Obj Div(Obj obj) => throw new ObjException("Div Error");

        public virtual Obj IDiv(Obj obj) => throw new ObjException("IDiv Error");

        public virtual Obj Mod(Obj obj) => throw new ObjException("Mod Error");

        public override string ToString() => "None";

        public static Obj Convert(string str)
        {
            if (string.IsNullOrEmpty(str)) return None;
            if (str[0] == '\"' && str[^1] == '\"') return new Str(str.Trim('\"'));
            if (str == "true") return new Bool(true);
            if (str == "false") return new Bool(false);
            if (long.TryParse(str, out var l)) return new Int(l);
            if (double.TryParse(str, out var d)) return new Float(d);

            throw new ObjException("Convert Error");
        }

        public virtual int CompareTo(Obj? obj) => throw new ObjException("compare Error");
    }

    public class ObjException : Exception
    {
        public ObjException() { }
        public ObjException(string message) : base(message) { }
        public ObjException(string message, Exception inner) : base(message, inner) { }
    }
}
