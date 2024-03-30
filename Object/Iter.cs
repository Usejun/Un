using Un.Class;
using System.Collections;

namespace Un.Object
{
    public class Iter : Cla, IEnumerable<Obj>
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
            className = "iter";
            value = [];
        }

        public Iter(string str, Dictionary<string, Obj> variable) : base("iter")
        {
            className = "iter";
            value = [];
            int index = 0, depth = 0;
            string buffer = "";

            while (str.Length - 2 > index)
            {
                index++;
                if (str[index] == '[') depth++;
                if (str[index] == ']') depth--;

                if (depth == 0 && str[index] == ',')
                {
                    if (buffer[0] == '[') Append(new Iter(buffer, variable));
                    else Append(Tokenizer.Calculator.Calculate(Tokenizer.All(buffer, variable), variable));
                    buffer = "";
                }
                else
                    buffer += str[index];
            }

            if (!string.IsNullOrEmpty(buffer))
                Append(Tokenizer.Calculator.Calculate(Tokenizer.All(buffer, variable), variable));
        }

        public Iter(IEnumerable<Obj> value) : base("iter")
        {
            className = "iter";
            this.value = [];
            foreach (var item in value)
                Append(item);
        }

        public Iter(Obj[] value) : base("iter")
        {
            className = "iter";
            this.value = [];
            Append(value);
        }

        public void Append(Obj obj)
        {
            if (IsFull)
                Resize();

            if (obj != None)
                value[Count++] = obj.Clone();
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

        public override void Ass(string value, Dictionary<string, Obj> variable)
        {
            if (Convert(value, variable) is Iter iter)
                this.value = iter.value;
            else throw new ObjException("Ass Error");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> variable)
        {
            if (value is Iter i)
                this.value = i.value;
            else
                throw new ObjException("Ass Error");
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
                foreach (var item in i)
                    Remove(item);
            else
                Remove(obj);

            return this;
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

        public override Cla Clone() => new Iter(value);

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
