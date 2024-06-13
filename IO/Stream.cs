namespace Un.IO;

public class Stream : Ref<System.IO.Stream>
{
    public StreamReader r;
    public StreamWriter w;

    public Stream() : base("stream", System.IO.Stream.Null) 
    {
        r = StreamReader.Null;
        w = StreamWriter.Null;
    }

    public Stream(System.IO.Stream stream) : base("stream", stream)
    {
        r = StreamReader.Null;
        w = StreamWriter.Null;

        if (Value.CanWrite) w = new(Value);
        if (Value.CanRead) r = new(Value);
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
                self.w.Write(args[i].CStr().Value);
            
            return None;
        }));
        field.Set("writeln", new NativeFun("writeln", -1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.w.WriteLine(args[i].CStr().Value);

            return None;
        }));
        field.Set("flush", new NativeFun("flush", 1, args =>
        {
            if (args[0] is not Stream self)
                throw new ValueError("invalid argument");

            self.w.Flush();

            return None;
        }));
        //field.Set("flush", new AsyncFun("flush", 1, args =>
        //{
        //    if (args[0] is not Stream self)
        //        throw new ValueError("invalid argument");

        //    self.w.WriteLineAsync();

        //    return None;
        //}));

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

    public override Obj Clone() => new Stream() { Value = Value };

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
        Value.Close();
    }
}
