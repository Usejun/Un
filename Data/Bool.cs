using Un.Collections;

namespace Un.Data
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

        public override Obj And(Obj obj) => new Bool(value && obj.CBool().value);

        public override Obj Or(Obj obj) => new Bool(value || obj.CBool().value);

        public override Obj Xor(Obj obj) => new Bool(value ^ obj.CBool().value);

        public override Str CStr() => new(value ? "true" : "false");

        public override Bool CBool() => new(value);

        public override Obj Clone() => new Bool(value);

        public override Obj Copy() => new Bool(value);
    }
}
