using Un.Object;
using Un.Object.Iter;
using Un.Object.Primitive;
using Un.Object.Collections;

namespace Un;

public class Parser(Context context)
{
    public Obj ReturnValue = null!;

    private List<Node> nodes;
    private readonly Context context = context;
    private Scope Scope => context.Scope;

    public Obj Parse(List<Node> nodes)
    {
        this.nodes = nodes;

        if (nodes.Count == 0)
            return Obj.None;

        (_, var type, _) = nodes[0];

        return type switch
        {
            TokenType.Use => ParseUse(),
            TokenType.Using => ParseUsing(),
            TokenType.Class => ParseClass(),
            TokenType.At => ParseAnotation(),
            TokenType.Enum => ParseEunm(),
            TokenType.Return => ParseReturn(),
            TokenType.Skip => ParseSkip(),
            TokenType.Break => ParseBreak(),
            TokenType.For => ParseFor(),
            TokenType.While => ParseWhile(),
            TokenType.Try => ParseTry(),
            TokenType.Defer => ParseDefer(),
            TokenType.If or TokenType.ElIf or TokenType.Else => ParseIf(),
            _ => ParseExpreession(),
        };

    }

    #region Parser
    private Obj ParseUse()
    {
        var splited = nodes.Split(TokenType.As);
        var modules = splited[0][1..];

        bool isNickname = splited is { Count: 2 } &&
                          splited[1] is { Count: 1 } &&
                          splited[1][0] is { Type: TokenType.Identifier };

        bool isSpread = splited is { Count: 2 } &&
                        splited[1] is { Count: 1 } &&
                        splited[1][0] is { Type: TokenType.Asterisk };

        bool isPart = modules[^1].Type == TokenType.Set;

        string[] path = [..modules[..^(isPart ? 1 : 0)].Select(x => x.Value)];

        lock (Global.SyncRoot)
        {
            if (!Global.IsClass(path[^1]))
                Global.Import(path: path,
                              nickname: isSpread ? "*" : isNickname ? splited[1][0].Value : "",
                              parts: isPart ? [.. modules[^1].Children.Where(x => x.Type != TokenType.Comma).Select(x => x.Value)] : []);
            else
                Global.Include(name: path[^1]);
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

        var innerContext = new Context(new(members), new UnFile(name, body), []);

        Runner.Load(innerContext, context).Run();

        if (isInherit)
        {
            var inherits = nodes.Split(TokenType.Colon);

            if (inherits.Count != 2) throw new Error("invalid class syntax", context);

            types.Add(nodes[3].Value);

            foreach (var super in inherits[1].Split(TokenType.Comma).Skip(1))
                if (super is { Count: 1 } && super[0] is { Type: TokenType.Identifier })
                {
                    var superName = super[0].Value;
                    if (!Global.TryGetClass(superName, out Obj? superObj))
                        throw new Error($"superclass {superName} is not defined", context);

                    types.Add(superName);
                    foreach (var (key, value) in superObj?.Members ?? [])
                        members.TryAdd(key, value);
                }
                else throw new Error("invalid class syntax", context);
        }

        lock (Global.SyncRoot)
        {
            Global.SetClass(name, new Obj(name)
            {
                Annotations = context.Annotations,
                Super = isInherit ? Global.GetClass(nodes[3].Value) : Obj.None,
                Types = types,
                Members = members
                
            });
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
            Global.SetClass(name, new Enu(name, 0)
            {
                Members = constants
            });
        }

        return Obj.None;
    }
    private Obj ParseReturn() => ReturnValue = Executor.On(nodes[1..], context);
    private Obj ParseBreak() => ReturnValue = new Obj("break");
    private Obj ParseSkip() => ReturnValue = new Obj("skip");
    private Obj ParseExpreession()
    {
        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i].Type.IsAssignmentOperator())
                return ParseAssignment(i);
        return Executor.On(nodes, context);
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
                Scope.Get(values[0].Value).ToTuple().As<Tup>().Value);
        else
            throw new Error($"invalid assignment {nodes[assign - 1].Type}.", context);

        if (context.Annotations.Count != 0)
            foreach (var obj in objs)
                foreach (var (key, value) in context.Annotations)
                    if (!obj.Annotations.TryAdd(key, value))
                        obj.Annotations[key] = value;

        int count = 0;
        var type = nodes[assign].Type;

        for (int i = 0; i < names.Count; i++)
        {
            if (IsEnd(i + 1))
            {
                switch (names[i].Type)
                {
                    case TokenType.Indexer:
                        var index = Executor.On(names[i].Children, context).Unwrap(context);
                        variable.SetItem(index, AssignValue(type, variable.GetItem(index).Unwrap(context), objs[count]));
                        break;
                    case TokenType.Property:
                        variable.SetAttr(names[i].Value, AssignValue(type, variable.GetAttr(names[i].Value).Unwrap(context), objs[count]));
                        break;
                    case TokenType.Identifier:
                        var name = names[i].Value;
                        if (Scope.Get(name, out Obj? value))
                            Scope.Set(name, AssignValue(type, value, objs[count]));
                        else if (Global.TryGetGlobalVariable(name, out value))
                            Global.SetGlobalVariable(name, AssignValue(type, value, objs[count]));
                        else if (type == TokenType.Assign)
                            Scope.Set(name, objs[count]);
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
                    TokenType.Indexer => variable.GetItem(Executor.On(names[i].Children, context).Unwrap(context)).Unwrap(context),
                    TokenType.Property => variable.GetAttr(names[i].Value).Unwrap(context),
                    TokenType.Identifier => Scope.Get(names[i].Value, out var obj) ? obj : throw new Error($"variable {names[i].Value} not found.", context),
                    _ => throw new Error($"invalid assignment {names[i].Type}.", context)
                };
        }

        if (objs.Count == 1) return objs[0];
        return new Tup([.. objs], new string[nameCount]);

        bool IsEnd(int index) => index >= names.Count || names[index].Type == TokenType.Comma || names[index].Type == TokenType.Colon;

        bool IsDeconstruct() => valueCount == 1 && (IsDeconstructableToken() || Scope.Get(values[0].Value).As<Tup, List>(out _));

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
        var iter = Executor.On(nodes[(inIdx + 1)..], context).Iter().As<Iters>().Value;
        var innerScope = new Scope(new Map(), Scope);
        var innerContext = new Context(innerScope, new("for", context.File.GetBody()), []);

        innerContext.EnterBlock("loop");

        foreach (var current in iter)
        {
            innerContext.File.Move(0, 0);

            if (vars.Count != 1 && vars.Count != current switch
            {
                List or Tup => current.Len().As<Int>().Value,
                _ => 1
            })
                throw new Error($"invalid for syntax", context);

            var values = current switch
            {
                List l when vars.Count != 1 => l,
                Tup t when vars.Count != 1 => [.. t],
                _ => new([current])
            };

            for (int i = 0; i < vars.Count; i++)
                innerScope.Set(vars[i].Value, values[i]);

            ReturnValue = Runner.Load(innerContext, context).Run();

            if (ReturnValue?.Type == "break")
            {
                ReturnValue = null!;
                break;
            }
            else if (ReturnValue?.Type == "skip")
                ReturnValue = null!;
        }

        innerContext.ExitBlock();

        return ReturnValue ?? Obj.None;
    }
    private Obj ParseWhile()
    {
        var expr = nodes[1..];
        var body = context.File.GetBody();
        var innerScope = new Scope(new Map(), Scope);
        var innerContext = new Context(innerScope, new("while", body), []);
        innerContext.EnterBlock("loop");

        while (Executor.On(expr, innerContext).As<Bool>("while keyword only boolearn").Value)
        {
            ReturnValue = Runner.Load(innerContext, context).Run();

            if (ReturnValue?.Type == "break")
            {
                ReturnValue = null!;
                break;
            }
            else if (ReturnValue?.Type == "skip")
                ReturnValue = null!;
        }
        
        innerContext.ExitBlock();

        return ReturnValue ?? Obj.None;
    }
    private Obj ParseIf()
    {
        Bool condition = nodes[0].Type == TokenType.Else ? new(true) : Executor.On(nodes[1..], context).ToBool().As<Bool>();
        var innerScope = new Scope(new Map(), Scope);


        if (condition.Value)
        {
            var body = context.File.GetBody();
            var innerContext = new Context(innerScope, new("if", body), [.. context.BlockStack]);

            ReturnValue = Runner.Load(innerContext, context).Run();

            var file = context.File;

            while (file.TryPeekLine(out var code))
            {
                code = code.Trim();
                if (code.StartsWith("elif") || code.StartsWith("else"))
                    file.GetBody();
                else break;
            }
        }
        else
        {
            context.File.GetBody();
        }

        return ReturnValue ?? Obj.None;
    }
    private Obj ParseTry()
    {
        var body = context.File.GetBody();
        var innerScope = new Scope(new Map(), Scope);
        var innerContext = new Context(innerScope, new("try", body), []);

        innerContext.EnterBlock("try");

        try
        {
            ReturnValue = Runner.Load(innerContext, context).Run();
            innerContext.ExitBlock();
        }
        catch (Exception e)
        {
            innerContext.ExitBlock();

            if (!context.File.EOL && context.File.PeekLine().StartsWith("catch"))
            {
                var parts = context.File.PeekLine().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var name = parts.Length > 1 ? parts[1] : null;
                body = context.File.GetBody();
                innerContext = new Context(innerScope, new("catch", body), context.BlockStack);

                if (name is not null)
                    innerContext.Scope.Set(name, new Err(e.Message));

                Runner.Load(innerContext, context).Run();
            }
        }
        finally
        {
            if (!context.File.EOL && context.File.PeekLine().StartsWith("fin"))
            {
                body = context.File.GetBody();
                innerContext = new Context(innerScope, new("fin", body), context.BlockStack);

                Runner.Load(innerContext, context).Run();
            }
        }

        return ReturnValue ?? Obj.None;
    }
    private Obj ParseUsing()
    {
        var name = nodes[1].Value;
        nodes = nodes[1..];
        int assign = -1;

        for (int i = 0; assign == -1 && i < nodes.Count; i++)
            if (nodes[i].Type.IsAssignmentOperator())
                assign = i;

        if (assign == -1)
            throw new Error("'using' keyword must be followed by an assignment operator:", context);

        ParseAssignment(assign);

        Scope.Get(name, out var obj);
        obj.Entry();
        context.Usings.Push(obj);

        return Obj.None;
    }
    private Obj ParseDefer()
    {
        context.Defers.Push(nodes[1..]);
        return Obj.None;
    }
    private Obj ParseAnotation()
    {
        if (nodes.Count <= 1 || nodes.Count >= 4)
            throw new Error("invalid annotation syntax", context);

        var name = nodes[1].Value;

        context.Annotations[name] = nodes.Count == 2 ? new Tup([], []) : Convert.ToTuple(nodes[2], context);
        return Obj.None;
    }
    #endregion


}