using Un.Data;

namespace Un.Collections
{
    public class Stack : Ref<Stack<Obj>>
    {
        public Stack() : base("queue", []) { }

        public override void Init()
        {
            properties.Add("push", new NativeFun("push", para =>
            {
                if (para[0] is not Stack self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Iter iter)
                {
                    for (int i = 0; i < iter.Count; i++)
                        self.value.Push(iter[i]);
                }
                else self.value.Push(para[1]);

                return None;
            }));
            properties.Add("pop", new NativeFun("pop", para =>
            {
                if (para[0] is not Stack self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return self.value.Pop();
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
