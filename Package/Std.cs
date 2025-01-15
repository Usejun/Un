using Un.Interpreter;

namespace Un.Package;

public class Std : Obj, IPackage
{
    public string Name => "std";

    private Dictionary<string, Dictionary<int, Obj>> memo = new(); 

    Obj Write(Collections.Tuple args, Field field)
    {
        foreach (var p in args)
            Console.Write(p.CStr().Value + " ");
        return None;
    }

    Obj Writeln(Collections.Tuple args, Field field)
    {
        Write(args, field);
        Console.Write('\n');
        return None;
    }

    Obj Clear(Collections.Tuple args, Field field)
    {
        Console.Clear();
        return None;
    }

    Str Readln(Collections.Tuple args, Field field) => new(Console.ReadLine()!);

    Str Type(Collections.Tuple args, Field field) => args[0].Type();

    List Array(Collections.Tuple args, Field field)
    {      
        if (args.Count < 2 || args[1] is not Int length) throw new ValueError("invalid arguments");

        List list = [];

        if (args.Count == 2)
        {
            for (int i = 0; i < length.Value; i++)
                list.Append(args[0]);         
        }
        else
        {
            List args2 = args.CList();
            args2.RemoveAt(new Int(2));
            for (int i = 0; i < length.Value; i++)
                list.Append(Array(args2.AsTuple(), field));
        }

        return list;
    }

    List Method(Collections.Tuple args, Field field)
    {
        List<string> methods = [];

        if (args.Count == 0)
        {
            foreach (var i in Process.Field.Keys)
                if (Process.Field[i] is Fun) methods.Add(i);
        }
        else if (args[0] is Fun f)
        {
            foreach (var i in f.Call([], new()).field.Keys)
                if (field[i] is Fun) methods.Add(i);
        }
        else if (args[0] is Obj o)
        {
            foreach (var i in o.field.Keys)
                if (o.field[i] is Fun) methods.Add(i);
        }
        else throw new SyntaxError();

        return new List(methods);
    }

    List Prop(Collections.Tuple args, Field field)
    {
        List<string> prop = [];

        if (args.Count == 0)
        {
            foreach (var i in Process.Field.Keys)
                if (Process.Field[i] is not Fun) prop.Add(i);
        }
        else if (args[0] is Fun f)
        {
            foreach (var i in f.Call([], new()).field.Keys)
                if (field[i] is not Fun) prop.Add(i);
        }
        else if (args[0] is Obj o)
        {
            foreach (var i in o.field.Keys)
                if (o.field[i] is not Fun) prop.Add(i);
        }
        else throw new SyntaxError();

        return new List(prop);
    }

    List Field(Collections.Tuple args, Field field)
    {
        List fields = [];

        foreach (var arg in args)
        {
            string[] keys;

            if (arg is Fun f) keys = f.Call([], new()).field.Keys;
            else if (arg is Obj o) keys = o.field.Keys;
            else throw new ValueError();

            fields.Extend(new List(keys));
        }        

        return args.Count == 0 ? new(Process.Field.Keys) : fields;
    }

    List Package(Collections.Tuple args, Field field) => new(Process.Package.Keys);        

    List Range(Collections.Tuple args, Field field)
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

    Int Len(Collections.Tuple args, Field field) => args[0].Len();

    Int Hash(Collections.Tuple args, Field field) => args[0].Hash();

    IO.Stream Open(Collections.Tuple args, Field field) 
    {
        if (args[0] is Str s) return new(File.Open(s.Value, FileMode.Open));
        if (args[0] is HttpsResponse hr) return new(hr.Value.Content.ReadAsStreamAsync().Result);

        throw new FileError("File types you can't open");
    }

    Obj Sum(Collections.Tuple args, Field field)
    {
        if (args[0] is List list) return Sum(new(list.Value), field);

        Obj sum = args[0];
        for (int i = 1; i < args.Count; i++)
            sum = sum.Add(args[i], field);
        return sum;
    }

    Obj Max(Collections.Tuple args, Field field)
    {
        if (args[0] is List list) return Max(new(list.Value), field);

        Obj max = args[0];
        for (int i = 1; i < args.Count; i++)
            if (max.Lt(args[i], field).Value)
                max = args[i];
        return max;
    }

