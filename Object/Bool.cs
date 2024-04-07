namespace Un.Object
{
    public class Bool(bool value) : Obj("bool")
    {
        public bool value = value;

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            this.value = value switch
            {
                "True" => true,
                "False" => false,
                _ => throw new ObjException("Ass Error")
            };
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Bool b)
                this.value = b.value;
            else
                throw new ObjException("Ass Error");
        }

        public override Str Type() => new("bool");

        public override Int Hash() => new(value.GetHashCode());

        public override Str CStr() => new ($"{value}");

        public override Bool CBool() => this;

        public override Int Comp(Obj obj)
        {
            if (obj is Bool b) return new(b.value.CompareTo(value));
            throw new ObjException("Comp Error");
        }

        public override Obj Clone() => new Bool(value);

    }
}
