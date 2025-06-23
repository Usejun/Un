using System.Collections;
using Un.Object.Function;
using Un.Object.Primitive;

namespace Un.Object.Collections;

public class List(Obj[] value) : Ref<Obj[]>(value, "list"), IEnumerable<Obj>
{
    public struct Enumerator(List list) : IEnumerator<Obj>
    {
        private readonly List list = list;
        private int index = -1;

        public readonly Obj Current => list[index];

        readonly object IEnumerator.Current => list[index];

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

    public List() : this([]) { }

    public Obj this[int index]
    {
        get => Value[index];
        set => Value[index] = value;
    }

    public int Count { get; protected set; } = value.Length;

    public bool IsFull => Count == Value.Length;

    public override Obj Init(Tup args) => args.ToList();

    public override Bool Eq(Obj other)
    {
        if (other is not List list)
            return new(false);

        for (int i = 0; i < Count; i++)
            if (Value[i].NEq(list[i]).As<Bool>().Value)
                return new(false);

        return new(true);
    }

    public override Obj GetItem(Obj key) => key switch
    {
        Int i => OutOfRange((int)i.Value) ? new Err("list index out of range") : this[(int)i.Value],
        _ => new Err("invalid index type")
    };

    public override void SetItem(Obj key, Obj value)
    {
        if (key is not Int i)
            throw new Panic("invalid index type");

        if (OutOfRange((int)i.Value))
            throw new Panic("list index out of range");

        this[(int)i.Value] = value;
    }

    public override Obj In(Obj obj) => obj switch
    {
        List list => new Bool(Overlap(list)),
        Tup tup => new Bool(Overlap(tup.ToList())),

        _ => new Err($"cannot check if '{obj.Type}' is in '{Type}'"),
    };

    public override Obj Len() => new Int(Count);

    public override Bool ToBool() => new(Count != 0);

    public override List ToList()
    {
        var newList = new List(new Obj[Count]);
        for (int i = 0; i < Count; i++)
            newList[i] = this[i].Copy();
        return newList;
    }

    public override Tup ToTuple() => new(Value[..Count], new string[Count]);

    public override Iters Iter() => new(this);

    public override Obj Copy() => this;

    public override Obj Clone()
    {
        var newList = new List(new Obj[Count]);
        for (int i = 0; i < Count; i++)
            newList[i] = this[i].Clone();
        return newList;
    }

    public override Str ToStr() => new($"[{string.Join(", ", Value[..Count].Select(v => v.ToStr().As<Str>().Value))}]");

    public override Spread Spread() => new(Value[..Count]);

    private bool OutOfRange(int index) => index < 0 || index >= Value.Length;

    private bool Overlap(List list)
    {
        foreach (var item in list)
        {
            if (!Value.Contains(item))
                return false;
        }
        return true;
    }

    public void Append(Obj value)
    {
        if (IsFull) Resize();
        this[Count] = value;
        Count++;
    }

    public void Extend(Obj value)
    {
        foreach (var v in value.Iter().As<Iters>().Value)
            Append(v);
    }

    public List ExtendInsert(Obj obj, int index)
    {
        if (IsFull)
            Resize();

        foreach (var item in obj.Iter().As<Iters>().Value)
            Insert(item, index);

        return this;
    }

    public List Insert(Obj obj, int index)
    {
        if (Count == 0)
        {
            Append(obj);
            return this;
        }

        if (IsFull)
            Resize();

        for (int i = Count - 1; i >= index; i--)
            this[i + 1] = this[i];
        this[index] = obj.Copy();
        Count++;

        return this;
    }

    public Bool Remove(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (this[i].Eq(obj).As<Bool>().Value)
                return RemoveAt(new(i));
        return new(false);
    }

    public Bool RemoveAt(Int index)
    {
        if (OutOfRange((int)index.Value))
            return new(false);

        for (int i = (int)index.Value; i < Count - 1; i++)
            this[i] = this[i + 1];
        Count--;
        return new(true);
    }

    public Int IndexOf(Obj obj)
    {
        for (int i = 0; i < Count; i++)
            if (this[i].Eq(obj).As<Bool>().Value)
                return new(i);
        return new(-1);
    }

    public Bool Contains(Obj obj) => new(IndexOf(obj).Value != -1);

    public void Order(Fn fn)
    {
        Array.Sort(Value, 0, Count, Comparer<Obj>.Create((i, j) => fn.Call(new([i], [])).CompareTo(fn.Call(new([j], [])))));
    }

