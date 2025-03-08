namespace Un.Net;

public class Https : Obj, IPackage, IStatic 
{
    private static readonly HttpClient client = new();

    public string Name => "https";

    public Obj Static()
    {
        Https https = new();

        https.field.Set("get", new NativeFun("get", 1, field =>
        {
            if (!field[Literals.Self].As<Https>(out var self))
                throw new ValueError("invalid argument : self");
            if (!field["url"].As<Str>(out var url))
                throw new ValueError("first argument must be a string(url)");

            return new HttpsResponse(client.GetAsync(url.Value).Result);
        }, [("url", null!)]));
        https.field.Set("get_async", new AsyncNativeFun<HttpsResponse>("get_async", 1, field =>
        {
            if (!field[Literals.Self].As<Https>(out var self))
                throw new ValueError("invalid argument : self");
            if (!field["url"].As<Str>(out var url))
                throw new ValueError("first argument must be a string(url)");

            return new HttpsResponse(client.GetAsync(url.Value).Result);
        }, [("url", null!)]));

        return https;
    }

    public IEnumerable<Obj> Include() =>
    [

    ];
}
