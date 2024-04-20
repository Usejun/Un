namespace Un.Object
{
    public class Bool : Obj
    {
        public bool value;

        public Bool() : base("bool")
        {
            value = false;            
        }

        public Bool(bool value) : base("bool")
        {
            this.value = value;
        }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CBool().value;
            return this;
        }

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            this.value = value switch
            {
                "True" => true,
                "False" => false,
                _ => throw new InvalidOperationException("This is a type that can't be assigned.")
            };
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Bool b)
                this.value = b.value;
            throw new InvalidOperationException("This is a type that can't be assigned.");
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

        public override Str Type() => new("bool");

        public override Str CStr() => new ($"{value}");

        public override Bool CBool() => new(value);

        public override Bool Equals(Obj obj)
        {
            if (obj is Bool b) return new(value == b.value);
            return base.Equals(obj);
        }

        public override Obj Clone() => new Bool(value);

        public override int GetHashCode() => value.GetHashCode();
    }
}
