using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Time(string packageName) : Pack(packageName), IStatic
    {
        Obj Sleep(Iter paras)
        {
            if (paras[0] is Int i &&
                i.value.TryInt(out var milliseconds))
                Thread.Sleep(milliseconds);
            return None;
        }

        Date Now(Iter iter) => new(DateTime.Now);

        Date Today(Iter iter) => new(DateTime.Today);

        public override IEnumerable<Fun> Import() =>
        [
            new NativeFun("sleep", Sleep),
        ];

        public Pack Static()
        {
            Time time = new(packageName);
            time.properties.Add("now", new NativeFun("now", Now));
            time.properties.Add("today", new NativeFun("today", Today));
            return time;
        }
    }
}
