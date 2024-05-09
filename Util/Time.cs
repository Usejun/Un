using Un.Collections;
using Un.Data;


namespace Un.Util
{
    public class Time(string packageName) : Pack(packageName), IStatic
    {
        Obj Sleep(Iter paras)
        {
            if (paras[0] is Int i)
                Thread.Sleep((int)i.value);
            return None;
        }

        Date Now(Iter iter) => new(DateTime.Now);

        Date Today(Iter iter) => new(DateTime.Today);

        public override IEnumerable<Fun> Import() =>
        [
            new NativeFun("sleep", 2, Sleep),
        ];

        public Obj Static()
        {
            Time time = new(packageName);
            time.properties.Add("now", new NativeFun("now", 1, Now));
            time.properties.Add("today", new NativeFun("today", 1, Today));
            return time;
        }
    }
}
