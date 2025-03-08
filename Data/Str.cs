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

    public override Obj Init(Collections.Tuple args, Field field)
    {
        field["count"] = Int.One;
        field.Merge(args, [("value", null!), ("count", Int.One)], 1);

        var value = field["value"].CStr();
        Value = "";

        if (!field["count"].As<Int>(out var count))
            throw new ClassError();

        for (long i = 0; i < count.Value; i++)
            Value += value.Value;

        return this;
    }

    public override void Init()
    {
        field.Set("split", new NativeFun("split", 1, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self) || !field["sep"].As<Str>(out var sep))
                throw new ArgumentError();

            return new List(self.Value.Split(sep.Value));
        }, [("sep", new Str(Literals.Space))]));
        field.Set("join", new NativeFun("join", 2, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self) || 
                !field["sep"].As<Str>(out var sep) ||
                !field["values"].As<List>(out _) ||
                !field["values"].As<Collections.Tuple>(out _))
                throw new ArgumentError();

            var values = field["values"].CList();

            return self.Add(new Str(string.Join(sep.Value, values.Select(i => i.CStr().Value))), field);
        }, [("values", null!), ("sep", new Str(Literals.Space))]));
        field.Set("index_of", new NativeFun("index_of", 1, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self) || 
                !field["text"].As<Str>(out var text) ||
                !field["start"].As<Int>(out var start))
                throw new ArgumentError();

            return new Int(self.Value.IndexOf(text.Value, (int)start.Value));
        }, [("text", null!), ("start", Int.Zero)]));
        field.Set("contains", new NativeFun("contains", 1, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self) ||
                !field["text"].As<Str>(out var text))
                throw new ArgumentError();

            return new Bool(self.Value.Contains(text.Value));
        }, []));
        field.Set("is_number", new NativeFun("is_number", 0, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self))
                throw new ArgumentError();

            bool isNumber = true;

            for (int i = 0; i < self.Value.Length; i++)
                isNumber &= char.IsDigit(self.Value[i]);

            return new Bool(isNumber);

        }, []));
        field.Set("is_alphabet", new NativeFun("is_number", 0, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self))
                throw new ArgumentError();

            bool isAlphabet = true;

            for (int i = 0; i < self.Value.Length; i++)
                isAlphabet &= char.IsLetter(self.Value[i]);

            return new Bool(isAlphabet);
        }, []));
        field.Set("is_white_space", new NativeFun("is_whitespace", 0, field =>
        {
            if (!field[Literals.Self].As<Str>(out var self))
                throw new ArgumentError();

            bool isWhitespace = true;

            for (int i = 0; isWhitespace && i < self.Value.Length; i++)
                isWhitespace &= char.IsWhiteSpace(self.Value[i]);

            return new Bool(isWhitespace);
        }, []));
    }

    public override Obj Add(Obj arg, Field field) => new Str(Value + arg.CStr().Value);

    public override Int CInt()
    {
        if (Value.Length > 2 && Value[..2] == Literals.Bin)
            return new(Convert.ToInt64(Value[2..], 2));
        else if (Value.Length > 2 && Value[..2] == Literals.Oct)
            return new(Convert.ToInt64(Value[2..], 8));
        else if (Value.Length > 2 && Value[..2] == Literals.Hex)
            return new(Convert.ToInt64(Value[2..], 16));
        else if (long.TryParse(Value, out var l))
            return new(l);
        return base.CInt();
    }

    public override Float CFloat()
    {
        if (Value == Literals.Inf) 
            return new(double.PositiveInfinity);
        if (decimal.TryParse(Value, out var d))
            return new((double)d);
        return base.CFloat();
    }

    public override Bool CBool()
    {
        if (Value == Literals.True) return new(true);
        if (Value == Literals.False) return new(false);
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

    public override Obj GetItem(Obj arg, Field field)
    {
        if (arg.As<Int>(out var index) || OutOfRange((int)index.Value)) 
            throw new IndexError("out of range");
            
        return new Str($"{Value[(int)index.Value]}");
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

    public static bool IsFStr(string str) => str[0] == Literals.Backtick && str[^1] == Literals.Backtick;

    public static Str FStr(string str, Field field)
    {
        StringBuffer result = new(), buffer = new();
        int index = 0;
        bool isFormat = false;

        while (index < str.Length)
        {
            if (str[index] == Literals.CRBrace)
            {
                isFormat = false;
                index++;
                result.Append(Calculator.All($"{buffer}", field).CStr().Value);
                buffer.Clear();
            }
            else if (isFormat && str[index] != Literals.Escape)
            {
                buffer.Append(str[index++]);
            }
            else if (str[index] == Literals.Escape)
            {
                index++;
                if (Token.Escape.TryGetValue(str[index], out var c))
                        throw new SyntaxError();

                if (isFormat) result.Append(c);
                else buffer.Append(c);

                index++;
            }
            else if (str[index] == Literals.CLBrace)
            {
                isFormat = true;
                index++;
            }
            else result.Append(str[index++]);
        }

        return new($"{result}");
    }
}
