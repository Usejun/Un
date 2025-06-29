using Un.Object;
using Un.Object.Function;
using Un.Object.Primitive;
using Un.Object.Collections;

namespace Un;

public class Parser(Context context)
{
    public Obj ReturnValue = null!;

    private List<Node> nodes;
    private int index = 0;

    private readonly Context context = context;
    private Scope Scope => context.Scope;

    private Obj Parse()
    {
        if (index >= nodes.Count)
            return Obj.None;

        (_, var type, _) = nodes[index];

        return type switch
        {
            TokenType.Use => ParseUse(),
            TokenType.Using => ParseUsing(),
            TokenType.Class => ParseClass(),
            TokenType.Enum => ParseEunm(),
            TokenType.Return => ParseReturn(),
            TokenType.For => ParseFor(),
            TokenType.If or TokenType.ElIf or TokenType.Else => ParseIf(),
            _ => ParseExpreession(),
        };

    }

    public Obj Parse(List<Node> nodes)
    {
        this.nodes = nodes;
        return Parse();
    }
    #region Parser
    private Obj ParseUse()
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

        lock (Global.SyncRoot)
        {
            if (!Global.Class.ContainsKey(name))
                Global.Import(name: name,
                              path: path,
                              nickname: isSpread ? "" : isNickname ? splited[1][0].Value : isPart ? modules[^2].Value : modules[^1].Value,
                              parts: isPart ? [.. modules[^1].Children.Select(x => x.Value)] : []);
            else
                Global.Include(name: name);
        }

