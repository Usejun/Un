namespace Un.Net;

public class HttpsResponse : Ref<HttpResponseMessage>
{
    public HttpsResponse() : base("https_response", new HttpResponseMessage()) { }

    public HttpsResponse(HttpResponseMessage response) : base("https_response", response) { }

    public override void Init()
    {
        properties.Add("status_code", new NativeFun("status_code", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Str($"{self.value.StatusCode}");
        }));
        properties.Add("content", new NativeFun("content", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsContent(self.value.Content);
        }));
        properties.Add("headers", new NativeFun("headers", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsHeaders(self.value.Headers);
        }));
        properties.Add("version", new NativeFun("version", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Version(self.value.Version);
        }));
        properties.Add("is_success_status_code", new NativeFun("is_success_status_code", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Bool(self.value.IsSuccessStatusCode);
        }));
        properties.Add("trailing_headers", new NativeFun("trailing_headers", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsHeaders(self.value.TrailingHeaders);
        }));
        properties.Add("reason_phrase", new NativeFun("status_code", 1, args =>
        {
            if (args[0] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Str($"{self.value.ReasonPhrase}");
        }));
    }
}
