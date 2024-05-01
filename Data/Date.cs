using Un.Collections;

namespace Un.Data
{
    public class Date : Val<DateTime>
    {
        public Date() : base("date", DateTime.MinValue) { }

        public Date(Times times) : base("date", new DateTime(times.value.Ticks)) { }

        public Date(DateTime value) : base("date", value) { }

        public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : base("date", new(year, month, day, hour, minute, second, milliseconds)) { }

        public Str Format(Iter para)
        {
            if (para[0] is Date date && para[1] is Str format)
                return new(date.value.ToString(format.value));
            throw new ArgumentException("invaild argument");
        }

        public Int Year(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Year);
        }

        public Int Month(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Month);
        }

        public Int Day(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Day);
        }

        public Int Hour(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Hour);
        }

        public Int Minute(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Minute);
        }

        public Int Second(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Second);
        }

        public Int Millisecond(Iter para)
        {
            if (para[0] is not Date date)
                throw new ArgumentException("invaild argument");
            return new(date.value.Millisecond);
        }

        public override void Init()
        {
            properties.Add("year", new NativeFun("year", Year));
            properties.Add("month", new NativeFun("month", Month));
            properties.Add("day", new NativeFun("day", Day));
            properties.Add("hour", new NativeFun("hour", Hour));
            properties.Add("minute", new NativeFun("minute", Minute));
            properties.Add("second", new NativeFun("second", Second));
            properties.Add("milliseconds", new NativeFun("millisecond", Millisecond));
            properties.Add("add_years", new NativeFun("add_years", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int years))
                    return new Date(value.AddYears(years));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_months", new NativeFun("add_months", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int months))
                    return new Date(value.AddMonths(months));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_days", new NativeFun("add_days", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int days))
                    return new Date(value.AddDays(days));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_hours", new NativeFun("add_hours", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int hours))
                    return new Date(value.AddHours(hours));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_minutes", new NativeFun("add_minutes", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int minutes))
                    return new Date(value.AddMinutes(minutes));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_seconds", new NativeFun("add_seconds", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int seconds))
                    return new Date(value.AddSeconds(seconds));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_milliseconds", new NativeFun("add_milliseconds", para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int milliseconds))
                    return new Date(value.AddMilliseconds(milliseconds));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("format", new NativeFun("format", Format));
        }

        public override Obj Init(Iter args)
        {
            if (args[0] is not Str str) throw new InitializationException();
            if (!DateTime.TryParse(str.value, out value)) throw new InitializationException();
            return this;
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Date date) return new Times(value - date.value);
            if (obj is Str) return CStr().Add(obj);
            return base.Add(obj);
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Date date) return new Times(value.Ticks - date.value.Ticks);
            return base.Sub(obj);
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Date date) return new(value == date.value);
            return base.Equals(obj);
        }

        public override Bool LessThen(Obj obj)
        {
            if (obj is Date date) return new(value < date.value);
            return base.LessThen(obj);
        }

        public override Str CStr() => new($"{value}");

        public override Obj Clone() => new Date(value);

        public override Obj Copy() => new Date(value);
    }
}
