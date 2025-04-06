namespace Un.Package;

public class Math : Obj, IPackage, IStatic
{
    public string Name => "math";

    public override Str Type() => new(Name);

    Obj Pow(Field field)
    {
        if (!field["y"].As<Int>(out var y)) 
            throw new ValueError("x^y, y must be an int");

        var x = field["x"];
        int count = y.Value > int.MaxValue ? throw new ArgumentError("y must be less than 2^32.") : (int)y.Value;

        if (count == 0) return new Int(0);
        if (count == 1) return x;
        if (count % 2 == 1) return x.Mul(Pow(new Collections.Tuple(x, new Int(count - 1)), Field.Null), Field.Null);
        var p = Pow(new Collections.Tuple(x, new Int(count / 2)), Field.Null);
        return p.Mul(p, Field.Null);
    }


    Int Gcd(Field field)
    {
        long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

        if (!field["a"].As<Int>(out var a) || !field["b"].As<Int>(out var b))
            throw new ValueError("argument only accept integers");
        return new Int(Gcd(a.Value, b.Value));
    }

    Int Permutation(Field field)
    {
        if (!field["n"].As<Int>(out var n) || !field["k"].As<Int>(out var k))
            throw new ValueError("argument only accept integers");

        if (n.Value < k.Value) return new(0);
        if (n.Value == k.Value || k.Value == 0) return new(1);

        long w = n.Value - k.Value, sum = 1;

        for (long i = w + 1; i <= n.Value; i++)
            sum *= i;

        return new(sum);
    }

    Int Combination(Field field)
    {
        if (!field["n"].As<Int>(out var n) || !field["k"].As<Int>(out var k))
            throw new ValueError("argument only accept integers");

        if (n.Value < k.Value) return new(0);
        if (n.Value == k.Value || k.Value == 0) return new(1);

        long sum = Permutation(field).CInt().Value;

        for (long i = 2; i <= k.Value; i++)
            sum /= i;
        
        return new(sum);
    }

    Int Factorial(Field field)
    {
        if (!field["n"].As<Int>(out var n))
            throw new ValueError("value must be an integer");

        if (n.Value > 20)
            throw new ValueError("overflow");

        long sum = 1;

        for (long i = 2; i <= n.Value; i++)
            sum *= i;

        return new(sum);
    }

    public Obj Static()
    {
        Obj math = new(Name);
        math.field.Set("pi", new Float(3.14159265));
        math.field.Set("e", new Float(2.718281828));
        math.field.Set("gcd", new NativeFun("gcd", 2, Gcd, [("a", null!), ("b", null!)]));
        math.field.Set("permutation", new NativeFun("permutation", 2, Permutation, [("n", null!), ("k", null!)]));
        math.field.Set("combination", new NativeFun("combination", 2, Combination, [("n", null!), ("k", null!)]));
        math.field.Set("factorial", new NativeFun("factorial", 1, Factorial, [("n", null!)]));
        return math;
    }
}
