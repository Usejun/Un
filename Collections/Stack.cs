namespace Un.Collections;

public class Stack : Ref<Stack<Obj>>
{
    public Stack() : base("queue", []) { }

    public override Obj Init(List args)
    {
        Value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CList())
                Value.Push(item);

        return this;
    }

    public override void Init()
    {
        field.Set("push", new NativeFun("push", -1, args =>
        {
            if (args[0] is not Stack self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                Value.Push(args[i]);

            return None;
        }));
        field.Set("push_extend", new NativeFun("push_extend", -1, args =>
        {
            if (args[0] is not Stack self)
                throw new ValueError("invalid argument");


            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[i].CList())
                    self.Value.Push(item);

            return None;
        }));
        field.Set("pop", new NativeFun("pop", -1, args =>
        {
            if (args[0] is not Stack self)
                throw new ValueError("invalid argument");

            if (args.Count == 1) return self.Value.Pop();
            else if (args.Count == 2 && args[1] is Int count)
            {
                List objs = [];
                for (int i = 0; i < count.Value; i++)
                    objs.Append(self.Value.Pop());
                return objs;
            }
            throw new ValueError("invalid argument");
        }));
    }

    public override Int Len() => new(Value.Count);

    public override Bool CBool() => new(Value.Count != 0);

    public override List CList() => new([.. Value]);

    public override Str CStr() => new($"[{string.Join(", ", Value.Select(i => i.CStr().Value))}]");

    public override Obj Clone() => new Stack()
    {
        Value = Value
    };

    public override Obj Copy() => this;

}
