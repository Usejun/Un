namespace Un.Util;

public class Std : Obj, IPackage
{
    public string Name => "std";

    Obj Write(Collections.Tuple args)
    {
        foreach (var p in args)
            Console.Write(p.CStr().Value + " ");
        return None;
    }

    Obj Writeln(Collections.Tuple args)
    {
        Write(args);
        Console.Write('\n');
        return None;
    }

    Obj Clear(Collections.Tuple args)
    {
        Console.Clear();
        return None;
    }

    Str Readln(Collections.Tuple args) => new(Console.ReadLine()!);

    Str Type(Collections.Tuple args) => args[0].Type();

    List Method(Collections.Tuple args)
    {
        List<string> methods = [];

        if (args[0] is Fun f)
        {
            foreach (var i in f.Call([]).field.Keys)                
                if (field[i] is Fun) methods.Add(i);
        }
        else if (args[0] is Obj o)
        {
            foreach (var i in o.field.Keys)
                if (o.field[i] is Fun) methods.Add(i);
        }
        else
        {
            foreach (var i in Process.Field.Keys)
                if (field[i] is Fun) methods.Add(i);
        }

        return new List(methods);
    }

    List Prop(Collections.Tuple args)
    {
        List<string> prop = [];

        if (args[0] is Fun f)
        {
            foreach (var i in f.Call([]).field.Keys)
                if (field[i] is not Fun) prop.Add(i);
        }
        else if (args[0] is Obj o)
        {
            foreach (var i in o.field.Keys)
                if (o.field[i] is not Fun) prop.Add(i);
        }
        else
        {
            foreach (var i in Process.Field.Keys)
                if (field[i] is not Fun) prop.Add(i);
        }

        return new List(prop);
    }

    List Field(Collections.Tuple args)
    {
        List fields = [];

        foreach (var arg in args)
        {
            string[] keys;

            if (arg is Fun f) keys = f.Call([]).field.Keys;
            else if (arg is Obj o) keys = o.field.Keys;
            else throw new ValueError();

            fields.Extend(new List(keys));
        }        

        return args.Count == 0 ? new(Process.Field.Keys) : fields;
    }

    List Package(Collections.Tuple args) => new(Process.Package.Keys);        

    List Range(Collections.Tuple args)
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

    Int Len(Collections.Tuple args) => args[0].Len();

    Int Hash(Collections.Tuple args) => args[0].Hash();

    IO.Stream Open(Collections.Tuple args) 
    {
        if (args[0] is Str s) return new(File.Open(s.Value, FileMode.Open));
        if (args[0] is HttpsResponse hr) return new(hr.Value.Content.ReadAsStreamAsync().Result);

        throw new FileError("File types you can't open");
    }

    Obj Sum(Collections.Tuple args)
    {
        if (args[0] is List list) return Sum(new(list.Value));

        Obj sum = args[0];
        for (int i = 1; i < args.Count; i++)
            sum = sum.Add(args[i]);
        return sum;
    }

    Obj Max(Collections.Tuple args)
    {
        if (args[0] is List list) return Max(new(list.Value));

        Obj max = args[0];
        for (int i = 1; i < args.Count; i++)
            if (max.LessThen(args[i]).Value)
                max = args[i];
        return max;
    }

    Obj Min(Collections.Tuple args)
    {
        if (args[0] is List list) return Min(new(list.Value));

        Obj min = args[0];
        for (int i = 1; i < args.Count; i++)
            if (min.GreaterThen(args[i]).Value)
                min = args[i];
        return min;
    }

    Obj Abs(Collections.Tuple args)
    {
        if (args[0] is Int i) return new Int(System.Math.Abs(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Abs(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Pow(Collections.Tuple args)
    {
        if (args[1] is not Int i) throw new ValueError("invalid argument");

        int count = (int)i.Value;

        if (count == 0) return new Int(0);
        if (count == 1) return args[0];
        if (count % 2 == 1) return args[0].Mul(Pow(new List([args[0], new Int(count - 1)])));
        var p = Pow(new List([args[0], new Int(count / 2)]));
        return p.Mul(p);
    }

    Obj Ceil(Collections.Tuple args)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Ceiling((decimal)i.Value));
        if (args[0] is Float f) return new Float(System.Math.Ceiling(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Floor(Collections.Tuple args)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Floor((decimal)i.Value));
        if (args[0] is Float f) return new Float(System.Math.Floor(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Round(Collections.Tuple args)
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

    Float Sqrt(Collections.Tuple args) 
    {
        if (args[0] is Int i) return new Float(System.Math.Sqrt(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Sqrt(f.Value));
        throw new ValueError("invalid argument");
    }

    Float Sin(Collections.Tuple args)
    {
        if (args[0] is Int i) return new Float(System.Math.Sin(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Sin(f.Value));
        throw new ValueError("invalid argument");
    }

    Float Cos(Collections.Tuple args)
    {
        if (args[0] is Int i) return new Float(System.Math.Cos(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Cos(f.Value));
        throw new ValueError("invalid argument");
    }

    Float Tan(Collections.Tuple args)
    {
        if (args[0] is Int i) return new Float(System.Math.Tan(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Tan(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Exit(Collections.Tuple args)
    {
        Environment.Exit(0);
        return None;
    }

    Obj Assert(Collections.Tuple args)
    {
        Debug.Assert(args[0].CBool().Value, args[1].CStr().Value);

        return None;
    }

    Obj Breakpoint(Collections.Tuple args) => None;

    Str Bin(Collections.Tuple args)
    {
        if (args[0] is Int i) return new("0b" + System.Convert.ToString(i.Value, 2));
        throw new ValueError("invalid argument");
    }

    Str Oct(Collections.Tuple args)
    {
        if (args[0] is Int i) return new("0o" + System.Convert.ToString(i.Value, 8));
        throw new ValueError("invalid argument");
    }

    Str Hex(Collections.Tuple args)
    {
        if (args[0] is Int i) return new("0x" + System.Convert.ToString(i.Value, 16));
        throw new ValueError("invalid argument");
    }

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("write", -1, Write),
        new NativeFun("writeln", -1, Writeln),
        new NativeFun("clear", -1, Clear),
        new NativeFun("readln", 0, Readln),
        new NativeFun("type", 1, Type),
        new NativeFun("method", -1, Method),
        new NativeFun("field", -1, Field),
        new NativeFun("prop", -1, Prop),
        new NativeFun("package", 0, Package),
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
        new NativeFun("bin", 1, Bin),
        new NativeFun("oct", 1, Oct),
        new NativeFun("hex", 1, Hex),
        new NativeFun("assert", 2, Assert),
        new NativeFun("breakpoint", 0, Breakpoint),
    ];

}
