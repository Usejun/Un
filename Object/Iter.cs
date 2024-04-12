using System.Collections;

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
            Calculator calculator = new();
            int index = 0, depth = 0;
            string buffer = "";

            while (str.Length - 2 > index)
            {
                index++;
                if (str[index] == '[' || str[index] == '(') depth++;
                if (str[index] == ']' || str[index] == ')') depth--;

                if (depth == 0 && str[index] == ',')
                {
                    if (IsIter(buffer)) Append(new Iter(buffer, properties));
                    else Append(calculator.Calculate(Tokenizer.All(buffer, properties), properties));
                    buffer = "";
                }
                else
                    buffer += str[index];
            }

            if (!string.IsNullOrEmpty(buffer))
                Append(calculator.Calculate(Tokenizer.All(buffer, properties), properties));            
        }

        public Iter(Obj[] value) : base("iter")
        {        
            this.value = value;
            Count = value.Length;
        }

        public override Obj Init(Iter arg)
        {
            var iter = arg[0].CIter();
            value = iter.value;
            Count = iter.Count;

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

        public override void Ass(string value, Dictionary<string, Obj> properties)
        {
            if (Convert(value, properties) is Iter iter)
                this.value = iter.value;
            base.Ass(value, properties);
        }

        public override void Ass(Obj value, Dictionary<string, Obj> properties)
        {
            if (value is Iter i)
                this.value = i.value;
            base.Ass(value, properties);
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

        public override Int Hash() => new(value.GetHashCode());

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

        public override Iter CIter() => this;

        public override Str CStr() => new($"[{string.Join(", ", value.Take(Count).Select(i => i.CStr().value))}]");

        public override Obj GetByIndex(Obj parameter)
        {
            if (parameter is not Int i || !i.value.TryInt(out var iIndex) || OutOfRange(iIndex)) throw new IndexOutOfRangeException();
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

        public static bool IsIter(string str) => str[0] == '[' && str[^1] == ']';
    }

}
