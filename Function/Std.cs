using Un.Object;

namespace Un.Function
{
    public class Std : Importable
    {
        public Obj Write(Obj parameter)
        {
            Console.Write(parameter.CStr().value);
            return None;
        }

        public Obj Writeln(Obj parameter)
        {
            Console.WriteLine(parameter.CStr().value);
            return None;
        }

        public Str Readln(Obj parameter) => new(Console.ReadLine()!);

        public Obj Type(Obj parameter) => parameter.Type();

        public Int Int(Obj parameter) => parameter.CInt();

        public Float Float(Obj parameter) => parameter.CFloat();

        public Str Str(Obj parameter) => parameter.CStr();

        public Bool Bool(Obj parameter) => parameter.CBool();

        public Iter Iter(Obj parameter) => parameter.CIter();

        public Iter Func(Obj parameter)
        {
            List<string> func = [];

            foreach (var i in Process.Properties)
                if (i.Value is Fun)
                    func.Add(i.Key);

            return new Iter(func.Select(i => new Str(i)).ToArray());
        }

        public Iter Range(Obj parameter)
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

        public Int Len(Obj parameter) => parameter.Len();

        public Int Hash(Obj parameter) => parameter.Hash();

        public override Dictionary<string, Fun> Methods() => new()
        {
            {"write", new NativeFun("write", "text", Write)},
            {"writeln", new NativeFun("writeln", "text", Writeln)},
            {"readln", new NativeFun("readln", "arg", Readln)},
            {"int", new NativeFun("int", "value", Int)},
            {"float", new NativeFun("float", "value", Float)},
            {"str", new NativeFun("str", "value", Str)},
            {"bool", new NativeFun("bool", "value", Bool)},
            {"iter", new NativeFun("iter", "value", Iter)},
            {"type", new NativeFun("type", "obj", Type)},
            {"func", new NativeFun("func", "arg", Func)},
            {"range", new NativeFun("range", "start_len", Range)},
            {"len", new NativeFun("len", "obj", Len)},
            {"hash", new NativeFun("hash", "obj", Hash)},
        };

    }
}
