using Newtonsoft.Json.Linq;

namespace Un.Data;

public class JObj : Ref<JToken>
{
    public JObj() : base("json", new JObject()) { }

    public JObj(JToken value) : base("json", value) { }

    public override Obj Init(Iter args)
    {
        if (args.Count == 0)
            value = new JObject();
        else if (args.Count == 1 && args[0] is Str s)
            value = JObject.Parse(s.value);
        else
            throw new ClassError("initialize error");

        return this;
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

    public override Obj GetItem(Iter args)
    {
        if (args[0] is Int i)
            return new JObj(value[(int)i.value] ?? 0);
        if (args[0] is Str s)
            return new JObj(value[s.value] ?? "");
        throw new IndexError();
    }

    public override Obj SetItem(Iter args)
    {
        if (args[0] is Int i)
        {
            value[(int)i.value] = args[1] switch
            {
                Int i1 => i1.value,
                Float f1 => f1.value,
                Bool b1 => b1.value,
                Date d1 => d1.value,
                Str s1 => s1.value,
                JObj j1 => j1.value,
                _ => null,
            };

            if (args[1] is Iter iter)
            {
                var ja = new JArray();

                foreach (var obj in iter)
                {
                    ja.Add(obj switch
                    {
                        Int i1 => i1.value,
                        Float f1 => f1.value,
                        Bool b1 => b1.value,
                        Date d1 => d1.value,
                        Str s1 => s1.value,
                        JObj j1 => j1.value,
                        _ => throw new ValueError("invalid argument"),
                    });
                }

                value[(int)i.value] = ja;
            }

            return this;
        }
        else if (args[0] is Str s)
        {
            value[s.value] = args[1] switch
            {
                Int i1 => i1.value,
                Float f1 => f1.value,
                Bool b1 => b1.value,
                Date d1 => d1.value,
                Str s1 => s1.value,
                JObj j1 => j1.value,
                _ => null,
            };

            if (args[1] is Iter iter)
            {
                var ja = new JArray();

                foreach (var obj in iter)
                {
                    ja.Add(obj switch
                    {
                        Int i1 => i1.value,
                        Float f1 => f1.value,
                        Bool b1 => b1.value,
                        Date d1 => d1.value,
                        Str s1 => s1.value,
                        JObj j1 => j1.value,
                        _ => throw new ValueError("invalid argument"),
                    });
                }

                value[s.value] = ja;
            }

            return this;
        }
        else throw new IndexError();
    }

    public override Obj Clone() => new JObj() { value = value };
}
