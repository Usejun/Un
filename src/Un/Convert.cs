using Un.Object;
using Un.Object.Primitive;
using Un.Object.Collections;
using Un.Object.Function;

namespace Un;

public static class Convert
{
    public static List ToList(Node node, Scope scope) => new([.. node.Children.Split(TokenType.Comma).Select(x => Executer.On(x, scope))]);

    public static Tup ToTuple(Node node, Scope scope)
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
                value = Executer.On(pair[0], scope);
            }
            else if (pair.Count == 2)
            {
                name = pair[0].Split(TokenType.Colon)[0][0].Value;
                value = Executer.On(pair[1], scope);
            }
            
            names.Add(name);
            list.Append(value);
        }

        return new Tup([.. list], [.. names]);
    }

    public static Tup ToIndex(Node node, Scope scope) => new([.. node.Children.Split(TokenType.Colon).Select(x => Executer.On(x, scope))], []);

    public static Tup ToPair(Node node, Scope scope)
    {
        var temp = node.Children.Split(TokenType.Colon).ToList();
        return new([new Str(temp[0][0].Value), Executer.On(temp[1], scope)], []);
    }

    public static Dict ToDict(Node node, Scope scope) => new(node.Children.Split(TokenType.Comma).Select(x => ToPair(new("pair", TokenType.Pair) { Children = x }, scope)).Select(y => (y[0], y[1])).ToDictionary());

    public static Set ToSet(Node node, Scope scope) => new([.. ToTuple(node, scope).Value]);

    public static Str ToFStr(Node node, Scope scope)
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
                str += "";//Executer.On(lexer.Lex(tokenizer.Tokenize(buf)), scope).ToStr().Value;
            }
            else str += c;

            i++;
        }

        return new(str);
    }

    public static Lambda ToLambda(Node node, Scope scope)
    {
        var (_, _, children) = node;
        return new()
        {
            Name = "lambda",
            Args = Fn.GetArgs(children[0].Children, scope),
            Nodes = children[1].Children,
            Closure = scope,
        };
    }

    public static Obj Auto(Node node, Scope scope)
    {
        var (value, type, _) = node;

        if (type == TokenType.Integer)
            return new Int(System.Convert.ToInt64(value));
        else if (type == TokenType.Float)
            return new Float(System.Convert.ToDouble(value));
        else if (type == TokenType.Boolean && bool.TryParse(value, out var boolValue))
            return new Bool(boolValue);
        else if (type == TokenType.String)
            return new Str(value);
        else if (DateTime.TryParse(value, out var dateValue))
            return new Date(dateValue);
        else if (type == TokenType.List)
            return ToList(node, scope);
        else if (type == TokenType.Tuple)
            return ToTuple(node, scope);
        else if (type == TokenType.Dict)
            return ToDict(node, scope);
        else if (type == TokenType.Set)
            return ToSet(node, scope);
        else if (type == TokenType.FString)
            return ToFStr(node, scope);
        else
            throw new Error($"conversion for {node.Type} is not implemented.");

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
}