using Un.Function;
using Un.Supporter;

namespace Un.Object
{
    public class Date : Obj
    {
        public DateTime value;

        public Date() : base("date")
        {
            value = DateTime.MinValue;
        }

        public Date(DateTime value) : base("date")
        {
            this.value = value;
            properties["year"] = new Int(value.Year);
            properties["month"] = new Int(value.Month);
            properties["day"] = new Int(value.Day);
            properties["hour"] = new Int(value.Hour);
            properties["minute"] = new Int(value.Minute);
            properties["second"] = new Int(value.Second);
            properties["milliseconds"] = new Int(value.Millisecond);
        }

        public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : base("date")
        {
            value = new(year, month, day, hour, minute, second, milliseconds);
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Date date) this.value = date.value;
            else base.Ass(value, properties);
        }

        public Str Format(Iter arg)
        {
            if (arg[0] is Date date && arg[1] is Str format)
                return new(date.value.ToString(format.value));
            throw new ArgumentException();
        }

        public override void Init()
        {
            properties.Add("year", new Int(value.Year));
            properties.Add("month", new Int(value.Month));
            properties.Add("days", new Int(value.Day));
            properties.Add("hour", new Int(value.Hour));
            properties.Add("minute", new Int(value.Minute)); 
            properties.Add("second", new Int(value.Second));
            properties.Add("milliseconds", new Int(value.Millisecond));
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

        public override Str CStr() => new($"{value:yyyy:MM:dd HH:mm:ss:ffff}");

        public override Int Hash() => new(value.GetHashCode());

        public override Bool LessThen(Obj obj)
        {
            if (obj is Date date) return new(value < date.value);
            return base.LessThen(obj);
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Date date) return new(value.CompareTo(date.value) == 0);
            return base.Equals(obj);
        }

        public override Obj Clone() => new Date(value);

    }
}
