using System.Net.Http.Headers;
using Un.Object.Function;
using Un.Object.Reference;
using Un.Object.Value;

namespace Un.Object.Net
{
    public class HttpsContent : Ref<HttpContent>
    {
        public HttpsContent() : base("https_content", null) { }

        public HttpsContent(HttpContent content) : base("https_content", content) { }

        public override void Init()
        {
            properties.Add("headers", new NativeFun("headers", para =>
            {
                if (para[0] is not HttpsContent self)
                    throw new ArgumentException(nameof(para));
                return new HttpsHeaders(self.value.Headers);
            }));
            properties.Add("stream", new NativeFun("stream", para =>
            {
                if (para[0] is not HttpsContent self)
                    throw new ArgumentException(nameof(para));
                return new Reference.Stream(self.value.ReadAsStreamAsync().Result);
            }));
            properties.Add("string", new NativeFun("string", para =>
            {
                if (para[0] is not HttpsContent self)
                    throw new ArgumentException(nameof(para));
                return new Str(self.value.ReadAsStringAsync().Result);
            }));
        }
    }
}
