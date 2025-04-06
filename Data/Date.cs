namespace Un.Data;

public class Date : Val<DateTime>
{
    public Date() : base("date", DateTime.MinValue) { }

    public Date(DateTime value) : base("date", value) { }

    public Date(TimeSpan value) : base("date", new(value.Ticks)) { }

    public Date(int year = 0, int month = 0, int day = 0, int hour = 0, int minute = 0, int second = 0, int milliseconds = 0) : 
           base("date", new(year, month, day, hour, minute, second, milliseconds)) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", null!)]);
        var value = field["value"];

        Value = value switch
        {
            Str s => DateTime.Parse(s.Value),
            Int ticks => new(ticks.Value),
            Date d => new(d.Value.Ticks),
            _ => throw new ClassError(),
        };
        
        return this;
    }

    public override void Init()
    {
        field.Set("year", new NativeFun("year", 0, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.Year);
        }, []));
        field.Set("month", new NativeFun("month", 0, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.Month);
        }, []));
        field.Set("day", new NativeFun("day", 0, field =>
        {
            if (field[Literals.Self]        is not Date self)
                throw new ArgumentError();

            return new Int(self.Value.Day);
        }, []));
        field.Set("hour", new NativeFun("hour", 0, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.Hour);
        }, []));
        field.Set("minute", new NativeFun("minute", 0, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.Minute);
        }, []));
        field.Set("second", new NativeFun("second", 0, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.Second);
        }, []));
        field.Set("milliseconds", new NativeFun("millisecond", 0, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self))
                throw new ArgumentError();

            return new Int(self.Value.Millisecond);
        }, []));
        field.Set("add_years", new NativeFun("add_years", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) || 
                !field["year"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddYears((int)i.Value));
        }, [("year", null!)]));
        field.Set("add_months", new NativeFun("add_months", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) || 
                !field["month"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddMonths((int)i.Value));
        }, [("month", null!)]));
        field.Set("add_days", new NativeFun("add_days", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) ||
                !field["day"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddDays((int)i.Value));
        }, [("day", null!)]));
        field.Set("add_hours", new NativeFun("add_hours", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) ||
                !field["hour"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddHours((int)i.Value));
        }, [("hour", null!)]));
        field.Set("add_minutes", new NativeFun("add_minutes", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) ||
                !field["minute"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddMinutes((int)i.Value));
        }, [("minute", null!)]));
        field.Set("add_seconds", new NativeFun("add_seconds", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) || 
                !field["seconds"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddSeconds((int)i.Value));
        }, [("seconds", null!)]));
        field.Set("add_milliseconds", new NativeFun("add_milliseconds", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) ||
                !field["milliseconds"].As<Int>(out var i))
                throw new ArgumentError();

            return new Date(Value.AddMilliseconds((int)i.Value));
        }, [("milliseconds", null!)]));
        field.Set("format", new NativeFun("format", 1, field =>
        {
            if (!field[Literals.Self].As<Date>(out var self) ||
                !field["format"].As<Str>(out var format))
                throw new ArgumentError();

            return new Str(self.Value.ToString(format.Value));
        }, [("format", null!)]));
    }

    public override Obj Add(Obj arg, Field field)
    {
        if (arg is Date date) return new Date(Value.Add(new(date.Value.Ticks)));
        if (arg is Str) return CStr().Add(arg, field);
        return base.Add(arg, field);
    }

    public override Obj Sub(Obj arg, Field field)
    {
        if (arg is Date date) return new Date(Value - date.Value);
        return base.Sub(arg, field);
    }

    public override Str CStr() => new($"{Value}");

    public override Obj Clone() => new Date(Value);

    public override Obj Copy() => new Date(Value);
}
