namespace Un;

public static class Process
{
    public static string Path { get; private set; } = "";

    public static string File { get; private set; } = "";

    public static string[] Code { get; private set; } = [];

    public static int Line { get; private set; } = 0;

    public static Parser Main { get; private set; } = new([], []);

    private readonly static HashSet<string> Imported = [];

    public readonly static Dictionary<string, Obj> Package = new()
    {
        {"std", new Util.Std()}, 
        {"math", new Util.Math()}, 
        {"time", new Util.Time()},
        {"https", new Net.Https()}, 
        {"rand", new Util.Rand()},

        {"stack", new Stack() },
        {"queue", new Queue() },
        {"json", new JObj() },
        {"long", new Long() },
        {"matrix", new Matrix() },
        {"date", new Date() },
    };

    public readonly static Dictionary<string, Obj> Class = [];

    public readonly static Dictionary<string, Obj> StaticClass = [];

    public readonly static Dictionary<string, Obj> Properties = [];

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

        Path = path;
    }

    public static void Run(string file)
    {
        Properties.Clear();
        Class.Clear();
        StaticClass.Clear();
        Imported.Clear();

        Class.Add("int", new Int());
        Class.Add("float", new Float());
        Class.Add("iter", new Iter());
        Class.Add("bool", new Bool());
        Class.Add("str", new Str());
        Class.Add("stream", new IO.Stream());
        Class.Add("map", new Map());
        Class.Add("dict", new Dict());
        Class.Add("set", new Set());

        Import("std");

        using StreamReader r = new(new FileStream($"{Path}\\{file}", FileMode.Open));

        File = file;
        Code = r.ReadToEnd().Split('\n');
        Main = new(Code, Properties);
        Properties.Add("__main__", new Str(file));
        Properties.Add("__name__", new Str(file));

        do Line++; 
        while (Main.TryInterpret()) ;
    }

    public static void Interpret(string[] code, Dictionary<string, Obj> properties, int index = 0, int line = 0, int nesting = 0)
    {
        var prevCode = Code;
        var prevProperties = Properties;
        var prevIndex = Main.Index;
        var prevLine = Main.Index;
        var prevNesting = Main.Nesting;
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
                    Properties.Add(fun.name, fun);

                foreach (var include in pack.Include())
                    Class[include.ClassName].Init();
            }
            else
            {
                Class.Add(name, Package[name]);
            }

            if (Package[name] is IStatic sta)
                StaticClass.Add(name, sta.Static());
        }
        else
        {
            if (Imported.Contains(name))
                return;

            using StreamReader r = new(new FileStream($"{Path}\\Package\\{name}.un", FileMode.Open));
            Imported.Add(name);

            Properties["__name__"] = new Str($"{name}.un");

            Parser interpreter = new(r.ReadToEnd().Split('\n'), []);

            while (interpreter.TryInterpret()) ;
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



    public static bool TryGetProperty(string name, out Obj property) => Properties.TryGetValue(name, out property);

    public static bool TryGetProperty(Token token, out Obj property) => Properties.TryGetValue(token.value, out property);

    public static bool TryGetClass(string name, out Obj cla) => Class.TryGetValue(name, out cla);

    public static bool TryGetClass(Token token, out Obj cla) => Class.TryGetValue(token.value, out cla);

    public static bool TryGetStaticClass(string name, out Obj cla) => StaticClass.TryGetValue(name, out cla);

    public static bool TryGetStaticClass(Token token, out Obj cla) => StaticClass.TryGetValue(token.value, out cla);

    public static bool IsClass(Token token) => Class.ContainsKey(token.value);

    public static bool IsClass(string str) => Class.ContainsKey(str);

    public static bool IsStaticClass(Token token) => StaticClass.ContainsKey(token.value);

    public static bool IsStaticClass(string str) => StaticClass.ContainsKey(str);

    public static bool IsPackage(Token token) => Package.ContainsKey(token.value);

    public static bool IsPackage(string str) => Package.ContainsKey(str);

    public static bool IsProperty(Token token) => Properties.ContainsKey(token.value);

    public static bool IsProperty(string str) => Properties.ContainsKey(str);
}
