namespace Un.Collections;

public class Queue : Ref<Queue<Obj>>
{
    public Queue() : base("queue", []) { }

    public override Obj Init(Tuple args)
    {
        Value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CList())
                Value.Enqueue(item);

        return this;
    }

    public override void Init()
    {
        field.Set("enqueue", new NativeFun("enqueue", -1, args =>
        {
            if (args[0] is not Queue self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Value.Enqueue(args[i]);                

            return None;
        }));
        field.Set("enqueue_extend", new NativeFun("enqueue_extend", -1, args =>
        {
            if (args[0] is not Queue self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[i].CList())
                    self.Value.Enqueue(item);

            return None;
        }));
        field.Set("dequeue", new NativeFun("dequeue", -1, args =>
        {
            if (args[0] is not Queue self)
                throw new ValueError("invalid argument");

            if (args.Count == 1) return self.Value.Dequeue();
            else if (args.Count == 2 && args[1] is Int count)
            {
                List objs = [];
                for (int i = 0; i < count.Value; i++)
                    objs.Append(self.Value.Dequeue());
                return objs;                   
            }
            throw new ValueError("invalid argument");
        }));
    }

    public override Int Len() => new(Value.Count);

    public override Bool CBool() => new(Value.Count != 0);

    public override List CList() => new([.. Value]);

    public override Str CStr() => new($"[{string.Join(", ", Value.Select(i => i.CStr().Value))}]");

    public override Obj Clone() => new Queue()
    {
        Value = Value
    };

    public override Obj Copy() => this;
}
