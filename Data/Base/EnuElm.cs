namespace Un.Data;

public class EnuElm(string type, int Value) : Val<int>(type ?? "enu_elm", Value)
{
    public override Str CStr() => new(type);

    public override Int CInt() => new(Value);

    public override Bool Equals(Obj arg)
    {
        if (arg is Int i) return new(Value == i.Value);
        if (arg is EnuElm e && ClassName == e.ClassName) return new(Value == e.Value);
        
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Int i) return new(Value < i.Value);
        if (arg is EnuElm e && ClassName == e.ClassName) return new(Value < e.Value);

        return base.LessThen(arg);
    }
}
