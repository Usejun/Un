using Un.Data;

namespace Un.Collections
{
    public class Queue : Ref<Queue<Obj>>
    {
        public Queue() : base("queue", []) { }

        public override Obj Init(Iter args)
        {
            value.Clear();
            return base.Init(args);
        }

        public override void Init()
        {
            properties.Add("enqueue", new NativeFun("enqueue", para =>
            {
                if (para[0] is not Queue self)
                    throw new ArgumentException("invalid argument", nameof(para));



                if (para[1] is Iter iter)
                {
                    for (int i = 0; i < iter.Count; i++)
                        self.value.Enqueue(iter[i]);
                }
                else self.value.Enqueue(para[1]);

                return None;
            }));
            properties.Add("dequeue", new NativeFun("dequeue", para =>
            {
                if (para[0] is not Queue self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return self.value.Dequeue();
            }));
        }

        public override Int Len() => new(value.Count);

        public override Bool CBool() => new(value.Count != 0);

        public override Iter CIter() => new([.. value]);

        public override Str CStr() => new($"[{string.Join(", ", value.Select(i => i.CStr().value))}]");

        public override Obj Clone() => new Queue()
        {
            value = value
        };

        public override Obj Copy() => this;
    }
}
