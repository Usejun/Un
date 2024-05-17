namespace Un.Data;

public class Date : Val<DateTime>
{
    public Date() : base("date", DateTime.MinValue) { }

    public Date(DateTime value) : base("date", value) { }

    public Date(TimeSpan value) : base("date", new(value.Ticks)) { }

    public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : base("date", new(year, month, day, hour, minute, second, milliseconds)) { }

    public override Obj Init(Iter args)
    {
        if (args[0] is Str str && DateTime.TryParse(str.value, out value)) { }
        else if (args[0] is Int ticks)
            value = new(ticks.value);
        else if (args[0] is Date date)
            value = date.value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override void Init()
    {
        properties.Add("year", new NativeFun("year", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Year);
        }));
        properties.Add("month", new NativeFun("month", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Month);
        }));
        properties.Add("day", new NativeFun("day", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Day);
        }));
        properties.Add("hour", new NativeFun("hour", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Hour);
        }));
        properties.Add("minute", new NativeFun("minute", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Minute);
        }));
        properties.Add("second", new NativeFun("second", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Second);
        }));
        properties.Add("milliseconds", new NativeFun("millisecond", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.value.Millisecond);
        }));
        properties.Add("add_years", new NativeFun("add_years", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddYears((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("add_months", new NativeFun("add_months", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddMonths((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("add_days", new NativeFun("add_days", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddDays((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("add_hours", new NativeFun("add_hours", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddHours((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("add_minutes", new NativeFun("add_minutes", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddMinutes((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("add_seconds", new NativeFun("add_seconds", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddSeconds((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("add_milliseconds", new NativeFun("add_milliseconds", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(value.AddMilliseconds((int)i.value));
            throw new ValueError("invalid argument");
        }));
        properties.Add("format", new NativeFun("format", 2, args =>
        {
            if (args[0] is not Date self || args[1] is not Str format)
                throw new ValueError("invalid argument");

            return new Str(self.value.ToString(format.value));
        }));
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Date date) return new Date(value.Add(new(date.value.Ticks)));
        if (arg is Str) return CStr().Add(arg);
        return base.Add(arg);
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Date date) return new Date(value - date.value);
        return base.Sub(arg);
    }

    public override Str CStr() => new($"{value}");

    public override Obj Clone() => new Date(value);

    public override Obj Copy() => new Date(value);
}
