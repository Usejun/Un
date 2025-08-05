using System.Collections;
using Un.Object.Function;
using Un.Object.Primitive;

namespace Un.Object.Collections;

public class Json(Obj obj) : Ref<Obj>(obj, "json")
{
    private static readonly Tokenizer tokenizer = new();
    private static readonly Lexer lexer = new();
    private static UnFile buf = new("json", []);

    public int Count => obj switch
    {
        Dict dict => dict.Value.Count,
        List list => list.Count,
        _ => 1
    };

    public override Obj Init(Tup args)
    {
        if (args.Count == 0)
            return new Json(None);

        if (args.Count != 1)
            return new Err($"invalid json: expected 1 argument, got {args.Count}");

        if (args[0] is Str s)
        {
            buf = new UnFile("json", [s.Value]);
            var tokens = tokenizer.Tokenize(buf);
            var lexed = lexer.Lex(tokens);

            if (lexed.Count != 1)
                return new Err($"invalid json: expected 1 expression, got {lexed.Count}");

            return new Json(Convert.Auto(lexed[0], new(Global.GetScope(), new UnFile("json", []), new())));
        }
        if (args[0] is Dict d)
            return new Json(d);
        if (args[0] is List l)
            return new Json(l);

        return new Err($"invalid json: expected string, dict, or list, got {args[0].Type}");
    }

    public override Int Len() => new(Count);

    public override Obj GetItem(Obj key)
    {
        if (Value is Dict dict)
            return dict.GetItem(key);
        if (Value is List list && key is Int index)
        {
            if (index.Value < 0 || index.Value >= list.Count)
                return new Err($"index {index.Value} out of range for list of length {list.Count}");
            return list.Value[index.Value];
        }
        return new Err($"cannot get item from {obj.Type} with key {key.Type}");
    }

    public override Str ToStr() => new(Stringfy(Value));

    public override Obj Clone() => new Json(Value.Clone());

    public static string Stringfy(Obj obj, int depth = 0)
    {
        if (obj is Json json)
            return Stringfy(json.Value, depth);
        if (obj is Dict dict)
        {
            string buf = "{\n";

            foreach (var (key, value) in dict.Value)
                buf += $"{new string(' ', 3 * depth + 1)}\"{key}\": {Stringfy(value, depth + 1)},\n";
            buf = buf.TrimEnd(',', '\n');
            buf += $"\n{new string(' ', 3 * depth)}}}";
            return buf;
        }
        if (obj is List list)
            return "[" + string.Join(", ", list.Value.Select(v => Stringfy(v, depth + 1))) + "]";
        if (obj is Str str)
            return $"\"{str.Value}\"";
        return obj.ToStr().As<Str>().Value;
    }
}