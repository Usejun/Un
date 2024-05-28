namespace Un.IO;

public class Stream : Ref<System.IO.Stream>
{
    public StreamReader r;
    public StreamWriter w;

    public Stream() : base("stream", null) { }

    public Stream(System.IO.Stream stream) : base("stream", stream)
    {
        if (value.CanWrite) w = new(value);
        if (value.CanRead) r = new(value);
    }

    public override void Init()
    {
        field.Set("read_all", new NativeFun("read_all", 1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            return new Str(self.r.ReadToEnd());
        }));
        field.Set("read", new NativeFun("read", 1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            return new Str((char)self.r.Read());
        }));
        field.Set("readln", new NativeFun("readln", 1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            return new Str(self.r.ReadLine()!);
        }));
        field.Set("write", new NativeFun("write", -1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.w.Write(args[i].CStr().value);
            
            return None;
        }));
        field.Set("writeln", new NativeFun("writeln", -1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.w.WriteLine(args[i].CStr().value);

            return None;
        }));
        field.Set("close", new NativeFun("close", 1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            self.Close();
            return None;
        }));
        field.Set("is_end", new NativeFun("is_end", 1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            return new Bool(self.r.EndOfStream);
        }));
    }

    public override Obj Clone() => new Stream() { value = value };

    public override Obj Copy() => this;

    public override Obj Entry()
    {
        return None;
    }

    public override Obj Exit()
    {
        Close();
        return None;
    }

    public void Close()
    {
        w?.Close();
        r?.Close();
        value.Close();
    }
}
