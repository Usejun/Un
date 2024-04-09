using Un.Function;

namespace Un.Object
{
    public class Date : Obj
    {
        public DateTime value;

        public Date() 
        {
            value = DateTime.MinValue;
        }

        public Date(DateTime value) : base("date")
        {
            this.value = value;
            Init();
        }

        public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : base("date")
        {
            value = new(year, month, day, hour, minute, second, milliseconds);
            Init();
        }

        public override void Init()
        {
            properties.TryAdd("year", new Int(value.Year));
            properties.TryAdd("month", new Int(value.Month));
            properties.TryAdd("days", new Int(value.Day));
            properties.TryAdd("hour", new Int(value.Hour));
            properties.TryAdd("minute", new Int(value.Minute)); 
            properties.TryAdd("second", new Int(value.Second));
            properties.TryAdd("milliseconds", new Int(value.Millisecond));
            properties.TryAdd("add_years", new NativeFun("add_years", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int years))                
                    return new Date(value.AddYears(years));
                return None;
            }));
            properties.TryAdd("add_months", new NativeFun("add_months", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int months))
                    return new Date(value.AddMonths(months));
                return None;
            }));
            properties.TryAdd("add_days", new NativeFun("add_days", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int days))
                    return new Date(value.AddDays(days));
                return None;
            }));
            properties.TryAdd("add_hours", new NativeFun("add_hours", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int hours))
                    return new Date(value.AddHours(hours));
                return None;
            }));
            properties.TryAdd("add_minutes", new NativeFun("add_minutes", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int minutes))
                    return new Date(value.AddMinutes(minutes));
                return None;
            }));
            properties.TryAdd("add_seconds", new NativeFun("add_seconds", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int seconds))
                    return new Date(value.AddSeconds(seconds));
                return None;
            }));
            properties.TryAdd("add_milliseconds", new NativeFun("add_milliseconds", (obj) =>
            {
                if (obj is Int i && i.value.TryInt(out int milliseconds))
                    return new Date(value.AddMilliseconds(milliseconds));
                return None;
            }));
        }

        public override Obj Init(Obj obj)
        {
            if (obj is not Str str) throw new InitializationException();
            if (!DateTime.TryParse(str.value, out value)) throw new InitializationException();
            return this;
        }

        public override Str CStr() => new($"{value:yyyy:MM:dd HH:mm:ss:ffff}");

        public override Int Hash() => new(value.GetHashCode());
    }
}
