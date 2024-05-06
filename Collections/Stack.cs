using Un.Data;

namespace Un.Collections
{
    public class Stack : Ref<Stack<Obj>>
    {
        public Stack() : base("queue", []) { }

        public override void Init()
        {
            properties.Add("push", new NativeFun("push", -1, para =>
            {
                if (para[0] is not Stack self)
                    throw new ArgumentException("invalid argument", nameof(para));

                for (int i = 1; i < para.Count; i++)
                    value.Push(para[i]);

                return None;
            }));
            properties.Add("push_extend", new NativeFun("push_extend", -1, para =>
            {
                if (para[0] is not Stack self)
                    throw new ArgumentException("invalid argument", nameof(para));


                for (int i = 1; i < para.Count; i++)
                    foreach (var item in para[i].CIter())
                        self.value.Push(item);

                return None;
            }));
            properties.Add("pop", new NativeFun("pop", -1, para =>
            {
                if (para[0] is not Stack self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para.Count == 1) return self.value.Pop();
                else if (para.Count == 2 && para[1] is Int count)
                {
                    Iter objs = [];
                    for (int i = 0; i < count.value; i++)
                        objs.Append(self.value.Pop());
                    return objs;
                }
                throw new ArgumentException("invalid argument", nameof(para));
            }));
        }

        public override Obj Init(Iter args)
        {
            value.Clear();
            return base.Init(args);
        }

        public override Int Len() => new(value.Count);

        public override Bool CBool() => new(value.Count != 0);

        public override Iter CIter() => new([.. value]);

        public override Str CStr() => new($"[{string.Join(", ", value.Select(i => i.CStr().value))}]");

        public override Obj Clone() => new Stack()
        {
            value = value
        };

        public override Obj Copy() => this;

    }
}
