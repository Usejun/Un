using System.Net.Http.Headers;

namespace Un.Net;

public class HttpsHeaders : Ref<HttpHeaders>
{
    public HttpsHeaders() : base("https_headers", null) { }

    public HttpsHeaders(HttpHeaders headers) : base("https_headers", headers) { }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not HttpsHeaders self || args[1] is not Str key)
                throw new ValueError("invalid argument");

            if (args[2] is Str Value)
                self.Value.Add(key.Value, Value.Value);
            else if (args[2] is List Values)
                self.Value.Add(key.Value, Values.Select(i => i.CStr().Value));
            else throw new ValueError("invalid argument");

            return None;
        }));
        field.Set("remove", new NativeFun("remove", 2, args =>
        {
            if (args[0] is not HttpsHeaders self || args[1] is not Str key)
                throw new ValueError("invalid argument");

            return new Bool(self.Value.Remove(key.Value));
        }));
        field.Set("clear", new NativeFun("clear", 1, args =>
        {
            if (args[0] is not HttpsHeaders self)
                throw new ValueError("invalid argument");

            self.Value.Clear();
            return None;
        }));
    }
}
