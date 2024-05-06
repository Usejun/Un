using math = System.Math;

using Un.Collections;
using Un.Data;


namespace Un.Util
{
    public class Std(string packageName) : Pack(packageName)
    {
        Obj Write(Iter para)
        {
            foreach (var p in para)
                Console.Write(p.CStr().value + " ");
            return None;
        }

        Obj Writeln(Iter para)
        {
            Write(para);
            Console.Write('\n');
            return None;
        }

        Str Readln(Iter para) => new(Console.ReadLine()!);

        Str Type(Iter para) => para[0].Type();

        Iter Func(Iter para)
        {
            List<string> func = [];

            foreach (var i in Process.Properties)
                if (i.Value is Fun)
                    func.Add(i.Key);

            return new Iter(func.Select(i => new Str(i)).ToArray());
        }

        Iter Range(Iter para)
        {
            if (para[0] is not Int i1 || !i1.value.TryInt(out int start))
                throw new ArgumentException("invaild argument", nameof(para));
            if (para[1] is not Int i2 || !i2.value.TryInt(out int end))
                throw new ArgumentException("invaild argument", nameof(para));

            Obj[] objs = new Obj[end - start];

            for (int i = 0; i < end - start; i++)
                objs[i] = new Int(i + start);

            return new Iter(objs);
        }

        Int Len(Iter para) => para[0].Len();

        Int Hash(Iter para) => para[0].Hash();

        IO.Stream Open(Iter paras) => new(File.Open(paras[0].CStr().value, FileMode.Open));

        Obj Sum(Iter para)
        {
            if (para[0] is Iter iter) return Sum(iter);

            Obj sum = para[0];
            for (int i = 1; i < para.Count; i++)
                sum = sum.Add(para[i]);
            return sum;
        }

        Obj Max(Iter para)
        {
            if (para[0] is Iter iter) return Max(iter);

            Obj max = para[0];
            for (int i = 1; i < para.Count; i++)
                if (max.LessThen(para[i]).value)
                    max = para[i];
            return max;
        }

        Obj Min(Iter para)
        {
            if (para[0] is Iter iter) return Min(iter);

            Obj min = para[0];
            for (int i = 1; i < para.Count; i++)
                if (min.GreaterThen(para[i]).value)
                    min = para[i];
            return min;
        }

        Obj Abs(Iter para)
        {
            if (para[0] is Int i) return new Int(math.Abs(i.value));
            if (para[0] is Float f) return new Float(math.Abs(f.value));
            throw new ArgumentException("invaild argument", nameof(para));
        }

        Obj Pow(Iter para)
        {
            if (para[1] is not Int i || !i.value.TryInt(out var count)) throw new ArgumentException("invaild argument", nameof(para));

            if (count == 0) return new Int(0);
            if (count == 1) return para[0];
            if (count % 2 == 1) return para[0].Mul(Pow(new Iter([para[0], new Int(count - 1)])));
            var p = Pow(new Iter([para[0], new Int(count / 2)]));
            return p.Mul(p);
        }

        Obj Ceil(Iter para)
        {
            if (para[0] is Int i) return new Int((long)math.Ceiling((decimal)i.value));
            if (para[0] is Float f) return new Float(math.Ceiling(f.value));
            throw new ArgumentException("invaild argument", nameof(para));
        }

        Obj Floor(Iter para)
        {
            if (para[0] is Int i) return new Int((long)math.Floor((decimal)i.value));
            if (para[0] is Float f) return new Float(math.Floor(f.value));
            throw new ArgumentException("invaild argument", nameof(para));
        }

        Obj Round(Iter para)
        {
            if (para.Count > 2) throw new ArgumentException("invaild argument", nameof(para));

            double v = para[0] switch
            {
                Int i => i.value,
                Float f => f.value,
                _ => throw new ArgumentException("invaild argument", nameof(para)),
            };

            if (para.Count == 2)
                if (para[1] is Int i && i.value < 15 && i.value >= 0) return new Float((double)math.Round(v, (int)i.value));
                else throw new ArgumentException("invaild argument", nameof(para));
            else return new Float((double)math.Round(v));
        }

        Obj Sqrt(Iter para) 
        {
            if (para[0] is Int i) return new Float(math.Sqrt(i.value));
            if (para[0] is Float f) return new Float(math.Sqrt(f.value));
            throw new ArgumentException("invaild argument", nameof(para));
        }

        Obj Exit(Iter paras)
        {
            Environment.Exit(0);
            return None;
        }

        Obj Assert(Iter paras)
        {
            if (paras[0].CBool().value)
                throw new AssertException(paras[1].CStr().value);

            return None;
        }

        public override IEnumerable<Fun> Import() =>
        [
            new NativeFun("write", -1, Write),
            new NativeFun("writeln", -1, Writeln),
            new NativeFun("readln", 0, Readln),
            new NativeFun("type", 1, Type),
            new NativeFun("func", 1, Func),
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
        ];

    }
}
