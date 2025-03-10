using System.Text;
using Un.Package;
using Un.Interpreter;

namespace Un;

public static class Process
{
    public static UTF8Encoding Encoding { get; } = new(false, false);

    public static string Path { get; private set; } = "";
    public static string File { get; private set; } = "";
    //public static string Roming => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static string[] Code => Main.Code;
    public static int Line => Main.Line;
    public static int Index => Main.Index;
    public static int Nesting => Main.Nesting;
    public static Field Field => Main.Field;

    public static readonly Stack<CallInfo> CallStack = new();

    private static Parser Main { get; set; } = new([], new());
    private static readonly Field Global = new();

    private static readonly HashSet<string> Imported = [];

    public readonly static Field Package = new();
    public readonly static Field Class = new();
    public readonly static Field StaticClass = new();    

    public static void Initialize(string path)
    {
        Package.Set("math", new Package.Math());
        Package.Set("https", new Https());        
        Package.Set("long", new Long());
        Package.Set("thread", new Package.Thread());
        Package.Set("env", new Env());
        Package.Set("random", new Package.Random());
        Package.Set("std", new Std());
        Package.Set("time", new Time());
        Package.Set("process", new Package.Process());
        Package.Set("os", new Os());
        Package.Set("guid", new Package.Guid());
        Package.Set("image", new Image());
        Package.Set("video", new Video());

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
        Class.Set("json", new Json());
        Class.Set("error", new Error());
        
        foreach (var name in Class.Keys)
            Class[name].Init();

        StaticClass.Set("time", new Time().Static()); 
        StaticClass.Set("random", new Package.Random().Static());

        Import(new Std());
        Import(new Time());
        
        foreach (var(name, error) in Error.Import())
            Global.Set(name, error);         

        using StreamReader r = new(new FileStream($"{Path}/{file}", FileMode.Open), Encoding);

        Global.Set(Literals.Main, new Str(file));
        Global.Set(Literals.Name, new Str(file));
        Global.Set(Literals.URL, new Str("https://kanjiapi.dev/v1/kanji/"));

        Interpret(file, r.ReadToEnd().Split(Literals.NewLine), Global, []);
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
            else 
            { 
                Class.Set(name, Package[name]);
                Class[name].Init();
            }
            if (Package[name] is IStatic sta)
                StaticClass.Set(name, sta.Static());
        }
        else
        {
            if (Imported.Contains(name))
                return;

            if (!System.IO.File.Exists($"{Path}/Package/{name}.{Literals.Extension}"))
                throw new FileError();

            using StreamReader r = new(new FileStream($"{Path}/Package/{name}.{Literals.Extension}", FileMode.Open));
            
            Imported.Add(name);
            Global[Literals.Name] = new Str($"{name}.{Literals.Extension}");

            Interpret($"{name}.{Literals.Extension}", r.ReadToEnd().Split(Literals.NewLine), Global, []);
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

            if (!System.IO.File.Exists($"{Path}/Package/{name}.{Literals.Extension}"))
                throw new FileError("file not exists");

            using StreamReader r = new(new FileStream($"{Path}/Package/{name}.{Literals.Extension}", FileMode.Open));

            Imported.Add(name);
            Global[Literals.Name] = new Str($"{name}.{Literals.Extension}");

            Interpret($"{name}.{Literals.Extension}", r.ReadToEnd().Split(Literals.NewLine), Global, []);

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
        Console.WriteLine(string.Join(Literals.NewLine, logs));
    }

    public static bool TryGetGlobalProperty(string name, out Obj property) => Global.Get(name, out property);

    public static bool TryGetGlobalProperty(Token token, out Obj property) => Global.Get(token.value, out property);

    public static bool TryGetClass(string name, out Obj cla) => Class.Get(name, out cla);

    public static bool TryGetClass(Token token, out Obj cla) => Class.Get(token.value, out cla);

    public static bool TryGetStaticClass(string name, out Obj cla) => StaticClass.Get(name, out cla);

    public static bool TryGetStaticClass(Token token, out Obj cla) => StaticClass.Get(token.value, out cla);

    public static bool IsClass(Token token) => Class.Key(token.value);

    public static bool IsClass(string str) => Class.Key(str);

    public static bool IsStaticClass(Token token) => token.type == Token.Type.Variable && StaticClass.Key(token.value);

    public static bool IsStaticClass(string str) => StaticClass.Key(str);

    public static bool IsPackage(Token token) => (token.type == Token.Type.Variable || token.type == Token.Type.Function) && Package.Key(token.value);

    public static bool IsPackage(string str) => Package.Key(str);
}
