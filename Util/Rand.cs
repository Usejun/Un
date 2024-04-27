using Un.Collections;
using Un.Data;

namespace Un.Util
{
    public class Rand(string packageName) : Pack(packageName), IStatic
    {
        public Random rand = new();

        public Obj Seed(Iter para)
        {
            if (para[1] is Int seed)
            {
                rand = new((int)(seed.value % int.MaxValue));
                return None;
            }
            throw new ArgumentException("invaild argument", nameof(para));
        }

        public Int Int(Iter para) => new(rand.NextInt64());

        public Float Random(Iter para) => new(rand.NextDouble());

        public Obj Range(Iter para)
        {
            if (para[1] is Int iS && para[2] is Int iE)
                return new Int(rand.NextInt64(iS.value, iE.value));
            if (para[1] is Float fS && para[2] is Float fE)
                return new Float(System.Math.Min(rand.NextDouble() + fS.value, fE.value));
            throw new ArgumentException("invaild argument", nameof(para));
        }

        public Iter Choice(Iter para)
        {
            if (para[1] is Iter iter)
            {
                if (para.Count > 2 && para[2] is Int count && count.value.TryInt(out var c)) return new(rand.GetItems(iter.value, c));
                else return new(rand.GetItems(iter.value, 1));
            }
            throw new ArgumentException("invaild argument", nameof(para));
        }

        public Obj Shuffle(Iter para)
        {
            if (para[1] is Iter iter)
            {
                rand.Shuffle(iter.value);
                return None;
            }
            throw new ArgumentException("invaild argument", nameof(para));
        }

        public Pack Static()
        {
            Rand rand = new("rand");

            rand.properties.Add("seed", new NativeFun("seed", Seed));
            rand.properties.Add("choice", new NativeFun("choice", Choice));
            rand.properties.Add("shuffle", new NativeFun("shuffle", Shuffle));
            rand.properties.Add("range", new NativeFun("range", Range));
            rand.properties.Add("int", new NativeFun("int", Int));
            rand.properties.Add("random", new NativeFun("random", Random));

            return rand;
        }
    }
}
