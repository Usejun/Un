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

        public override void Init()
        {
            properties.Add("major", new NativeFun("major", 1, para =>
            {
                if (para[0] is not Version self)
                    throw new ArgumentException(nameof(para));

                return value is null ? None : new Int(value.Major);
            }));
            properties.Add("minor", new NativeFun("minor", 1, para =>
            {
                if (para[0] is not Version self)
                    throw new ArgumentException(nameof(para));

                return value is null ? None : new Int(value.Minor);
            }));
            properties.Add("build", new NativeFun("build", 1, para =>
            {
                if (para[0] is not Version self)
                    throw new ArgumentException(nameof(para));

                return value is null ? None : new Int(value.Build);
            }));
            properties.Add("revision", new NativeFun("revision", 1, para =>
            {
                if (para[0] is not Version self)
                    throw new ArgumentException(nameof(para));

                return value is null ? None : new Int(value.Revision);
            }));
            properties.Add("major_revision", new NativeFun("major_revision", 1, para =>
            {
                if (para[0] is not Version self)
                    throw new ArgumentException(nameof(para));

                return value is null ? None : new Int(value.MajorRevision);
            }));
            properties.Add("minor_revision", new NativeFun("minor_revision", 1, para =>
            {
                if (para[0] is not Version self)
                    throw new ArgumentException(nameof(para));

                return value is null ? None : new Int(value.MajorRevision);
            }));
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
