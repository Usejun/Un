namespace Un.Package;

public class Os : Obj, IPackage, IStatic
{
    public string Name => "os";

    public Obj Static()
    {
        Obj obj = new("os");
        obj.field.Set("machine_name", new Str(Environment.MachineName));
        obj.field.Set("user_name", new Str(Environment.UserName));
        obj.field.Set("domain_name", new Str(Environment.UserDomainName));
        obj.field.Set("os_version", new Str(Environment.OSVersion.VersionString));
        obj.field.Set("processor_count", new Int(Environment.ProcessorCount));
        obj.field.Set("system_directory", new Str(Environment.SystemDirectory));
        obj.field.Set("current_directory", new Str(Environment.CurrentDirectory));
        obj.field.Set("current_managed_thread_id", new Int(Environment.CurrentManagedThreadId));
        obj.field.Set("is_64_bit_operating_system", new Bool(Environment.Is64BitOperatingSystem));
        obj.field.Set("is_64_bit_process", new Bool(Environment.Is64BitProcess));
        obj.field.Set("new_line", new Str(Environment.NewLine));
        obj.field.Set("tick_count", new Int(Environment.TickCount));
        obj.field.Set("version", new Str(Environment.Version.ToString()));
        obj.field.Set("working_set", new Int(Environment.WorkingSet));

        return obj;
    }
}