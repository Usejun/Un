using Un.Collections;
using Un.Data;

namespace Un.Net
{
    public class Https(string packageName) : Pack(packageName), IStatic
    {
        private static readonly HttpClient client = new();

        public Obj Static()
        {
            Https https = new("https");

            https.properties.Add("get", new NativeFun("get", 2, para =>
            {
                if (para[0] is not Https self)
                    throw new ArgumentException("invaild argument", nameof(para));
                if (para[1] is not Str url)
                    throw new ArgumentException("invaild argument", nameof(url));

                return new HttpsResponse(client.GetAsync(url.value).Result);
            }));

            return https;
        }

        public override IEnumerable<Obj> Include() =>
        [
            new HttpsContent(),
            new HttpsResponse(),
            new HttpsHeaders(),
        ];
    }
}
