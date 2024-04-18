using Un.Function;

namespace Un.Object
{
    public class Times : Obj
    {
        public TimeSpan value;

        public Times() : base("times")
        {
            value = TimeSpan.Zero;
        }

        public Times(TimeSpan value) : base("times")
        {
            this.value = value;
        }

        public Times(long value) : base("times")
        {
            this.value = new TimeSpan(value);
        }

        public override void Init()
        {
            properties.Add("format", new NativeFun("format", Format));

            properties.Add("ticks", new NativeFun("ticks", para => new Int(value.Ticks)));
        }

        public override Obj Init(Iter args)
        {
            if (args[0] is Str s && TimeSpan.TryParse(s.value, out value)) return this;
            if (args[1] is Int i)
            {
                value = new(i.value);
                return this;
            }
            return base.Init(args);
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Times times) this.value = times.value;
            else base.Ass(value, properties);
        }

        public Str Format(Iter arg)
        {
            if (arg[0] is Times times && arg[1] is Str format)
                return new(times.value.ToString(format.value));
            throw new ArgumentException();
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Times times) return new Times(value + times.value);
            return base.Add(obj);
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Times times) return new Times(value - times.value);
            return base.Sub(obj);
        }

        public override Int CInt()
        {
            return new Int(value.Ticks);
        }

        public override Str CStr() => new($"{value}");

        public override Int Hash() => new(value.GetHashCode());

        public override Obj Clone() => new Times(value);
    }
}
