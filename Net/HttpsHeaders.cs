using System.Net.Http.Headers;
using Un.Collections;
using Un.Data;

namespace Un.Net
{
    public class HttpsHeaders : Ref<HttpHeaders>
    {
        public HttpsHeaders() : base("https_headers", null) { }

        public HttpsHeaders(HttpHeaders headers) : base("https_headers", headers) { }

        public override void Init()
        {
            properties.Add("add", new NativeFun("add", -1, para =>
            {
                if (para[0] is not HttpsHeaders self || para[1] is not Str key)
                    throw new ArgumentException("invaild argument", nameof(para));

                if (para[2] is Str value)
                    self.value.Add(key.value, value.value);
                else if (para[2] is Iter values)
                    self.value.Add(key.value, values.Select(i => i.CStr().value));
                else throw new ArgumentException("invaild argument", nameof(para));

                return None;
            }));
            properties.Add("remove", new NativeFun("remove", 2, para =>
            {
                if (para[0] is not HttpsHeaders self || para[1] is not Str key)
                    throw new ArgumentException("invaild argument", nameof(para));

                return new Bool(self.value.Remove(key.value));
            }));
            properties.Add("clear", new NativeFun("clear", 1, para =>
            {
                if (para[0] is not HttpsHeaders self)
                    throw new ArgumentException("invaild argument", nameof(para));

                self.value.Clear();
                return None;
            }));
        }
    }
}
