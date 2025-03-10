namespace Un.Package;

public class Guid : Obj, IPackage, IStatic
{
    public string Name => "guid";

    public override Str Type() => new(Name);

    public Obj Static()
    {
        Obj guid = new(Name);
        guid.field.Set("get", new NativeFun("get", 0, field => new Str(System.Guid.NewGuid().ToString()), []));
        return guid;
    }
}