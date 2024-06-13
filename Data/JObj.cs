using Newtonsoft.Json.Linq;

namespace Un.Data;

public class JObj : Ref<JToken>
{
    public JObj() : base("json", new JObject()) { }

    public JObj(JObject value) : base("json", value) { }

    public JObj(JToken value) : base("json", value) { }

    public override Obj Init(List args)
    {
        if (args.Count == 0)
            Value = new JObject();
        else if (args[0] is Str s)
            Value = JObject.Parse(s.Value);
        else if (args[0] is Dict d)
            Value = Parse(d);
        else if (args[0] is Object o)
            Value = Parse(o);
        else
            throw new ClassError("initialize error");

        return this;
    }

    public override Int CInt()
    {
        if (Convert($"{Value}", new()) is Int i)
            return i;
        return base.CInt();
    }

    public override Bool CBool()
    {
        if (Convert($"{Value}", new()) is Bool b)
            return b;
        return base.CBool();
    }

    public override Float CFloat()
    {
        if (Convert($"{Value}", new()) is Float f)
            return f;
        return base.CFloat();
    }

    public override List CList()
    {
        if (Convert($"{Value}", new()) is List i)
            return i;
        return base.CList();
    }

    public override Obj GetItem(List args)
    {
        if (args[0] is Int i)
            return Value[(int)i.Value] is null ? None : new JObj(Value[(int)i.Value]);
        if (args[0] is Str s)
            return Value[s.Value] is null ? None : new JObj(Value[s.Value]);
        throw new IndexError();
    }

    public override Obj SetItem(List args)
    {
        if (args[0] is Int i)
        {
            Value[(int)i.Value] = args[1] switch
            {
                Int i1 => i1.Value,
                Float f1 => f1.Value,
                Bool b1 => b1.Value,
                Date d1 => d1.Value,
                Str s1 => s1.Value,
                RStr rs1 => $"{rs1.Value}",
                JObj j1 => j1.Value,
                _ => null,
            };

            if (args[1] is List list)
            {
                var ja = new JArray();

                foreach (var obj in list)
                {
                    ja.Add(obj switch
                    {
                        Int i1 => i1.Value,
                        Float f1 => f1.Value,
                        Bool b1 => b1.Value,
                        Date d1 => d1.Value,
                        Str s1 => s1.Value,
                        RStr rs1 => $"{rs1.Value}",
                        JObj j1 => j1.Value,
                        _ => throw new ValueError("invalid argument"),
                    });
                }

                Value[(int)i.Value] = ja;
            }

            return this;
        }
        else if (args[0] is Str s)
        {
            Value[s.Value] = args[1] switch
            {
                Int i1 => i1.Value,
                Float f1 => f1.Value,
                Bool b1 => b1.Value,
                Date d1 => d1.Value,
                Str s1 => s1.Value,
                RStr rs1 => $"{rs1.Value}",
                JObj j1 => j1.Value,
                _ => null,
            };

            if (args[1] is List list)
            {
                var ja = new JArray();

                foreach (var obj in list)
                {
                    ja.Add(obj switch
                    {
                        Int i1 => i1.Value,
                        Float f1 => f1.Value,
                        Bool b1 => b1.Value,
                        Date d1 => d1.Value,
                        Str s1 => s1.Value,
                        RStr rs1 => $"{rs1.Value}",
                        JObj j1 => j1.Value,
                        _ => throw new ValueError("invalid argument"),
                    });
                }

                Value[s.Value] = ja;
            }

            return this;
        }
        else throw new IndexError();
    }

    public override Obj Clone() => new JObj() { Value = Value };

    private static JObject Parse(Dict dict)
    {
        JObject jObj = [];

        foreach ((var key, var value) in dict.Value)
        {
            if (key is not Str sk) throw new ValueError("invalid key");

            if (value is Int i) jObj.Add(sk.Value, i.Value);
            else if (value is Float f) jObj.Add(sk.Value, f.Value);
            else if (value is Str s) jObj.Add(sk.Value, s.Value);
            else if (value is RStr rs) jObj.Add(sk.Value, $"{rs.Value}");
            else if (value is Bool b) jObj.Add(sk.Value, b.Value);
            else if (value is Dict d) jObj.Add(sk.Value, Parse(d));
            else if (value is Object o) jObj.Add(sk.Value, Parse(o));
            else if (value is List it) jObj.Add(sk.Value, Array(it));
            else if (value is Fun fu) jObj.Add(sk.Value, null);
            else if (value == None) jObj.Add(sk.Value, null);
            else throw new ValueError("invalid type");
        }

        return jObj;
    }

    private static JObject Parse(Object obj)
    {
        JObject jObj = [];

        foreach (var key in obj.field.Keys)
        {
            if (obj.field[key] is Int i) jObj.Add(key, i.Value);
            else if (obj.field[key] is Float f) jObj.Add(key, f.Value);
            else if (obj.field[key] is Str s) jObj.Add(key, s.Value);
            else if (obj.field[key] is RStr rs) jObj.Add(key, $"{rs.Value}");
            else if (obj.field[key] is Bool b) jObj.Add(key, b.Value);
            else if (obj.field[key] is Dict d) jObj.Add(key, Parse(d));
            else if (obj.field[key] is Object o) jObj.Add(key, Parse(o));
            else if (obj.field[key] is List it) jObj.Add(key, Array(it));
            else if (obj.field[key] is Fun fu) jObj.Add(key, null);
            else if (obj.field[key] == None) jObj.Add(key, null);
            else throw new ValueError("invalid type");
        }

        return jObj;
    }

    private static JArray Array(List array)
    {
        JArray jArr = [];

        foreach (var value in array)
        {
            if (value is Int i) jArr.Add(i.Value);  
            else if (value is Float f) jArr.Add(f.Value);
            else if (value is Str s) jArr.Add(s.Value);
            else if (value is RStr rs) jArr.Add($"{rs.Value}");
            else if (value is Bool b) jArr.Add(b.Value);
            else if (value is Dict d) jArr.Add(Parse(d));
            else if (value is Object o) jArr.Add(Parse(o));
            else if (value is List it) jArr.Add(Array(it));
            else if (value == None) jArr.Add(null);
            else throw new ValueError("invalid type");
        }

        return jArr;
    }
}
