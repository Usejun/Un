namespace Un.Package;

public class Random : Obj, IPackage, IStatic
{
    public System.Random random = new();

    public string Name => "random";
    
    public Obj Seed(Field field)
    {
        if (field["seed"].As<Int>(out var seed))
            throw new ValueError("invalid argument");
        
        random = new((int)(seed.Value % int.MaxValue));
        return None;
    }

    public Int Int(Field field) => new(random.NextInt64());

    public Float Double(Field field) => new(random.NextDouble());

    public Obj Range(Field field)
    {
        if (field["min"].As<Int>(out var min) && field["max"].As<Int>(out var max))
        {
            if (min.CompareTo(max) > 0)
                (min, max) = (max, min);

            return new Int(random.NextInt64(min.Value, max.Value));
        }
        if (field["min"].As<Float>(out var fmin) && field["max"].As<Float>(out var fmax))
        {
            if (fmin.CompareTo(fmax) > 0)
                (fmin, fmax) = (fmax, fmin);

            return new Float(double.Min((fmax.Value - fmin.Value) * random.NextDouble() + fmin.Value, fmax.Value));
        }
        throw new ValueError("invalid argument");
    }

    public List Choice(Field field)
    {
        if (!field["count"].As<Int>(out var count))
            throw new ValueError("count is must be int");

        if (field["value"].As<List>(out _) || field["value"].As<Collections.Tuple>(out _))
        {
            List list = field["value"].CList();
            return new(random.GetItems(list.Value[..list.Count], (int)count.Value));
        }
        throw new ValueError("values cannot be chosen.");
    }

    public Obj Shuffle(Field field)
    {
        if (field["value"].As<List>(out _) || field["value"].As<Collections.Tuple>(out _))
        {
            var values = field["value"].CList().Value;
            var len = field["value"].Len().Value;
            var indices = Enumerable.Range(0, (int)len).ToArray();
 
            random.Shuffle(indices);

            for (long i = 0; i < len; i++)
                (values[i], values[indices[i]]) = (values[indices[i]], values[i]);
                
            return None;
        }
        throw new ValueError("invalid argument");
    }

    public Obj Static()
    {
        Random random = new();

        random.field.Set("seed", new NativeFun("seed", 1, Seed, [("seed", null!)]));
        random.field.Set("choice", new NativeFun("choice", 1, Choice, [("value", null!)]));
        random.field.Set("shuffle", new NativeFun("shuffle", 1, Shuffle, [("value", null!)]));
        random.field.Set("range", new NativeFun("range", 2, Range, [("min", null!), ("max", null!)]));
        random.field.Set("int", new NativeFun("int", 0, Int, []));
        random.field.Set("double", new NativeFun("double", 0, Double, []));

        return random;
    }
}
