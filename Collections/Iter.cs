using System.Collections;

namespace Un.Collections;

public class Iter : Ref<Obj[]>, IEnumerable<Obj>
{
    public static Iter Empty => [];

    public bool IsFull => Count + 1 >= value.Length;
    public int Count { get; protected set; }

    public Iter() : base("iter", []) { }

    public Iter(string str, Field field) : base("iter", [])
    {
        int index = 0, depth = 0;
        bool isStr = false;
        string buffer = "";

        while (str.Length - 2 > index)
        {
            index++;
            if (str[index] == '\"') isStr = !isStr;
            if (str[index] == '[' || str[index] == '(') depth++;
            if (str[index] == ']' || str[index] == ')') depth--;

            if (depth == 0 && !isStr && str[index] == ',')
            {
                if (!string.IsNullOrWhiteSpace(buffer) && IsIter(buffer)) 
                    Append(new Iter(buffer, field));
                else Append(Calculator.Calculate(Lexer.Lex(Tokenizer.Tokenize(buffer), field), field));
                buffer = "";
            }
            else
                buffer += str[index];
        }

        if (!string.IsNullOrWhiteSpace(buffer))
            Append(Calculator.Calculate(Lexer.Lex(Tokenizer.Tokenize(buffer), field), field));
    }

    public Iter(IEnumerable enumerable) : base("iter", [])
    {
        foreach (var i in enumerable)
            Append(i switch
            {
                int or long => new Int((long)i),
                float or double or decimal => new Float((double)i),
                bool => new Bool((bool)i),
                string => new Str((string)i),
                Obj => (Obj)i,
                _ => throw new ValueError()
            });
    }

    public Iter(Obj[] value) : base("iter", value)
    {
        Count = value.Length;
    }

