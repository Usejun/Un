using System.Text;

namespace Un.IO;

public class Reader : Ref<StreamReader>
{
    public Reader() : base("reader", StreamReader.Null) { }

    public Reader(System.IO.Stream stream) : base("reader", new(stream, Process.Unicode)) { }

    public Reader(Stream stream) : base("reader", new(stream.Value, Process.Unicode)) { }

    public Reader(Reader reader) : base("reader", new(reader.Value.BaseStream, Process.Unicode)) { }

    public override Obj Init(Collections.Tuple args)
    {
        if (args.Count != 1) throw new ClassError("initialize error");
        else if (args[0] is Stream st) Value = new(st.Value);
        else if (args[0] is Str s) Value = new(File.Open(s.Value, FileMode.Open));
        else throw new ClassError("initialize error");

        return this;
    }

    public override void Init()
    {
        field.Set("peek", new NativeFun("peek", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Peek());
        }));
        field.Set("read", new NativeFun("read", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Read());
        }));
        field.Set("read_async", new AsyncNativeFun<Int>("read_async", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Int(self.Value.Read());
        }));
        field.Set("read_token", new NativeFun("read_token", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            RStr rStr = new();
            int chr = 0;

            while ((chr = self.Value.Read()) != -1)
            {
                if (chr == 10) continue;
                else if (char.IsWhiteSpace((char)chr)) break;
                rStr.Value.Append((char)chr);
            }
           
            return rStr.CStr();
        }));
        field.Set("read_token_async", new AsyncNativeFun<Str>("read_token_async", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            RStr rStr = new();
            int chr = 0;

            while ((chr = self.Value.Read()) != -1)
            {
                if (chr == 10) continue;
                else if (char.IsWhiteSpace((char)chr)) break;
                rStr.Value.Append((char)chr);
            }

            return rStr.CStr();
        }));
        field.Set("readln", new NativeFun("readln", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadLine());
        }));
        field.Set("readln_async", new AsyncNativeFun<Str>("readln_async", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadLine());
        }));
        field.Set("read_all", new NativeFun("read_all", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadToEnd());
        }));
        field.Set("read_all_async", new AsyncNativeFun<Str>("read_all_async", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Str(self.Value.ReadToEnd());
        }));
        field.Set("is_end", new NativeFun("is_end", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.EndOfStream);
        }));
        field.Set("close", new NativeFun("close", 1, args =>
        {
            if (args[0] is not Reader self)
                throw new ValueError("invalid argument");

            self.Value.Close();                
            return None;
        })); 
        field.Set("dispose", new NativeFun("dispose", 1, args =>
        {
            if (args[0] is not Reader self)
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

    public override Obj Clone() => new Reader(this);

    public override Obj Copy() => this;
}
