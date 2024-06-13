﻿namespace Un.Net;

public class Https : Obj, IPackage, IStatic 
{
    private static readonly HttpClient client = new();

    public string Name => "https";

    public Obj Static()
    {
        Https https = new();

        https.field.Set("get", new NativeFun("get", 2, args =>
        {
            if (args[0] is not Https self)
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
