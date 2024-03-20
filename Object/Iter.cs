using System.Collections;

namespace Un.Object
{
    public class Iter : Obj, IEnumerable<Obj>
    {
        public Obj[] value;
        public bool IsFull => Count >= value.Length;
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

        public Obj this[Obj obj]
        {
            get
            {
                if (obj is not Int i || !i.value.TryInt(out var index) || OutOfRange(index)) throw new IndexOutOfRangeException();
                return value[index];
            }
            set
            {
                if (obj is not Int i || !i.value.TryInt(out var index) || OutOfRange(index)) throw new IndexOutOfRangeException();
                this.value[index] = value;
            }
        }

        public Iter()
        {
            value = [];
        }

        public Iter(string str)
        {
            value = [];
            int index = 1;

            Create(this);

            Obj Create(Iter iter)
            {
                if (str[index] == ',')
                    index++;

                if (str[index] == '[')
                {
                    index++;
                    iter.Append(Create([]));
                }

                if (str[index] == ']')
                {
                    index++;
                    return iter;
                }
                else
                {
                    string value = "";
                    while (index < str.Length && str[index] != ',')
                    {
                        if (str[index] == ']')
                            break;

                        value += str[index++];
                    }
                    iter.Append(Convert(value));

                    return Create(iter);
                }
            }
        }

        public Iter(IEnumerable<Obj> value)
        {
            this.value = [];
            foreach (var item in value)
                Append(item);
        }

        public Iter(Obj[] value)
        {
            this.value = [];
            foreach (var item in value)
                Append(item);
        }

        public void Append(Obj obj)
        {
            if (IsFull)
                Resize();

            if (obj != None)
                value[Count++] = obj;
        }

        public void Append(Obj[] objs)
        {
            foreach (var obj in objs)
                Append(obj);
        }

        public bool Remove(Obj obj)
        {
            for (int i = 0; i < Count; i++)
                if (value[i].CompareTo(obj) == 0)
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

        public override Obj Add(Obj obj)
        {
            Iter iter = new(value);

            if (obj is Iter l) iter.Append(l.value);
            else iter.Append(obj);

            return iter;
        }

        public override Obj Sub(Obj obj)
        {
            Iter iter = new(value);

            if (obj is Iter i)
                foreach (var item in i)
                    iter.Remove(item);
            else
                iter.Remove(obj);

            return iter;
        }

        public override Obj Mul(Obj obj)
        {
            if (obj is Int i)
            {
                Iter iter = [];

                for (int j = 0; j < i.value; j++)
                    iter.Append(value);

                return iter;
            }

            throw new Exception("Mul Error");
        }

        protected void Resize()
        {
            Array.Resize(ref value, Count * 9 / 5 + 2);
        }

        protected bool OutOfRange(int index) => index < 0 || index >= Count;

        public override string ToString() => $"[{string.Join(", ", value.Take(Count))}]";

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
    }

}
