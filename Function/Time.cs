using Un.Object;

namespace Un.Function
{
    public class Time : Obj, IStatic
    {
        public Time()
        {
            Properties.TryAdd("now", new NativeFun("now", "none", Now));
            Properties.TryAdd("today", new NativeFun("today", "none", Today));
            Properties.TryAdd("sleep", new NativeFun("sleep", "milliseconds", Sleep));
        }

        public Date Now(Obj parameter) => new (DateTime.Now);

        public Date Today(Obj parameter) => new (DateTime.Today);

        public Obj Sleep(Obj parameter)
        {
            if (parameter is Iter iter &&
                iter[1] is Int i &&
                i.value.TryInt(out var milliseconds))            
                Thread.Sleep(milliseconds);
            return None;
        }
    }
}
