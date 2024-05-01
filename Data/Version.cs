using Un.Collections;
using Un.Data;

namespace Un
{
    public class Version : Ref<System.Version>
    {
        public Version() : base("version", null) { }

        public Version(System.Version value) : base("version", value) { }

        public override Obj Init(Iter args)
        {
            value = null;
            return base.Init(args);
        }

        public Obj Major(Iter para) => value is null ? None : new Int(value.Major);

        public Obj Minor(Iter para) => value is null ? None : new Int(value.Minor);

        public Obj Build(Iter para) => value is null ? None : new Int(value.Build);

        public Obj Revision(Iter para) => value is null ? None : new Int(value.Revision);

        public Obj MajorRevision(Iter para) => value is null ? None : new Int(value.MajorRevision);

        public Obj MinorRevision(Iter para) => value is null ? None : new Int(value.MinorRevision);

        public override void Init()
        {
            properties.Add("major", new NativeFun("major", Major));
            properties.Add("minor", new NativeFun("minor", Minor));
            properties.Add("build", new NativeFun("build", Build));
            properties.Add("revision", new NativeFun("revision", Revision));
            properties.Add("major_revision", new NativeFun("major_revision", MajorRevision));
            properties.Add("minor_revision", new NativeFun("minor_revision", MinorRevision));
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
