namespace Un.Data
{
    public class EnuElm(string type, int value) : Val<int>(type, value)
    {
        public override Str CStr() => new(type);

        public override Int CInt() => new(value);

        public override Bool Equals(Obj obj)
        {
            if (obj is Int i) return new(value == i.value);
            if (obj is EnuElm e && ClassName == e.ClassName) return new(value == e.value);
            
            return base.Equals(obj);
        }

        public override Bool LessThen(Obj obj)
        {
            if (obj is Int i) return new(value < i.value);
            if (obj is EnuElm e && ClassName == e.ClassName) return new(value < e.value);

            return base.LessThen(obj);
        }
    }
}
