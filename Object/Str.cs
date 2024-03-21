namespace Un.Object
{
    public class Str(string value) : Obj
    {
        public string value = value;

        public Str this[int index]
        {
            get
            {
                if (OutOfRange(index)) throw new IndexOutOfRangeException();
                return new Str($"{value[index]}");
            }
        }
        
        public Str this[Int i]
        {
            get
            {
                if (!i.value.TryInt(out int index) || OutOfRange(index) ) throw new IndexOutOfRangeException();
                return new Str($"{value[index]}");
            }
        }

        public override Obj Add(Obj obj) => new Str(value + obj.ToString());

        public override int CompareTo(Obj? obj)
        {
            if (obj is Str s) return value.CompareTo(s.value);

            throw new ObjException("compare Error");
        }

        protected bool OutOfRange(int index) => 0 > index || index >= value.Length;

        public override string ToString() => value;
    }
}
