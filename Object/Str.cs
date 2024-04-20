namespace Un.Object
{
    public class Str : Obj
    {
        public string value;

        public Str() : base("str") 
        {
            value = "";
        }

        public Str(string value) : base("str")
        {
            this.value = value;
        }

        public Str(char value) : base("str")
        {
            this.value = $"{value}";
        }

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

        public override Obj Init(Iter arg)
        {
            value = arg[0].CStr().value;
            return this;
        }

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (value[0] == '\"' && value[^1] == '\"')
                this.value = value;
            else throw new InvalidOperationException("This is a type that can't be assigned.");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Str s)
                this.value = s.value;
            throw new InvalidOperationException("This is a type that can't be assigned.");
        }

        public override Obj Add(Obj obj) => new Str(value + obj.CStr().value);

        public override Str Type() => new ("str");

        public override Int CInt()
        {
            if (long.TryParse(value, out var l))
                return new(l);
            return base.CInt();
        }

        public override Float CFloat()
        {
            if (double.TryParse(value, out var d))
                return new(d);
            return base.CFloat();
        }

        public override Bool CBool()
        {
            if (value == "True") return new(true);
            if (value == "False") return new(false);
            return base.CBool();
        }

        public override Iter CIter()
        {
            Iter iter = new();

            foreach (char c in value)            
                iter.Append(new Str(c));            

            return iter;
        }

        public override Str CStr() => new(value);

        public override Int Len() => new(value.Length);

        public override Bool LessThen(Obj obj)
        {
            if (obj is Str s) return new(value.CompareTo(s.value) < 0);
            return base.LessThen(obj);
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Str s) return new(value.CompareTo(s.value) == 0);
            return base.Equals(obj);
        }

        protected bool OutOfRange(int index) => 0 > index || index >= value.Length;

        public override Obj Clone() => new Str(value);

        public override Obj GetByIndex(Iter para)
        {
            if (para[0] is not Int i || !i.value.TryInt(out int index) || OutOfRange(index)) throw new IndexOutOfRangeException();
            return new Str($"{value[index]}");
        }

        public override int GetHashCode() => value.GetHashCode();
    }
}
