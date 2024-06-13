namespace Un.Util;

public class Std : Obj, IPackage
{
    public string Name => "std";

    Obj Write(List args)
    {
        foreach (var p in args)
            Console.Write(p.CStr().Value + " ");
        return None;
    }

    Obj Writeln(List args)
    {
        Write(args);
        Console.Write('\n');
        return None;
    }

    Obj Clear(List args)
    {
        Console.Clear();
        return None;
    }

    Str Readln(List args) => new(Console.ReadLine()!);

    Str Type(List args) => args[0].Type();

    List Func(List args)
    {
        List<string> func = [];

        if (args[0] is Fun f)
        {
            foreach (var i in f.Call([]).field.Keys)                
                if (field[i] is Fun)
                    func.Add(i);
        }
        else if (args[0] is Obj o)
        {
            foreach (var i in o.field.Keys)
                if (o.field[i] is Fun)
                    func.Add(i);
        }
        else
        {
            foreach (var i in Process.Field.Keys)
                if (field[i] is Fun)
                    func.Add(i);
        }

        return new List([.. func.Select(i => new Str(i))]);
    }

    List Memb(List args)
    {
        List membs = [];

        foreach (var arg in args)
        {
            string[] keys;

            if (arg is Fun f) keys = f.Call([]).field.Keys;
            else if (arg is Obj o) keys = o.field.Keys;
            else throw new ValueError();

            membs.Extend(new List(keys));
        }        

        return args.Count == 0 ? new(Process.Field.Keys) : membs;
    }

    List Pack(List args) => new(Process.Package.Keys);        

    List Range(List args)
    {
        if (args[0] is not Int start)
            throw new ValueError("invalid argument");
        if (args[1] is not Int end)
            throw new ValueError("invalid argument");

        int diff = (int)(end.Value - start.Value);
        Obj[] objs = new Obj[diff];

        for (int i = 0; i < diff; i++)
            objs[i] = new Int(i + start.Value);

        return new List(objs);
    }

    Int Len(List args) => args[0].Len();

    Int Hash(List args) => args[0].Hash();

    IO.Stream Open(List args) 
    {
        if (args[0] is Str s) return new(File.Open(s.Value, FileMode.Open));
        if (args[0] is HttpsResponse hr) return new(hr.Value.Content.ReadAsStreamAsync().Result);

        throw new FileError("File types you can't open");
    }

    Obj Sum(List args)
    {
        if (args[0] is List list) return Sum(list);

        Obj sum = args[0];
        for (int i = 1; i < args.Count; i++)
            sum = sum.Add(args[i]);
        return sum;
    }

    Obj Max(List args)
    {
        if (args[0] is List list) return Max(list);

        Obj max = args[0];
        for (int i = 1; i < args.Count; i++)
            if (max.LessThen(args[i]).Value)
                max = args[i];
        return max;
    }

    Obj Min(List args)
    {
        if (args[0] is List list) return Min(list);

        Obj min = args[0];
        for (int i = 1; i < args.Count; i++)
            if (min.GreaterThen(args[i]).Value)
                min = args[i];
        return min;
    }

    Obj Abs(List args)
    {
        if (args[0] is Int i) return new Int(System.Math.Abs(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Abs(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Pow(List args)
    {
        if (args[1] is not Int i) throw new ValueError("invalid argument");

        int count = (int)i.Value;

        if (count == 0) return new Int(0);
        if (count == 1) return args[0];
        if (count % 2 == 1) return args[0].Mul(Pow(new List([args[0], new Int(count - 1)])));
        var p = Pow(new List([args[0], new Int(count / 2)]));
        return p.Mul(p);
    }

    Obj Ceil(List args)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Ceiling((decimal)i.Value));
        if (args[0] is Float f) return new Float(System.Math.Ceiling(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Floor(List args)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Floor((decimal)i.Value));
        if (args[0] is Float f) return new Float(System.Math.Floor(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Round(List args)
    {
        if (args.Count > 2) throw new ValueError("invalid argument");

        double v = args[0] switch
        {
            Int i => i.Value,
            Float f => f.Value,
            _ => throw new ValueError("invalid argument"),
        };

        if (args.Count == 2)
            if (args[1] is Int i && i.Value < 15 && i.Value >= 0) return new Float((double)System.Math.Round(v, (int)i.Value));
            else throw new ValueError("invalid argument");
        else return new Int((long)System.Math.Round(v));
    }

    Obj Sqrt(List args) 
    {
        if (args[0] is Int i) return new Float(System.Math.Sqrt(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Sqrt(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Sin(List args)
    {
        if (args[0] is Int i) return new Float(System.Math.Sin(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Sin(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Cos(List args)
    {
        if (args[0] is Int i) return new Float(System.Math.Cos(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Cos(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Tan(List args)
    {
        if (args[0] is Int i) return new Float(System.Math.Tan(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Tan(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Exit(List args)
    {
        Environment.Exit(0);
        return None;
    }

    Obj Assert(List args)
    {
        Debug.Assert(args[0].CBool().Value, args[1].CStr().Value);

        return None;
    }

    Obj Breakpoint(List args) => None;

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("write", -1, Write),
        new NativeFun("writeln", -1, Writeln),
        new NativeFun("clear", -1, Clear),
        new NativeFun("readln", 0, Readln),
        new NativeFun("type", 1, Type),
        new NativeFun("func", -1, Func),
        new NativeFun("memb", -1, Memb),
        new NativeFun("pack", 0, Pack),
        new NativeFun("range", 2, Range),
        new NativeFun("len", 1, Len),
        new NativeFun("hash", 1, Hash),
        new NativeFun("open", 1, Open),
        new NativeFun("sum", -1, Sum),
        new NativeFun("max", -1, Max),
        new NativeFun("min", -1, Min),
        new NativeFun("pow", 2, Pow),
        new NativeFun("abs", 1, Abs),
        new NativeFun("sin", 1, Sin),
        new NativeFun("cos", 1, Cos),
        new NativeFun("tan", 1, Tan),
        new NativeFun("ceil", 1, Ceil),
        new NativeFun("floor", 1, Floor),
        new NativeFun("round", -1, Round),
        new NativeFun("sqrt", 1, Sqrt),
        new NativeFun("exit", 0, Exit),
        new NativeFun("assert", 2, Assert),
        new NativeFun("breakpoint", 0, Breakpoint),
    ];

}
