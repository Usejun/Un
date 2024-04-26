using Un.Object.Function;
using Un.Object.Reference;
using Un.Object.Value;

namespace Un.Object.Net
{
    public class HttpsResponse : Ref<HttpResponseMessage>
    {
        public HttpsResponse() : base("https_response", new HttpResponseMessage()) { }

        public HttpsResponse(HttpResponseMessage response) : base("https_response", response) { }

        public override void Init()
        {
            properties.Add("status_code", new NativeFun("status_code", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new Str($"{self.value.StatusCode}");
            }));
            properties.Add("content", new NativeFun("content", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new HttpsContent(self.value.Content);
            }));
            properties.Add("headers", new NativeFun("headers", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new HttpsHeaders(self.value.Headers);
            }));
            properties.Add("version", new NativeFun("version", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new Reference.Version(self.value.Version);
            }));
            properties.Add("is_success_status_code", new NativeFun("is_success_status_code", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new Bool(self.value.IsSuccessStatusCode);
            }));
            properties.Add("trailing_headers", new NativeFun("trailing_headers", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new HttpsHeaders(self.value.TrailingHeaders);
            }));
            properties.Add("reason_phrase", new NativeFun("status_code", para =>
            {
                if (para[0] is not HttpsResponse self)
                    throw new ArgumentException(nameof(para));
                return new Str($"{self.value.ReasonPhrase}");
            }));
        }
    }
}
