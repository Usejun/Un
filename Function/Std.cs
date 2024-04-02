using Un.Object;

namespace Un.Function
{
    public class Std : Importable
    {
        public Obj Write(Obj parameter)
        {
            Console.Write(parameter == Obj.None ? "" : parameter);
            return Obj.None;
        }

        public Obj Writeln(Obj parameter)
        {
            Console.WriteLine(parameter == Obj.None ? "" : parameter);
            return Obj.None;
        }

        public Str Readln(Obj parameter) => new(Console.ReadLine()!);

        public Obj Type(Obj parametr) => new Str(parametr.GetType().Name.ToLower());

        public Int Int(Obj parameter)
        {
            if (parameter is Int i) return new(i.value);
            if (parameter is Float f) return new((long)f.value);
            if (parameter is Str s && long.TryParse(s.value.Trim('\"'), out var l)) return new(l);
            throw new ObjException("Convert Error");
        }

        public Float Float(Obj parameter)
        {
            if (parameter is Int i) return new(i.value);
            if (parameter is Float f) return new(f.value);
            if (parameter is Str s && double.TryParse(s.value, out var d)) return new(d);
            throw new ObjException("Convert Error");
        }

        public Str Str(Obj parameter) => new(parameter.ToString());

        public Bool Bool(Obj parameter) => parameter.ToString() switch
        {
            "True" => new(true),
            "False" => new(false),
            _ => throw new ObjException("Convert Error"),
        };

        public Iter Iter(Obj parameter) => Obj.Convert(parameter.ToString(), Process.Variable, Process.Func) is Iter i ? i : throw new ObjException("Convert Error");

        public Iter Func(Obj parameter) => new(Process.Func.Keys.Select(key => new Str(key)));

        public Iter Range(Obj parameter)
        {
            if (parameter is Iter iter)
            {
                Iter range = [];
                if (iter[0] is not Int i1 || !i1.value.TryInt(out int start)) return [];
                if (iter[1] is not Int i2 || !i2.value.TryInt(out int count)) return [];

                for (int i = 0; i < count; i++)
                    range.Append(new Int(i + start));
                return range;
            }

            return [];
        }

        public Int Len(Obj parameter) => parameter.Len();

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
            {"len", new NativeFun("len", "obj", Len)},
            {"range", new NativeFun("range", "start_len", Range)},
        };

    }
}
