namespace Un.Net;

public class HttpsContent : Ref<HttpContent>
{
    public HttpsContent() : base("https_content", null) { }

    public HttpsContent(HttpContent content) : base("https_content", content) { }

    public override void Init()
    {
        field.Set("headers", new NativeFun("headers", 1, args =>
        {
            if (args[0] is not HttpsContent self)
                throw new ValueError("invalid argument");
            return new HttpsHeaders(self.value.Headers);
        }));
        field.Set("stream", new NativeFun("stream", 1, args =>
        {
            if (args[0] is not HttpsContent self)
                throw new ValueError("invalid argument");
            return new IO.Stream(self.value.ReadAsStreamAsync().Result);
        }));
        field.Set("str", new NativeFun("str", 1, args =>
        {
            if (args[0] is not HttpsContent self)
                throw new ValueError("invalid argument");
            return new Str(self.value.ReadAsStringAsync().Result);
        }));
    }
}
