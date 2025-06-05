using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;

namespace Un;

public class Parser(Scope scope)
{
    private List<Node> nodes;
    private int index = 0;
    private Obj returned = Obj.Error;
    private Scope scope = scope;

    private Obj Parse()
    {
        if (index >= nodes.Count)
                return Obj.None;

        (_, var type, _) = nodes[index];

        return type switch
        {
            TokenType.Use => ParseUse(),
            TokenType.Using => ParseUsing(),
            //TokenType.Func => ParseFn(),
            TokenType.Class => ParseClass(),
            TokenType.Enum => ParseEunm(),
            TokenType.Return => ParseReturn(),
            TokenType.For => ParseFor(),
            TokenType.If => ParseIf(),
            _ => ParseExpreession(),
        };

    }

    public Obj Parse(List<Node> nodes)
    {
        this.nodes = nodes;
        return Parse();
    }
    #region Parser
    public Obj ParseSub()
    {
        Global.Sub(nodes[1..], scope);
        return Obj.None;
    }
    public Obj ParseUse()
    {
        var splited = nodes.Split(TokenType.As);
        var modules = splited[0][1..];
        var name = modules[0].Value;

        bool isNickname = splited is { Count: 2 } &&
                          splited[1] is { Count: 1 } &&
                          splited[1][0] is { Type: TokenType.Identifier };

        bool isSpread = splited is { Count: 2 } &&
                        splited[1] is { Count: 1 } &&
                        splited[1][0] is { Type: TokenType.Asterisk };

        bool isPart = modules[^1].Type == TokenType.Set;

        var path = $"{string.Join("/", modules[..^(isPart ? 1 : 0)].Select(x => x.Value))}.un";

        if (!Global.Class.ContainsKey(name))
            Global.Import(name: name,
                          path: path,
                          nickname: isSpread ? "" : isNickname ? splited[1][0].Value : isPart ? modules[^2].Value : modules[^1].Value,
                          parts: isPart ? [.. modules[^1].Children.Select(x => x.Value)] : []);
        else
            Global.Include(name: name);

        return Obj.None;
    }
    public Obj ParseFn()
    {
        if (nodes.Count != 3 && nodes.Count != 5)
            throw new Error("invalid function declaration");
        if (nodes.Count == 5 && nodes[3].Type != TokenType.Return)
            throw new Error("expected return type after '->'");

        var name = nodes[1].Value;
        var args = nodes[2];

        return scope[name] = new LFn()
        {
            Name = name,
            Args = Fn.GetArgs(args.Children, scope),
            ReturnType = nodes.Count == 3 ? "any" : nodes[4].Value,
            Body = Global.File.GetBody(),
        };
    }
    public Obj ParseClass()
    {
        if (returned == Obj.Error)
            throw new Error("No value returned");

        return returned;
    }
    public Obj ParseEunm()
    {
        if (returned == Obj.Error)
            throw new Error("No value returned");

        return returned;
    }
    public Obj ParseReturn() => returned = Executer.On(nodes[1..], scope);
    public Obj ParseUsing()
    {
        if (!scope.TryGetValue("__using__", out var usings))
        {
            usings = new List();
            scope.Add("__using__", usings);
        }

        var name = nodes[1].Value;
        nodes = nodes[1..];

        ParseExpreession();

        if (!scope.TryGetValue(name, out var obj))
            throw new Error("'using' keyword must be followed by an assignment operator:");

        usings.As<List>().Append(scope[name]);

        return Obj.None;
    }
    public Obj ParseExpreession()
    {
        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i].Type.IsAssignmentOperator())
                return ParseAssignment(i);
        return Executer.On(nodes, scope);
    }
    public Obj ParseAssignment(int assign)
    {
        var names = nodes[..assign];
        var values = nodes[(assign + 1)..];

        int nameCount = names.Count(i => i.Type == TokenType.Comma);
        int valueCount = values.Count(i => i.Type == TokenType.Comma);

        if (nameCount != valueCount)
            throw new Error($"invalid assignment {nodes[assign - 1].Type}.");

        var variable = Obj.None;
        var objs = new List<Obj>();
        var buf = new List<Node>();

        for (int i = 0; i < values.Count; i++)
        {
            if (values[i].Type == TokenType.Comma)
            {
                objs.Add(Executer.On(buf, scope));
                buf.Clear();
            }
            else
                buf.Add(values[i]);
        }

        objs.Add(Executer.On(buf, scope));
        int count = 0;

        for (int i = 0; i < names.Count; i++)
        {
            if (IsEnd(i + 1))
            {
                switch (names[i].Type)
                {
                    case TokenType.Indexer:
                        variable.SetItem(Convert.Auto(names[i].Children[0], scope), objs[count++]);
                        break;
                    case TokenType.Property:
                        variable.SetAttr(names[i].Value, objs[count++]);
                        break;
                    case TokenType.Identifier:
                        scope[names[i].Value] = objs[count++];
                        break;
                    default:
                        throw new Error($"invalid assignment {names[i].Type}.");
                }
                i++;

                if (names.Count > i && names[i].Type == TokenType.Colon)
                    while (names.Count > i && names[i++].Type != TokenType.Comma) { }
            }
            else
                variable = names[i].Type switch
                {
                    TokenType.Indexer => variable.GetItem(Convert.Auto(names[i].Children[0], scope)),
                    TokenType.Property => variable.GetAttr(names[i].Value),
                    TokenType.Identifier => scope.TryGetValue(names[i].Value, out var obj) || Global.TryGetGlobalVariable(names[i].Value, out obj) ? obj : throw new Error($"variable {names[i].Value} not found."),
                    _ => throw new Error($"invalid assignment {names[i].Type}.")
                };
        }

        if (objs.Count == 1) return objs[0];
        return new Tup()
        {
            Value = [.. objs]
        };

        bool IsEnd(int index) => index >= names.Count || names[index].Type == TokenType.Comma || names[index].Type == TokenType.Colon;
    }
    public Obj ParseFor()
    {
        var inIdx = nodes.FindIndex(x => x.Type == TokenType.In);
        var vars = nodes[..inIdx][1..].Split(TokenType.Comma).Select(x => x[0]).ToList();
        var iter = Executer.On(nodes[(inIdx + 1)..], scope).Iter().Value.GetEnumerator();
        var body = Global.File.GetBody();
        var nscope = new Scope(scope);

        while (iter.MoveNext())
        {
            var current = iter.Current;

            if (vars.Count != current switch
            {
                List or Tup => current.Len().As<Int>().Value,
                _ => 1
            })
                throw new Error($"invalid for syntax");

            var values = current switch
            {
                List l => l,
                Tup t => [.. t],
                _ => new([current])
            };

            for (int i = 0; i < vars.Count; i++)
                nscope[vars[i].Value] = values[i];

            Global.Swap(Global.File.Name, body, nscope);
        }

        return Obj.None;
    }
    public Obj ParseIf()
    {
        Bool condition = nodes[0].Type == TokenType.Else ? new(true) : Executer.On(nodes[1..], scope).ToBool();

        if (condition.Value)
        {
            var body = Global.File.GetBody();
            Global.Swap(Global.File.Name, body, new(scope));

            var file = Global.File;

            while (!file.EOF)
            {
                var code = file.GetLine();

                if (code.StartsWith("elif") || code.StartsWith("else"))
                    file.GetBody();
                else break;
            }
        }
        else
        {
            Global.File.GetBody();
        }

        return Obj.None;
    }
    #endregion
}