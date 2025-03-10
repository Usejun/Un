namespace Un.Package;

public class Os : Obj, IPackage, IStatic
{
    public string Name => "os";

    public override Str Type() => new(Name);

    public Obj Static()
    {
        Obj os = new(Name);
        os.field.Set("machine_name", new Str(Environment.MachineName));
        os.field.Set("user_name", new Str(Environment.UserName));
        os.field.Set("domain_name", new Str(Environment.UserDomainName));
        os.field.Set("os_version", new Str(Environment.OSVersion.VersionString));
        os.field.Set("processor_count", new Int(Environment.ProcessorCount));
        os.field.Set("system_directory", new Str(Environment.SystemDirectory));
        os.field.Set("current_directory", new Str(Environment.CurrentDirectory));
        os.field.Set("current_managed_thread_id", new Int(Environment.CurrentManagedThreadId));
        os.field.Set("architecture", new Str(Environment.Is64BitOperatingSystem ? "64bit" : "32bit"));
        os.field.Set("process_bit", new Str(Environment.Is64BitProcess ? "x64" : "x86"));
        os.field.Set("new_line", new Str(Environment.NewLine));
        os.field.Set("tick_count", new Int(Environment.TickCount));
        os.field.Set("version", new Str(Environment.Version.ToString()));
        os.field.Set("working_set", new Int(Environment.WorkingSet));
        os.field.Set("process_id", new Int(Environment.ProcessId));
        os.field.Set("cpu_count ", new Int(Environment.ProcessorCount));
        os.field.Set("exit_code", new Int(Environment.ExitCode));
        os.field.Set("process_priority", new Int(System.Diagnostics.Process.GetCurrentProcess().BasePriority));
        os.field.Set("total_memory", new Int(GC.GetTotalMemory(false)));
        os.field.Set("available_memory", new Int(Environment.WorkingSet));

        return os;
    }
}