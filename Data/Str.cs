using System.Collections;

namespace Un.Data;

public class Str : Val<string>, IEnumerable<char>
{
    public Str() : base("str", "") { }

    public Str(string value) : base("str", value) { }

    public Str(char value) : base("str", $"{value}") { }

    public Str this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            return new Str($"{value[index]}");
        }
    }

    public Str this[Int i]
    {
        get
        {
            if (OutOfRange((int)i.value)) throw new IndexError("out of range");
            return new Str($"{value[(int)i.value]}");
        }
    }

    public override Obj Init(Iter arg)
    {
        if (arg.Count == 1)
            value = arg[0].CStr().value;
        
        return this;
    }

    public override void Init()
    {
        properties.Add("split", new NativeFun("split", 2, args =>
        {
            if (args[0] is not Str self || args[1] is not Str sep)
                throw new ValueError("invalid argument");

            return new Iter(self.value.Split(sep.value));
        }));
        properties.Add("join", new NativeFun("join", 3, args =>
        {
            if (args[0] is not Str self || args[1] is not Str sep)
                throw new ValueError("invalid argument");

            return self.Add(new Str(string.Join(sep.value, args[2].CIter().Select(i => i.CStr().value))));
        }));
        properties.Add("index_of", new NativeFun("index_of", -1, args =>
        {
            if (args[0] is not Str self || args[1] is not Str str)
                throw new ValueError("invalid argument");

            return new Int(self.value.IndexOf(str.value, args.Count == 2 && args[2] is Int index ? (int)index.value : 0));
        }));
        properties.Add("contains", new NativeFun("contains", -1, args =>
        {
            if (args[0] is not Str self || args[1] is not Str str)
                throw new ValueError("invalid argument");               

            return new Bool(self.value.Contains(str.value));
        }));
        properties.Add("is_number", new NativeFun("is_number", 1, args =>
        {
            if (args[0] is not Str self)
                throw new ValueError("invalid argument");

            bool isNumber = true;

            for (int i = 0; i < self.value.Length; i++)
                isNumber &= char.IsDigit(self.value[i]);

            return new Bool(isNumber);

        }));
        properties.Add("is_alphabet", new NativeFun("is_number", 1, args =>
        {
            if (args[0] is not Str self)
                throw new ValueError("invalid argument");

            bool isAlphabet = true;

            for (int i = 0; i < self.value.Length; i++)
                isAlphabet &= char.IsLetter(self.value[i]);

            return new Bool(isAlphabet);
        }));
    }

    public override Obj Add(Obj obj) => new Str(value + obj.CStr().value);

    public override Int CInt()
    {
        if (long.TryParse(value, out var l))
            return new(l);
        return base.CInt();
    }

    public override Float CFloat()
    {
        if (decimal.TryParse(value, out var d))
            return new((double)d);
        return base.CFloat();
    }

    public override Bool CBool()
    {
        if (value == "true") return new(true);
        if (value == "false") return new(false);
        return new(string.IsNullOrWhiteSpace(value));
    }

    public override Iter CIter()
    {
        Iter iter = [];

        foreach (char c in value)
            iter.Append(new Str(c));

        return iter;
    }

    public override Int Len() => new(value.Length);

    public override Obj GetItem(Iter args)
    {
        if (args[0] is not Int i || OutOfRange((int)i.value)) throw new IndexError("out of range");
        return new Str($"{value[(int)i.value]}");
    }

    public override Obj Clone() => new Str(value) { };

    public override Obj Copy() => new Str(value);


    bool OutOfRange(int index) => 0 > index || index >= value.Length;


    public IEnumerator<char> GetEnumerator()
    {
        foreach (var c in value)
            yield return c;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var c in value)
            yield return c;        
    }


    public static bool IsDoubleStr(string str) => str[0] == '"' && str[^1] == '"';
    public static bool IsSingleStr(string str) => str[0] == '\'' && str[^1] == '\'';
}
