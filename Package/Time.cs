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

        public override IEnumerable<Fun> Import() =>
        [
            new NativeFun("sleep", Sleep),
        ];

        public Pack Static()
        {
            Time time = new(packageName);
            time.properties.Add("now", new Date(DateTime.Now));
            time.properties.Add("today", new Date(DateTime.Today));
            return time;
        }
    }
}
