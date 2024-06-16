namespace Un.IO;

public class Writer : Ref<StreamWriter>
{
    public Writer() : base("writer", StreamWriter.Null) { }

    public Writer(string path, bool append) : base("writer", new(path, append, Process.Unicode)) { }

    public Writer(Stream stream) : base("writer", new(stream.Value, Process.Unicode)) { }

    public Writer(System.IO.Stream stream) : base("writer", new(stream, Process.Unicode)) { }

    public Writer(Writer writer) : base("writer", new(writer.Value.BaseStream, Process.Unicode)) { }

    public override Obj Init(List args)
    {
        if (args.Count == 1)
        {
            if (args[0] is Stream st) Value = new(st.Value);
            else if (args[0] is Str s) Value = new(s.Value);
            else throw new ClassError("initialize error");
        }
        else if (args.Count == 2)
        {
            if (args[1] is not Bool b) throw new ClassError("initialize error");

            if (args[0] is Stream st && !b.Value) Value = new(st.Value);
            else if (args[0] is Str s) Value = new(s.Value, b.Value);
            else throw new ClassError("initialize error");
        }
        else throw new ClassError("initialize error");

        return this;
    }

    public override void Init()
    {          
        field.Set("auto_flush", new NativeFun("auto_flush", 2, args =>
        {
            if (args[0] is not Writer self || args[1] is not Bool cond)
                throw new ValueError("invalid argument");

            self.Value.AutoFlush = cond.Value;

            return None;
        }));
        field.Set("write", new NativeFun("write", -1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Value.Write(args[i].CStr().Value);                

            return None;
        }));
        field.Set("write_async", new AsyncNativeFun<Obj>("write_async", -1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Value.Write(args[i].CStr().Value);

            return None;
        }));
        field.Set("writeln", new NativeFun("writeln", -1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Value.Write(args[i].CStr().Value);
            self.Value.WriteLine();

            return None;
        }));
        field.Set("writeln_async", new AsyncNativeFun<Obj>("writeln_async", -1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Value.Write(args[i].CStr().Value);
            self.Value.WriteLine();

            return None;
        }));
        field.Set("flush", new NativeFun("flush", -1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            self.Value.Flush();

            return None;
        }));
        field.Set("flush_async", new AsyncNativeFun<Obj>("flush_async", -1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            self.Value.Flush();

            return None;
        }));
        field.Set("close", new NativeFun("close", 1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            self.Value.Close();
            return None;
        }));
        field.Set("dispose", new NativeFun("dispose", 1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            self.Value.Dispose();
            return None;
        }));
        field.Set("dispose_async", new AsyncNativeFun<Obj>("dispose_async", 1, args =>
        {
            if (args[0] is not Writer self)
                throw new ValueError("invalid argument");

            self.Value.Dispose();
            return None;
        }));
    }

    public override Str CStr() => new(ClassName);

    public override Obj Entry()
    {
        return None;
    }

    public override Obj Exit()
    {
        Value.Close();
        Value.Dispose();
        return None;
    }

    public override Obj Clone() => new Writer(this);

    public override Obj Copy() => this;
}
