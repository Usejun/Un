using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Https(string packageName) : Pack(packageName), IStatic
    {
        private static readonly HttpClient client = new();

        Str Get(Iter para)
        {
            if (para[1] is not Str url)
                throw new ArgumentException("invaild argument", nameof(url));

            return new(client.GetAsync(url.value).Result.Content.ReadAsStringAsync().Result);
        }

        public Pack Static()
        {
            Https https = new("https");

            https.properties.Add("get", new NativeFun("get", Get));

            return https;
        }
    }
}
