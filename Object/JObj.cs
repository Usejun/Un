using Newtonsoft.Json.Linq;
using Un.Supporter;

namespace Un.Object
{
    public class JObj : Obj
    {
        public JToken value;

        public JObj() : base("json")
        {
            value = new JObject();    
        }

        public JObj(JToken value) : base("json")
        {
            this.value = value; 
        }

        public override Obj Init(Iter args)
        {
            if (args[0] is Str s)
            {
                value = JObject.Parse(s.value);
                return this;
            }
            return base.Init(args);
        }

        public override Int CInt()
        {
            if (Convert($"{value}", []) is Int i)
                return i;
            return base.CInt();
        }

        public override Bool CBool()
        {
            if (Convert($"{value}", []) is Bool b)
                return b;
            return base.CBool();
        }

        public override Float CFloat()
        {
            if (Convert($"{value}", []) is Float f)
                return f;
            return base.CFloat();
        }

        public override Iter CIter()
        {
            if (Convert($"{value}", []) is Iter i)
                return i;
            return base.CIter();
        }

        public override Str CStr() => new(value.ToString());

        public override Obj GetByIndex(Iter para)
        {
            if (para[0] is Int i && i.value.TryInt(out var index))
                return new JObj(value[index]);
            if (para[0] is Str s)
                return new JObj(value[s.value]);
            throw new IndexerException();
        }

        public override Obj SetByIndex(Iter para)
        {
            if (para[0] is Int i)
            {
                value[i] = para[1] switch
                {
                    Int i1 => i1.value,
                    Float f1 => f1.value,
                    Bool b1 => b1.value,
                    Date d1 => d1.value,
                    Str s1 => s1.value,
                    JObj j1 => j1.value,                    
                    _ => null,
                };
            }
            if (para[0] is Str s)
            {
                value[s] = para[1] switch
                {
                    Int i1 => i1.value,
                    Float f1 => f1.value,
                    Bool b1 => b1.value,
                    Date d1 => d1.value,
                    Str s1 => s1.value,
                    JObj j1 => j1.value,
                    _ => null,
                };
            }
            throw new IndexerException();
        }

        public override int GetHashCode() => value.GetHashCode();

        public override Obj Clone() => new JObj(value);
    }
}
