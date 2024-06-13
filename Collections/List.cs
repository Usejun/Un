using System.Collections;

namespace Un.Collections;

public class List : Ref<Obj[]>, IEnumerable<Obj>
{
    public struct Enumerator : IEnumerator<Obj>
    {
        private readonly List list;
        private int index;

        public readonly Obj Current => list[index];

        readonly object IEnumerator.Current => list[index];

        internal Enumerator(List list)
        {
            this.list = list;
            index = -1;
        }

        public bool MoveNext()
        {
            index++;
            return index < list.Count;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {

        }
    }

    public static List Empty => [];

    public bool IsFull => Count + 1 >= Value.Length;
    public int Count { get; protected set; }

    public List() : base("list", []) { }

    public List(string str, Field field) : base("list", [])
    {
        int index = 0, depth = 0;
        bool isStr = false;
        string buffer = "";

        while (str.Length - 2 > index)
        {
            index++;
            if (str[index] == '\"' || str[index] == '\'' || str[index] == '`') isStr = !isStr;
            if (str[index] == '[' || str[index] == '(') depth++;
            if (str[index] == ']' || str[index] == ')') depth--;

            if (depth == 0 && !isStr && str[index] == ',')
            {
                if (!string.IsNullOrWhiteSpace(buffer) && IsList(buffer)) 
                    Append(new List(buffer, field));
                else Append(Calculator.Calculate(Lexer.Lex(Tokenizer.Tokenize(buffer), field), field));
                buffer = "";
            }
            else
                buffer += str[index];
        }

        if (!string.IsNullOrWhiteSpace(buffer))
            Append(Calculator.Calculate(Lexer.Lex(Tokenizer.Tokenize(buffer), field), field));
    }

    public List(IEnumerable enumerable) : base("list", [])
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

    public List(Obj[] Value) : base("list", Value)
    {
        Count = Value.Length;
    }

    public Obj this[int index]
    {
        get
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            return Value[index];
        }
        set
        {
            if (OutOfRange(index)) throw new IndexError("out of range");
            this.Value[index] = value;
        }
    }

