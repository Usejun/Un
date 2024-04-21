using Un.Object.Reference;

namespace Un.Object.Value
{
    public class Float : Val<double>
    {
        public Float() : base("float", 0) { }

        public Float(double value) : base("float", value) { }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CFloat().value;
            return this;
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Int i) return new Float(value + i.value);
            if (obj is Float f) return new Float(value + f.value);
            if (obj is Str) return CStr().Add(obj);

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

        public override Int CInt() => new((long)value);

        public override Float CFloat() => new(value);

        public override Bool CBool()
        {
            if (value == 0)
                return new(false);
            return new(true);
        }

        public override Obj Clone() => new Float(value);
    }
}
