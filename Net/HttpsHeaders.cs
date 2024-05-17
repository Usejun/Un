using System.Net.Http.Headers;

namespace Un.Net;

public class HttpsHeaders : Ref<HttpHeaders>
{
    public HttpsHeaders() : base("https_headers", null) { }

    public HttpsHeaders(HttpHeaders headers) : base("https_headers", headers) { }

    public override void Init()
    {
        properties.Add("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not HttpsHeaders self || args[1] is not Str key)
                throw new ValueError("invalid argument");

            if (args[2] is Str value)
                self.value.Add(key.value, value.value);
            else if (args[2] is Iter values)
                self.value.Add(key.value, values.Select(i => i.CStr().value));
            else throw new ValueError("invalid argument");

            return None;
        }));
        properties.Add("remove", new NativeFun("remove", 2, args =>
        {
            if (args[0] is not HttpsHeaders self || args[1] is not Str key)
                throw new ValueError("invalid argument");

            return new Bool(self.value.Remove(key.value));
        }));
        properties.Add("clear", new NativeFun("clear", 1, args =>
        {
            if (args[0] is not HttpsHeaders self)
                throw new ValueError("invalid argument");

            self.value.Clear();
            return None;
        }));
    }
}
