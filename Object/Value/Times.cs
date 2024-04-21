using Un.Object.Function;
using Un.Object.Reference;

namespace Un.Object.Value
{
    public class Times : Val<TimeSpan>
    {
        public Times() : base("times", TimeSpan.Zero) { }

        public Times(TimeSpan value) : base("times", value) { }

        public Times(long value) : base("times", new TimeSpan(value)) { }

        public Str Format(Iter arg)
        {
            if (arg[0] is Times times && arg[1] is Str format)
                return new(times.value.ToString(format.value));
            throw new ArgumentException();
        }

        public override void Init()
        {
            properties.Add("format", new NativeFun("format", Format));

            properties.Add("ticks", new NativeFun("ticks", para => new Int(value.Ticks)));
        }

        public override Obj Init(Iter args)
        {
            if (args[0] is Str s && TimeSpan.TryParse(s.value, out value)) return this;
            if (args[0] is Int i)
            {
                value = new(i.value);
                return this;
            }
            return base.Init(args);
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

        public override Int CInt() => new(value.Ticks);

        public override Obj Clone() => new Times(value);
    }
}
