using Un.Data;

namespace Un.Net
{
    public class HttpsContent : Ref<HttpContent>
    {
        public HttpsContent() : base("https_content", null) { }

        public HttpsContent(HttpContent content) : base("https_content", content) { }

        public override void Init()
        {
            properties.Add("headers", new NativeFun("headers", 1, para =>
            {
                if (para[0] is not HttpsContent self)
                    throw new ArgumentException(nameof(para));
                return new HttpsHeaders(self.value.Headers);
            }));
            properties.Add("stream", new NativeFun("stream", 1, para =>
            {
                if (para[0] is not HttpsContent self)
                    throw new ArgumentException(nameof(para));
                return new IO.Stream(self.value.ReadAsStreamAsync().Result);
            }));
            properties.Add("string", new NativeFun("string", 1, para =>
            {
                if (para[0] is not HttpsContent self)
                    throw new ArgumentException(nameof(para));
                return new Str(self.value.ReadAsStringAsync().Result);
            }));
        }
    }
}
