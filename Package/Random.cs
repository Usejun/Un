namespace Un.Package;

public class Random : Obj, IPackage, IStatic
{
    public System.Random random = new();

    public string Name => "random";
    
    public Obj Seed(Collections.Tuple args, Field field)
    {
        if (args[0] is not Int seed)
            throw new ValueError("invalid argument");
        
        random = new((int)(seed.Value % int.MaxValue));
        return None;
    }

    public Int Int(Collections.Tuple args, Field field) => new(random.NextInt64());

    public Float Double(Collections.Tuple args, Field field) => new(random.NextDouble());

    public Obj Range(Collections.Tuple args, Field field)
    {
        if (args[0] is Int a && args[1] is Int b)
        {
            if (a.CompareTo(b) > 0)
                (a, b) = (b, a);

            return new Int(random.NextInt64(a.Value, b.Value));
        }
        if (args[0] is Float c && args[1] is Float d)
        {
            if (c.CompareTo(d) > 0)
                (c, d) = (d, c);

            return new Float(double.Min((d.Value - c.Value) * random.NextDouble() + c.Value, d.Value));
        }
        throw new ValueError("invalid argument");
    }

    public List Choice(Collections.Tuple args, Field field)
    {
        if (args.Count == 1 && args[0] is List || args[0] is Collections.Tuple)
        {
            List list = args.CList();
            return new(random.GetItems(list.Value[..list.Count], 1));
        }
        if (args.Count == 2 && args[1] is Int count && (args[0] is List || args[0] is Collections.Tuple))
        {
            List list = args.CList();
            return new(random.GetItems(list.Value[..list.Count], (int)count.Value));
        }
        throw new ValueError("invalid argument");
    }

    public Obj Shuffle(Collections.Tuple args, Field field)
    {
        if (args.Count == 1 && args[0] is List || args[0] is Collections.Tuple)
        {
            var values = args[0] is not Collections.Tuple ? args[0].CList().Value : (args[0] as Collections.Tuple).Value;
            var len = args[0].Len().Value;
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

        random.field.Set("seed", new NativeFun("seed", 2, Seed));
        random.field.Set("choice", new NativeFun("choice", -1, Choice));
        random.field.Set("shuffle", new NativeFun("shuffle", 2, Shuffle));
        random.field.Set("range", new NativeFun("range", 3, Range));
        random.field.Set("int", new NativeFun("int", 1, Int));
        random.field.Set("double", new NativeFun("double", 1, Double));

        return random;
    }
}
