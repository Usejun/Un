using System.Collections;
using System.Text;

namespace Un.Data;

public class Str : Val<string>, IEnumerable<char>
{
    public Str() : base("str", "") { }

    public Str(string? value) : base("str", value ?? "") { }

    public Str(char value) : this($"{value}") { }

    public Str this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            return new Str($"{Value[index]}");
        }
    }

    public Str this[Int i]
    {
        get
        {
            if (OutOfRange((int)i.Value)) throw new IndexError("out of range");
            return new Str($"{Value[(int)i.Value]}");
        }
    }

    public override Obj Init(Collections.Tuple arg)
    {
        if (arg.Count == 1)
            Value = arg[0].CStr().Value;
        else if (arg.Count == 2)
        {
            if (arg[1] is Int count)
            {
                RStr rStr = new();
                for (long i = 0; i < count.Value; i++)
                    rStr.Value.Append(arg[0].CStr().Value);
                Value = rStr.CStr().Value;
            }
        }
        
        return this;
    }

    public override void Init()
    {
        field.Set("split", new NativeFun("split", 2, args =>
        {
            if (args[0] is not Str self || args[1] is not Str sep)
                throw new ValueError("invalid argument");

            return new List(self.Value.Split(sep.Value));
        }));
        field.Set("join", new NativeFun("join", 3, args =>
        {
            if (args[0] is not Str self || args[1] is not Str sep)
                throw new ValueError("invalid argument");

            return self.Add(new Str(string.Join(sep.Value, args[2].CList().Select(i => i.CStr().Value))));
        }));
        field.Set("index_of", new NativeFun("index_of", -1, args =>
        {
            if (args[0] is not Str self || args[1] is not Str str)
                throw new ValueError("invalid argument");

            return new Int(self.Value.IndexOf(str.Value, args.Count == 2 && args[2] is Int index ? (int)index.Value : 0));
        }));
        field.Set("contains", new NativeFun("contains", -1, args =>
        {
            if (args[0] is not Str self || args[1] is not Str str)
                throw new ValueError("invalid argument");               

            return new Bool(self.Value.Contains(str.Value));
        }));
        field.Set("is_number", new NativeFun("is_number", 1, args =>
        {
            if (args[0] is not Str self)
                throw new ValueError("invalid argument");

            bool isNumber = true;

            for (int i = 0; i < self.Value.Length; i++)
                isNumber &= char.IsDigit(self.Value[i]);

            return new Bool(isNumber);

        }));
        field.Set("is_alphabet", new NativeFun("is_number", 1, args =>
        {
            if (args[0] is not Str self)
                throw new ValueError("invalid argument");

            bool isAlphabet = true;

            for (int i = 0; i < self.Value.Length; i++)
                isAlphabet &= char.IsLetter(self.Value[i]);

            return new Bool(isAlphabet);
        }));
        field.Set("is_white_space", new NativeFun("is_whitespace", 1, args =>
        {
            if (args[0] is not Str self)
                throw new ValueError("invalid argument");

            bool isWhitespace = true;

            for (int i = 0; i < self.Value.Length; i++)
                isWhitespace &= char.IsWhiteSpace(self.Value[i]);

            return new Bool(isWhitespace);
        }));
    }

    public override Obj Add(Obj obj) => new Str(Value + obj.CStr().Value);

    public override Int CInt()
    {
        if (Value.Length > 2 && Value[..2] == "0b")
            return new(System.Convert.ToInt64(Value[2..], 2));
        else if (Value.Length > 2 && Value[..2] == "0o")
            return new(System.Convert.ToInt64(Value[2..], 8));
        else if (Value.Length > 2 && Value[..2] == "0x")
            return new(System.Convert.ToInt64(Value[2..], 16));
        else if (long.TryParse(Value, out var l))
            return new(l);
        return base.CInt();
    }

    public override Float CFloat()
    {
        if (decimal.TryParse(Value, out var d))
            return new((double)d);
        return base.CFloat();
    }

    public override Bool CBool()
    {
        if (Value == "true") return new(true);
        if (Value == "false") return new(false);
        return new(string.IsNullOrWhiteSpace(Value));
    }

    public override List CList()
    {
        List list = [];

        foreach (char c in Value)
            list.Append(new Str(c));

        return list;
    }

    public override Int Len() => new(Value.Length);

    public override Obj GetItem(List args)
    {
        if (args[0] is not Int i || OutOfRange((int)i.Value)) throw new IndexError("out of range");
        return new Str($"{Value[(int)i.Value]}");
    }

    public override Obj Clone() => new Str(Value);

    public override Obj Copy() => new Str(Value);


    bool OutOfRange(int index) => 0 > index || index >= Value.Length;


    public IEnumerator<char> GetEnumerator()
    {
        foreach (var c in Value)
            yield return c;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var c in Value)
            yield return c;        
    }


    public static bool IsDoubleStr(string str) => str[0] == Literals.Double && str[^1] == Literals.Double;
    
    public static bool IsSingleStr(string str) => str[0] == Literals.Single && str[^1] == Literals.Single;
    
    public static bool IsFStr(string str) => str[0] == Literals.Grave && str[^1] == Literals.Grave;

    public static Str FStr(string str, Field field)
    {
        StringBuilder result = new(), buffer = new();
        int index = 0;
        bool isFormat = false;

        while (index < str.Length)
        {
            if (str[index] == '}')
            {
                isFormat = false;
                index++;
                result.Append(Calculator.All($"{buffer}", field).CStr().Value);
            }
            else if (isFormat)
            {
                if (str[index] == '\\')
                {
                    index++;
                    buffer.Append($"\\{str[index]}");
                    index++;
                }
                else buffer.Append(str[index++]);
            }
            else if(str[index] == '\\')
            {
                index++;
                result.Append($"\\{str[index]}");
                index++;
            }
            else if (str[index] == '{')
            {
                isFormat = true;
                index++;
            }
            else result.Append(str[index++]);
        }

        return new($"{result}");
    }
}