    public Obj this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            return value[index];
        }
        set
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            this.value[index] = value;
        }
    }

    public Obj this[Int i]
    {
        get
        {
            if (OutOfRange((int)i.value)) throw new IndexError("out of range");
            return value[(int)i.value];
        }
        set
        {
            if (OutOfRange((int)i.value)) throw new IndexError("out of range");
            this.value[(int)i.value] = value;
        }
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Append(args.value[i]);                

            return None;
        }));
        field.Set("insert", new NativeFun("insert", 3, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");
            if (args[2] is not Int i)
                throw new ValueError("invalid argument");

            self.Insert(args[1], (int)i.value);

            return None;
        }));
        field.Set("extend", new NativeFun("extend", -1, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[i].CIter())
                    self.Append(item);

            return None;
        }));
        field.Set("extend_insert", new NativeFun("extend_insert", 3, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");
            if (args[2] is not Int i)
                throw new ValueError("invalid argument");

            self.ExtendInsert(args[1], (int)i.value);

            return None; 
        }));
        field.Set("remove", new NativeFun("remove", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            if (args[1] is Obj obj) return self.Remove(obj);
            throw new ValueError("invalid argument");
        }));
        field.Set("remove_at", new NativeFun("remove_at", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            if (args[1] is Int i) return self.RemoveAt(i);
            throw new ValueError("invalid argument");
        }));
        field.Set("index_of", new NativeFun("index_of", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            if (args[1] is Obj obj) return self.IndexOf(obj);
            throw new ValueError("invalid argument");
        }));
        field.Set("contains", new NativeFun("contains", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            if (args[1] is Obj obj) return self.Contains(obj);
            throw new ValueError("invalid argument");
        }));
        field.Set("clone", new NativeFun("clone", 1, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            return self.Clone();
        }));
        field.Set("sort", new NativeFun("sort", 1, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            self.Sort();
            return None;
        }));
        field.Set("reverse", new NativeFun("reverse", 1, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            self.Reverse();
            return None;
        }));
        field.Set("order", new NativeFun("order", 2, args =>
        {
            if (args[0] is not Iter self || args[1] is not Fun f)
                throw new ValueError("invalid argument");

            self.Order(f);
            return None;
        }));
        field.Set("binary_search", new NativeFun("binary_search", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            return self.BinarySearch(args[1]);
        }));
        field.Set("lower_bound", new NativeFun("lower_bound", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            return self.LowerBound(args[1]);
        }));
        field.Set("upper_bound", new NativeFun("upper_bound", 2, args =>
        {
            if (args[0] is not Iter self)
                throw new ValueError("invalid argument");

            return self.UpperBound(args[1]);
        }));
    }

    public override Obj Init(Iter args)
    {
        if (args.Count == 0)
        {
            value = [];
        }
        else if (args[0] is Fun fun && args[1] is Iter iter)
        {
            value = [];

            for (int i = 0; i < iter.Count; i++)
                Append(fun.Call([iter[i]]));                                
        }
        else
        {
            var cIter = args[0].CIter();
            value = cIter.value;
            Count = cIter.Count;
        }

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is Iter l) Extend(l);
        else Append(arg);

        return this;
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is Iter args)
        {
            foreach (var value in args)
                Remove(value);
        }
        else Remove(arg);

        return this;
    }

    public override Obj Mul(Obj arg)
    {
        if (arg is Int pow)
        {
            int len = Count;
            Obj[] objs = new Obj[len * pow.value];

            for (int i = 0; i < pow.value; i++)
                for (int j = 0; j < len; j++)
                    objs[len * i + j] = value[j].Clone();

            return new Iter(objs);
        }

        return base.Mul(arg);
    }

    public override Int Len() => new(Count);

    public override Bool LessThen(Obj arg)
    {
        if (arg is Iter i) return new(Count.CompareTo(i.Count) < 0);
        return base.LessThen(arg);
    }

    public override Bool Equals(Obj arg)
    {
        if (arg is Iter i) return new(Count.CompareTo(i.Count) == 0);
        return base.Equals(arg);
    }

    public override Iter CIter() => this;

    public override Str CStr() => new($"[{string.Join(", ", value.Take(Count).Select(o => o.CStr().value))}]");

    public override Obj GetItem(Iter args)
    {
        if (args[0] is not Int i) throw new IndexError("out of range");

        int index = (int)i.value < 0 ? Count + (int)i.value : (int)i.value;

        if (OutOfRange(index)) throw new IndexError("out of range");

        return value[index];
    }

    public override Obj SetItem(Iter args)
    {
        if (args[0] is not Int i || OutOfRange((int)i.value)) throw new IndexError("out of range");

        int index = (int)i.value < 0 ? Count + (int)i.value : (int)i.value;

        if (OutOfRange(index)) throw new IndexError("out of range");

        value[index] = args[1];

        return value[index];
    }

    public override Obj Clone() => new Iter()
    {
        value = value,
        Count = Count
    };

    public override Obj Copy() => this;



    public Iter Extend(Obj obj)
    {
        if (IsFull)
            Resize();

        foreach (var item in obj.CIter())
            Append(item);

        return this;
    }

    public Iter Extend(Obj[] objs)
    {
        foreach (var obj in objs)
            Extend(obj);
        return this;
    }

    public Iter ExtendInsert(Obj obj, int index)
    {
        if (IsFull)
            Resize();

        foreach (var item in obj.CIter())
            Insert(item, index);

        return this;
    }

    public Iter ExtendInsert(Obj[] objs, int index)
    {
        foreach (var obj in objs)
            ExtendInsert(obj, index);
        return this;
    }

    public Iter Append(Obj obj)
    {
        if (IsFull)
            Resize();

        value[Count++] = obj.Copy();

        return this;
    }

    public Iter Append(Obj[] objs)
    {
        foreach (var obj in objs)
            Append(obj);
        return this;
    }

    public Iter Insert(Obj obj, int index)
    {
        if (Count == 0)
        {
            Append(obj);
            return this;
        }

        if (OutOfRange(index)) throw new IndexError("out of range");

        if (IsFull)
            Resize();

        for (int i = Count - 1; i >= index; i--)
            value[i + 1] = value[i];
        value[index] = obj.Copy();
        Count++;

        return this;
    }

    public Iter Insert(Obj[] objs, int index)
    {
        foreach (var obj in objs)
            Insert(obj, index);
        return this;
    }

    public Bool Remove(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (value[i].Equals(obj).value)
                return RemoveAt(new(i));
        return new(false);
    }

    public Bool RemoveAt(Int index)
    {
        for (int i = (int)index.value; i < Count - 1; i++)
            value[i] = value[i + 1];
        Count--;
        return new(true);
    }

    public Int IndexOf(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (value[i].Equals(obj).value)
                return new(i);
        return new(-1);
    }

    public Bool Contains(Obj obj) => new(IndexOf(obj).value != -1);

    public void Sort()
    {
        Array.Sort(value, 0, Count);
    }

    public void Order(Fun fun)
    {
        Array.Sort(value, 0, Count, Comparer<Obj>.Create((i, j) => fun.Call([i]).CompareTo(fun.Call([j]))));
    }

    public void Reverse()
    {
        Array.Reverse(value, 0, Count);
    }

    public Int BinarySearch(Obj obj) => new(Array.BinarySearch(value, 0, Count, obj));

    public Int LowerBound(Obj obj)
    {
        int l = 0, r = Count, m = 0;
        while (r > l)
        {
            m = (l + r) / 2;
            if (value[m].LessThen(obj).value) l = m + 1;
            else r = m;
        }
        return new(r);
    }

    public Int UpperBound(Obj obj)
    {
        int l = 0, r = Count, m = 0;
        while (r > l)
        {
            m = (l + r) / 2;
            if (value[m].LessOrEquals(obj).value) l = m + 1;
            else r = m;
        }
        return new(r);
    }


    void Resize()
    {
        Array.Resize(ref value, Count * 9 / 5 + 2);
    }

    bool OutOfRange(int index) => index < 0 || index >= Count;


    public IEnumerator<Obj> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return value[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return value[i];
    }


    public static bool IsIter(string str) => str[0] == '[' && str[^1] == ']';
}
