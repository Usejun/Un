using System.Collections;
using Un.Object.Value;
using Un.Object.Function;
using Un.Supporter;

namespace Un.Object.Reference
{
    public class Iter : Ref<Obj[]>, IEnumerable<Obj>
    {
        public static Iter Empty => [];

        public bool IsFull => Count + 1 >= value.Length;
        public int Count { get; protected set; }

        public Obj this[int index]
        {
            get
            {
                if (OutOfRange(index)) throw new IndexOutOfRangeException();
                return value[index];
            }
            set
            {
                if (OutOfRange(index)) throw new IndexOutOfRangeException();
                this.value[index] = value;
            }
        }

        public Obj this[Int i]
        {
            get
            {
                if (!i.value.TryInt(out var index) || OutOfRange(index)) throw new IndexOutOfRangeException();
                return value[index];
            }
            set
            {
                if (!i.value.TryInt(out var index) || OutOfRange(index)) throw new IndexOutOfRangeException();
                this.value[index] = value;
            }
        }

        public Iter() : base("iter", []) { }

        public Iter(string str, Dictionary<string, Obj> properties) : base("iter", [])
        {
            int index = 0, depth = 0;
            bool isString = false;
            string buffer = "";

            while (str.Length - 2 > index)
            {
                index++;
                if (str[index] == '\"') isString = !isString;
                if (str[index] == '[' || str[index] == '(') depth++;
                if (str[index] == ']' || str[index] == ')') depth--;

                if (depth == 0 && !isString && str[index] == ',')
                {
                    if (IsIter(buffer)) Append(new Iter(buffer, properties));
                    else Append(Calculator.Calculate(Lexer.Lex(Tokenizer.Tokenize(buffer), properties), properties));
                    buffer = "";
                }
                else
                    buffer += str[index];
            }

            if (!string.IsNullOrEmpty(buffer))
                Append(Calculator.Calculate(Lexer.Lex(Tokenizer.Tokenize(buffer), properties), properties));
        }

        public Iter(IEnumerable enumerable) : base("iter", [])
        {
            foreach (var i in enumerable)
                Append(i switch
                {
                    int or long => new Int((long)i),
                    float or double => new Float((double)i),
                    bool => new Bool((bool)i),
                    string => new Str((string)i),
                    _ => throw new InvalidConvertException()
                });          
        }

        public Iter(Obj[] value) : base("iter", value)
        {
            Count = value.Length;
        }

        public override void Init()
        {
            properties.Add("add", new NativeFun("add", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return self.Append(para[1]);
            }));
            properties.Add("insert", new NativeFun("insert", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));
                if (para[2] is not Int i || !i.value.TryInt(out var index))
                    throw new ArgumentException("invalid argument", nameof(para));

                return self.Insert(para[1], index);
            }));
            properties.Add("extend", new NativeFun("extend", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return self.Extend(para[1]);
            }));
            properties.Add("extend_insert", new NativeFun("extend_insert", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));
                if (para[2] is not Int i || !i.value.TryInt(out var index))
                    throw new ArgumentException("invalid argument", nameof(para));
                
                return self.ExtendInsert(para[1], index);
            }));
            properties.Add("remove", new NativeFun("remove", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Obj obj) return self.Remove(obj);
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("remove_at", new NativeFun("remove_at", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Int i) return self.RemoveAt(i);
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("index_of", new NativeFun("index_of", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Obj obj) return self.IndexOf(obj);
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("contains", new NativeFun("contains", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Obj obj) return self.Contains(obj);
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("clone", new NativeFun("clone", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                return self.Clone();
            }));
        }

        public override Obj Init(Iter arg)
        {
            if (arg.Count == 0)
            {
                value = [];
            }
            else
            {
                var iter = arg[0].CIter();
                value = iter.value;
                Count = iter.Count;
            }

            return this;
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Iter l) Append([..l.Take(l.Count)]);
            else Append(obj);

            return this;
        }

        public override Obj Sub(Obj obj)
        {
            if (obj is Iter i)
            {
                foreach (var item in i)
                    Remove(item);
            }
            else Remove(obj);

            return this;
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int pow)
            {
                int len = Count;
                Obj[] objs = new Obj[len * pow.value];

                for (int i = 0; i < pow.value; i++)
                    for (int j = 0; j < len; j++)
                        objs[len * i + j] = value[j];

                return new Iter(objs);
            }

            throw new Exception("Mul Error");
        }

        public override Int Len() => new(Count);

        public override Bool LessThen(Obj obj)
        {
            if (obj is Iter i) return new(Count.CompareTo(i.Count) < 0);
            return base.LessThen(obj);
        }

        public override Bool Equals(Obj obj)
        {
            if (obj is Iter i) return new(Count.CompareTo(i.Count) == 0);
            return base.Equals(obj);
        }

        public override Iter CIter()
        {
            return this;
        }

        public override Str CStr() => new($"[{string.Join(", ", value.Take(Count).Select(i => i.CStr().value))}]");

        public override Obj GetItem(Iter parameter)
        {
            if (parameter[0] is not Int i || !i.value.TryInt(out var iIndex) || OutOfRange(iIndex)) throw new IndexOutOfRangeException();
            return value[iIndex];
        }

        public override Obj SetItem(Iter parameter)
        {
            if (parameter[0] is not Int i || !i.value.TryInt(out var iIndex) || OutOfRange(iIndex)) throw new IndexOutOfRangeException();
            value[iIndex] = parameter[1];
            return value[iIndex];
        }

        public override Obj Clone() => new Iter() { value = value, Count = Count };

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

            if (OutOfRange(index)) throw new IndexOutOfRangeException();

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

        public Bool RemoveAt(Int idx)
        {
            if (!idx.value.TryInt(out var index))
                return new(false);

            for (int i = index; i < Count - 1; i++)
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

        public Bool Contains(Obj obj) => new (IndexOf(obj).value != -1);

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

}
