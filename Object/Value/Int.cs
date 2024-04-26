using Un.Object.Reference;

namespace Un.Object.Value
{    
    public class Int : Val<long>
    {
        public Int() : base("int", 0) { }        

        public Int(long value) : base("int", value) { }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CInt().value;
            return this;
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
            if (obj is Int i) return new Float((double)value % i.value);
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

        public override Bool Equals(Obj obj)
        {
            if (obj is Int i) return new(value == i.value);
            if (obj is Float f) return new(value == f.value);
            return base.Equals(obj);
        }

        public override Bool LessThen(Obj obj)
        {
            if (obj is Int i) return new(value < i.value);
            if (obj is Float f) return new(value < f.value);
            return base.LessThen(obj);
        }

        public override Int CInt() => new(value);

        public override Float CFloat() => new(value);

        public override Bool CBool()
        {
            if (value == 0) return new(false);
            return new(true);
        }

        public override Obj Clone() => new Int(value);

        public override Obj Copy() => new Int(value);
    }
}
