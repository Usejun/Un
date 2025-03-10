namespace Un.Package;

public class Env : Obj, IPackage, IStatic
{
    public string Name => "env";

    public override Str Type() => new(Name);

    public Obj Static()
    {
        Obj env = new(Name);
        env.field.Set("get", new NativeFun("get", 1, field =>
        {
            if (!field["key"].As<Str>(out var key))
                throw new ArgumentError("key must be a str");            
            return new Str(Environment.GetEnvironmentVariable(key.Value));
        }, [("key", null!)]));
        env.field.Set("set", new NativeFun("set", 2, field =>
        {
            if (!field["key"].As<Str>(out var key))
                throw new ArgumentError();
            Environment.SetEnvironmentVariable(key.Value, field["value"].CStr().Value);
            return None;
        }, [("key", null!), ("value", null!)]));
        env.field.Set("del", new NativeFun("del", 1, field =>
        {
            if (!field["key"].As<Str>(out var key))
                throw new ArgumentError("key must be a string");
            Environment.SetEnvironmentVariable(key.Value, null);
            return None;
        }, [("key", null!)]));
        return env;
    }
}