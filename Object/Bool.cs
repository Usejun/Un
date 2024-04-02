using Un.Function;

namespace Un.Object
{
    public class Bool(bool value) : Obj
    {
        public bool value = value;

        public override void Ass(string value, Dictionary<string, Obj> variable, Dictionary<string, Fun> method)
        {
            this.value = value switch
            {
                "True" => true,
                "False" => false,
                _ => throw new ObjException("Ass Error")
            };
        }

        public override void Ass(Obj value, Dictionary<string, Obj> variable, Dictionary<string, Fun> method)
        {
            if (value is Bool b)
                this.value = b.value;
            else
                throw new ObjException("Ass Error");
        }

        public override string ToString() => $"{value}";

        public override int CompareTo(Obj? obj)
        {
            if (obj is Bool b) return value.CompareTo(b.value);

            throw new ObjException("compare Error");
        }

        public override Obj Clone() => new Bool(value);

    }
}
