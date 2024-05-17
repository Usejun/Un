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
        properties.Add("major", new NativeFun("major", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Major);
        }));
        properties.Add("minor", new NativeFun("minor", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Minor);
        }));
        properties.Add("build", new NativeFun("build", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Build);
        }));
        properties.Add("revision", new NativeFun("revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.Revision);
        }));
        properties.Add("major_revision", new NativeFun("major_revision", 1, args =>
        {
            if (args[0] is not Version self)
                throw new ValueError("invalid argument");

            return value is null ? None : new Int(value.MajorRevision);
        }));
        properties.Add("minor_revision", new NativeFun("minor_revision", 1, args =>
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
