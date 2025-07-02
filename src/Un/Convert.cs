using Un.Object;
using Un.Object.Primitive;
using Un.Object.Collections;

namespace Un;

public static class Convert
{
    public static List ToList(Node node, Context context) => new([.. node.Children.Split(TokenType.Comma).Select(x => Executer.On(x, context))]);

    public static Tup ToTuple(Node node, Context context)
    {
        var list = new List();
        var names = new List<string>();
        var splited = node.Children.Split(TokenType.Comma).Select(x => x.Split(TokenType.Assign)).ToList();

        foreach (var pair in splited)
        {
            var name = "";
            var value = Obj.None;

            if (pair.Count == 1)
            {
                value = Executer.On(pair[0], context);
            }
            else if (pair.Count == 2)
            {
                name = pair[0].Split(TokenType.Colon)[0][0].Value;
                value = Executer.On(pair[1], context);
            }

            names.Add(name);
            list.Append(value);
        }

        return new Tup([.. list], [.. names]);
    }

    public static Tup ToIndex(Node node, Context context) => new([.. node.Children.Split(TokenType.Colon).Select(x => Executer.On(x, context))], []);

    public static Tup ToPair(Node node, Context context)
    {
        var temp = node.Children.Split(TokenType.Colon).ToList();
        return new([new Str(temp[0][0].Value), Executer.On(temp[1], context)], []);
    }

    public static Dict ToDict(Node node, Context context) => new(node.Children.Split(TokenType.Comma).Select(x => ToPair(new("pair", TokenType.Pair) { Children = x }, context)).Select(y => (y[0], y[1])).ToDictionary());

    public static Set ToSet(Node node, Context context) => new([.. ToTuple(node, context).Value]);

    public static Str ToFStr(Node node, Context context)
    {
        string src = node.Value, buf = "", str = "";
        int len = src.Length, depth = 0, i = 0;
        var tokenizer = new Tokenizer();
        var lexer = new Lexer();

        while (i < len)
        {
            char c = src[i];

            if (c == '{')
            {
                depth++;
                while (i < len)
                {
                    c = src[++i];
                    depth = c switch
                    {
                        '{' => depth + 1,
                        '}' => depth - 1,
                        _ => depth
                    };
                    if (depth == 0)
                        break;
                    buf += c;
                }
                str += Executer.On(lexer.Lex(tokenizer.Tokenize(new("", [buf]))), context).ToStr().As<Str>().Value;
                buf = "";
            }
            else str += c;

            i++;
        }

        return new(str);
    }

    public static Obj Auto(Node node, Context context)
    {
        var (value, type, _) = node;

        if (type == TokenType.Integer)
            return new Int(System.Convert.ToInt64(value));
        else if (type == TokenType.None)
            return Obj.None;
        else if (type == TokenType.Float)
            return new Float(System.Convert.ToDouble(value));
        else if (type == TokenType.Boolean && bool.TryParse(value, out var boolValue))
            return new Bool(boolValue);
        else if (type == TokenType.String)
            return new Str(value);
        else if (DateTime.TryParse(value, out var dateValue))
            return new Date(dateValue);
        else if (type == TokenType.List)
            return ToList(node, context);
        else if (type == TokenType.Tuple)
            return ToTuple(node, context);
        else if (type == TokenType.Dict)
            return ToDict(node, context);
        else if (type == TokenType.Set)
            return ToSet(node, context);
        else if (type == TokenType.FString)
            return ToFStr(node, context);
        else
            return new Err($"conversion for {node.Type} is not implemented.");

    }

    public static List<List<Node>> Split(this List<Node> nodes, TokenType type)
    {
        List<List<Node>> splited = [];
        int start = 0, end = 0;

        while (end < nodes.Count)
        {
            if (nodes[end].Type == type)
            {
                splited.Add(nodes[start..end]);
                start = end + 1;
            }
            end++;
        }

        if (start != end)
            splited.Add(nodes[start..end]);

        return splited;
    }

    public static List<List<Node>> Split(this List<Node> nodes, params TokenType[] types)
    {
        List<List<Node>> splited = [];
        int start = 0, end = 0;

        while (end < nodes.Count)
        {
            if (types.Contains(nodes[end].Type))
            {
                splited.Add(nodes[start..end]);
                start = end + 1;
            }
            end++;
        }

        if (start != end)
            splited.Add(nodes[start..end]);

        return splited;
    }
}