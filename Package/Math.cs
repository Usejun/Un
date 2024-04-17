using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Math(string packageName) : Pack(packageName), IStatic
    {
        Obj Sum(Iter para)
        {
            Obj result = para[0];
            for (int i = 1; i < para.Count; i++)
                result = result.Add(para[i]);
            return result;
        }

        Obj Pow(Iter para)
        {
            if (para[1] is not Int count) throw new ArgumentException();

            Obj result = para[0];
            for (int i = 0; i < count.value; i++)
                result = result.Mul(para[0]);
            return result.Div(para[0]);
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
