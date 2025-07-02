global using Scope = System.Collections.Generic.IDictionary<string, Un.Object.Obj>;
global using Attributes = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;
global using Map = System.Collections.Generic.Dictionary<string, Un.Object.Obj>;

using Un.Object;
using Un.Object.Flow;
using Un.Object.Util;
using Un.Object.Media;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;
using System.Collections.Concurrent;

namespace Un;

public static class Global
{
    public static object SyncRoot = new();
    public static bool WriteLog { get; private set; }

    public static readonly int BASEHASH = Math.Abs(DateTime.Now.Millisecond * 6929891 + DateTime.Now.Second * 1025957);
    public static readonly int HASHPRIME = 11;

    public static int MaxDepth = 1000;

    public static ConcurrentDictionary<string, Obj> Scope { get; set; } = new();
    public static ConcurrentDictionary<string, Obj> Class { get; private set; } = new();

    static Global()
    {
        Class["video"] = new Video(null!);
        Class["audio"] = new Audio(null!);
        Class["image"] = new Image(null!);
        Class["time"] = new Time();
        Class["long"] = new Long();
        Class["flow"] = new Flow();
    }

    public static Attributes Package { get; private set; } = [];

    public static void Init(bool writeLog = false)
    {
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

        Scope["__name__"] = new Str("__main__");

        foreach (var (key, value) in std.GetOriginalMembers())
            Scope[key] = value;

        foreach (var (key, value) in std.GetOriginalMethods())
            Scope[key] = value;

        WriteLog = writeLog;
    }

    public static void InitTypeFromInstance<T>(T obj)
        where T : Obj, new()
    {
        var name = obj.Type;

        Class[name].Members = obj.GetOriginal();

        if (obj is IPack pack)
        {
            foreach (var (key, value) in pack.GetOriginalMembers())
                Scope[key] = value; ;

            var group = pack.GetOriginalMethods();

            if (group.Count != 0)
                Scope[name] = new Obj(name)
                {
                    Members = group,
                };
        }
    }

    public static void InitTypeByName<T>(string name = null!)
        where T : Obj, new()
    {
        var t = new T();

        Class.TryAdd(name ?? typeof(T).Name.ToLower(), new T
        {
            Members = t.GetOriginal()
        });
    }

    public static void Include(string name)
    {
        InitTypeFromInstance(Class[name]);
    }

    public static void Import(string name, string path, string nickname, string[] parts)
    {
        var fullPath = Path.Combine(path, name);

        if (!Path.Exists(fullPath))
            throw new Panic($"file {name} not found in {path}");

        var scope = new Map();

        Runner.Load(name, scope, fullPath);

        if (!string.IsNullOrEmpty(nickname))
        {
            Obj module = new(nickname);
            CreateNamespace(module.Members);
            Scope[nickname] = module;
        }
        else
            CreateNamespace(Scope);

        void CreateNamespace(Scope destination)
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

    public static bool IsClass(string name) => Class.ContainsKey(name);

    public static bool TryGetOriginalValue(string type, string name, out Obj value)
    {
        if (Class.TryGetValue(type, out var original))
            return original.Members.TryGetValue(name, out value);

        value = Obj.Error;
        return false;
    }

    public static Obj GetGlobalVariable(string name) => Scope[name];

    public static Obj SetGlobalVariable(string name, Obj obj) => Scope[name] = obj;

    public static bool TryGetGlobalVariable(string name, out Obj obj) => Scope.TryGetValue(name, out obj!); 
}