    Obj Min(Collections.Tuple args, Field field)
    {
        if (args[0] is List list) return Min(new(list.Value), field);

        Obj min = args[0];
        for (int i = 1; i < args.Count; i++)
            if (min.Gt(args[i], field).Value)
                min = args[i];
        return min;
    }

    Obj Abs(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new Int(System.Math.Abs(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Abs(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Pow(Collections.Tuple args, Field field)
    {
        if (args[1] is not Int i) throw new ValueError("invalid argument");

        int count = (int)i.Value;

        if (count == 0) return new Int(0);
        if (count == 1) return args[0];
        if (count % 2 == 1) return args[0].Mul(Pow(new Collections.Tuple(args[0], new Int(count - 1)), Un.Field.Null), Un.Field.Null);
        var p = Pow(new Collections.Tuple(args[0], new Int(count / 2)), Un.Field.Null);
        return p.Mul(p, Un.Field.Null);
    }

    Obj Ceil(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Ceiling((decimal)i.Value));
        if (args[0] is Float f) return new Float(System.Math.Ceiling(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Floor(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new Int((long)System.Math.Floor((decimal)i.Value));
        if (args[0] is Float f) return new Float(System.Math.Floor(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Round(Collections.Tuple args, Field field)
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

    Float Sqrt(Collections.Tuple args, Field field) 
    {
        if (args[0] is Int i) return new Float(System.Math.Sqrt(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Sqrt(f.Value));
        throw new ValueError("invalid argument");
    }

    Float Sin(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new Float(System.Math.Sin(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Sin(f.Value));
        throw new ValueError("invalid argument");
    }

    Float Cos(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new Float(System.Math.Cos(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Cos(f.Value));
        throw new ValueError("invalid argument");
    }

    Float Tan(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new Float(System.Math.Tan(i.Value));
        if (args[0] is Float f) return new Float(System.Math.Tan(f.Value));
        throw new ValueError("invalid argument");
    }

    Obj Exit(Collections.Tuple args, Field field)
    {
        Environment.Exit(0);
        return None;
    }

    Obj Assert(Collections.Tuple args, Field field)
    {
        if (args[0].CBool().Value)
            throw new AssertError(args[1].CStr().Value);

        return None;
    }

    Obj Breakpoint(Collections.Tuple args, Field field) => None;

    Str Bin(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new(Literals.Bin + System.Convert.ToString(i.Value, 2));
        throw new ValueError("invalid argument");
    }

    Str Oct(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new(Literals.Oct + System.Convert.ToString(i.Value, 8));
        throw new ValueError("invalid argument");
    }

    Str Hex(Collections.Tuple args, Field field)
    {
        if (args[0] is Int i) return new(Literals.Hex + System.Convert.ToString(i.Value, 16));
        throw new ValueError("invalid argument");
    }

    Obj Eval(Collections.Tuple args, Field field)
    {
        Field local = new();
        local.Add(field);
        Parser parser = new(args.Select(arg => arg.CStr().Value).ToArray(), local);

        while (parser.TryInterpret()) ;

        return parser.ReturnValue ?? None;
    }

    Obj Memo(Collections.Tuple args, Field field)
    {
        if (args[0] is not Fun fun)
            throw new ArgumentError();

        if (!memo.ContainsKey(fun.Name))
            memo.Add(fun.Name, new());

        var key = new Collections.Tuple(values: args.CList().Value[1..]);
        var hash = key.GetHashCode();

        if (!memo[fun.Name].TryGetValue(hash, out Obj result))
        {
            Obj value = fun.Call(key, field);
            memo[fun.Name].Add(hash, value);
            return value;
        }
        else
        {
            return result;
        }
        
    }

    public IEnumerable<Fun> Import() =>
    [
        new NativeFun("write", -1, Write),
        new NativeFun("writeln", -1, Writeln),
        new NativeFun("clear", -1, Clear),
        new NativeFun("readln", 0, Readln),
        new NativeFun("type", 1, Type),
        new NativeFun("array", -1, Array),
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
        new NativeFun("eval", -1, Eval),
        new NativeFun("memo", -1, Memo),
    ];

}
