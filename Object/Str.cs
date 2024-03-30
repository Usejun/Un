﻿namespace Un.Object
{
    public class Str(string value) : Obj
    {
        public string value = value;

        public Str this[int index]
        {
            get
            {
                if (OutOfRange(index)) throw new IndexOutOfRangeException();
                return new Str($"{value[index]}");
            }
        }
        
        public Str this[Int i]
        {
            get
            {
                if (!i.value.TryInt(out int index) || OutOfRange(index) ) throw new IndexOutOfRangeException();
                return new Str($"{value[index]}");
            }
        }

        public override void Ass(string value, Dictionary<string, Obj> variable)
        {
            if (value[0] == '\"' && value[^1] == '\"')
                this.value = value;
            else throw new ObjException("Ass Error");
        }

        public override void Ass(Obj value, Dictionary<string, Obj> variable)
        {
            if (value is Str s)
                this.value = s.value;
            else
                throw new ObjException("Ass Error");
        }

        public override Obj Add(Obj obj) => new Str(value + obj.ToString());

        public override int CompareTo(Obj? obj)
        {
            if (obj is Str s) return value.CompareTo(s.value);

            throw new ObjException("compare Error");
        }

        protected bool OutOfRange(int index) => 0 > index || index >= value.Length;

        public override string ToString() => value;

        public override Obj Clone() => new Str(value);
    }
}
