namespace Un.Net;

public class HttpsContent : Ref<HttpContent>
{
    public HttpsContent() : base("https_content", null!) { }

    public HttpsContent(HttpContent content) : base("https_content", content) { }

    public override void Init()
    {
        field.Set("headers", new NativeFun("headers", 0, field =>
        {
            if (!field[Literals.Self].As<HttpsContent>(out var self))
                throw new ValueError("invalid argument");

            Dict d = new();

            foreach (var v in self.Value.Headers)
                d.Value.Add(new Str(v.Key), new List(v.Value));
            return d;
        }, []));
        field.Set("stream", new NativeFun("stream", 0, field =>
        {
            if (!field[Literals.Self].As<HttpsContent>(out var self))
                throw new ValueError("invalid argument");
            return new IO.Stream(self.Value.ReadAsStreamAsync().Result);
        }, []));
        field.Set("json", new NativeFun("json", 0, field =>
        {
            if (!field[Literals.Self].As<HttpsContent>(out var self))
                throw new ValueError("invalid argument");
            return new Json(self.Value.ReadAsStringAsync().Result);
        }, []));
    }

    public override Str CStr() => new(Value.ReadAsStringAsync().Result);
}
