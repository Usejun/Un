namespace Un.Collections;

public class Queue : Ref<Queue<Obj>>
{
    public Queue() : base("queue", []) { }

    public override Obj Init(Iter args)
    {
        value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CIter())
                value.Enqueue(item);

        return this;
    }

    public override void Init()
    {
        field.Set("enqueue", new NativeFun("enqueue", -1, args =>
        {
            if (args[0] is not Queue self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.value.Enqueue(args[i]);                

            return None;
        }));
        field.Set("enqueue_extend", new NativeFun("enqueue_extend", -1, args =>
        {
            if (args[0] is not Queue self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[i].CIter())
                    self.value.Enqueue(item);

            return None;
        }));
        field.Set("dequeue", new NativeFun("dequeue", -1, args =>
        {
            if (args[0] is not Queue self)
                throw new ValueError("invalid argument");

            if (args.Count == 1) return self.value.Dequeue();
            else if (args.Count == 2 && args[1] is Int count)
            {
                Iter objs = [];
                for (int i = 0; i < count.value; i++)
                    objs.Append(self.value.Dequeue());
                return objs;                   
            }
            throw new ValueError("invalid argument");
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
