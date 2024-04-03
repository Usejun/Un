using Un.Object;

namespace Un.Function
{
    public class Math : Importable
    {
        public Obj Sum(Obj parameter)
        {
            if (parameter is Iter iter)
            {
                Obj result = iter[0];
                for (int i = 1; i < iter.Count; i++)
                    result = result.Add(iter[i]);
                return result;
            }
            else throw new ObjException("Sum Error");
        }

        public Obj Pow(Obj parameter)
        {
            Obj result = None;

            if (parameter is Iter iter && iter[1] is Int count)
            {
                result = new Int(1);
                for (int i = 0; i < count.value; i++)
                    result = result.Mul(iter[0]);                                    
            }
            else throw new ObjException("Pow Error");

            return result;
        }

        public Obj Pi(Obj parameter) => new Float(3.14159265);

        public Obj E(Obj parameter) => new Float(2.718281828);

        public override Dictionary<string, Fun> Methods() => new()
        {
            {"sum", new NativeFun("sum", "iter", Sum) },
            {"pow", new NativeFun("pow", "number_count", Pow) },
            {"pi", new NativeFun("pi", "none", Pi) },
            {"e", new NativeFun("e", "none", E) },
        };
    }
}
