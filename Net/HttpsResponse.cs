namespace Un.Net;

public class HttpsResponse : Ref<HttpResponseMessage>
{
    public HttpsResponse() : base("https_response", new HttpResponseMessage()) { }

    public HttpsResponse(HttpResponseMessage response) : base("https_response", response) { }

    public override void Init()
    {
        field.Set("status_code", new NativeFun("status_code", 0, (args, field) =>
        {
            if (field[Literals.Self] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Str($"{self.Value.StatusCode}");
        }));
        field.Set("content", new NativeFun("content", 0, (args, field) =>
        {
            if (field[Literals.Self] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new HttpsContent(self.Value.Content);
        }));
        field.Set("headers", new NativeFun("headers", 0, (args, field) =>
        {
            if (field[Literals.Self] is not HttpsResponse self)
                throw new ValueError("invalid argument");

            Dict d = new();

            foreach (var v in self.Value.Headers)
                d.Value.Add(new Str(v.Key), new List(v.Value));
            return d;
        }));
        field.Set("is_success_status_code", new NativeFun("is_success_status_code", 0, (args, field) =>
        {
            if (field[Literals.Self] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Bool(self.Value.IsSuccessStatusCode);
        }));
        field.Set("trailing_headers", new NativeFun("trailing_headers", 0, (args, field) =>
        {
            if (field[Literals.Self] is not HttpsResponse self)
                throw new ValueError("invalid argument");

            Dict d = new();

            foreach (var v in self.Value.TrailingHeaders)
                d.Value.Add(new Str(v.Key), new List(v.Value));
            return d;
        }));
        field.Set("reason_phrase", new NativeFun("reason_phrase", 0, (args, field) =>
        {
            if (field[Literals.Self] is not HttpsResponse self)
                throw new ValueError("invalid argument");
            return new Str($"{self.Value.ReasonPhrase}");
        }));
    }

    public override Str CStr() => new(Value.Content.ReadAsStringAsync().Result);
}
