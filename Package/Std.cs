using Un.Interpreter;

namespace Un.Package;

public class Std : Obj, IPackage
{
    public string Name => "std";

    private readonly Dictionary<string, Dictionary<int, Obj>> memo = []; 

    Obj Write(Field field)
    {
        if (!field["values"].As<List>(out var list))
            throw new ValueError("invalid argument");

        foreach (var obj in list)
        {
            Console.Write(obj.CStr().Value);
            Console.Write(field["sep"].CStr().Value);
        }
        return None;
    }

    Obj Writeln(Field field)
    {
        Write(field);
        Console.Write(field["end"].CStr().Value);
        return None;
    }

    Obj Clear(Field field)
    {
        Console.Clear();
        return None;
    }

    Str Readln(Field field) => new(Console.ReadLine()!);

    Str Type(Field field) => field["value"].Type();

    List Array(Field field)
    {      
        if (!field.Key("default"))
            throw new ValueError("invalid argument");

        if (!field.Key("length") || !field["length"].As<List>(out var length))
            throw new ValueError("invalid argument");
        
        List Create(List lengths)
        {
            List list = [];
    
            if (!lengths[0].As<Int>(out var count))
                    throw new ValueError("length argument is expected an integer");

            for (int i = 0; i < count.Value; i++)
                    list.Append(lengths.Count == 1 ? field["default"].Clone() : Create(new(lengths.Value[1..])));     

            return list;
        }

        return Create(length);
    }

    List Method(Field field)
    {        
        List<string> methods = [];
        var value = field["value"];

        if (IsNone(value))
        {
            foreach (var i in Un.Process.Field.Keys)
                if (Un.Process.Field[i].As<Fun>(out _)) 
                methods.Add(i);
        }
        else if (value.As<Fun>(out var f))
        {
            foreach (var i in f.Call([], new()).field.Keys)
                if (field[i].As<Fun>(out _)) methods.Add(i);
        }
        else
        {
            foreach (var i in value.field.Keys)
                if (value.field[i].As<Fun>(out _)) 
                    methods.Add(i);
        }

        return new List(methods);
    }

    List Prop(Field field)
    {
        List<string> prop = [];

        if (!field.Get("value", out Obj value))
        {
            foreach (var i in Un.Process.Field.Keys)
                if (Un.Process.Field[i].As<Fun>(out _)) 
                    prop.Add(i);
        }
        else if (value.As<Fun>(out var f))
        {
            foreach (var i in f.Call([], new()).field.Keys)
                if (field[i].As<Fun>(out _)) 
                    prop.Add(i);
        }
        else
        {
            foreach (var i in value.field.Keys)
                if (value.field[i].As<Fun>(out _)) 
                    prop.Add(i);
        }

        return new List(prop);
    }

    List Field(Field field) => field.Get("value", out var value) ? new(value.field.Keys) : new(Un.Process.Field.Keys);

    List Package(Field field) => new(Un.Process.Package.Keys);        

    List Range(Field field)
    {
        if (!field["start"].As<Int>(out var start))
            throw new ArgumentError();
        if (!field["end"].As<Int>(out var end))
            throw new ValueError();

        int diff = (int)(end.Value - start.Value);
        int step = field.Get("step", out var v1) && v1.As<Int>(out var v2) ? 
                   v2.Value > int.MaxValue ? throw new ArgumentError("step must be less than 2^32.") : (int)v2.Value : 1;
        Obj[] objs = new Obj[diff];

        for (int i = 0; i < diff; i+=step)
            objs[i] = new Int(i + start.Value);

        return new List(objs);
    }

    Int Len(Field field) => field["value"].Len();

    Int Hash(Field field) => field["value"].Hash();

    IO.Stream Open(Field field) 
    {
        if (field["value"].As<Str>(out var path)) return new(File.Open(path.Value, FileMode.Open));
        if (field["value"].As<HttpsResponse>(out var hr)) return new(hr.Value.Content.ReadAsStreamAsync().Result);

        throw new FileError("File types you can't open");
    }

