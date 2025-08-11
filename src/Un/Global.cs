global using Attributes = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;
global using Map = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;
global using IMap = System.Collections.Generic.IDictionary<string, Un.Object.Obj>;

using Un.Object;
using Un.Object.IO;
using Un.Object.Flow;
using Un.Object.Util;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;
using System.Collections.Concurrent;

namespace Un;

public static class Global
{
    public static object SyncRoot = new();

    public static string PATH { get; private set; } = "";

    public static readonly int BASEHASH = Math.Abs(DateTime.Now.Millisecond * 6929891 + DateTime.Now.Second * 1025957);
    public static readonly int HASHPRIME = 11;
    public static int MAXRECURSIONDEPTH = 1000;

    private static Scope scope = new(new ConcurrentDictionary<string, Obj>(), null!);
    private static ConcurrentDictionary<string, Obj> classes = new();

    static Global()
    {
        classes["time"] = new Time();
        classes["flow"] = new Flow();
        classes["json"] = new Json(Obj.None);
    }

    public static Attributes Package { get; private set; } = [];

    public static void Init(string path)
    {
        PATH = path;

        var std = new Std();

        InitTypeByName<Int>();
        InitTypeByName<Float>();
        InitTypeByName<Bool>();
        InitTypeByName<Str>();
        InitTypeByName<Date>();
        InitTypeByName<List>();
        InitTypeByName<Tup>("tuple");
        InitTypeByName<Set>();
        InitTypeByName<Dict>();
        InitTypeByName<Iters>("iter");
        InitTypeByName<Future>();

        scope.Set("__name__", new Str("__main__"));

        foreach (var (key, value) in std.GetOriginalMembers())
            scope.Set(key, value);

        foreach (var (key, value) in std.GetOriginalMethods())
            scope.Set(key, value);
    }

    public static void InitTypeFromInstance<T>(T obj)
        where T : Obj, new()
    {
        var name = obj.Type;

        classes[name].Members = obj.GetOriginal();

        if (obj is IPack pack)
        {
            foreach (var (key, value) in pack.GetOriginalMembers())
                scope.Set(key, value);

            var group = pack.GetOriginalMethods();

            if (group.Count != 0)
                scope.Set(name, new Obj(name)
                {
                    Members = group,
                });
        }
    }

    public static void InitTypeByName<T>(string name = null!)
        where T : Obj, new()
    {
        var t = new T();

        classes.TryAdd(name ?? typeof(T).Name.ToLower(), new T
        {
            Members = t.GetOriginal()
        });
    }

    public static void Include(string name)
    {
        InitTypeFromInstance(classes[name]);
    }

    public static void Import(string name, string path, string nickname, string[] parts)
    {
        var fullPath = Path.Combine(PATH, path);

        if (!Path.Exists(fullPath))
            throw new Panic($"file {name} not found in {path}");

        var scope = new Map();

        Runner.Load(name, new Scope(scope)).Run();

        if (!string.IsNullOrEmpty(nickname))
        {
            Obj module = new(nickname);
            CreateNamespace(module.Members);
            Global.scope.Set(nickname, module);
        }
        else
            CreateNamespace(Global.scope.GetScope());

        void CreateNamespace(IMap destination)
        {
            if (parts.Length != 0)
                foreach (var part in parts)
                {
                    if (!scope.TryGetValue(part, out Obj? value))
                        throw new Panic($"module: {name} doesn't have {part}");
                    destination.Add(part, value);
                }
            else
                foreach (var (key, value) in scope)
                    destination.Add(key, value);
        }
    }

    public static bool IsClass(string name) => classes.ContainsKey(name);

    public static Obj GetClass(string name)
    {
        if (classes.TryGetValue(name, out var obj))
            return obj;

        throw new Panic($"class '{name}' not found");
    }

    public static bool TryGetClass(string name, out Obj? obj)
    {
        if (classes.TryGetValue(name, out obj))
            return true;

        obj = null;
        return false;
    }

    public static void SetClass(string name, Obj obj)
    {
        classes[name] = obj;
    }


    public static bool TryGetOriginalValue(string type, string name, out Obj? value)
    {
        if (classes.TryGetValue(type, out var original))
            return original.Members.TryGetValue(name, out value);

        value = null!;
        return false;
    }


    public static Obj GetGlobalVariable(string name) => scope.Get(name, out var value) ? value : throw new Panic($"global variable '{name}' not found");

    public static void SetGlobalVariable(string name, Obj obj) => scope.Set(name, obj);

    public static bool TryGetGlobalVariable(string name, out Obj obj) => scope.Get(name, out obj!);


    public static Scope GetScope() => scope;


    public static Map New(this Map map)
    {
        Map newMap = [];
        foreach (var (key, value) in map)
            if (value is Obj obj)
                newMap[key] = obj.Clone();
        return newMap;
    }
}