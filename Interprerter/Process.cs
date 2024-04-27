using Un.Data;
using Un.Collections;

namespace Un
{
    public static class Process
    {
        public static string Path { get; private set; } = "";

        public static string File { get; private set; } = "";

        public static string[] Code { get; private set; } = [];

        public static Parser Main = new([], []);

        public readonly static Dictionary<string, Pack> Package = new()
        {
            {"std", new Util.Std("std")}, 
            {"math", new Util.Math("math")}, 
            {"time", new Util.Time("time")},
            {"https", new Net.Https("https")}, 
            {"rand", new Util.Rand("rand")}
        };

        public readonly static Dictionary<string, Obj> Class = new()
        {
            {"int", new Int() },
            {"float", new Float() },
            {"iter", new Iter() },
            {"bool", new Bool() },
            {"str", new Str() },
            {"date", new Date() },
            {"times", new Times() },
            {"json", new JObj() },
            {"stream", new IO.Stream() },
            {"map", new Map() },
            {"stack", new Stack() },
            {"queue", new Queue() },
            {"dict", new Dict() },
        };

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
            }

            foreach ((_, Obj obj) in Class)
                obj.Init();

            Path = path;
        }

        public static void Run(string file)
        {
            Properties.Clear();

            Import("std");

            using StreamReader r = new(new FileStream($"{Path}\\{file}", FileMode.Open));

            File = file;
            Code = r.ReadToEnd().Split('\n');
            Main = new(Code, Properties);
            Properties.Add("__main__", new Str(file));
            Properties.Add("__name__", new Str(file));

            while (Main.TryInterpret()) ;
        }

        public static void Import(string name)
        {
            if (IsPackage(name))
            {
                foreach (var fun in Package[name].Import())
                    Properties.Add(fun.name, fun);

                foreach (var include in Package[name].Include())
                    if (Class.TryAdd(include.ClassName, include))
                        Class[include.ClassName].Init();

                if (Package[name] is IStatic sta)
                    StaticClass.Add(name, sta.Static());
            }
            else
            {
                if (IsClass(name))
                    return;

                using StreamReader r = new(new FileStream($"{Path}\\Package\\{name}.un", FileMode.Open));

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
}
