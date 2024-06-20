using System.Text;
using Un.Util;

namespace Un;

public static class Process
{
    public static UnicodeEncoding Unicode { get; } = new(false, false);

    public static string Path { get; private set; } = "";
    public static string File { get; private set; } = "";

    public static string[] Code => Main.Code;
    public static int Line => Main.Line;
    public static int Index => Main.Index;
    public static int Nesting => Main.Nesting;
    public static Field Field => Main.Field;

    private static Parser Main { get; set; } = new([], new());
    private static Field Global = new();

    private static HashSet<string> Imported = [];

    public readonly static Field Package = new();

    public readonly static Field Class = new();

    public readonly static Field StaticClass = new();    

    public static void Initialize(string path)
    {
        using StreamReader config = new(new FileStream($"D:\\User\\Un\\keywords.txt", FileMode.Open));

        while (!config.EndOfStream)
        {
            var data = config.ReadLine()!.Split();
            string keyword = data[0];
            string str = data[1];

            Token.Types.Add(str, Enum.Parse<Token.Type>(keyword));
            if (str.Length > 1) Token.Union.Add(str);
        }

        InitializePackage();

        Path = path;
    }

    public static void Run(string file)
    {
        Global.Clear();
        Class.Clear();
        StaticClass.Clear();
        Imported.Clear();
        File = file;

        InitializeClass();
        Import(new Std());

        using StreamReader r = new(new FileStream($"{Path}\\{file}", FileMode.Open));

        Global.Set("__main__", new Str(file));
        Global.Set("__name__", new Str(file));

        Interpret(file, r.ReadToEnd().Split('\n'), Global, []);
    }

    public static Obj Interpret(string name, string[] code, Field field, List<string> usings, int index = 0, int line = 0, int nesting = 0)
    {
        (var prevCode, var pervField, var prevUsings, var prevIndex, var prevLine, var prevNesting, var prevName) = (Code, Field, Main.Usings, Index, Line, Nesting, Global["__name__"]);

        Global["__name__"] = new Str(name);
        Main.Change(code, field, usings, index, line, nesting);

        while (Main.TryInterpret()) { };

        var returnValue = Main.ReturnValue;
        Main.ReturnValue = null;

        Global["__name__"] = prevName;
        Main.Change(prevCode, pervField, prevUsings, prevIndex, prevLine, prevNesting);

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
            Global["__name__"] = new Str($"{name}.un");

            Interpret($"{name}.un", r.ReadToEnd().Split('\n'), Global, []);
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


    private static void InitializePackage()
    {
        Package.Set("math", new Util.Math());
        Package.Set("time", new Time());
        Package.Set("https", new Https());
        Package.Set("rand", new Rand());
        Package.Set("stack", new Stack());
        Package.Set("queue", new Queue());
        Package.Set("json", new JObj());
        Package.Set("long", new Long());
        Package.Set("matrix", new Matrix());
        Package.Set("date", new Date());
        Package.Set("rstr", new RStr());
        Package.Set("sys", new Sys());
    }

    private static void InitializeClass()
    {
        Class.Set("int", new Int());
        Class.Set("float", new Float());
        Class.Set("list", new List());
        Class.Set("bool", new Bool());
        Class.Set("str", new Str());
        Class.Set("map", new Map());
        Class.Set("dict", new Dict());
        Class.Set("set", new Set());
        Class.Set("obj", new Data.Object());
        Class.Set("reader", new Reader());
        Class.Set("writer", new Writer());
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
