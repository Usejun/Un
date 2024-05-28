namespace Un;

public class Version : Ref<System.Version>
{
    public Version() : base("version", new System.Version()) { }

    public Version(System.Version value) : base("version", value) { }

    public override Obj Init(Iter args)
    {
        value = null;
        return this;
    }

    public override void Init()
    {
        field.Set("major", new NativeFun("major", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Major);
        }));
        field.Set("minor", new NativeFun("minor", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Minor);
        }));
        field.Set("build", new NativeFun("build", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Build);
        }));
        field.Set("revision", new NativeFun("revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Revision);
        }));
        field.Set("major_revision", new NativeFun("major_revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.MajorRevision);
        }));
        field.Set("minor_revision", new NativeFun("minor_revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.MajorRevision);
        }));
    }

    public override Bool Equals(Obj arg)
    {
        if (arg is Version version) return new Bool(value == version.value);
        return base.Equals(arg);
    }

    public override Bool LessThen(Obj arg)
    {
        if (arg is Version version) return new Bool(value < version.value);
        return base.LessThen(arg);
    }
}
