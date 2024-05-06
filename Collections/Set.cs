using Un.Data;

namespace Un.Collections
{
    public class Set : Ref<HashSet<Obj>>
    {
        public Set() : base("set", []) { }

        public override Obj Init(Iter args)
        {
            value.Clear();

            foreach (var arg in args)
                foreach (var item in arg.CIter())
                    value.Add(item);

            return base.Init(args);
        }

        public override void Init()
        {
            properties.Add("add", new NativeFun("add", -1, para =>
            {
                if (para[0] is not Set self)
                    throw new ArgumentException("invalid argument", nameof(para));

                for (int i = 1; i < para.Count; i++)                
                    self.value.Add(para[i]);

                return None;
            }));
            properties.Add("extend", new NativeFun("extend", -1, para =>
            {
                if (para[0] is not Set self)
                    throw new ArgumentException("invalid argument", nameof(para));

                for (int i = 1; i < para.Count; i++)
                    foreach (var item in para[1].CIter())
                        self.value.Add(item);

                return None;
            }));
            properties.Add("remove", new NativeFun("remove", 2, para =>
            {                
                if (para[0] is not Set self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Bool(self.value.Remove(para[1]));
            }));
            properties.Add("contains", new NativeFun("contains", 2, para =>
            {
                if (para[0] is not Set self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Bool(self.value.Contains(para[1]));
            }));
            properties.Add("clear", new NativeFun("clear", 1, para =>
            {
                if (para[0] is not Set self)
                    throw new ArgumentException("invalid argument", nameof(para));

                self.value.Clear();

                return None;
            }));
            properties.Add("values", new NativeFun("values", 1, para =>
            {
                if (para[0] is not Set self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return new Iter([.. self.value]);
            }));
        }

        public override Obj GetItem(Iter para) => new Bool(value.Contains(para[0]));

        public override Obj Add(Obj obj)
        {
            foreach (var item in obj.CIter())
                value.Add(item);            

            return this;
        }

        public override Obj Sub(Obj obj)
        {
            foreach (var item in obj.CIter())
                value.Remove(item);

            return this;
        }

        public override Int Len() => new(value.Count);

        public override Str CStr() => new($"{{{string.Join(", ", value.Select(i => i.CStr().value))}}}");

        public override Obj Clone() => new Set()
        {
            value = value
        };

        public override Obj Copy() => this;
    }
}
