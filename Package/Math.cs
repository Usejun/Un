using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Math(string packageName) : Pack(packageName), IStatic
    {
        Obj Sum(Obj parameter)
        {
            if (parameter is Iter iter)
            {
                Obj result = iter[0];
                for (int i = 1; i < iter.Count; i++)
                    result = result.Add(iter[i]);
                return result;
            }
            else throw new ArgumentException();
        }

        Obj Pow(Obj parameter)
        {
            Obj result;

            if (parameter is Iter iter && iter[1] is Int count)
            {
                result = new Int(1);
                for (int i = 0; i < count.value; i++)
                    result = result.Mul(iter[0]);
                return result;
            }
            throw new ArgumentException();
        }

        public override IEnumerable<Fun> Import() =>
        [
            new NativeFun("sum", Sum),
            new NativeFun("pow", Pow),
        ];

        public Pack Static()
        {
            Math math = new(packageName);
            math.properties.Add("pi", new Float(3.14159265));
            math.properties.Add("e", new Float(2.718281828));
            return math;
        }
    }
}
