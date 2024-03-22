namespace Un.Object
{
    public class Float(double value) : Obj
    {
        public double value = value;

        public override void Ass(string value)
        {
            if (double.TryParse(value, out var v))
                this.value = v;
            else throw new ObjException("Ass Error");
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Int i) return new Float(value + i.value);
            if (obj is Float f) return new Float(value + f.value);

            throw new ObjException("Add Error");
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Int i) return new Float(value - i.value);
            if (obj is Float f) return new Float(value * f.value);

            throw new ObjException("Sub Error");
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int i) return new Float(value * i.value);
            if (obj is Float f) return new Float(value * f.value);

            throw new ObjException("Mul Error");
        }

        public override Obj Div(Obj obj)
        {
            if (obj is Int i) return new Float(value / i.value);
            if (obj is Float f) return new Float(value / f.value);

            throw new ObjException("Div Error");
        }

        public override Obj IDiv(Obj obj)
        {
            if (obj is Int i) return new Int((long)value / i.value);
            if (obj is Float f) return new Int((long)value / (long)f.value);

            throw new ObjException("IDiv Error");
        }

        public override Obj Mod(Obj obj)
        {
            if (obj is Int i) return new Float(value % i.value);
            if (obj is Float f) return new Float(value % f.value);

            throw new ObjException("Mod Error");
        }

        public override int CompareTo(Obj? obj)
        {
            if (obj is Int i) return value.CompareTo(i.value);
            if (obj is Float f) return value.CompareTo(f.value);

            throw new ObjException("compare Error");
        }

        public override string ToString() => $"{value}";

        public override Obj Clone() => new Float(value);

    }
}
