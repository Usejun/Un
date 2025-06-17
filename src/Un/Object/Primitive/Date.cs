using Un.Object.Collections;
using Un.Object.Function;

namespace Un.Object.Primitive;

public class Date(DateTime value) : Val<DateTime>(value, "date")
{
    public Date() : this(DateTime.Now) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Date(),
        { Count: 1 } => args[0] switch
        {
            Str s => DateTime.TryParse(s.Value, out var result) ? new Date(result) : throw new Error("invalid date str"),
            _ => throw new Error($"cannot convert '{args[0].Type}' to 'date'"),
        },
        _ => throw new Error($"cannot convert to 'date'"),
    };

    public override Obj Add(Obj other) => other switch
    {
        Date d => new Date(Value.AddDays(d.Value.Day)),
        Str s => new Str(Value.ToString("yyyy-MM-dd") + s.Value),
        _ => throw new Error($"unsupported operand type(s) for +: 'date' and '{other.Type}'")
    };

    public override Obj Sub(Obj other) => other switch
    {
        Date d => new Date(new DateTime(Value.Subtract(d.Value).Ticks)),
        _ => throw new Error($"unsupported operand type(s) for -: 'date' and '{other.Type}'")
    };

    public override Str ToStr() => new(Value.ToString("yyyy-MM-dd HH:mm:ss.fff"));

    public override Obj Copy() => new Date(Value);

    public override Obj Clone() => new Date(Value);

    public override Attributes GetOriginal() => new()
    {
        { "year", new NFn()
            {
                Name = "year",
                ReturnType = "int",
                Func = _ => new Int(Value.Year)
            }
        },
        { "month", new NFn()
            {
                Name = "month",
                ReturnType = "int",
                Func = _ => new Int(Value.Month)
            }
        },
        { "day", new NFn()
            {
                Name = "day",
                ReturnType = "int",
                Func = _ => new Int(Value.Day)
            }
        },
        { "hour", new NFn()
            {
                Name = "hour",
                ReturnType = "int",
                Func = _ => new Int(Value.Hour)
            }
        },
        { "minute", new NFn()
            {
                Name = "minute",
                ReturnType = "int",
                Func = _ => new Int(Value.Minute)
            }
        },
        { "second", new NFn()
            {
                Name = "second",
                ReturnType = "int",
                Func = _ => new Int(Value.Second)
            }
        },
        { "ms", new NFn()
            {
                Name = "ms",
                ReturnType = "int",
                Func = _ => new Int(Value.Millisecond)
            }
        },
    };
}