using Un.Object.Reference;

namespace Un.Object.Value
{
    public class Bool : Val<bool>
    {
        public Bool() : base("bool", false) { }

        public Bool(bool value) : base("bool", value) { }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CBool().value;
            return this;
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Str) return CStr().Add(obj);
            return base.Add(obj);
        }

        public override Obj And(Obj obj)
        {
            if (obj is Bool b)
                return new Bool(value & b.value);
            throw new InvalidOperationException();
        }

        public override Obj Or(Obj obj)
        {
            if (obj is Bool b)
                return new Bool(value || b.value);
            throw new InvalidOperationException();
        }

        public override Obj Xor(Obj obj)
        {
            if (obj is Bool b)
                return new Bool(value ^ b.value);
            throw new InvalidOperationException();
        }

        public override Str CStr() => new(value ? "true" : "false");

        public override Bool CBool() => new(value);

        public override Obj Clone() => new Bool(value);
    }
}
