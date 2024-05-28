namespace Un.Collections;

public class Stack : Ref<Stack<Obj>>
{
    public Stack() : base("queue", []) { }

    public override Obj Init(Iter args)
    {
        value.Clear();

        foreach (var arg in args)
            foreach (var item in arg.CIter())
                value.Push(item);

        return this;
    }

    public override void Init()
    {
        field.Set("push", new NativeFun("push", -1, args =>
        {
            if (args[0] is not Stack self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                value.Push(args[i]);

            return None;
        }));
        field.Set("push_extend", new NativeFun("push_extend", -1, args =>
        {
            if (args[0] is not Stack self)
                throw new ValueError("invalid argument");


            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[i].CIter())
                    self.value.Push(item);

            return None;
        }));
        field.Set("pop", new NativeFun("pop", -1, args =>
        {
            if (args[0] is not Stack self)
                throw new ValueError("invalid argument");

            if (args.Count == 1) return self.value.Pop();
            else if (args.Count == 2 && args[1] is Int count)
            {
                Iter objs = [];
                for (int i = 0; i < count.value; i++)
                    objs.Append(self.value.Pop());
                return objs;
            }
            throw new ValueError("invalid argument");
        }));
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
