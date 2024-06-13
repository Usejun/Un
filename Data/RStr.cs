using System.Text;

namespace Un.Data
{
    public class RStr : Ref<StringBuilder>
    {
        public RStr() : base("rstr", new()) { }

        public RStr(string str) : base("rstr", new(str)) { }

        public RStr(Str str) : base("rstr", new(str.Value)) { }

        public override Obj Init(List args)
        {
            if (args.Count == 0)
                Value = new();
            else if (args[0] is Str s)
                Value = new(s.Value);
            else
                throw new ClassError("initialize error");

            return this;
        }

        public override void Init()
        {
            field.Set("append", new NativeFun("append", -1, args =>
            {
                if (args[0] is not RStr self)
                    throw new ValueError("invalid argument");

                for (int i = 1; i < args.Count; i++)
                    self.Value.Append(args[i].CStr().Value);                

                return self;
            }));
            field.Set("appendln", new NativeFun("appendln", -1, args =>
            {
                if (args[0] is not RStr self)
                    throw new ValueError("invalid argument");

                for (int i = 1; i < args.Count; i++)
                    self.Value.AppendLine(args[i].CStr().Value);

                return self;
            }));
            field.Set("clear", new NativeFun("clear", 1, args =>
            {
                if (args[0] is not RStr self)
                    throw new ValueError("invalid argument");

                self.Value.Clear();

                return self;
            }));
        }

        public override Obj Add(Obj arg)
        {
            Value.Append(arg.CStr().Value);
            return this;
        }

        public override Str CStr() => new($"{Value}");

        public override Int Len() => new(Value.Length);

        public override Obj GetItem(List args)
        {
            if (args[0] is not Int i || OutOfRange((int)i.Value)) throw new IndexError("out of range");
            return new Str($"{Value[(int)i.Value]}");
        }

        public override Obj Clone() => new RStr($"{Value}");

        public override Obj Copy() => this;


        bool OutOfRange(int index) => 0 > index || index >= Value.Length;
    }
}
