using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Std(string packageName) : Pack(packageName)
    {
        Obj Write(Obj parameter)
        {
            Console.Write(parameter.CStr().value);
            return None;
        }

        Obj Writeln(Obj parameter)
        {
            Console.WriteLine(parameter.CStr().value);
            return None;
        }

        Str Readln(Obj parameter) => new(Console.ReadLine()!);

        Str Type(Obj parameter) => parameter.Type();

        Iter Func(Obj parameter)
        {
            List<string> func = [];

            foreach (var i in Process.Properties)
                if (i.Value is Fun)
                    func.Add(i.Key);

            return new Iter(func.Select(i => new Str(i)).ToArray());
        }

        Iter Range(Obj parameter)
        {
            if (parameter is Iter iter)
            {
                if (iter[0] is not Int i1 || !i1.value.TryInt(out int start)) return [];
                if (iter[1] is not Int i2 || !i2.value.TryInt(out int count)) return [];
                Obj[] objs = new Obj[count];

                for (int i = 0; i < count; i++)
                    objs[i] = new Int(start + i);

                return new Iter(objs);
            }

            return [];
        }

        Int Len(Obj parameter) => parameter.Len();

        Int Hash(Obj parameter) => parameter.Hash();

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
