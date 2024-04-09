using Un.Object;
using Un.Package;

namespace Un
{
    public static class Process
    {
        public static string File { get; private set; } = "";

        public static string Path { get; private set; } = "";

        public static string[] Code { get; private set; } = [];

        public static Interpreter Main = new([], []);

        public readonly static Dictionary<string, Pack> Package = new()
        {
            {"std", new Std("std")}, {"math", new Package.Math("math")}, {"time", new Time("time")}           
        };

        public readonly static Dictionary<string, Obj> Class = new()
        {
            {"int", new Int() },
            {"float", new Float() },
            {"iter", new Iter() },
            {"bool", new Bool() },
            {"str", new Str() },
            {"date", new Date() },
        };

        public readonly static Dictionary<string, Obj> StaticClass = [];

        public readonly static Dictionary<string, Token.Type> Control = new()
        {
            {"if", Token.Type.If}, {"elif", Token.Type.ElIf}, {"else", Token.Type.Else}
        };

        public readonly static Dictionary<string, Token.Type> Loop = new()
        {
            {"for", Token.Type.For}, {"while", Token.Type.While}
        };

        public readonly static Dictionary<string, Obj> Properties = [];

        public static void Initialize(string path, string file)
        {            
            Path = path;
            File = file;

            using StreamReader r = new(new FileStream($"{path}\\{file}", FileMode.Open));

            Code = r.ReadToEnd().Split('\n');

            Import("std");
        }

        public static void Run()
        {
            Main = new(Code, Properties);

            while (Main.TryInterpret()) ;            
        }

        public static void Import(string name)
        {
            if (IsPackage(name))
            {
                foreach (var fun in Package[name].Import())
                    Properties.Add(fun.name, fun);

                if (Package[name] is IStatic sta)
                    StaticClass.Add(name, sta.Static());                                
            }
            else
            {
                using StreamReader r = new(new FileStream($"{Path}\\{name}.un", FileMode.Open));

                Interpreter interpreter = new(r.ReadToEnd().Split('\n'), []);

                while (interpreter.TryInterpret()) ;
            }
        }

        public static bool TryGetProperty(string name, out Obj property) => Properties.TryGetValue(name, out property);

        public static bool TryGetProperty(Token token, out Obj property) => Properties.TryGetValue(token.value, out property);

        public static bool TryGetClass(string name, out Obj cla) => Class.TryGetValue(name, out cla);

        public static bool TryGetClass(Token token, out Obj cla) => Class.TryGetValue(token.value, out cla);

        public static bool TryGetStaticClass(string name, out Obj cla) => StaticClass.TryGetValue(name, out cla);

        public static bool TryGetStaticClass(Token token, out Obj cla) => StaticClass.TryGetValue(token.value, out cla);        

        public static bool IsLoop(Token token) => Loop.ContainsKey(token.value);

        public static bool IsLoop(string str) => Loop.ContainsKey(str);

        public static bool IsControl(Token token) => Control.ContainsKey(token.value);

        public static bool IsControl(string str) => Control.ContainsKey(str);

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
