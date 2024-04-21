using Un.Object.Value;

namespace Un.Object.Reference
{
    public class Ref<T>(string type, T value) : Obj(type)
    {
        public T value = value;

        public override Str CStr() => new($"{value}"); 

        public override int GetHashCode() => value.GetHashCode();
    }
}