        return Obj.None;
    }
    private Obj ParseClass()
    {
        var name = nodes[1].Value;
        var isInherit = nodes.Count >= 3;

        var body = context.File.GetBody();
        var members = new Map();
        var types = new HashSet<string>();

        var runner = Runner.Load("class", body, members);
        runner.Run();

        if (isInherit)
        {
            var inherits = nodes.Split(TokenType.Colon);

            if (inherits.Count != 2) throw new Error("invalid class syntax", context);

            foreach (var super in inherits[1].Split(TokenType.Comma).Skip(1))
                if (super is { Count: 1 } && super[0] is { Type: TokenType.Identifier })
                {
                    var superName = super[0].Value;
                    if (!Global.Class.TryGetValue(superName, out Obj? superObj))
                        throw new Error($"superclass {superName} is not defined", context);

                    types.Add(superName);
                    foreach (var (key, value) in superObj.Members)
                        members.TryAdd(key, value);
                }
                else throw new Error("invalid class syntax", context);
        }

        lock (Global.SyncRoot)
        {
            Global.Class[name] = new Obj(name)
            {
                Super = isInherit ? Global.Class[nodes[3].Value] : Obj.None,
                Types = types,
                Members = members
            };
        }

        return Obj.None;
    }
    private Obj ParseEunm()
    {
        var name = nodes[1].Value;
        var body = context.File.GetBody();
        var constants = new Map();
        var i = 0;

        foreach (var line in body)
        {
            var splited = line.Split(",");
            foreach (var member in splited)
                if (!string.IsNullOrWhiteSpace(member.Trim()))
                    constants.Add(member.Trim(), new Int(i++));
        }

        lock (Global.SyncRoot)
        {
            Global.Class[name] = new Obj(name)
            {
                Members = constants
            };

            Global.SetGlobalVariable(name, new Obj(name)
            {
                Members = constants
            });
        }

        return Obj.None;
    }
    private Obj ParseReturn() => ReturnValue = Executer.On(nodes[1..], context);
    private Obj ParseUsing()
    {
        if (!Scope.TryGetValue("__using__", out var usings))
        {
            usings = new List();
            Scope.Add("__using__", usings);
        }

        var name = nodes[1].Value;
        nodes = nodes[1..];
        int assign = -1;

        for (int i = 0; assign == -1 && i < nodes.Count; i++)
            if (nodes[i].Type.IsAssignmentOperator())
                assign = i;

        if (assign == -1)
            throw new Error("'using' keyword must be followed by an assignment operator:", context);

        ParseAssignment(assign);

        Scope[name].Entry();
        usings.As<List>().Append(Scope[name]);

        return Obj.None;
    }
    private Obj ParseExpreession()
    {
        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i].Type.IsAssignmentOperator())
                return ParseAssignment(i);
        return Executer.On(nodes, context);
    }
    private Obj ParseAssignment(int assign)
    {
        var names = nodes[..assign];
        var values = nodes[(assign + 1)..];

        int nameCount = names.Count(i => i.Type == TokenType.Comma) + 1;
        int valueCount = values.Count(i => i.Type == TokenType.Comma) + 1;

        var variable = Obj.None;
        var objs = new List<Obj>();
        var buf = new List<Node>();

        if (nameCount == valueCount)
            objs.AddRange(Convert.ToTuple(new Node("tuple", TokenType.Tuple)
            {
                Children = values
            }, context).Value);
        else if (IsDeconstruct())
            objs.AddRange(IsDeconstructableToken() ?
                Convert.ToTuple(values[0], context).Value
            :
                context.Scope[values[0].Value].ToTuple().As<Tup>().Value);
        else
            throw new Error($"invalid assignment {nodes[assign - 1].Type}.", context);

        int count = 0;
        var type = nodes[assign].Type;

        for (int i = 0; i < names.Count; i++)
        {
            if (IsEnd(i + 1))
            {
                switch (names[i].Type)
                {
                    case TokenType.Indexer:
                        var index = Executer.On(names[i].Children, context).Unwrap(context);            
                        variable.SetItem(index, AssignValue(type, variable.GetItem(index).Unwrap(context), objs[count]));
                        break;
                    case TokenType.Property:
                        variable.SetAttr(names[i].Value, AssignValue(type, variable.GetAttr(names[i].Value).Unwrap(context), objs[count]));
                        break;
                    case TokenType.Identifier:
                        var name = names[i].Value;
                        if (Scope.TryGetValue(name, out Obj? value))
                            Scope[name] = AssignValue(type, value, objs[count]);
                        else if (Global.TryGetGlobalVariable(name, out value))
                            Global.SetGlobalVariable(name, AssignValue(type, value, objs[count]));
                        else if (type == TokenType.Assign)
                            Scope.Add(name, objs[count]);
                        else
                            throw new Error($"invalid assignment {names[i].Type}.", context);
                        break;
                    default:
                        throw new Error($"invalid assignment {names[i].Type}.", context);
                }
                count++;
                i++;

                if (names.Count > i && names[i].Type == TokenType.Colon)
                    while (names.Count > i && names[i++].Type != TokenType.Comma) { }
            }
            else
                variable = names[i].Type switch
                {
                    TokenType.Indexer => variable.GetItem(Executer.On(names[i].Children, context).Unwrap(context)).Unwrap(context),
                    TokenType.Property => variable.GetAttr(names[i].Value).Unwrap(context),
                    TokenType.Identifier => Scope.TryGetValue(names[i].Value, out var obj) ? obj : throw new Error($"variable {names[i].Value} not found.", context),
                    _ => throw new Error($"invalid assignment {names[i].Type}.", context)
                };
        }

        if (objs.Count == 1) return objs[0];
        return new Tup([.. objs], new string[nameCount]);

        bool IsEnd(int index) => index >= names.Count || names[index].Type == TokenType.Comma || names[index].Type == TokenType.Colon;

        bool IsDeconstruct() => valueCount == 1 && (IsDeconstructableToken() || context.Scope[values[0].Value].As<Tup, List>(out _));

        bool IsDeconstructableToken() => (values[0].Type == TokenType.List || values[0].Type == TokenType.Tuple) && nameCount == values[0].Children.Count(i => i.Type == TokenType.Comma) + 1;

        Obj AssignValue(TokenType type, Obj a, Obj b) => type switch
        {
            TokenType.Assign => b,
            TokenType.PlusAssign => a.Add(b),
            TokenType.MinusAssign => a.Sub(b),
            TokenType.SlashAssign => a.Div(b),
            TokenType.DoubleSlashAssign => a.IDiv(b),
            TokenType.AsteriskAssign => a.Mul(b),
            TokenType.DoubleAsteriskAssign => a.Pow(b),
            TokenType.PercentAssign => a.Mod(b),
            TokenType.BAndAssign => a.BAnd(b),
            TokenType.BOrAssign => a.BOr(b),
            TokenType.BXorAssign => a.BXor(b),
            TokenType.LeftShiftAssign => a.LShift(b),
            TokenType.RightShiftAssign => a.RShift(b),
            _ => throw new Error("invalid assign operator", context),
        };
    }
    private Obj ParseFor()
    {
        var inIdx = nodes.FindIndex(x => x.Type == TokenType.In);
        var vars = nodes[..inIdx][1..].Split(TokenType.Comma).Select(x => x[0]).ToList();
        var iter = Executer.On(nodes[(inIdx + 1)..], context).Iter().As<Iters>().Value.GetEnumerator();
        var body = context.File.GetBody();
        var nscope = new Map(Scope);

        while (iter.MoveNext())
        {
            var current = iter.Current;

            if (vars.Count != current switch
            {
                List or Tup => current.Len().As<Int>().Value,
                _ => 1
            })
                throw new Error($"invalid for syntax", context);

            var values = current switch
            {
                List l => l,
                Tup t => [.. t],
                _ => new([current])
            };

            for (int i = 0; i < vars.Count; i++)
                nscope[vars[i].Value] = values[i];

            ReturnValue = Runner.Load(context.File.Name, body, nscope).Run();
        }

        return ReturnValue.Type != "error" ? ReturnValue : Obj.None;
    }
    private Obj ParseIf()
    {
        Bool condition = nodes[0].Type == TokenType.Else ? new(true) : Executer.On(nodes[1..], context).ToBool().As<Bool>();

        if (condition.Value)
        {
            var body = context.File.GetBody();
            ReturnValue = Runner.Load(context.File.Name, body, new Map(Scope)).Run();

            var file = context.File;

            while (file.TryPeekLine(out var code))
            {
                code = code.Trim();
                if (code.StartsWith("elif") || code.StartsWith("else"))
                {
                    file.GetBody();
                }
                else break;
            }
        }
        else
        {
            context.File.GetBody();
        }

        return ReturnValue is not null ? ReturnValue : Obj.None;
    }
    #endregion
}