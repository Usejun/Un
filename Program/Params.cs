using System.Collections;

using Pair = (string name, Un.Data.Obj value);

public class Params : IEnumerable<Pair>
{
    public int Len => Names.Count;
    public List<string> Names { get; private set; } = [];
    public List<Obj> Values  { get; private set; } = [];

    public Pair this[int index]
    {
        get => (Names[index], Values[index]);
    }

    public void Add(string name, Obj obj)
    {
        Names.Add(name);
        Values.Add(obj);
    }

    public void Add((string name, Obj obj) v)
    {
        Names.Add(v.name);
        Values.Add(v.obj);
    }

    public int Positional(int start = 0)
    {
        int count = 0;
        for (int i = start; i < Len; i++)
            if (Values[i] is null ||
                Obj.IsNone(Values[i]))
                ++count;
            else break; 
        return count;
    }

    public int Named(int start = 0)
    {
        int count = 0;
        for (int i = start; i < Len; i++)
            if (Values[i] is not null &&
                !Obj.IsNone(Values[i]))
                ++count;
            else break; 
        return count;
    }

    public bool Match(Un.Collections.Tuple args)
    {
        int required = 0;

        while (required < args.Count && string.IsNullOrEmpty(args.Names[required]))
            required++;

        if (required < Positional())
            return false;

        return true;
    }

    public IEnumerator<Pair> GetEnumerator()
    {
        for (int i = 0; i < Len; i++)        
            yield return (Names[i], Values[i]);        
    } 

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < Len; i++)        
            yield return (Names[i], Values[i]);        
    }        

}