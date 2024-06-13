namespace Un.Net;

public class HttpsResponse : Ref<HttpResponseMessage>
{
    public HttpsResponse() : base("https_response", new HttpResponseMessage()) { }

    public HttpsResponse(HttpResponseMessage response) : base("https_response", response) { }

    public override void Init()
    {
        field.Set("status_code", new NativeFun("status_code", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Str($"{self.Value.StatusCode}");
        }));
        field.Set("content", new NativeFun("content", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsContent(self.Value.Content);
        }));
        field.Set("headers", new NativeFun("headers", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsHeaders(self.Value.Headers);
        }));
        field.Set("version", new NativeFun("version", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Version(self.Value.Version);
        }));
        field.Set("is_success_status_code", new NativeFun("is_success_status_code", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Bool(self.Value.IsSuccessStatusCode);
        }));
        field.Set("trailing_headers", new NativeFun("trailing_headers", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsHeaders(self.Value.TrailingHeaders);
        }));
        field.Set("reason_phrase", new NativeFun("status_code", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Str($"{self.Value.ReasonPhrase}");
        }));
    }

    public override Str CStr() => new(Value.Content.ReadAsStringAsync().Result);
}
