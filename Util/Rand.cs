namespace Un.Util;

public class Rand : Obj, IPackage, IStatic
{
    public Random rand = new();

    public string Name => "rand";
    
    public Obj Seed(Iter args)
    {
        if (args[1] is Int seed)
        {
            rand = new((int)(seed.value % int.MaxValue));
            return None;
        }

        throw new ValueError("invalid argument");
    }

    public Int Int(Iter args) => new(rand.NextInt64());

    public Float Random(Iter args) => new(rand.NextDouble());

    public Obj Range(Iter args)
    {
        if (args[1] is Int iS && args[2] is Int iE)
            return new Int(rand.NextInt64(iS.value, iE.value));
        if (args[1] is Float fS && args[2] is Float fE)
            return new Float(double.Min(fE.value * rand.NextDouble() + fS.value, fE.value));
        throw new ValueError("invalid argument");
    }

    public Iter Choice(Iter args)
    {
        if (args[1] is Iter iter)
        {
            if (args.Count > 2 && args[2] is Int count)
                return new(rand.GetItems(iter.value[..iter.Count], (int)count.value));
            else return new(rand.GetItems(iter.value[..iter.Count], 1));
        }
        throw new ValueError("invalid argument");
    }

    public Obj Shuffle(Iter args)
    {
        if (args[1] is Iter iter)
        {
            for (int i = 0; i < iter.Count; i++)
            {
                int index = rand.Next(0, iter.Count - 1);
                (iter[index], iter[i]) = (iter[i], iter[index]);
            }
            return None;
        }
        throw new ValueError("invalid argument");
    }

    public Obj Static()
    {
        Rand rand = new();

        rand.properties.Add("seed", new NativeFun("seed", 2, Seed));
        rand.properties.Add("choice", new NativeFun("choice", -1, Choice));
        rand.properties.Add("shuffle", new NativeFun("shuffle", 2, Shuffle));
        rand.properties.Add("range", new NativeFun("range", 3, Range));
        rand.properties.Add("int", new NativeFun("int", 1, Int));
        rand.properties.Add("random", new NativeFun("random", 1, Random));

        return rand;
    }
}
