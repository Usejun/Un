using Un.Collections;

namespace Un.Data
{
    public class Str : Val<string>
    {
        public Str() : base("str", "") { }

        public Str(string value) : base("str", value) { }

        public Str(char value) : base("str", $"{value}") { }

        public Str this[int index]
        {
            get
            {
                if (OutOfRange(index)) throw new IndexOutOfRangeException();
                return new Str($"{value[index]}");
            }
        }

        public Str this[Int i]
        {
            get
            {
                if (!i.value.TryInt(out int index) || OutOfRange(index)) throw new IndexOutOfRangeException();
                return new Str($"{value[index]}");
            }
        }

        public override void Init()
        {
            properties.Add("split", new NativeFun("split", 2, para =>
            {
                if (para[0] is not Str self || para[1] is not Str sep)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Iter(self.value.Split(sep.value));
            }));
        }

        public override Obj Init(Iter arg)
        {
            value = arg[0].CStr().value;
            return this;
        }

        public override Obj Add(Obj obj) => new Str(value + obj.CStr().value);

        public override Int CInt()
        {
            if (long.TryParse(value, out var l))
                return new(l);
            return base.CInt();
        }

        public override Float CFloat()
        {
            if (decimal.TryParse(value, out var d))
                return new((double)d);
            return base.CFloat();
        }

        public override Bool CBool()
        {
            if (value == "true") return new(true);
            if (value == "false") return new(false);
            return new(string.IsNullOrWhiteSpace(value));
        }

        public override Iter CIter()
        {
            Iter iter = [];

            foreach (char c in value)
                iter.Append(new Str(c));

            return iter;
        }

        public override Int Len() => new(value.Length);

        public override Obj GetItem(Iter para)
        {
            if (para[0] is not Int i || !i.value.TryInt(out int index) || OutOfRange(index)) throw new IndexOutOfRangeException();
            return new Str($"{value[index]}");
        }

        public override Obj Clone() => new Str(value) { };

        public override Obj Copy() => new Str(value);

        bool OutOfRange(int index) => 0 > index || index >= value.Length;
    }
}