    Obj Sum(Field field)
    {
        var list = field["value"].CList();
        
        if (list.Count == 0) 
            throw new ArgumentError("expected more then one argmuent"); 
        
        if (list.Count == 1)
        {
            if (list[0].As<List>(out var l)) 
                list = l;
            else if (list[0].As<Collections.Tuple>(out var t))
                list = t.CList();
            else 
                return list[0];
        }

        Obj sum = list[0];

        for (int i = 1; i < list.Count; i++)
            sum = sum.Add(list[i], Un.Field.Null);

        return sum;
    }

    Obj Max(Field field)
    {
       var list = field["value"].CList();
        
        if (list.Count == 0) 
            throw new ArgumentError("expected more then one argmuent"); 
        
        if (list.Count == 1)
        {
            if (list[0].As<List>(out var l)) 
                list = l;
            else if (list[0].As<Collections.Tuple>(out var t))
                list = t.CList();
            else 
                return list[0];
        }

        Obj max = list[0];

        for (int i = 1; i < list.Count; i++)
            if (max.Lt(list[i], Un.Field.Null).Value)
                max = list[i];

        return max;
    }

    Obj Min(Field field)
    {
        var list = field["value"].CList();
        
        if (list.Count == 0) 
            throw new ArgumentError("expected more then one argmuent"); 
        
        if (list.Count == 1)
        {
            if (list[0].As<List>(out var l)) 
                list = l;
            else if (list[0].As<Collections.Tuple>(out var t))
                list = t.CList();
            else 
                return list[0];
        }

        Obj min = list[0];

        for (int i = 1; i < list.Count; i++)
            if (min.Gt(list[i], Un.Field.Null).Value)
                min = list[i];

        return min;
    }

    Obj Pow(Field field)
    {
        if (!field["y"].As<Int>(out var y)) 
            throw new ValueError("x^y, y must be an int");

        var x = field["x"];
        int count = y.Value > int.MaxValue ? throw new ArgumentError("y must be less than 2^32.") : (int)y.Value;

        if (count == 0) return new Int(0);
        if (count == 1) return x;
        if (count % 2 == 1) return x.Mul(Pow(new Collections.Tuple(x, new Int(count - 1)), Un.Field.Null), Un.Field.Null);
        var p = Pow(new Collections.Tuple(x, new Int(count / 2)), Un.Field.Null);
        return p.Mul(p, Un.Field.Null);
    }

    Obj Round(Field field)
    {
        double v = field["value"] switch
        {
            Int i => i.Value,
            Float f => f.Value,
            _ => throw new ArgumentError("expected numbers")
        };

        if (!field["digit"].As<Int>(out var digit) || digit.Value > 15 || digit.Value < 0)
            throw new ArgumentError("digit is must be int and greater then 0 and less then 15");

        int d = (int)digit.Value;

        v = System.Math.Round(v, d);

        return d == 0 ? new Int((long)v) : new Float(v);
    }