    public Obj this[Int i]
    {
        get
        {
            if (OutOfRange((int)i.Value)) throw new IndexError("out of range");
            return Value[(int)i.Value];
        }
        set
        {
            if (OutOfRange((int)i.Value)) throw new IndexError("out of range");
            this.Value[(int)i.Value] = value;
        }
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", -1, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                self.Append(args.Value[i]);                

            return None;
        }));
        field.Set("insert", new NativeFun("insert", 3, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");
            if (args[2] is not Int i)
                throw new ValueError("invalid argument");

            self.Insert(args[1], (int)i.Value);

            return None;
        }));
        field.Set("extend", new NativeFun("extend", -1, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            for (int i = 1; i < args.Count; i++)
                foreach (var item in args[i].CList())
                    self.Append(item);

            return None;
        }));
        field.Set("extend_insert", new NativeFun("extend_insert", 3, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");
            if (args[2] is not Int i)
                throw new ValueError("invalid argument");

            self.ExtendInsert(args[1], (int)i.Value);

            return None; 
        }));
        field.Set("remove", new NativeFun("remove", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            if (args[1] is Obj obj) return self.Remove(obj);
            throw new ValueError("invalid argument");
        }));
        field.Set("remove_at", new NativeFun("remove_at", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            if (args[1] is Int i) return self.RemoveAt(i);
            throw new ValueError("invalid argument");
        }));
        field.Set("index_of", new NativeFun("index_of", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            if (args[1] is Obj obj) return self.IndexOf(obj);
            throw new ValueError("invalid argument");
        }));
        field.Set("contains", new NativeFun("contains", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            if (args[1] is Obj obj) return self.Contains(obj);
            throw new ValueError("invalid argument");
        }));
        field.Set("clone", new NativeFun("clone", 1, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            return self.Clone();
        }));
        field.Set("sort", new NativeFun("sort", 1, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            self.Sort();
            return None;
        }));
        field.Set("reverse", new NativeFun("reverse", 1, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            self.Reverse();
            return None;
        }));
        field.Set("order", new NativeFun("order", 2, args =>
        {
            if (args[0] is not List self || args[1] is not Fun f)
                throw new ValueError("invalid argument");

            self.Order(f);
            return None;
        }));
        field.Set("binary_search", new NativeFun("binary_search", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            return self.BinarySearch(args[1]);
        }));
        field.Set("lower_bound", new NativeFun("lower_bound", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            return self.LowerBound(args[1]);
        }));
        field.Set("upper_bound", new NativeFun("upper_bound", 2, args =>
        {
            if (args[0] is not List self)
                throw new ValueError("invalid argument");

            return self.UpperBound(args[1]);
        }));
    }

    public override Obj Init(List args)
    {
        if (args.Count == 0)
        {
            Value = [];
        }
        else if (args[0] is Fun fun && args[1] is List list)
        {
            Value = [];

            for (int i = 0; i < list.Count; i++)
                Append(fun.Call([list[i]]));                                
        }
        else
        {
            var cList = args[0].CList();
            Value = cList.Value;
            Count = cList.Count;
        }

        return this;
    }

    public override Obj Add(Obj arg)
    {
        if (arg is List l) Extend(l);
        else Append(arg);

        return this;
    }

    public override Obj Sub(Obj arg)
    {
        if (arg is List args)
        {
            foreach (var Value in args)
                Remove(Value);
        }
        else Remove(arg);

        return this;
    }

    public override Obj Mul(Obj arg)
    {
        if (arg is Int pow)
        {
            int len = Count;
            Obj[] objs = new Obj[len * pow.Value];

            for (int i = 0; i < pow.Value; i++)
                for (int j = 0; j < len; j++)
                    objs[len * i + j] = Value[j].Clone();

            return new List(objs);
        }

        return base.Mul(arg);
    }

    public override Int Len() => new(Count);

    public override Bool LessThen(Obj arg)
    {
        if (arg is List i) return new(Count.CompareTo(i.Count) < 0);
        return base.LessThen(arg);
    }

    public override Bool Equals(Obj arg)
    {
        if (arg is List i) return new(Count.CompareTo(i.Count) == 0);
        return base.Equals(arg);
    }

    public override List CList() => this;

    public override Str CStr() => new($"[{string.Join(", ", Value.Take(Count).Select(o => o.CStr().Value))}]");

    public override Obj GetItem(List args)
    {
        if (args[0] is not Int i) throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Count + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) throw new IndexError("out of range");

        return Value[index];
    }

    public override Obj SetItem(List args)
    {
        if (args[0] is not Int i || OutOfRange((int)i.Value)) throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Count + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) throw new IndexError("out of range");

        Value[index] = args[1];

        return Value[index];
    }

    public override Obj Clone() => new List()
    {
        Value = Value,
        Count = Count
    };

    public override Obj Copy() => this;



    public List Extend(Obj obj)
    {
        if (IsFull)
            Resize();

        foreach (var item in obj.CList())
            Append(item);

        return this;
    }

    public List Extend(Obj[] objs)
    {
        foreach (var obj in objs)
            Extend(obj);
        return this;
    }

    public List ExtendInsert(Obj obj, int index)
    {
        if (IsFull)
            Resize();

        foreach (var item in obj.CList())
            Insert(item, index);

        return this;
    }

    public List ExtendInsert(Obj[] objs, int index)
    {
        foreach (var obj in objs)
            ExtendInsert(obj, index);
        return this;
    }

    public List Append(Obj obj)
    {
        if (IsFull)
            Resize();

        Value[Count++] = obj.Copy();

        return this;
    }

    public List Append(Obj[] objs)
    {
        foreach (var obj in objs)
            Append(obj);
        return this;
    }

    public List Insert(Obj obj, int index)
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
            Value[i + 1] = Value[i];
        Value[index] = obj.Copy();
        Count++;

        return this;
    }

    public List Insert(Obj[] objs, int index)
    {
        foreach (var obj in objs)
            Insert(obj, index);
        return this;
    }

    public Bool Remove(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (Value[i].Equals(obj).Value)
                return RemoveAt(new(i));
        return new(false);
    }

    public Bool RemoveAt(Int index)
    {
        for (int i = (int)index.Value; i < Count - 1; i++)
            Value[i] = Value[i + 1];
        Count--;
        return new(true);
    }

    public Int IndexOf(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (Value[i].Equals(obj).Value)
                return new(i);
        return new(-1);
    }

    public Bool Contains(Obj obj) => new(IndexOf(obj).Value != -1);

    public void Sort()
    {
        Array.Sort(Value, 0, Count);
    }

    public void Order(Fun fun)
    {
        Array.Sort(Value, 0, Count, Comparer<Obj>.Create((i, j) => fun.Call([i]).CompareTo(fun.Call([j]))));
    }

    public void Reverse()
    {
        Array.Reverse(Value, 0, Count);
    }

    public Int BinarySearch(Obj obj) => new(Array.BinarySearch(Value, 0, Count, obj));

    public Int LowerBound(Obj obj)
    {
        int l = 0, r = Count - 1, m = 0;
        while (r > l)
        {
            m = (l + r) / 2;
            if (Value[m].LessThen(obj).Value) l = m + 1;
            else r = m;
        }
        return new(r);
    }

    public Int UpperBound(Obj obj)
    {
        int l = 0, r = Count - 1, m = 0;
        while (r > l)
        {
            m = (l + r) / 2;
            if (Value[m].LessOrEquals(obj).Value) l = m + 1;
            else r = m;
        }
        return new(r);
    }


    void Resize()
    {
        var resized = new Obj[Count * 9 / 5 + 2];

        for (int i = 0; i < Count; i++)
            resized[i] = Value[i];

        Value = resized;     
    }

    bool OutOfRange(int index) => index < 0 || index >= Count;


    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public static bool IsList(string str) => str[0] == '[' && str[^1] == ']';
}
