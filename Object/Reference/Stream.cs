using Un.Object.Function;
using Un.Object.Interfaces;
using Un.Object.Value;

namespace Un.Object.Reference
{
    public class Stream : Ref<System.IO.Stream>, IUsing
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
            properties.Add("readln", new NativeFun("readline", para =>
            {
                if (para[0] is not Stream self)
                    throw new ArgumentException("invaild argument");

                return new Str(self.r.ReadLine());
            }));
            properties.Add("write", new NativeFun("write", para =>
            {
                if (para[0] is not Stream self)
                    throw new ArgumentException("invaild argument");

                self.w.Write(para[1].CStr().value);
                return None;
            }));
            properties.Add("writeln", new NativeFun("writeln", para =>
            {
                if (para[0] is not Stream self)
                    throw new ArgumentException("invaild argument");

                self.w.WriteLine(para[1].CStr().value);
                return None;
            }));
            properties.Add("close", new NativeFun("close", para =>
            {
                if (para[0] is not Stream self)
                    throw new ArgumentException("invaild argument");

                self.Close();
                return None;
            }));
            properties.Add("is_end", new NativeFun("is_end", para =>
            {
                if (para[0] is not Stream self)
                    throw new ArgumentException("invaild argument");

                return new Bool(self.r.EndOfStream);
            }));
        }

        public override Obj Clone() => new Stream() { value = value };

        public override Obj Copy() => this;

        public void Close()
        {
            w?.Close();
            r?.Close();
            value.Close();
        }
    }
}
