namespace Un.Package;

public class Os : Obj, IPackage, IStatic
{
    public string Name => "os";

    public override Str Type() => new(Name);

    public Obj Static()
    {
        Obj os = new(Name);

        os.field.Set("machine_name", new NativeFun("machine_name", 0, _ => new Str(Environment.MachineName), []));
        os.field.Set("user_name", new NativeFun("user_name", 0, _ => new Str(Environment.UserName), []));
        os.field.Set("domain_name", new NativeFun("domain_name", 0, _ => new Str(Environment.UserDomainName), []));
        os.field.Set("os_version", new NativeFun("os_version", 0, _ => new Str(Environment.OSVersion.VersionString), []));
        os.field.Set("processor_count", new NativeFun("processor_count", 0, _ => new Int(Environment.ProcessorCount), []));
        os.field.Set("system_directory", new NativeFun("system_directory", 0, _ => new Str(Environment.SystemDirectory), []));
        os.field.Set("current_directory", new NativeFun("current_directory", 0, _ => new Str(Environment.CurrentDirectory), []));
        os.field.Set("current_managed_thread_id", new NativeFun("current_managed_thread_id", 0, _ => new Int(Environment.CurrentManagedThreadId), []));
        os.field.Set("architecture", new NativeFun("architecture", 0, _ => new Str(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit"), []));
        os.field.Set("process_bit", new NativeFun("process_bit", 0, _ => new Str(Environment.Is64BitProcess ? "64-bit" : "32-bit"), []));
        os.field.Set("new_line", new NativeFun("new_line", 0, _ => new Str(Environment.NewLine), []));
        os.field.Set("tick_count", new NativeFun("tick_count", 0, _ => new Int(Environment.TickCount), []));
        os.field.Set("version", new NativeFun("version", 0, _ => new Str(Environment.Version.ToString()), []));
        os.field.Set("working_set", new NativeFun("working_set", 0, _ => new Int(Environment.WorkingSet), []));
        os.field.Set("process_id", new NativeFun("process_id", 0, _ => new Int(Environment.ProcessId), []));
        os.field.Set("cpu_count", new NativeFun("cpu_count", 0, _ => new Int(Environment.ProcessorCount), []));
        os.field.Set("exit_code", new NativeFun("exit_code", 0, _ => new Int(Environment.ExitCode), []));
        os.field.Set("process_priority", new NativeFun("process_priority", 0, _ => new Int(System.Diagnostics.Process.GetCurrentProcess().BasePriority), []));
        os.field.Set("total_memory", new NativeFun("total_memory", 0, _ => new Int(GC.GetTotalMemory(false)), []));
        os.field.Set("available_memory", new NativeFun("available_memory", 0, _ => new Int(Environment.WorkingSet), []));

        return os;

    }
}