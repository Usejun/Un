namespace Un.Object
{
    public class Int : Obj
    {
        public long value;

        public Int() : base("int")
        { 
            value = 0;
        }

        public Int(long value) : base("int")
        {
            this.value = value;
        }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CInt().value;
            return this;
        }

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (long.TryParse(value, out var v))
                this.value = v;
            throw new InvalidOperationException("This is a type that can't be assigned.");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Int i)
                this.value = i.value;
            throw new InvalidOperationException("This is a type that can't be assigned.");
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Int i) return new Int(value + i.value);
            if (obj is Float f) return new Float(value + f.value);
            if (obj is Str) return CStr().Add(obj);

            return base.Add(obj);
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Int i) return new Int(value - i.value);
            if (obj is Float f) return new Float(value - f.value);

            return base.Sub(obj);
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int i) return new Int(value * i.value);
            if (obj is Float f) return new Float(value * f.value);

            return base.Mul(obj);
        }

        public override Obj Div(Obj obj)
        {
            if (obj is Int i) return new Float((double)value / i.value);
            if (obj is Float f) return new Float(value / f.value);

            return base.Div(obj);
        }

        public override Obj IDiv(Obj obj)
        {
            if (obj is Int i) return new Int(value / i.value);
            if (obj is Float f) return new Int(value / (long)f.value);

            return base.IDiv(obj);
        }

        public override Obj Mod(Obj obj)
        {
            if (obj is Int i) return new Float(value % (double)i.value);
            if (obj is Float f) return new Float(value % f.value);

            return base.Mod(obj);
        }

        public override Obj And(Obj obj)
        {
            if (obj is Int i)
                return new Int(value & i.value);
            throw new InvalidOperationException();
        }

        public override Obj Or(Obj obj)
        {
            if (obj is Int i)
                return new Int(value | i.value);
            throw new InvalidOperationException();
        }

        public override Obj Xor(Obj obj)
        {
            if (obj is Int i)
                return new Int(value ^ i.value);
            throw new InvalidOperationException();
        }

        public override Str Type() => new("int");

        public override Int Hash() => new(value.GetHashCode());

        public override Int CInt() => this;

        public override Float CFloat() => new(value);

        public override Str CStr() => new ($"{value}");

        public override Bool CBool()
        {
            if (value == 0) return new(false);
            return new(true);
        }

        public override Bool LessThen(Obj obj)
        {
            if (obj is Int i) return new(value < i.value);
            if (obj is Float f) return new(value < f.value);
            return base.LessThen(obj);
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Int i) return new(value == i.value);
            if (obj is Float f) return new(value == f.value);
            return base.LessThen(obj);
        }

        public override Obj Clone() => new Int(value);
    }
}
