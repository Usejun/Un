namespace Un.Package;

public class Process : Obj, IPackage, IStatic
{
    public string Name => "process";
    
    Str Run(Field field)
    {
        if (field["file_name"].As<Str>(out var name)) throw new ValueError("argument only accept str");

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = name.Value,
                WorkingDirectory = field["working_directory"].CStr().Value,
                UseShellExecute = field["use_shell_excute"].CBool().Value,
                RedirectStandardError = field["redirect_standard_error"].CBool().Value,
                RedirectStandardInput = field["redirect_standard_input"].CBool().Value,
                RedirectStandardOutput = field["redirect_standard_output"].CBool().Value,
                CreateNoWindow = field["create_no_window"].CBool().Value,
                Arguments = field["arguments"].CStr().Value,        
                UserName = field["user_name"].CStr().Value
            }
        };
        
        process.Start();
        process.WaitForExit();

        return new(process.StandardOutput.ReadToEnd());
    }

    public Obj Static()
    {
        var process = new Process();
        process.field.Set("run", new NativeFun("run", 1, Run, [("file_name", null!), 
                                                               ("working_directory", new Str()),
                                                               ("use_shell_excute", Bool.False), 
                                                               ("redirect_standard_error", Bool.False),
                                                               ("redirect_standard_input", Bool.False),
                                                               ("redirect_standard_output", Bool.False), 
                                                               ("create_no_window",Bool.False), 
                                                               ("arguments", new Str()),
                                                               ("user_name", new Str())]));
        return process;
    }
}