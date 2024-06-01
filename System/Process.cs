namespace Un;

public static class Process
{
    public static string Path { get; private set; } = "";
    public static string File { get; private set; } = "";

    public static string[] Code => Main.Code;
    public static int Line => Main.Line;
    public static int Index => Main.Index;
    public static int Nesting => Main.Nesting;
    public static Field Field => Main.Field;

    public static Parser Main { get; private set; } = new([], new());

    public static Field Public = new();

    private readonly static HashSet<string> Imported = [];
    public readonly static Field Package = new();

    public readonly static Field Class = new();
    public readonly static Field StaticClass = new();    

    public static void Initialize(string path)
    {
        using StreamReader config = new(new FileStream($"D:\\User\\Un\\config.txt", FileMode.Open));

        while (!config.EndOfStream)
        {
            var data = config.ReadLine()!.Split();
            string keyword = data[0];
            string str = data[1];

            Token.Types.Add(str, Enum.Parse<Token.Type>(keyword));
            if (str.Length > 1)
                Token.Union.Add(str);
        }

        InitializePackage();

        Path = path;
    }

    public static void Run(string file)
    {
        Public.Clear();
        Class.Clear();
        StaticClass.Clear();
        Imported.Clear();

        Class.Set("int", new Int());
        Class.Set("float", new Float());
        Class.Set("iter", new Iter());
        Class.Set("bool", new Bool());
        Class.Set("str", new Str());
        Class.Set("stream", new IO.Stream());
        Class.Set("map", new Map());
        Class.Set("dict", new Dict());
        Class.Set("set", new Set());
        Class.Set("obj", new Data.Object());

        Import("std");

        using StreamReader r = new(new FileStream($"{Path}\\{file}", FileMode.Open));

        Public.Set("__main__", new Str(file));
        Public.Set("__name__", new Str(file));

        Interpret(file, r.ReadToEnd().Split('\n'), Public, []);
    }

    public static Obj Interpret(string name, string[] code, Field field, List<string> usings, int index = 0, int line = 0, int nesting = 0)
    {
        (var prevCode, var pervField, var prevUsings, var prevIndex, var prevLine, var prevNesting, var prevName) = (Code, Field, Main.Usings, Index, Line, Nesting, Public["__name__"]);

        Public["__name__"] = new Str(name);
        Main.Change(code, field, usings, index, line, nesting);

        while (Main.TryInterpret()) { };

        var returnValue = Main.ReturnValue;
        Main.ReturnValue = null;

        Public["__name__"] = prevName;
        Main.Change(prevCode, pervField, prevUsings, prevIndex, prevLine, prevNesting);

        return returnValue ?? Obj.None;
    }

    public static void Import(string name)
    {
        if (IsPackage(name))
        {
            if (Imported.Contains(name))
                return;

            Imported.Add(name);

            if (Package[name] is IPackage pack)
            {
                foreach (var fun in pack.Import())
                    Public.Set(fun.name, fun);

                foreach (var include in pack.Include())
                    Class.Set(include.ClassName, include);                 
            }
            else
            {
                Class.Set(name, Package[name]);
            }

            if (Package[name] is IStatic sta)
                StaticClass.Set(name, sta.Static());
        }
        else
        {
            if (Imported.Contains(name))
                return;

            using StreamReader r = new(new FileStream($"{Path}\\Package\\{name}.un", FileMode.Open));
            Imported.Add(name);

            Public["__name__"] = new Str($"{name}.un");

            Interpret($"{name}.un", r.ReadToEnd().Split('\n'), new(), []);
        }
    }

    public static void Test(string[] files)
    {
        List<string> logs = [];

        foreach (var file in files)
        {
            try
            {
                Console.WriteLine($"\n{file} : Start\n");
                Run(file);
            }
            catch
            {
                logs.Add($"{file} : Failed");
                continue;
            }
            finally
            {
                Console.WriteLine($"\n{file} : End\n");
            }
            logs.Add($"{file} : Succeed");
        }

        Console.WriteLine();
        Console.WriteLine(string.Join("\n", logs));
    }

    private static void InitializePackage()
    {
        Package.Set("std", new Util.Std());
        Package.Set("math", new Util.Math());
        Package.Set("time", new Util.Time());
        Package.Set("https", new Https());
        Package.Set("rand", new Util.Rand());
        Package.Set("stack", new Stack());
        Package.Set("queue", new Queue());
        Package.Set("json", new JObj());
        Package.Set("long", new Long());
        Package.Set("matrix", new Matrix());
        Package.Set("date", new Date());
    }



    public static bool TryGetPublicProperty(string name, out Obj property) => Public.Get(name, out property);

    public static bool TryGetPublicProperty(Token token, out Obj property) => Public.Get(token.value, out property);

    public static bool TryGetClass(string name, out Obj cla) => Class.Get(name, out cla);

    public static bool TryGetClass(Token token, out Obj cla) => Class.Get(token.value, out cla);

    public static bool TryGetStaticClass(string name, out Obj cla) => StaticClass.Get(name, out cla);

    public static bool TryGetStaticClass(Token token, out Obj cla) => StaticClass.Get(token.value, out cla);

    public static bool IsClass(Token token) => Class.Key(token.value);

    public static bool IsClass(string str) => Class.Key(str);

    public static bool IsStaticClass(Token token) => StaticClass.Key(token.value);

    public static bool IsStaticClass(string str) => StaticClass.Key(str);

    public static bool IsPackage(Token token) => Package.Key(token.value);

    public static bool IsPackage(string str) => Package.Key(str);
}
