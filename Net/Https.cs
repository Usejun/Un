using Un.Collections;
using Un.Data;

namespace Un.Net
{
    public class Https(string packageName) : Pack(packageName), IStatic
    {
        private static readonly HttpClient client = new();

        HttpsResponse Get(Iter para)
        {
            if (para[1] is not Str url)
                throw new ArgumentException("invaild argument", nameof(url));

            return new(client.GetAsync(url.value).Result);
        }

        public Pack Static()
        {
            Https https = new("https");

            https.properties.Add("get", new NativeFun("get", Get));

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
