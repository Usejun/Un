namespace Un.IO;

public class Writer : Ref<StreamWriter>
{
    public Writer() : base("writer", StreamWriter.Null) { }

    public Writer(string path, bool append) : base("writer", new(path, append, Process.Encoding)) { }

    public Writer(Stream stream) : base("writer", new(stream.Value, Process.Encoding)) { }

    public Writer(System.IO.Stream stream) : base("writer", new(stream, Process.Encoding)) { }

    public Writer(Writer writer) : base("writer", new(writer.Value.BaseStream, Process.Encoding)) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", null!), ("append", Bool.False)]);

        var value = field["value"];

        if (!field["append"].As<Bool>(out var append))        
            throw new ClassError();

        if (value.As<Str>(out var path))        
            Value = new(path.Value, append.Value);
        else if (value.As<Stream>(out var stream))
            Value = new(stream.Value);
        else 
            throw new ClassError();

        return this;
    }

    public override void Init()
    {          
        field.Set("auto_flush", new NativeFun("auto_flush", 1, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) || !field["on"].As<Bool>(out var on))
                throw new ArgumentError();

            self.Value.AutoFlush = on.Value;

            return None;
        }, [("on", null!)]));
        field.Set("write", new NativeFun("write", 1, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) || !field["values"].As<List>(out var values))
                throw new ArgumentError();

            for (int i = 0; i < values.Count; i++)
                self.Value.Write(values[i].CStr().Value);                

            return None;
        }, [("values", null!)],  true));
        field.Set("write_async", new AsyncNativeFun<Obj>("write_async", 1, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) || !field["values"].As<List>(out var values))
                throw new ArgumentError();

            for (int i = 0; i < values.Count; i++)
                self.Value.Write(values[i].CStr().Value);                

            return None;
        }, [("values", null!)], true));
        field.Set("writeln", new NativeFun("writeln", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) || !field["values"].As<List>(out var values))
                throw new ArgumentError();

            for (int i = 0; i < values.Count; i++)
                self.Value.Write(values[i].CStr().Value);
            self.Value.WriteLine();

            return None;
        }, [("values", null!)], true));
        field.Set("writeln_async", new AsyncNativeFun<Obj>("writeln_async", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) || !field["values"].As<List>(out var values))
                throw new ArgumentError();

            for (int i = 0; i < values.Count; i++)
                self.Value.Write(values[i].CStr().Value);
            self.Value.WriteLine();

            return None;
        }, [("values", null!)], true));
        field.Set("flush", new NativeFun("flush", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) )
                throw new ArgumentError();

            self.Value.Flush();

            return None;
        }, []));
        field.Set("flush_async", new AsyncNativeFun<Obj>("flush_async", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) )
                throw new ArgumentError();

            self.Value.Flush();

            return None;
        }, []));
        field.Set("close", new NativeFun("close", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) )
                throw new ArgumentError();

            self.Value.Close();
            return None;
        }, []));
        field.Set("dispose", new NativeFun("dispose", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) )
                throw new ArgumentError();

            self.Value.Dispose();
            return None;
        }, []));
        field.Set("dispose_async", new AsyncNativeFun<Obj>("dispose_async", 0, field =>
        {
            if (!field[Literals.Self].As<Writer>(out var self) )
                throw new ArgumentError();

            self.Value.Dispose();
            return None;
        }, []));
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
