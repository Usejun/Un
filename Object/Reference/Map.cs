using Un.Object.Function;
using Un.Object.Value;

namespace Un.Object.Reference
{
    public class Map : Ref<Obj[]>
    {
        public Map() : base("map", []) { }

        public Map(string type, Obj[] value) : base(type, value) { }

        public override Obj Init(Iter args)
        {
            if (args[0] is not Fun fun) throw new ArgumentException(nameof(args));
            if (args[1] is not Iter iter) throw new ArgumentException(nameof(args));

            value = new Obj[iter.Count];

            for (int i = 0; i < iter.Count; i++)
                value[i] = fun.Call(iter[i].CIter()).Clone();

            return this;
        }

        public override Iter CIter() => new(value);

        public override Str CStr() => new($"[{string.Join(", ", value.Select(i => CStr().value))}]");

        public override Obj Clone() => new Map()
        {
            value = value,
        };
    }
}
