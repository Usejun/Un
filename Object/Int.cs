using Un.Function;

namespace Un.Object
{
    public class Int(long value) : Obj
    {
        public long value = value;

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (long.TryParse(value, out var v))
                this.value = v;
            else throw new ObjException("Ass Error");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Int i)
                this.value = i.value;
            else
                throw new ObjException("Ass Error");
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Int i) return new Int(value + i.value);
            if (obj is Float f) return new Float(value + f.value);
            if (obj is Str s) return new Str($"{value}{s.value}");

            throw new ObjException("Add Error");
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Int i) return new Int(value - i.value);
            if (obj is Float f) return new Float(value - f.value);

            throw new ObjException("Sub Error");
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int i) return new Int(value * i.value);
            if (obj is Float f) return new Float(value * f.value);

            throw new ObjException("Mul Error");
        }

        public override Obj Div(Obj obj)
        {
            if (obj is Int i) return new Float((double)value / i.value);
            if (obj is Float f) return new Float(value / f.value);

            throw new ObjException("Div Error");
        }

        public override Obj IDiv(Obj obj)
        {
            if (obj is Int i) return new Int(value / i.value);
            if (obj is Float f) return new Int(value / (long)f.value);

            throw new ObjException("IDiv Error");
        }

        public override Obj Mod(Obj obj)
        {
            if (obj is Int i) return new Float(value % (double)i.value);
            if (obj is Float f) return new Float(value % f.value);

            throw new ObjException("Mod Error");
        }

        public override Str Type() => new("int");

        public override Int CInt() => this;

        public override Float CFloat() => new(value);

        public override Str CStr() => new ($"{value}");

        public override Bool CBool()
        {
            if (value == 0) return new(false);
            return new(true);
        }

        public override int CompareTo(Obj? obj)
        {
            if (obj is Int i) return value.CompareTo(i.value);
            if (obj is Float f) return value.CompareTo(f.value);

            throw new ObjException("compare Error");
        }

        public override Obj Clone() => new Int(value);
    }

}
