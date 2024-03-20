namespace Un.Object
{
    public class Str(string value) : Obj
    {
        public string value = value;

        public override Obj Add(Obj obj) => new Str(value + obj.ToString());

        public override int CompareTo(Obj? obj)
        {
            if (obj is Str s) return value.CompareTo(s.value);

            throw new ObjException("compare Error");
        }

        public override string ToString() => value;
    }
}
