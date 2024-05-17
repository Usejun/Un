namespace Un.Data;

public class EnuElm(string type, int value) : Val<int>(type ?? "enu_elm", value)
{
    public override Str CStr() => new(type);

    public override Int CInt() => new(value);

    public override Bool Equals(Obj arg)
    {
        if (arg is Int i) return new(value == i.value);
        if (arg is EnuElm e && ClassName == e.ClassName) return new(value == e.value);
        
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Int i) return new(value < i.value);
        if (arg is EnuElm e && ClassName == e.ClassName) return new(value < e.value);

        return base.LessThen(arg);
    }
}
