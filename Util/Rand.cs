namespace Un.Util;

public class Rand : Obj, IPackage, IStatic
{
    public Random rand = new();

    public string Name => "rand";
    
    public Obj Seed(Collections.Tuple args)
    {
        if (args[1] is Int seed)
        {
            rand = new((int)(seed.Value % int.MaxValue));
            return None;
        }

        throw new ValueError("invalid argument");
    }

    public Int Int(Collections.Tuple args) => new(rand.NextInt64());

    public Float Random(Collections.Tuple args) => new(rand.NextDouble());

    public Obj Range(Collections.Tuple args)
    {
        if (args[1] is Int iS && args[2] is Int iE && iS.CompareTo(iE) < 0)
            return new Int(rand.NextInt64(iS.Value, iE.Value));
        if (args[1] is Float fS && args[2] is Float fE && fS.CompareTo(fE) < 0)
            return new Float(double.Min(fE.Value * rand.NextDouble() + fS.Value, fE.Value));
        throw new ValueError("invalid argument");
    }

    public List Choice(Collections.Tuple args)
    {
        if (args[1] is List list)
        {
            if (args.Count > 2 && args[2] is Int count)
                return new(rand.GetItems(list.Value[..list.Count], (int)count.Value));
            else return new(rand.GetItems(list.Value[..list.Count], 1));
        }
        throw new ValueError("invalid argument");
    }

    public Obj Shuffle(Collections.Tuple args)
    {
        if (args[1] is List list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int index = rand.Next(0, list.Count - 1);
                (list[index], list[i]) = (list[i], list[index]);
            }
            return None;
        }
        throw new ValueError("invalid argument");
    }

    public Obj Static()
    {
        Rand rand = new();

        rand.field.Set("seed", new NativeFun("seed", 2, Seed));
        rand.field.Set("choice", new NativeFun("choice", -1, Choice));
        rand.field.Set("shuffle", new NativeFun("shuffle", 2, Shuffle));
        rand.field.Set("range", new NativeFun("range", 3, Range));
        rand.field.Set("int", new NativeFun("int", 1, Int));
        rand.field.Set("random", new NativeFun("random", 1, Random));

        return rand;
    }
}
