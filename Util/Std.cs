namespace Un.Util;

public class Std : Obj, IPackage
{
    public string Name => "std";

    Obj Write(Iter args)
    {
        foreach (var p in args)
            Console.Write(p.CStr().value + " ");
        return None;
    }

    Obj Writeln(Iter args)
    {
        Write(args);
        Console.Write('\n');
        return None;
    }

    Str Readln(Iter args) => new(Console.ReadLine()!);

    Str Type(Iter args) => args[0].Type();

    Iter Func(Iter args)
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

        return new Iter([.. func.Select(i => new Str(i))]);
    }

    Iter Memb(Iter args)
    {
        List<string> memb = [];

        if (args[0] is Fun f)
        {
            foreach (var i in f.Call([]).field.Keys)
                memb.Add(i);
        }
        else if (args[0] is Obj o)
        {
            foreach (var i in o.field.Keys)
                memb.Add(i);
        }
        else
        {
            foreach (var i in Process.Field.Keys)
                memb.Add(i);
        }

        return new Iter([.. memb.Select(i => new Str(i))]);
    }

    Iter Range(Iter args)
    {
        if (args[0] is not Int start)
            throw new ValueError("invalid argument");
        if (args[1] is not Int end)
            throw new ValueError("invalid argument");

        int diff = (int)(end.value - start.value);
        Obj[] objs = new Obj[diff];

        for (int i = 0; i < diff; i++)
            objs[i] = new Int(i + start.value);

        return new Iter(objs);
    }

    Int Len(Iter args) => args[0].Len();

    Int Hash(Iter args) => args[0].Hash();

    IO.Stream Open(Iter args) 
    {
        if (args[0] is Str s) return new(File.Open(s.value, FileMode.Open));
        if (args[0] is HttpsResponse hr) return new(hr.value.Content.ReadAsStreamAsync().Result);

        throw new FileError("File types you can't open");
    }

    Obj Sum(Iter args)
    {
        if (args[0] is Iter iter) return Sum(iter);

        Obj sum = args[0];
        for (int i = 1; i < args.Count; i++)
            sum = sum.Add(args[i]);
        return sum;
    }

    Obj Max(Iter args)
    {
        if (args[0] is Iter iter) return Max(iter);

        Obj max = args[0];
        for (int i = 1; i < args.Count; i++)
            if (max.LessThen(args[i]).value)
                max = args[i];
        return max;
    }

    Obj Min(Iter args)
    {
        if (args[0] is Iter iter) return Min(iter);

        Obj min = args[0];
        for (int i = 1; i < args.Count; i++)
            if (min.GreaterThen(args[i]).value)
                min = args[i];
        return min;
    }

    Obj Abs(Iter args)
    {
        if (args[0] is Int i) return new Int(System.Math.Abs(i.value));
        if (args[0] is Float f) return new Float(System.Math.Abs(f.value));
        throw new ValueError("invalid argument");
    }

    Obj Pow(Iter args)
    {
        if (args[1] is not Int i) throw new ValueError("invalid argument");

        int count = (int)i.value;

        if (count == 0) return new Int(0);
        if (count == 1) return args[0];
        if (count % 2 == 1) return args[0].Mul(Pow(new Iter([args[0], new Int(count - 1)])));
        var p = Pow(new Iter([args[0], new Int(count / 2)]));
        return p.Mul(p);
    }

    Obj Ceil(Iter args)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Ceiling((decimal)i.value));
        if (args[0] is Float f) return new Float(System.Math.Ceiling(f.value));
        throw new ValueError("invalid argument");
    }

    Obj Floor(Iter args)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Floor((decimal)i.value));
        if (args[0] is Float f) return new Float(System.Math.Floor(f.value));
        throw new ValueError("invalid argument");
    }

    Obj Round(Iter args)
    {
        if (args.Count > 2) throw new ValueError("invalid argument");

        double v = args[0] switch
        {
            Int i => i.value,
            Float f => f.value,
            _ => throw new ValueError("invalid argument"),
        };

        if (args.Count == 2)
            if (args[1] is Int i && i.value < 15 && i.value >= 0) return new Float((double)System.Math.Round(v, (int)i.value));
            else throw new ValueError("invalid argument");
        else return new Int((long)System.Math.Round(v));
    }

    Obj Sqrt(Iter args) 
    {
        if (args[0] is Int i) return new Float(System.Math.Sqrt(i.value));
        if (args[0] is Float f) return new Float(System.Math.Sqrt(f.value));
        throw new ValueError("invalid argument");
    }

    Obj Exit(Iter args)
    {
        Environment.Exit(0);
        return None;
    }

    Obj Assert(Iter args)
    {
        Debug.Assert(args[0].CBool().value, args[1].CStr().value);

        return None;
    }

    Obj Breakpoint(Iter args) => None;

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("write", -1, Write),
        new NativeFun("writeln", -1, Writeln),
        new NativeFun("readln", 0, Readln),
        new NativeFun("type", 1, Type),
        new NativeFun("func", -1, Func),
        new NativeFun("memb", -1, Memb),
        new NativeFun("range", 2, Range),
        new NativeFun("len", 1, Len),
        new NativeFun("hash", 1, Hash),
        new NativeFun("open", 1, Open),
        new NativeFun("sum", -1, Sum),
        new NativeFun("max", -1, Max),
        new NativeFun("min", -1, Min),
        new NativeFun("pow", 2, Pow),
        new NativeFun("abs", 1, Abs),
        new NativeFun("ceil", 1, Ceil),
        new NativeFun("floor", 1, Floor),
        new NativeFun("round", -1, Round),
        new NativeFun("sqrt", 1, Sqrt),
        new NativeFun("exit", 0, Exit),
        new NativeFun("assert", 2, Assert),
        new NativeFun("breakpoint", 0, Breakpoint),
    ];

}
