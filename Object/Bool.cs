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

        public override Obj Init(Obj obj)
        {
            value = obj.CBool().value;
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

        public override Str Type() => new("bool");

        public override Int Hash() => new(value.GetHashCode());

        public override Str CStr() => new ($"{value}");

        public override Bool CBool() => this;

        public override Int Comp(Obj obj)
        {
            if (obj is Bool b) return new(b.value.CompareTo(value));
            return base.Comp(obj);
        }

        public override Obj Clone() => new Bool(value);

    }
}
