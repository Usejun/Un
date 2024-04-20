using System.Collections;
using Un.Function;
using Un.Supporter;

namespace Un.Object
{
    public class Iter : Obj, IEnumerable<Obj>
    {
        public static Iter Empty => [];

        public Obj[] value;
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

        public Iter() : base("iter")
        {
            value = [];
        }

        public Iter(string str, Dictionary<string, Obj> properties) : base("iter")
        {
            value = [];
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
                    else Append(Calculator.Calculate(Parser.All(buffer, properties), properties));
                    buffer = "";
                }
                else
                    buffer += str[index];
            }

            if (!string.IsNullOrEmpty(buffer))
                Append(Calculator.Calculate(Parser.All(buffer, properties), properties));            
        }

        public Iter(Obj[] value) : base("iter")
        {        
            this.value = value;
            Count = value.Length;
        }

        public override void Init()
        {
            properties.Add("add", new NativeFun("add", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Iter iter) return self.Append(iter);
                return self.Append(para[1]);
            }));
            properties.Add("insert", new NativeFun("insert", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));
                if (para[2] is not Int i || !i.value.TryInt(out var index))
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Iter iter) return self.Insert(iter, index);
                else return self.Insert(para[1], index);
            }));
            properties.Add("remove", new NativeFun("remove", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Obj obj) return new Bool(self.Remove(obj));
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("remove_at", new NativeFun("remove_at", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Int i && i.value.TryInt(out var index)) return new Bool(self.RemoveAt(index));
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("index_of", new NativeFun("index_of", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Obj obj) return new Int(self.IndexOf(obj));
                throw new ArgumentException("invalid argument", nameof(para));
            }));
            properties.Add("contains", new NativeFun("contains", para =>
            {
                if (para[0] is not Iter self)
                    throw new ArgumentException("invalid argument", nameof(para));

                if (para[1] is Obj obj) return new Bool(self.Contains(obj));
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
            if (arg[0] is Int i && i.value.TryInt(out var size))
            {
                value = new Obj[size];
            }
            else
            {
                var iter = arg[0].CIter();
                value = iter.value;
                Count = iter.Count;
            }

            return this;
        }

        public Iter Append(Obj obj, bool doClone = true)
        {
            if (IsFull)
                Resize();

            value[Count++] = doClone ? obj.Clone() : obj;

            return this;
        }

        public Iter Append(Iter iter, bool doClone = true)
        {
            foreach (var item in iter)
                Append(item, doClone);
            return this;
        }

        public Iter Append(Obj[] objs, bool doClone = true)
        {
            foreach (var obj in objs)
                Append(obj, doClone);
            return this;
        }

        public Iter Insert(Obj obj, int index, bool doClone = true)
        {
            if (Count == 0)
            {
                Append(obj, doClone);
                return this;
            }

            if (OutOfRange(index)) throw new IndexOutOfRangeException();

            if (IsFull)
                Resize();

            for (int i = Count - 1; i >= index; i--)
                value[i + 1] = value[i];
            value[index] = doClone ? obj.Clone() : obj;
            Count++;

            return this;
        }

        public Iter Insert(Iter iter, int index, bool doClone = true)
        {
            foreach (var item in iter)
                Insert(item, index, doClone);
            return this;
        }

        public Iter Insert(Obj[] objs, int index, bool doClone = true)
        {
            foreach (var obj in objs)
                Insert(obj, index, doClone);
            return this;
        }

        public bool Remove(Obj obj)
        {
            for (int i = 0; i < Count; i++)
                if (value[i].Equals(obj).value)
                    return RemoveAt(i);
            return false;
        }

        public bool RemoveAt(int index)
        {
            for (int i = index; i < Count - 1; i++)
                value[i] = value[i + 1];
            Count--;
            return true;
        }

        public int IndexOf(Obj obj)
        {
            for (int i = 0; i < Count; i++)
                if (value[i].Equals(obj).value)
                    return i;
            return -1;            
        }

        public bool Contains(Obj obj) => IndexOf(obj) != -1;

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (Convert(value, properties) is Iter iter)
                this.value = iter.value;
            else base.Ass(value, properties);
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Iter i)
                this.value = i.value;
            else base.Ass(value, properties);
        }

        public override Obj Add(Obj obj)
        {
            if (obj is Iter l) Append(l);
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

        public override Str Type() => new("iter");

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

        public override Iter CIter() => Clone() as Iter;

        public override Str CStr() => new($"[{string.Join(", ", value.Take(Count).Select(i => i.CStr().value))}]");

        public override Obj GetByIndex(Iter parameter)
        {
            if (parameter[0] is not Int i || !i.value.TryInt(out var iIndex) || OutOfRange(iIndex)) throw new IndexOutOfRangeException();
            return value[iIndex];
        }

        public override Obj SetByIndex(Iter parameter)
        {
            if (parameter[0] is not Int i || !i.value.TryInt(out var iIndex) || OutOfRange(iIndex)) throw new IndexOutOfRangeException();
            value[iIndex] = parameter[1];
            return value[iIndex];
        }

        protected void Resize()
        {
            Array.Resize(ref value, Count * 9 / 5 + 2);
        }

        protected bool OutOfRange(int index) => index < 0 || index >= Count;

        public override Obj Clone() => new Iter(value.Take(Count).ToArray());

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

        public override int GetHashCode() => value.GetHashCode();

        public static bool IsIter(string str) => str[0] == '[' && str[^1] == ']';
    }

}