    Obj Abs(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new Int(System.Math.Abs(i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Abs(f.Value));
        throw new ValueError("expected int or float");
    }

    Obj Ceil(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new Int((long)System.Math.Ceiling((decimal)i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Ceiling(f.Value));
        throw new ValueError("value must be a number");
    }

    Obj Floor(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new Int((long)System.Math.Floor((decimal)i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Floor(f.Value));
        throw new ValueError("value must be a number");
    }

    Float Sqrt(Field field) 
    {
        if (field["value"].As<Int>(out var i)) return new Float(System.Math.Sqrt(i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Sqrt(f.Value));
        throw new ValueError("value must be a number");
    }

    Float Sin(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new Float(System.Math.Sin(i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Sin(f.Value));
        throw new ValueError("value must be a number");
    }

    Float Cos(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new Float(System.Math.Cos(i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Cos(f.Value));
        throw new ValueError("value must be a number");
    }

    Float Tan(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new Float(System.Math.Tan(i.Value));
        if (field["value"].As<Float>(out var f)) return new Float(System.Math.Tan(f.Value));
        throw new ValueError("value must be a number");
    }

    Obj Exit(Field field)
    {
        field["exit_code"].As<Int>(out var v);            

        Environment.Exit((int)v.Value);
        return None;
    }

    Obj Assert(Field field)
    {
        if (field["condition"].CBool().Value)
            throw new AssertError(field["text"].CStr().Value);

        return None;
    }

    Obj Breakpoint(Field field) => None;

    Str Bin(Field field)
    {
        if (field["value"].As<Int>(out var i)) return new(Literals.Bin + Convert.ToString(i.Value, 2));
        throw new ValueError("value must be an int");
    }

    Str Oct(Field field)
    {
         if (field["value"].As<Int>(out var i)) return new(Literals.Oct + Convert.ToString(i.Value, 8));
        throw new ValueError("value must be an int");
    }

    Str Hex(Field field)
    {
         if (field["value"].As<Int>(out var i))return new(Literals.Hex + Convert.ToString(i.Value, 16));
        throw new ValueError("value must be an int");
    }

    Obj Eval(Field field)
    {
        if (!field["code"].As<Str>(out var code))
            throw new ArgumentError("code is must be str");

        Field local = new();
        local.Merge(field);
        Parser parser = new([code.Value], local);

        while (parser.TryInterpret()) ;

        return parser.ReturnValue ?? None;
    }

    Obj Memo(Field field)
    {
        if (!field["func"].As<Fun>(out var fun))
            throw new ArgumentError("argument 1 is expected a function");

        if (!field["args"].As<List>(out var args))
            throw new ArgumentError("argument more then 2");

        if (!memo.ContainsKey(fun.Name))
            memo.Add(fun.Name, []);

        var key = args.AsTuple();
        var hash = key.GetHashCode();

        if (!memo[fun.Name].TryGetValue(hash, out Obj? result))
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
        new NativeFun("write", 0, Write, [("sep", new Str(Literals.Space)), ("values",null!)], true),
        new NativeFun("writeln", 0, Writeln, [("sep", new Str(Literals.Space)), ("end", new Str(Literals.NewLine)), ("values", null!)], true),
        new NativeFun("clear", 0, Clear, []),
        new NativeFun("readln", 0, Readln, []),
        new NativeFun("type", 1, Type, [("value", null!)]),
        new NativeFun("array", 1, Array, [("default", null!), ("lengths", null!)], true),
        new NativeFun("method", 0, Method, [("value", None)]),
        new NativeFun("field", 0, Field, [("value", None)]),
        new NativeFun("prop", 0, Prop, [("value", None)]),
        new NativeFun("package", 0, Package, []),
        new NativeFun("range", 2, Range, [("start", null!), ("end", null!), ("step", null!)]),
        new NativeFun("len", 1, Len, [("value", null!)]),
        new NativeFun("hash", 1, Hash, [("value", null!)]),
        new NativeFun("open", 1, Open, [("value", null!)]),
        new NativeFun("sum", 0, Sum, [("values", null!)], true),
        new NativeFun("max", 0, Max, [("values", null!)], true),
        new NativeFun("min", 0, Min, [("values", null!)], true),
        new NativeFun("pow", 2, Pow, [("x", null!), ("y", null!)]),
        new NativeFun("abs", 1, Abs, [("value", null!)]),
        new NativeFun("sin", 1, Sin, [("value", null!)]),
        new NativeFun("cos", 1, Cos, [("value", null!)]),
        new NativeFun("tan", 1, Tan, [("value", null!)]),
        new NativeFun("ceil", 1, Ceil, [("value", null!)]),
        new NativeFun("floor", 1, Floor, [("value", null!)]),
        new NativeFun("round", 1, Round, [("value", null!), ("digit", Int.One)]),
        new NativeFun("sqrt", 1, Sqrt, [("value", null!)]),
        new NativeFun("exit", 0, Exit, [("exit_code", Int.Zero)]),
        new NativeFun("bin", 1, Bin, [("value", null!)]),
        new NativeFun("oct", 1, Oct, [("value", null!)]),
        new NativeFun("hex", 1, Hex, [("value", null!)]),
        new NativeFun("assert", 2, Assert, [("codition", null!), ("text", null!)]),
        new NativeFun("breakpoint", 0, Breakpoint, []),
        new NativeFun("eval", 1, Eval, [("code", null!)]),
        new NativeFun("memo", 1, Memo, [("func", null!), ("args", null!)], true),
    ];

}
