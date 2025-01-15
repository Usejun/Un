using System.Text;
using Un.Package;

using Un.Interpreter;

namespace Un;

public static class Process
{
    public static UnicodeEncoding Unicode { get; } = new(false, false);

    public static string Path { get; private set; } = "";
    public static string File { get; private set; } = "";
    public static string Roming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static string[] Code => Main.Code;
    public static int Line => Main.Line;
    public static int Index => Main.Index;
    public static int Nesting => Main.Nesting;
    public static Field Field => Main.Field;

    private static Stack<CallInfo> CallStack = new();

    private static Parser Main { get; set; } = new([], new());
    private static Field Global = new();

    private static HashSet<string> Imported = [];

    public readonly static Field Package = new();
    public readonly static Field Class = new();
    public readonly static Field StaticClass = new();    

    public static void Initialize(string path)
    {
        Package.Set("math", new Package.Math());
        Package.Set("https", new Https());        
        Package.Set("long", new Long());
        Package.Set("thread", new Package.Thread());

        Path = path;
    }

    public static void Clear()
    {
        Global.Clear();
        Class.Clear();
        StaticClass.Clear();
        Imported.Clear();
    }

    public static void Run(string file)
    {
        Clear();

        File = file;

        Class.Set("int", new Int());
        Class.Set("float", new Float());
        Class.Set("list", new List());
        Class.Set("bool", new Bool());
        Class.Set("str", new Str());
        Class.Set("dict", new Dict());
        Class.Set("set", new Set());
        Class.Set("reader", new Reader());
        Class.Set("writer", new Writer());
        Class.Set("date", new Date());
        Class.Set("error", new Error());

        Import(new Std());
        Import(new Time());
        Import(new Package.Random());

        foreach (var(name, error) in Error.Import())
            Global.Set(name, error);         

        using StreamReader r = new(new FileStream($"{Path}\\{file}", FileMode.Open));

        Global.Set(Literals.Main, new Str(file));
        Global.Set(Literals.Name, new Str(file));

        Interpret(file, r.ReadToEnd().Split('\n'), Global, []);
    }

    public static Obj Interpret(string name, string[] code, Field field, List<string> usings, int index = 0, int line = 0, int nesting = 0)
    {
        (var prevCode, var pervField, var prevUsings, var prevIndex, var prevLine, var prevNesting, var prevName) = (Code, Field, Main.Usings, Index, Line, Nesting, Global["__name__"]);

        Global[Literals.Name] = new Str(name);
        Main.Change(code, field, usings, index, line, nesting);
        CallStack.Push(new(name, line, new(Field)));

        while (Main.TryInterpret()) { };

        var returnValue = Main.ReturnValue;
        Main.ReturnValue = null;

        Global[Literals.Name] = prevName;
        Main.Change(prevCode, pervField, prevUsings, prevIndex, prevLine, prevNesting);
        CallStack.Pop();

        return returnValue ?? Obj.None;
    }

    public static void Import(IPackage package)
    {
        foreach (var fun in package.Import())
            Global.Set(fun.Name, fun);

        foreach (var include in package.Include())
            Class.Set(include.ClassName, include);
    }

    public static void Import(string name)
    {
        if (IsPackage(name))
        {
            if (Imported.Contains(name))
                return;

            Imported.Add(name);

            if (Package[name] is IPackage pack) Import(pack);
            else Class.Set(name, Package[name]);

            if (Package[name] is IStatic sta)
                StaticClass.Set(name, sta.Static());
        }
        else
        {
            if (Imported.Contains(name))
                return;

            if (!System.IO.File.Exists($"{Path}\\Package\\{name}.un"))
                throw new FileError();

            using StreamReader r = new(new FileStream($"{Path}\\Package\\{name}.un", FileMode.Open));
            
            Imported.Add(name);
            Global[Literals.Name] = new Str($"{name}.un");

            Interpret($"{name}.un", r.ReadToEnd().Split('\n'), Global, []);
        }
    }

    public static void Import(IPackage package, string nickname)
    {
        Obj obj = new(nickname);

        foreach (var fun in package.Import())
            obj.field.Set(fun.Name, fun);

        foreach (var include in package.Include())
            obj.field.Set(include.ClassName, include);

        Global.Set(nickname, obj);
    }

    public static void Import(string name, string nickname)
    {
        if (IsPackage(name))
        {
            if (Imported.Contains(name))
                return;

            Imported.Add(name);            

            if (Package[name] is IPackage pack) Import(pack, nickname);
            else Class.Set(nickname, Package[name]);

            if (Package[name] is IStatic sta)
                StaticClass.Set(nickname, sta.Static());
        }
        else
        {
            if (Imported.Contains(name))
                return;

            if (!System.IO.File.Exists($"{Path}\\Package\\{name}.un"))
                throw new FileError();

            using StreamReader r = new(new FileStream($"{Path}\\Package\\{name}.un", FileMode.Open));

            Imported.Add(name);
            Global["__name__"] = new Str($"{name}.un");

            Interpret($"{name}.un", r.ReadToEnd().Split('\n'), Global, []);

            Class[nickname] = Class[name];
            Class[nickname].ClassName = nickname;
            Class.Remove(name);
        }
    }

    public static void Test(string[] files, bool writeState = false)
    {
        List<string> logs = [];

        foreach (var file in files)
        {
            try
            {
                if (writeState) Console.WriteLine($"\n{file} : Start\n");
                else Console.WriteLine();
                Run(file);
            }
            catch
            {
                logs.Add($"{file} : Failed");
                continue;
            }
            finally
            {
                if (writeState) Console.WriteLine($"\n{file} : End\n");
                else Console.WriteLine();
            }
            logs.Add($"{file} : Succeed");
        }

        Console.WriteLine();
        Console.WriteLine(string.Join("\n", logs));
    }



    public static bool TryGetGlobalProperty(string name, out Obj property) => Global.Get(name, out property);

    public static bool TryGetGlobalProperty(Token token, out Obj property) => Global.Get(token.Value, out property);

    public static bool TryGetClass(string name, out Obj cla) => Class.Get(name, out cla);

    public static bool TryGetClass(Token token, out Obj cla) => Class.Get(token.Value, out cla);

    public static bool TryGetStaticClass(string name, out Obj cla) => StaticClass.Get(name, out cla);

    public static bool TryGetStaticClass(Token token, out Obj cla) => StaticClass.Get(token.Value, out cla);

    public static bool IsClass(Token token) => Class.Key(token.Value);

    public static bool IsClass(string str) => Class.Key(str);

    public static bool IsStaticClass(Token token) => token.type == Token.Type.Variable && StaticClass.Key(token.Value);

    public static bool IsStaticClass(string str) => StaticClass.Key(str);

    public static bool IsPackage(Token token) => (token.type == Token.Type.Variable || token.type == Token.Type.Function) && Package.Key(token.Value);

    public static bool IsPackage(string str) => Package.Key(str);
}