    public void Sort()
    {
        Array.Sort(Value, 0, Count);
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
            if (this[m].Lt(obj).As<Bool>().Value) l = m + 1;
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
            if (this[m].LtOrEq(obj).As<Bool>().Value) l = m + 1;
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
            return new Err("list is empty");

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
        Obj value = this[(int)index.Value];
        RemoveAt(index);
        return value;
    }

    public override int GetHashCode()
    {
        int hash = Global.BASEHASH;

        foreach (var item in this)
        {
            hash <<= Global.HASHPRIME;
            hash ^= value.GetHashCode();
            hash >>= Global.HASHPRIME;
        }

        return hash;
    }

    public void Resize()
    {
        var newValue = new Obj[Value.Length * 2 + 1];
        for (var i = 0; i < Value.Length; i++)
            newValue[i] = Value[i];
        Value = newValue;
    }

    public IEnumerator<Obj> GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public override Attributes GetOriginal() => new()
    {
        { "add", new NFn
            {
                Name = "add",
                ReturnType = "none",
                Args = [new Arg("values") {
                    Type = "tuple[any]",    
                    IsPositional = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["values"].As<Tup>(out var values))
                        return new Err("invalid argument: values");

                    for (int i = 0; i < values.Count; i++)
                        self.Append(values[i]);

                    return None;
                }
            }
        },
        { "insert", new NFn
            {
                Name = "insert",
                ReturnType = "none",
                Args =
                [
                    new Arg("value") {
                        IsEssential = true
                    },
                    new Arg("index") {
                        Type = "int",
                        IsEssential = true
                    }
                ],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["index"].As<Int>(out var i))
                        return new Err("invalid argument: index");

                    self.Insert(args["value"], (int)i.Value);
                    return None;
                }
            }
        },
        { "extend", new NFn
            {
                Name = "extend",
                ReturnType = "none",
                Args = [new Arg("value") {
                    IsEssential = true
                }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    self.Extend(args["value"]);
                    return None;
                }
            }
        },
        { "extend_insert", new NFn
            {
                Name = "extend_insert",
                ReturnType = "none",
                Args =
                [
                    new Arg("value") {
                        IsEssential = true
                    },
                    new Arg("index") {
                        Type = "int",
                        IsEssential = true
                    }
                ],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["index"].As<Int>(out var i))
                        return new Err("invalid argument: index");

                    self.ExtendInsert(args["value"], (int)i.Value);
                    return None;
                }
            }
        },
        { "remove", new NFn
            {
                Name = "remove",
                ReturnType = "bool",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.Remove(args["value"]);
                }
            }
        },
        { "remove_at", new NFn
            {
                Name = "remove_at",
                ReturnType = "bool",
                Args = [new Arg("index") {
                    Type = "int",
                    IsEssential = true
                }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["index"].As<Int>(out var i))
                        return new Err("invalid argument: index");

                    return self.RemoveAt(i);
                }
            }
        },
        { "index_of", new NFn
            {
                Name = "index_of",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.IndexOf(args["value"]);
                }
            }
        },
        { "contains", new NFn
            {
                Name = "contains",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.Contains(args["value"]);
                }
            }
        },
        { "clone", new NFn
            {
                Name = "clone",
                Args = [],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.Clone();
                }
            }
        },
        { "reverse", new NFn
            {
                Name = "reverse",
                Args = [],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    self.Reverse();
                    return None;
                }
            }
        },
        { "sort", new NFn
            {
                Name = "sort",
                Args = [new Arg("key") { IsOptional = true, DefaultValue = NFn.My }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["key"].As<Fn>(out var f))
                        return new Err("invalid argument: key");

                    self.Order(f);
                    return None;
                }
            }
        },
        { "pop", new NFn
            {
                Name = "pop",
                Args = [new Arg("index") { IsOptional = true, DefaultValue = new Int(0) }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");
                    if (!args["index"].As<Int>(out var index))
                        return new Err("invalid argument: index");

                    return self.Pop(index);
                }
            }
        },
        { "hpush", new NFn
            {
                Name = "hpush",
                ReturnType = "none",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    self.HPush(args["value"]);
                    return None;
                }
            }
        },
        { "hpop", new NFn
            {
                Name = "hpop",
                Args = [],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.HPop();
                }
            }
        },
        { "binary_search", new NFn
            {
                Name = "binary_search",
                ReturnType = "int",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.BinarySearch(args["value"]);
                }
            }
        },
        { "lower_bound", new NFn
            {
                Name = "lower_bound",
                ReturnType = "int",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.LowerBound(args["value"]);
                }
            }
        },
        { "upper_bound", new NFn
            {
                Name = "upper_bound",
                ReturnType = "int",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<List>(out var self))
                        return new Err("invalid argument: self");

                    return self.UpperBound(args["value"]);
                }
            }
        },
    };

}