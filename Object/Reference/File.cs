using Un.Object.Function;
using Un.Object.Interfaces;
using Un.Object.Value;

namespace Un.Object.Reference
{
    public class File : Ref<FileStream>, IUsing
    {
        public StreamReader r;
        public StreamWriter w;

        public File() : base("file", null) { }

        public File(Str path) : base("file", System.IO.File.Open(path.value, FileMode.Open))
        {
            w = new(value);
            r = new(value);
        }

        public override void Init()
        {
            properties.Add("readln", new NativeFun("readline", para =>
            {
                if (para[0] is not File self)
                    throw new ArgumentException("invaild argument");

                return new Str(self.r.ReadLine());
            }));
            properties.Add("write", new NativeFun("write", para =>
            {
                if (para[0] is not File self)
                    throw new ArgumentException("invaild argument");

                self.w.Write(para[1].CStr().value);
                return None;
            }));
            properties.Add("writeln", new NativeFun("writeln", para =>
            {
                if (para[0] is not File self)
                    throw new ArgumentException("invaild argument");

                self.w.WriteLine(para[1].CStr().value);
                return None;
            }));
            properties.Add("close", new NativeFun("close", para =>
            {
                if (para[0] is not File self)
                    throw new ArgumentException("invaild argument");

                self.Close();
                return None;
            }));
            properties.Add("is_end", new NativeFun("is_end", para =>
            {
                if (para[0] is not File self)
                    throw new ArgumentException("invaild argument");

                return new Bool(self.r.EndOfStream);
            }));
        }

        public override Obj Clone() => new File() { value = value };

        public override Obj Copy() => this;

        public void Close()
        {
            w.Close();
            r.Close();
            value.Close();
        }
    }
}
