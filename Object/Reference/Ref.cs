using Un.Object.Value;

namespace Un.Object.Reference
{
    public class Ref<T>(string type, T value) : Obj(type)
    {
        public T value = value;

        public override Str CStr() => new($"{value}");

        public override Iter CIter() => new([Copy()]);

        public override int GetHashCode() => value.GetHashCode();

        public override Obj Copy() => this;
    }
}
