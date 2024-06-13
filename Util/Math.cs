namespace Un.Util;

public class Math : Obj, IPackage, IStatic
{
    public string Name => "math";

    Obj Pow(List args)
    {
        if (args[1] is not Int i) throw new ValueError("invalid argument");

        int count = (int)i.Value;

        if (count == 0) return new Int(0);
        if (count == 1) return args[1];
        if (count % 2 == 1) return args[0].Mul(Pow(new List([args[1], new Int(count - 1)])));
        var p = Pow(new List([args[1], new Int(count / 2)]));
        return p.Mul(p);
    }

    Int Gcd(List args)
    {
        long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

        if (args[1] is not Int a || args[2] is not Int b)
            throw new ValueError("invalid argument");
        return new Int(Gcd(a.Value, b.Value));
    }

    Int Permutation(List args)
    {
        if (args[1] is not Int n || args[2] is not Int k)
            throw new ValueError("invalid argument");

        if (n.Value < k.Value) return new(0);
        if (n.Value == k.Value || k.Value == 0) return new(1);

        long w = n.Value - k.Value, sum = 1;

        for (long i = w + 1; i <= n.Value; i++)
            sum *= i;

        return new(sum);
    }

    Int Combination(List args)
    {
        if (args[1] is not Int n || args[2] is not Int k)
            throw new ValueError("invalid argument");

        if (n.Value < k.Value) return new(0);
        if (n.Value == k.Value || k.Value == 0) return new(1);

        long sum = Permutation(args).CInt().Value;

        for (long i = 2; i <= k.Value; i++)
            sum /= i;

        return new(sum);
    }

    Int Factorial(List args)
    {
        if (args[1] is not Int n)
            throw new ValueError("invalid argument");

        if (n.Value > 20)
            throw new ValueError("overflow");

        long sum = 1;

        for (long i = 2; i <= n.Value; i++)
            sum *= i;

        return new(sum);
    }

    public Obj Static()
    {
        Math math = new();
        math.field.Set("pi", new Float(3.14159265));
        math.field.Set("e", new Float(2.718281828));
        math.field.Set("gcd", new NativeFun("gcd", 3, Gcd));
        math.field.Set("permutation", new NativeFun("permutation", 3, Permutation));
        math.field.Set("combination", new NativeFun("combination", 3, Combination));
        math.field.Set("factorial", new NativeFun("factorial", 2, Factorial));
        return math;
    }
}
