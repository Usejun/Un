namespace Un.Object
{
    public class Float : Obj
    {
        public double value;

        public Float() : base("float")
        {
            value = 0;
        }

        public Float(double value) : base("float")
        {
            this.value = value;
        }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CFloat().value;
            return this;
        }

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (double.TryParse(value, out var v))
                this.value = v;
            throw new InvalidOperationException("This is a type that can't be assigned.");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Float f)
                this.value = f.value;
            throw new InvalidOperationException("This is a type that can't be assigned.");
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Int i) return new Float(value + i.value);
            if (obj is Float f) return new Float(value + f.value);

            return base.Add(obj);
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Int i) return new Float(value - i.value);
            if (obj is Float f) return new Float(value * f.value);

            return base.Sub(obj);
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int i) return new Float(value * i.value);
            if (obj is Float f) return new Float(value * f.value);

            return base.Mul(obj);
        }

        public override Obj Div(Obj obj)
        {
            if (obj is Int i) return new Float(value / i.value);
            if (obj is Float f) return new Float(value / f.value);

            return base.Div(obj);
        }

        public override Obj IDiv(Obj obj)
        {
            if (obj is Int i) return new Int((long)value / i.value);
            if (obj is Float f) return new Int((long)value / (long)f.value);

            return base.IDiv(obj);
        }

        public override Obj Mod(Obj obj)
        {
            if (obj is Int i) return new Float(value % i.value);
            if (obj is Float f) return new Float(value % f.value);

            return base.Mod(obj);
        }

        public override Str Type() => new("float");

        public override Int Hash() => new(value.GetHashCode());

        public override Int CInt() => new((long)value);

        public override Float CFloat() => new(value);

        public override Bool CBool()
        {
            if (value == 0)
                return new(false);
            return new(true);
        }

        public override Str CStr() => new($"{value}"); 

        public override Bool LessThen(Obj obj)
        {
            if (obj is Float f) return new(value < f.value);
            if (obj is Int i) return new(value < i.value);
            return base.LessThen(obj);
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Float f) return new(value == f.value);
            if (obj is Int i) return new(value == i.value);
            return base.LessThen(obj);
        }

        public override Obj Clone() => new Float(value);

    }
}
