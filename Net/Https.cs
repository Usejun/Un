namespace Un.Net;

public class Https : Obj, IPackage, IStatic 
{
    private static readonly HttpClient client = new();

    public string Name => "https";

    public Obj Static()
    {
        Https https = new();

        https.field.Set("get", new NativeFun("get", 2, (args, field) =>
        {
            if (field[Literals.Self] is not Https self)
                throw new ValueError("invalid argument");
            if (args[1] is not Str url)
                throw new ValueError("invalid argument");

            return new HttpsResponse(client.GetAsync(url.Value).Result);
        }));
        https.field.Set("get_async", new AsyncNativeFun<HttpsResponse>("get_async", 2, (args, field) =>
        {
            if (field[Literals.Self] is not Https self)
                throw new ValueError("invalid argument");
            if (args[1] is not Str url)
                throw new ValueError("invalid argument");

            return new HttpsResponse(client.GetAsync(url.Value).Result);
        }));

        return https;
    }

    public IEnumerable<Obj> Include() =>
    [

    ];
}
