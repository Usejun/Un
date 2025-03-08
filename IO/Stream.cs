namespace Un.IO;

public class Stream : Ref<System.IO.Stream>
{
    public string Path { get; private set; } = "";

    public Stream() : base("stream", System.IO.Stream.Null) { }

    public Stream(System.IO.Stream stream) : base("stream", stream) { }

    public Stream(Stream stream) : base("stream", stream.Value) 
    {
        Path = stream.Path;
    }

    public override void Init()
    {                
        field.Set("close", new NativeFun("close", 0, field =>
        {
            if (!field[Literals.Self].As<Stream>(out var self) )
                throw new ArgumentError("invalid argument");

            self.Value.Close();
            return None;
        }, []));
    }

    public override Obj Clone() => new Stream(this);

    public override Obj Copy() => this;
}
