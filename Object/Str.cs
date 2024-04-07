namespace Un.Object
{
    public class Str(string value) : Obj("str"), IIndexable
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

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (value[0] == '\"' && value[^1] == '\"')
                this.value = value;
            else throw new ObjException("Ass Error");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Str s)
                this.value = s.value;
            else
                throw new ObjException("Ass Error");
        }

        public override Obj Add(Obj obj) => new Str(value + obj.CStr().value);

        public override Str Type() => new ("str");

        public override Int Hash() => new (value.GetHashCode());

        public override Int CInt()
        {
            if (long.TryParse(value, out var l))
                return new(l);
            throw new ObjException("Int Error");
        }

        public override Float CFloat()
        {
            if (double.TryParse(value, out var d))
                return new(d);
            throw new ObjException("Float Error");
        }

        public override Bool CBool()
        {
            if (value == "True") return new(true);
            if (value == "False") return new(false);
            throw new ObjException("Bool Error");
        }

        public override Iter CIter()
        {
            return new Iter(value, Process.Properties);
        }

        public override Str CStr() => this;

        public override Int Len() => new(value.Length);

        public override Int Comp(Obj obj)
        {
            if (obj is Str s) return new(s.value.CompareTo(value));
            throw new ObjException("Comp Error");
        }

        protected bool OutOfRange(int index) => 0 > index || index >= value.Length;

        public override Obj Clone() => new Str(value);

        public Obj GetByIndex(Obj obj)
        {
            if (obj is not Int i || !i.value.TryInt(out int index) || OutOfRange(index)) throw new IndexOutOfRangeException();
            return new Str($"{value[index]}");
        }

        public Obj SetByIndex(Obj obj)
        {
            throw new ObjException("Set Error");
        }
    }
}
