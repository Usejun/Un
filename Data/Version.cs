namespace Un;

public class Version : Ref<System.Version>
{
    public Version() : base("version", new System.Version()) { }

    public Version(System.Version Value) : base("version", Value) { }

    public override Obj Init(List args)
    {
        Value = null;
        return this;
    }

    public override void Init()
    {
        field.Set("major", new NativeFun("major", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return Value is null ? None : new Int(Value.Major);
        }));
        field.Set("minor", new NativeFun("minor", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return Value is null ? None : new Int(Value.Minor);
        }));
        field.Set("build", new NativeFun("build", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return Value is null ? None : new Int(Value.Build);
        }));
        field.Set("revision", new NativeFun("revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return Value is null ? None : new Int(Value.Revision);
        }));
        field.Set("major_revision", new NativeFun("major_revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return Value is null ? None : new Int(Value.MajorRevision);
        }));
        field.Set("minor_revision", new NativeFun("minor_revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return Value is null ? None : new Int(Value.MajorRevision);
        }));
    }

    public override Bool Equals(Obj arg)
    {
        if (arg is Version version) return new Bool(Value == version.Value);
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Version version) return new Bool(Value < version.Value);
        return base.LessThen(arg);
    }
}
