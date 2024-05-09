using System.Numerics;
using Un.Collections;

namespace Un.Data
{
    public class Long : Val<BigInteger>
    {
        public Long() : base("long", 0) { }

        public Long(BigInteger value) : base("long", value) { }

        public override Obj Init(Iter arg)
        {
            if (arg[0] is Str s) value = BigInteger.Parse(s.value);
            else if (arg[0] is Int i) value = i.value;
            else if (arg[0] is Float f) value = (long)f.value;
            return this;
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Int i) return new Long(value + i.value);
            if (obj is Long l) return new Long(value + l.value);
            if (obj is Str) return CStr().Add(obj);

            return base.Add(obj);
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Int i) return new Long(value - i.value);
            if (obj is Long l) return new Long(value - l.value);

            return base.Sub(obj);
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int i) return new Long(value * i.value);
            if (obj is Long l) return new Long(value * l.value);

            return base.Mul(obj);
        }

        public override Obj Div(Obj obj)
        {
            if (obj is Int i) return new Long(value / i.value);
            if (obj is Long l) return new Long(value / l.value);

            return base.Div(obj);
        }

        public override Obj IDiv(Obj obj)
        {
            if (obj is Int i) return new Long(value / i.value);
            if (obj is Long l) return new Long(value / l.value);

            return base.IDiv(obj);
        }

        public override Obj Mod(Obj obj)
        {
            if (obj is Int i) return new Long(value % i.value);
            if (obj is Long l) return new Long(value % l.value);

            return base.Mod(obj);
        }

        public override Obj Pow(Obj obj)
        {
            if (obj is Int i) return new Long(BigInteger.Pow(value, (int)i.value));
            if (obj is Long l) return new Long(BigInteger.Pow(value, (int)l.value));

            return base.Pow(obj);
        }

        public override Obj BAnd(Obj obj)
        {
            if (obj is Int i) return new Long(value & i.value);
            if (obj is Long l) return new Long(value & l.value);

            throw new InvalidOperationException();
        }

        public override Obj BOr(Obj obj)
        {
            if (obj is Int i) return new Long(value | i.value);
            if (obj is Long l) return new Long(value | l.value);

            throw new InvalidOperationException();
        }

        public override Obj BXor(Obj obj)
        {
            if (obj is Int i) return new Long(value ^ i.value);
            if (obj is Long l) return new Long(value ^ l.value);

            throw new InvalidOperationException();
        }

        public override Obj BNot() => new Long(~value);

        public override Obj LSh(Obj obj)
        {
            if (obj is Int i) return new Long(value << (int)i.value);
            if (obj is Long l) return new Long(value << (int)l.value);
            throw new InvalidOperationException();
        }

        public override Obj RSh(Obj obj)
        {
            if (obj is Int i) return new Long(value >> (int)i.value);
            if (obj is Long l) return new Long(value >> (int)l.value);

            throw new InvalidOperationException();
        }

        public override Obj And(Obj obj) => new Bool(CBool().value || obj.CBool().value);

        public override Obj Or(Obj obj) => new Bool(CBool().value || obj.CBool().value);

        public override Obj Xor(Obj obj) => new Bool(CBool().value || obj.CBool().value);

        public override Bool Equals(Obj obj)
        {
            if (obj is Int i) return new(value == i.value);
            if (obj is Long l) return new(value == l.value);

            return base.Equals(obj);
        }

        public override Bool LessThen(Obj obj)
        {
            if (obj is Int i) return new(value < i.value);
            if (obj is Long l) return new(value < l.value);
            return base.LessThen(obj);
        }

        public override Int CInt() => new((long)value);

        public override Float CFloat() => new((long)value);

        public override Bool CBool()
        {
            if (value == 0) return new(false);
            return new(true);
        }

        public override Obj Clone() => new Long(value);

        public override Obj Copy() => new Long(value);
    }
}
