using Un.Object;
using Un.Object.Function;
using Un.Object.Reference;
using Un.Object.Value;
using Un.Supporter;

namespace Un.Package
{
    public class Std(string packageName) : Pack(packageName)
    {
        Obj Write(Iter paras)
        {
            foreach (var para in paras)
                Console.Write(para.CStr().value);
            return None;
        }

        Obj Writeln(Iter paras)
        {
            Write(paras);
            Console.Write('\n');
            return None;
        }

        Str Readln(Iter paras) => new(Console.ReadLine()!);

        Str Type(Iter paras) => paras[0].Type();

        Iter Func(Iter paras)
        {
            List<string> func = [];

            foreach (var i in Process.Properties)
                if (i.Value is Fun)
                    func.Add(i.Key);

            return new Iter(func.Select(i => new Str(i)).ToArray());
        }

        Iter Range(Iter paras)
        {
            if (paras[0] is not Int i1 || !i1.value.TryInt(out int start)) throw new ArgumentException(nameof(paras));
            if (paras[1] is not Int i2 || !i2.value.TryInt(out int count)) throw new ArgumentException(nameof(paras));

            Obj[] objs = new Obj[count];

            for (int i = 0; i < count; i++)
                objs[i] = new Int(start + i);

            return new Iter(objs);
        }

        Int Len(Iter paras) => paras[0].Len();

        Int Hash(Iter paras) => paras[0].Hash();

        Object.Reference.File Open(Iter paras) => new(paras[0].CStr());

        Obj Sum(Iter para)
        {
            if (para[0] is Iter iter)
            {
                Obj obj = iter[0];
                for (int i = 1; i < iter.Count; i++)
                    obj = obj.Add(iter[i]);
                return obj;
            }

            throw new ArgumentException(nameof(para));
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

        Int Abs(Iter para)
        {
            if (para[0] is not Int i) throw new ArgumentException("invaild argument", nameof(para));

            if (i.value < 0) return new(-i.value);
            return new Int(i.value);
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
            new NativeFun("write", Write),
            new NativeFun("writeln", Writeln),
            new NativeFun("readln", Readln),
            new NativeFun("type", Type),
            new NativeFun("func", Func),
            new NativeFun("range", Range),
            new NativeFun("len", Len),
            new NativeFun("hash", Hash),
            new NativeFun("open", Open),
            new NativeFun("sum", Sum),
            new NativeFun("max", Max),
            new NativeFun("min", Min),
            new NativeFun("pow", Pow),
            new NativeFun("abs", Abs),
            new NativeFun("exit", Exit),
            new NativeFun("assert", Assert),
        ];
       
    }
}
