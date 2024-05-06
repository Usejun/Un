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
            properties.Add("enqueue", new NativeFun("enqueue", -1, para =>
            {
                if (para[0] is not Queue self)
                    throw new ArgumentException("invalid argument", nameof(para));

                for (int i = 1; i < para.Count; i++)
                    self.value.Enqueue(para[i]);                

                return None;
            }));
            properties.Add("enqueue_extend", new NativeFun("enqueue_extend", -1, para =>
            {
                if (para[0] is not Queue self)
                    throw new ArgumentException("invalid argument", nameof(para));

                for (int i = 1; i < para.Count; i++)
                    foreach (var item in para[i].CIter())
                        self.value.Enqueue(item);

                return None;
            }));
            properties.Add("dequeue", new NativeFun("dequeue", -1, para =>
            {
                if (para[0] is not Queue self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para.Count == 1) return self.value.Dequeue();
                else if (para.Count == 2 && para[1] is Int count)
                {
                    Iter objs = [];
                    for (int i = 0; i < count.value; i++)
                        objs.Append(self.value.Dequeue());
                    return objs;                   
                }
                throw new ArgumentException("invalid argument", nameof(para));
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
