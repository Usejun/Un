using Un.Collections;
using Un.Data;

namespace Un.Util
{
    public class Math(string packageName) : Pack(packageName), IStatic
    {
        Obj Pow(Iter para)
        {
            if (para[1] is not Int i || !i.value.TryInt(out var count)) throw new ArgumentException("invaild argument", nameof(para));

            if (count == 0) return new Int(0);
            if (count == 1) return para[1];
            if (count % 2 == 1) return para[0].Mul(Pow(new Iter([para[1], new Int(count - 1)])));
            var p = Pow(new Iter([para[1], new Int(count / 2)]));
            return p.Mul(p);
        }

        Int Gcd(Iter para)
        {
            long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

            if (para[1] is not Int a || para[2] is not Int b)
                throw new ArgumentException("invaild argument", nameof(para));
            return new Int(Gcd(a.value, b.value));
        }

        Int Permutation(Iter para)
        {
            if (para[1] is not Int n || para[2] is not Int k)
                throw new ArgumentException("invaild argument", nameof(para));

            if (n.value < k.value) return new(0);
            if (n.value == k.value || k.value == 0) return new(1);

            long w = n.value - k.value, sum = 1;

            for (long i = w + 1; i <= n.value; i++)
                sum *= i;

            return new(sum);
        }

        Int Combination(Iter para)
        {
            if (para[1] is not Int n || para[2] is not Int k)
                throw new ArgumentException("invaild argument", nameof(para));

            if (n.value < k.value) return new(0);
            if (n.value == k.value || k.value == 0) return new(1);

            long sum = Permutation(para).CInt().value;

            for (long i = 2; i <= k.value; i++)
                sum /= i;

            return new(sum);
        }

        Int Factorial(Iter para)
        {
            if (para[1] is not Int n)
                throw new ArgumentException("invaild argument", nameof(para));

            long sum = 1;

            for (long i = 2; i <= n.value; i++)
                sum *= i;

            return new(sum);
        }

        public Pack Static()
        {
            Math math = new(packageName);
            math.properties.Add("pi", new Float(3.14159265));
            math.properties.Add("e", new Float(2.718281828));
            math.properties.Add("gcd", new NativeFun("gcd", Gcd));
            math.properties.Add("permutation", new NativeFun("permutation", Permutation));
            math.properties.Add("combination", new NativeFun("combination", Combination));
            math.properties.Add("factorial", new NativeFun("factorial", Factorial));
            return math;
        }
    }
}
