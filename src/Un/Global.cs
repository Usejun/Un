global using Scope = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;
global using Attributes = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;

using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;

using Un.Package;

namespace Un;

public static class Global
{
    public static UnFile File { get; private set; }
    public static string Path { get; private set; }

    public static bool WriteLog { get; private set; }

    public static readonly int BASEHASH = DateTime.Now.Millisecond * 6929891 + DateTime.Now.Second * 1025957;
    public static readonly int HASHPRIME = 11;

    private readonly static Scope global = [];

    public static Attributes Class { get; private set; } = new()
    {
        { "video", new Video(null!) },
        { "audio", new Audio(null!) },
        { "image", new Image(null!) },
    };
    public static Attributes Package { get; private set; } = [];

    public static void Init(string file, bool writeLog = false)
    {
        var path = "/workspaces/Un/src/";
        //var file = "main.un"; //Path.Combine("/workspaces/Un/src/main.un", name);
        var name = file[..^3];
        var std = new Std();

        if (!System.IO.File.Exists(path + file))
            throw new Error($"file {file} not found in {path}");

        InitType<Int>();
        InitType<Float>();
        InitType<Bool>();
        InitType<Str>();
        InitType<Date>();
        InitType<List>();
        InitType<Tup>("tuple");
        InitType<Set>();
        InitType<Dict>();
        InitType<Iters>("iter");

        global["__name__"] = new Str("__main__");

        foreach (var (key, value) in std.GetOriginalMembers())
            global.Add(key, value);

        foreach (var (key, value) in std.GetOriginalMethods())
            global.Add(key, value);  

        File = new UnFile(name, System.IO.File.ReadAllLines(path + file));
        Path = path;
        WriteLog = writeLog;
    }

    public static void InitType<T>(T obj)
        where T : Obj, new()
    {
        var name = obj.Type;

        Class[name].Members = obj.GetOriginal();

        if (obj is IPack pack)
        {
            foreach (var (key, value) in pack.GetOriginalMembers())
                global.Add(key, value);

            var group = pack.GetOriginalMethods();

            if (group.Count != 0)
                global.Add(name, new Obj(name)
                {
                    Members = group,
                });
        }
    }

    public static void InitType<T>(string name = null!)
        where T : Obj, new()
    {
        var t = new T();

        Class.Add(name ?? typeof(T).Name.ToLower(), new T()
        {
            Members = t.GetOriginal()
        });
    }

    public static void Import(string name, string path, string nickname, string[] parts)
    {
        var fullPath = System.IO.Path.Combine(Path, path);

        if (!System.IO.File.Exists(fullPath))
            throw new Error($"file {name} not found in {Path}");

        var codes = System.IO.File.ReadAllLines(fullPath);
        var scope = new Scope();

        Swap(name, codes, scope);

        if (!string.IsNullOrEmpty(nickname))
        {
            Obj module = new(nickname);
            CreateNamespace(module.Members);
            global.Add(nickname, module);
        }
        else
            CreateNamespace(global);

        void CreateNamespace(Scope destination)
        {
            if (parts.Length != 0)
                foreach (var part in parts)
                {
                    if (!scope.TryGetValue(part, out Obj? value))
                        throw new Error($"module: {name} doesn't have {part}");
                    destination.Add(part, value);
                }
            else
                foreach (var (key, value) in scope)
                    destination.Add(key, value);
        }
    }

    public static void Include(string name)
    {
        InitType(Class[name]);
    }

    public static Obj Swap(string name, string[] code, Scope scope)
    {
        UnFile file = new(name, code);
        Str prevName = global["__name__"].ToStr();
        global["__name__"] = new Str(name);

        (File, file) = (file, File);

        var returned = Run(scope);

        File = file;
        global["__name__"] = prevName;
        return returned;
    }

    public static Obj Run(Scope scope)
    {
        var tokenizer = new Tokenizer();
        var lexer = new Lexer();
        var parser = new Parser(scope);
        var returned = Obj.None;

        while (!File.EOF)
        {
            var tokens = tokenizer.Tokenize(File);
            var nodes = lexer.Lex(tokens);
            returned = parser.Parse(nodes);

            if (File.EOL)
                File.Move(0, File.Line + 1);
        }

        Free(scope);

        return returned;
    }

    public static void Sub(List<Node> nodes, Scope scope)
    {
        Parallel.Invoke(() =>
        {
            var parser = new Parser(scope);
            parser.Parse(nodes);
        });
    }

    private static void Free(Scope scope)
    {
        if (scope.TryGetValue("__using__", out var usings))
            foreach (var obj in usings.ToList())
                obj.Exit();
    }

    public static bool IsClass(string name) => Class.ContainsKey(name);

    public static bool TryGetGlobalVariable(string name, out Obj value) => global.TryGetValue(name, out value);

    public static bool TryGetOriginalValue(string type, string name, out Obj value)
    {
        if (Class.TryGetValue(type, out var original))
            return original.Members.TryGetValue(name, out value);

        value = Obj.Error;
        return false;
    }
}