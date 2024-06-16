namespace Un.Util;

public class Sys : Obj, IPackage, IStatic
{
    public string Name => "sys";

    public Obj Static()
    {
        Sys sys = new();
        var _in = new Reader(Console.OpenStandardInput());
        var _out = new Writer(Console.OpenStandardOutput());
        var console = new Data.Object();       
        var os = new Data.Object();

        sys.field.Set("path", new NativeFun("path", 1, args =>
        {
            return new Str($"{Process.Path}\\{Process.File}");
        }));
        sys.field.Set("process_path", new NativeFun("process_path", 1, args =>
        {
            return new Str(Environment.ProcessPath);
        }));
        sys.field.Set("directory_path", new NativeFun("directory_path", 1, args =>
        {
            return new Str(Process.Path);
        }));
        sys.field.Set("exit", new NativeFun("exit", 1, args =>
        {
            if (args[0] is not Int code)
                throw new ValueError("invalid arguments");

            Environment.Exit((int)code.Value);
            return None;
        }));

        sys.field.Set("in", _in);

        sys.field.Set("out", _out);
        _out.Value.AutoFlush = true;

        sys.field.Set("console", console);
        console.field.Set("clear", new NativeFun("clear", 0, args =>
        {
            Console.Clear();
            return None;
        }));

        sys.field.Set("os", os);        

        return sys;
    }
}
