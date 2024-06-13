﻿namespace Un.Data;

public class Date : Val<DateTime>
{
    public Date() : base("date", DateTime.MinValue) { }

    public Date(DateTime Value) : base("date", Value) { }

    public Date(TimeSpan Value) : base("date", new(Value.Ticks)) { }

    public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : base("date", new(year, month, day, hour, minute, second, milliseconds)) { }

    public override Obj Init(List args)
    {
        if (args[0] is Str str && DateTime.TryParse(str.Value, out var v)) 
            Value = v; 
        else if (args[0] is Int ticks)
            Value = new(ticks.Value);
        else if (args[0] is Date date)
            Value = date.Value;
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override void Init()
    {
        field.Set("year", new NativeFun("year", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Year);
        }));
        field.Set("month", new NativeFun("month", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Month);
        }));
        field.Set("day", new NativeFun("day", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Day);
        }));
        field.Set("hour", new NativeFun("hour", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Hour);
        }));
        field.Set("minute", new NativeFun("minute", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Minute);
        }));
        field.Set("second", new NativeFun("second", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Second);
        }));
        field.Set("milliseconds", new NativeFun("millisecond", 1, args =>
        {
            if (args[0] is not Date self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Millisecond);
        }));
        field.Set("add_years", new NativeFun("add_years", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddYears((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("add_months", new NativeFun("add_months", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddMonths((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("add_days", new NativeFun("add_days", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddDays((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("add_hours", new NativeFun("add_hours", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddHours((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("add_minutes", new NativeFun("add_minutes", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddMinutes((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("add_seconds", new NativeFun("add_seconds", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddSeconds((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("add_milliseconds", new NativeFun("add_milliseconds", 2, args =>
        {
            if (args[0] is Int i)
                return new Date(Value.AddMilliseconds((int)i.Value));
            throw new ValueError("invalid argument");
        }));
        field.Set("format", new NativeFun("format", 2, args =>
        {
            if (args[0] is not Date self || args[1] is not Str format)
                throw new ValueError("invalid argument");

            return new Str(self.Value.ToString(format.Value));
        }));
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Date date) return new Date(Value.Add(new(date.Value.Ticks)));
        if (arg is Str) return CStr().Add(arg);
        return base.Add(arg);
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Date date) return new Date(Value - date.Value);
        return base.Sub(arg);
    }

    public override Str CStr() => new($"{Value}");

    public override Obj Clone() => new Date(Value);

    public override Obj Copy() => new Date(Value);
}
