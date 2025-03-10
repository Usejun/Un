using System.Collections;
using System.Reflection.Metadata.Ecma335;

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
            if (OutOfRange(index)) 
                throw new IndexError("out of range");
            return Value[index];
        }
        set
        {
            if (OutOfRange(index)) 
                throw new IndexError("out of range");
            Value[index] = value;
        }
    }

    public Obj this[Int i]
    {
        get
        {
            if (OutOfRange((int)i.Value)) 
                throw new IndexError("out of range");
            return Value[(int)i.Value];
        }
        set
        {
            if (OutOfRange((int)i.Value)) 
                throw new IndexError("out of range");
            Value[(int)i.Value] = value;
        }
    }

    public override void Init()
    {
        field.Set("add", new NativeFun("add", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) ||
                !field["values"].As<List>(out var values))
                throw new ValueError("invalid argument");

            for (int i = 0; i < values.Count; i++)
                self.Append(values[i]);                

            return None;
        }, [("values", null!)], true));
        field.Set("insert", new NativeFun("insert", 2, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) ||
                !field["index"].As<Int>(out var i))
                throw new ValueError("invalid argument");

            self.Insert(field["value"], (int)i.Value);

            return None;
        }, [("value", null!), ("index", null!)]));
        field.Set("extend", new NativeFun("extend", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) ||
                !field["value"].As<List>(out var value))
                throw new ValueError("invalid argument");

            self.Extend(value);

            return None;
        }, [("value", null!)]));
        field.Set("extend_insert", new NativeFun("extend_insert", 2, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) ||
                !field["index"].As<Int>(out var i))
                throw new ValueError("invalid argument");

            self.ExtendInsert(field["value"], (int)i.Value);

            return None; 
        }, [("value", null!), ("index", null!)]));
        field.Set("remove", new NativeFun("remove", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.Remove(field["value"]);
        }, [("value", null!)]));
        field.Set("remove_at", new NativeFun("remove_at", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) ||
                !field["index"].As<Int>(out var i))
                throw new ValueError("invalid argument");

            return self.RemoveAt(i);
        }, [("index", null!)]));
        field.Set("index_of", new NativeFun("index_of", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.IndexOf(field["value"]);
        }, [("value", null!)]));
        field.Set("contains", new NativeFun("contains", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.Contains(field["value"]);
        }, [("value", null!)]));
        field.Set("clone", new NativeFun("clone", 0, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.Clone();
        }, [("value", null!)]));
        field.Set("reverse", new NativeFun("reverse", 0, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            self.Reverse();
            return None;
        }, [("value", null!)]));
        field.Set("sort", new NativeFun("sort", 0, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) || 
                !field["key"].As<Fun>(out var f))
                throw new ValueError("invalid argument");

            self.Order(f);
            return None;
        }, [("key", Lambda.Self)]));
        field.Set("pop", new NativeFun("pop", 0, field =>
        {
            if (!field[Literals.Self].As<List>(out var self) ||
                !field["index"].As<Int>(out var index))
                throw new ValueError("invalid argument");

            return self.Pop(index);
        }, [("index", Int.Zero)]));
        field.Set("hpush", new NativeFun("hpush", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            self.HPush(field["value"]);
            return None;
        }, [("value", null!)]));
        field.Set("hpop", new NativeFun("hpop", 0, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.HPop();
        }, []));
        field.Set("binary_search", new NativeFun("binary_search", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.BinarySearch(field["value"]);
        }, [("value", null!)]));
        field.Set("lower_bound", new NativeFun("lower_bound", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.LowerBound(field["value"]);
        }, [("value", null!)]));
        field.Set("upper_bound", new NativeFun("upper_bound", 1, field =>
        {
            if (!field[Literals.Self].As<List>(out var self))
                throw new ValueError("invalid argument");

            return self.UpperBound(field["value"]);
        }, [("value", null!)]));
    }

    public override Obj Init(Tuple args, Field field)
    {
        field.Merge(args, [("key", Lambda.Self), ("values", null!)], 0, true);
        Value = [];

        if (!field["values"].As<List>(out var values) ||
            !field["key"].As<Fun>(out var key))
            throw new ClassError();

        foreach (var value in values)
            Append(Fun.Invoke(key, new(value), field));
        return this;
    }

    public override Obj Add(Obj arg, Field field)
    {
        if (arg.As<List>(out var l)) Extend(l);
        else Append(arg);

        return this;
    }

    public override Obj Sub(Obj arg, Field field)
    {
        if (arg.As<List>(out var l))
        {
            foreach (var value in l)
                Remove(value);
        }
        else Remove(arg);

        return this;
    }

    public override Obj Mul(Obj arg, Field field)
    {
        if (arg.As<Int>(out var pow))
        {
            var len = Count;
            Obj[] objs = new Obj[len * pow.Value];

            for (int i = 0; i < pow.Value; i++)
                for (int j = 0; j < len; j++)
                    objs[len * i + j] = Value[j].Clone();

            return new List(objs);
        }

        return base.Mul(arg, field);
    }

    public override Int Len() => new(Count);

    public override Bool Eq(Obj arg, Field field)
    {
        if (arg.As<List>(out var list))
        {
            if (list.Count != Count) return Bool.False;

            for (int i = 0; i < Count; i++)
                if (list[i] != this[i])
                    return Bool.False;

            return Bool.True;
        }
        return base.Eq(arg, field);
    }

    public override Bool Lt(Obj arg, Field field)
    {
        if (arg.As<List>(out var list))
        {
            if (list.Count != Count) return new(list.Count > Count);

            for (int i = 0; i < Count; i++)
                if (list[i].Lt(this[i], field).Value)
                    return Bool.False;

            return Bool.True;
        }
        return base.Lt(arg, field);
    }



    public override List CList() => this;

    public override Str CStr() => new($"[{string.Join(", ", Value.Take(Count).Select(o => o.CStr().Value))}]");

    public override Obj GetItem(Obj arg, Field field)
    {
        if (arg.As<Int>(out var i)) 
            throw new IndexError("out of range");

        int index = (int)i.Value < 0 ? Count + (int)i.Value : (int)i.Value;

        if (OutOfRange(index)) 
            throw new IndexError("out of range");

        return Value[index];
    }

    public override Obj SetItem(Obj arg, Obj index, Field field)
    {
        if (index.As<Int>(out var i) || OutOfRange((int)i.Value)) 
            throw new IndexError("out of range");

        int idx = (int)i.Value < 0 ? Count + (int)i.Value : (int)i.Value;

        if (OutOfRange(idx)) throw new IndexError("out of range");

        Value[idx] = arg;

        return Value[idx];
    }

    public override Obj Clone() => new List()
    {
        Value = Value[..],
        Count = Count
    };

    public override Obj Copy() => this;


    public List Extend(Obj obj)
    {
        if (IsFull)
            Resize();
        if (obj.As<List>(out _))
            foreach (var item in obj.CList())
                Append(item);
        else
            Append(obj);

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
            if (Value[i].Eq(obj, Field.Null).Value)
                return RemoveAt(new(i));
        return new(false);
    }

    public Bool RemoveAt(Int index)
    {
        if (OutOfRange(index.Value))
            throw new IndexError("out of range");

        for (int i = (int)index.Value; i < Count - 1; i++)
            Value[i] = Value[i + 1];
        Count--;
        return new(true);
    }

    public Int IndexOf(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (Value[i].Eq(obj, Field.Null).Value)
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
        Array.Sort(Value, 0, Count, Comparer<Obj>.Create((i, j) => fun.Call(new(i), new()).CompareTo(fun.Call(new(j), new()))));
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
            if (Value[m].Lt(obj, Field.Null).Value) l = m + 1;
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
            if (Value[m].Loe(obj, Field.Null).Value) l = m + 1;
            else r = m;
        }
        return new(r);
    }

    public void HPush(Obj obj)
    {
        int child = Count;
        Append(obj);

        while (child != 0)
        {
            int parent = (child - 1) / 2;

            if (parent < child)
                parent = child;

            child = parent;
        }
    } 

    public Obj HPop()
    {
        if (Count == 0)
            throw new ValueError("list is empty");

        Obj value = this[0];
        this[0] = this[^1];
        Count--;

        int parent = 0;

        while (Count / 2 > parent)
        {
            int index = parent, left = 2 * parent + 1, right = 2 * parent + 2;

            if (right < Count && index < right)
                index = right;
            if (left < Count && index < left)
                index = left;

            (parent, index) = (index, parent);

            if (parent == index) break;

            parent = index;
        }

        return value;
    }

    public Obj Pop(Int index)
    {
        Obj value = this[index];
        RemoveAt(index);
        return value;
    }

    public override int GetHashCode()
    {
        int hash = 123456789;

        foreach (var item in this)
        {
            hash <<= 5;
            hash |= item.GetHashCode();
            hash += item.GetHashCode();
            hash = Math.Abs(hash);
        }

        return hash;
    }

    public Tuple AsTuple()
    {
        Obj[] objs = new Obj[Count];

        for (int i = 0; i < Count; i++)
            objs[i] = this[i];

        return new(objs);
    }

    void Resize()
    {
        var resized = new Obj[Count * 9 / 5 + 2];

        for (int i = 0; i < Count; i++)
            resized[i] = Value[i];

        Value = resized;     
    }

    bool OutOfRange(int index) => index < 0 || index >= Count;

    bool OutOfRange(long index) => index < 0 || index >= Count;


    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public static bool IsList(string str) => str[0] == Literals.CLBrack && str[^1] == Literals.CRBrack;
}
