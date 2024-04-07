﻿namespace Un.Object
{
    public class Float(double value) : Obj("float")
    {
        public double value = value;

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (double.TryParse(value, out var v))
                this.value = v;
            else throw new ObjException("Ass Error");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Float f)
                this.value = f.value;
            else
                throw new ObjException("Ass Error");
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

        public override Int Comp(Obj obj)
        {
            if (obj is Float f) return new(f.value.CompareTo(value));
            throw new ObjException("Comp Error");
        }

        public override Obj Clone() => new Float(value);

    }
}
