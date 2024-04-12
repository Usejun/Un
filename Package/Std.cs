using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Std(string packageName) : Pack(packageName)
    {
        Obj Write(Iter paras)
        {
            Console.Write(paras[0].CStr().value);
            return None;
        }

        Obj Writeln(Iter paras)
        {
            Console.WriteLine(paras[0].CStr().value);
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
            if (paras[0] is not Int i1 || !i1.value.TryInt(out int start)) return [];
            if (paras[1] is not Int i2 || !i2.value.TryInt(out int count)) return [];

            Obj[] objs = new Obj[count];

            for (int i = 0; i < count; i++)
                objs[i] = new Int(start + i);

            return new Iter(objs);
        }

        Int Len(Iter paras) => paras[0].Len();

        Int Hash(Iter paras) => paras[0].Hash();

        public override IEnumerable<Fun> Import() =>
        [
            new NativeFun("write", Write),
            new NativeFun("writeln", Writeln),
            new NativeFun("readln", Readln),
            new NativeFun("type", Type),
            new NativeFun("func", Func),
            new NativeFun("range", Range),
            new NativeFun("len", Len),
            new NativeFun("hash", Hash)
        ];
       
    }
}
