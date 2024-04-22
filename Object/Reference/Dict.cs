using Un.Object.Function;
using Un.Object.Value;

namespace Un.Object.Reference
{
    public class Dict : Ref<Dictionary<Obj, Obj>>
    {
        public Dict() : base("dict", []) { }

        public override void Init()
        {
            properties.Add("add", new NativeFun("add", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                self.value.Add(para[1], para[2]);

                return self;
            }));
            properties.Add("remove", new NativeFun("remove", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Bool(self.value.Remove(para[1]));
            }));
            properties.Add("contains_key", new NativeFun("contains_key", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Bool(self.value.ContainsKey(para[1]));
            }));
            properties.Add("contains_value", new NativeFun("contains_value", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Bool(self.value.ContainsValue(para[1]));
            }));
            properties.Add("clear", new NativeFun("clear", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                self.value.Clear();

                return None;
            }));
            properties.Add("keys", new NativeFun("keys", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Iter([.. self.value.Keys]);
            }));
            properties.Add("values", new NativeFun("values", para =>
            {
                if (para[0] is not Dict self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Iter([.. self.value.Values]);
            }));
        }

        public override Obj GetItem(Iter para) => value[para[0]];

        public override Obj SetItem(Iter para) => value[para[0]] = para[1];

        public override Str CStr() => new($"{{{string.Join(", ", value.Select(i => $"{i.Key.CStr().value}:{i.Value.CStr().value}"))}}}");

        public override Obj Clone() => new Dict() { value = value };

        public override Obj Copy() => this;
    }
}
