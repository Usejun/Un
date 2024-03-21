using Un.Object;

namespace Un
{
    public static class Std
    {
        public static Obj Write(Obj parameter)
        {
            Console.Write(parameter == Obj.None ? "" : parameter);
            return Obj.None;
        }

        public static Obj Writeln(Obj parameter)
        {
            Console.WriteLine(parameter == Obj.None ? "" : parameter);
            return Obj.None;
        }

        public static Str Readln(Obj parameter) => new(Console.ReadLine()!);

        public static Obj Type(Obj parametr) => new Str(parametr.GetType().Name.ToLower());

        public static Int Int(Obj parameter)
        {
            if (parameter is Int i) return new(i.value);
            if (parameter is Float f) return new((long)f.value);
            if (parameter is Str s && long.TryParse(s.value.Trim('\"'), out var l)) return new(l);
            throw new ObjException("Convert Error");
        }

        public static Float Float(Obj parameter)
        {
            if (parameter is Int i) return new(i.value);
            if (parameter is Float f) return new(f.value);
            if (parameter is Str s && double.TryParse(s.value, out var d)) return new(d);
            throw new ObjException("Convert Error");
        }

        public static Str Str(Obj parameter) => new(parameter.ToString());

        public static Iter Iter(Obj parameter) => Obj.Convert(parameter.ToString()) is Iter i ? i : throw new ObjException("Convert Error"); 

        public static Iter Func(Obj parameter) => new(Process.Function.Keys.Select(key => new Str(key)));

        public static Iter Range(Obj parameter)
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

        public static Int Len(Obj parameter)
        {
            if (parameter is Iter i) return new(i.Count);
            else return new(1);
        }
    }
}
