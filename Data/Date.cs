using Un.Collections;

namespace Un.Data
{
    public class Date : Val<DateTime>
    {
        public Date() : base("date", DateTime.MinValue) { }

        public Date(DateTime value) : base("date", value) { }

        public Date(TimeSpan value) : base("date", new(value.Ticks)) { }

        public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : base("date", new(year, month, day, hour, minute, second, milliseconds)) { }

        public override void Init()
        {
            properties.Add("year", new NativeFun("year", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Year);
            }));
            properties.Add("month", new NativeFun("month", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Month);
            }));
            properties.Add("day", new NativeFun("day", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Day);
            }));
            properties.Add("hour", new NativeFun("hour", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Hour);
            }));
            properties.Add("minute", new NativeFun("minute", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Minute);
            }));
            properties.Add("second", new NativeFun("second", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Second);
            }));
            properties.Add("milliseconds", new NativeFun("millisecond", 1, para =>
            {
                if (para[0] is not Date self)
                    throw new ArgumentException("invaild argument");

                return new Int(self.value.Millisecond);
            }));
            properties.Add("add_years", new NativeFun("add_years", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int years))
                    return new Date(value.AddYears(years));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_months", new NativeFun("add_months", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int months))
                    return new Date(value.AddMonths(months));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_days", new NativeFun("add_days", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int days))
                    return new Date(value.AddDays(days));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_hours", new NativeFun("add_hours", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int hours))
                    return new Date(value.AddHours(hours));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_minutes", new NativeFun("add_minutes", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int minutes))
                    return new Date(value.AddMinutes(minutes));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_seconds", new NativeFun("add_seconds", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int seconds))
                    return new Date(value.AddSeconds(seconds));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("add_milliseconds", new NativeFun("add_milliseconds", 2, para =>
            {
                if (para[0] is Int i && i.value.TryInt(out int milliseconds))
                    return new Date(value.AddMilliseconds(milliseconds));
                throw new ArgumentException("Invalid Parameter", nameof(para));
            }));
            properties.Add("format", new NativeFun("format", 2, para =>
            {
                if (para[0] is not Date self || para[1] is not Str format)
                    throw new ArgumentException("invaild argument");

                return new Str(self.value.ToString(format.value));
            }));
        }

        public override Obj Init(Iter args)
        {
            if (args[0] is Str str && DateTime.TryParse(str.value, out value)) return this;
            if (args[0] is Int ticks)
            {
                value = new(ticks.value);
                return this;
            }
            if (args[0] is Date date)
            {
                value = date.value;
                return this;
            }
            throw new InitializationException();
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Date date) return new Date(value.Add(new(date.value.Ticks)));
            if (obj is Str) return CStr().Add(obj);
            return base.Add(obj);
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Date date) return new Date(value - date.value);
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
