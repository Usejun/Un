namespace Un.Data;

public class EnuElm(string type, int Value) : Val<int>(type ?? "enu_elm", Value)
{
    public override Str CStr() => new(type);

    public override Int CInt() => new(Value);

    public override Bool Eq(Obj arg, Field field) => arg switch
    {
        Int i => new(Value == i.Value),
        EnuElm e => ClassName == e.ClassName ? new(Value == e.Value) : base.Eq(e, field),
        _ => base.Eq(arg, field),
    };


    public override Bool Lt(Obj arg, Field field) => arg switch
    {
        Int i => new(Value < i.Value),
        EnuElm e => ClassName == e.ClassName ? new(Value < e.Value) : base.Eq(e, field),
        _ => base.Eq(arg, field),
    };
}
