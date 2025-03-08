namespace Un.IO;

public class Reader : Ref<StreamReader>
{
    public Reader() : base("reader", StreamReader.Null) { }

    public Reader(System.IO.Stream stream) : base("reader", new(stream, Process.Encoding)) { }

    public Reader(Stream stream) : base("reader", new(stream.Value, Process.Encoding)) { }

    public Reader(Reader reader) : base("reader", new(reader.Value.BaseStream, Process.Encoding)) { }

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field.Merge(args, [("value", null!)], 1);

        var value = field["value"];

        if (value.As<Str>(out var path))        
            Value = new(path.Value);
        else if (value.As<Stream>(out var stream))
            Value = new(stream.Value);
        else 
            throw new ClassError();

        return this;
    }

    public override void Init()
    {
        field.Set("peek", new NativeFun("peek", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Int(self.Value.Peek());
        }, []));
        field.Set("read", new NativeFun("read", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Int(self.Value.Read());
        }, []));
        field.Set("read_async", new AsyncNativeFun<Int>("read_async", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Int(self.Value.Read());
        }, []));
        field.Set("read_token", new NativeFun("read_token", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            string s = "";
            int chr = 0;

            while ((chr = self.Value.Read()) != -1)
            {
                if (chr == 10) continue;
                else if (char.IsWhiteSpace((char)chr)) break;
                s += (char)chr;
            }
           
            return new Str(s);
        }, []));
        field.Set("read_token_async", new AsyncNativeFun<Str>("read_token_async", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            string s = "";
            int chr = 0;

            while ((chr = self.Value.Read()) != -1)
            {
                if (chr == 10) continue;
                else if (char.IsWhiteSpace((char)chr)) break;
                s += (char)chr;
            }

            return new Str(s);
        }, []));
        field.Set("readln", new NativeFun("readln", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadLine());
        }, []));
        field.Set("readln_async", new AsyncNativeFun<Str>("readln_async", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadLine());
        }, []));
        field.Set("read_all", new NativeFun("read_all", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadToEnd());
        }, []));
        field.Set("read_all_async", new AsyncNativeFun<Str>("read_all_async", 1, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadToEnd());
        }, []));
        field.Set("is_end", new NativeFun("is_end", 0, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            return new Bool(self.Value.EndOfStream);
        }, []));
        field.Set("close", new NativeFun("close", 0, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

            self.Value.Close();                
            return None;
        }, [])); 
        field.Set("dispose", new NativeFun("dispose", 0, field =>
        {
            if (!field[Literals.Self].As<Reader>(out var self))
                throw new ValueError("invalid argument");

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

    public override Obj Clone() => new Reader(this);

    public override Obj Copy() => this;
}
