namespace Un.Data;

public class EnuElm(string type, int Value) : Val<int>(type ?? "enu_elm", Value)
{
    public override Str CStr() => new(type);

    public override Int CInt() => new(Value);

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg is Int i) return new(Value == i.Value);
        if (arg is EnuElm e && ClassName == e.ClassName) return new(Value == e.Value);
        
        return base.Eq(arg, field);
    }

    public override Bool Lt(Obj arg, Field field)
    {
        if (arg is Int i) return new(Value < i.Value);
        if (arg is EnuElm e && ClassName == e.ClassName) return new(Value < e.Value);

        return base.Lt(arg, field);
    }
}
