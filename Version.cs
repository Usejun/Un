using Un.Data;

namespace Un
{
    public class Version : Ref<System.Version>
    {
        public Version() : base("version", null) { }

        public Version(System.Version value) : base("version", value) { }

        public override void Init()
        {
            properties.Add("major", new Int(value.Major));
            properties.Add("minor", new Int(value.Minor));
            properties.Add("build", new Int(value.Build));
            properties.Add("revision", new Int(value.Revision));
            properties.Add("major_revision", new Int(value.MajorRevision));
            properties.Add("minor_revision", new Int(value.MinorRevision));
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Version version) return new Bool(value == version.value);
            return base.Equals(obj);
        }

        public override Bool LessThen(Obj obj)
        {
            if (obj is Version version) return new Bool(value < version.value);
            return base.LessThen(obj);
        }
    }
}
