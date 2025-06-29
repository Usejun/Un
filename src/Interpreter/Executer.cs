using Un.Object;
using Un.Object.Primitive;
using Un.Object.Collections;
using Un.Object.Function;

namespace Un;

public static class Executer
{
    public static Obj On(List<Node> nodes, Context context)
    {
        var scope = context.Scope;
        var postfix = GetPostfix(nodes);
        var values = new Stack<Obj>();

        foreach (var node in postfix)
        {
            var type = node.Type;

            if (type.IsLiteral())
            {
                var value = Convert.Auto(node, context);

                values.Push(value switch
                {
                    Err e => throw new Error(e.Message, context),
                    Tup t => t.Count == 1 && (t.Name.Length == 0 || string.IsNullOrEmpty(t.Name[0])) ? t[0] : t,
                    _ => value,
                });
            }
            else if (type == TokenType.Call)
            {
                var value = values.Pop();
                var args = Convert.ToTuple(node, context).Unwrap<Tup>(context);

                var result = value.IsType() ? Global.Class[value.Type[2..^2]].Init(args) : value.As<Fn>().Call(args);

                values.Push(result.Unwrap(context));
            }
            else if (type == TokenType.Identifier)
            {
                if (scope.TryGetValue(node.Value, out Obj? value))
                    values.Push(value);
                else if (Global.TryGetGlobalVariable(node.Value, out value))
                    values.Push(value);
                else if (Global.IsClass(node.Value))
                    values.Push(new Obj($"__{node.Value}__"));
                else
                    throw new Error($"undefined variable: {node.Value}.", context);
            }
            else if (type == TokenType.Property)
            {
                var obj = values.Pop();
                var prop = node.Value;
                var value = obj.GetAttr(prop);

                values.Push(value.Unwrap(context));
            }
            else if (type == TokenType.NullableProperty)
            {
                var obj = values.Pop();
                var prop = node.Value;
                var value = obj.Has(prop) ? obj.GetAttr(prop) : Obj.None;

                values.Push(value.Unwrap(context));
            }
            else if (type == TokenType.Func)
            {
                Fn fn = node.Value == "fn"
                ? new PFn()
                {
                    Name = node.Value,
                    Args = Fn.GetArgs(node.Children[0].Children, context),
                    Nodes = node.Children[2..],
                    Closure = new Map(scope),
                }
                : new LFn()
                {
                    Name = node.Value,
                    Args = Fn.GetArgs(node.Children[0].Children, context),
                    Body = context.File.GetBody(),
                    Closure = new Map(scope),
                };

                if (node.Value != "fn")
                    scope[node.Value] = fn;

                values.Push(fn);
            }
            else if (type == TokenType.Go)
            {
                var fn = values.Pop();

                if (fn is not Fn function)
                    throw new Error($"cannot call {fn.Type} as a function.", context);

                values.Push(new GFn()
                {
                    Name = function.Name,
                    Func = function,
                    Args = function.Args,
                    Closure = function.Closure,
                });
            }
            else if (type == TokenType.Wait)
            {
                var fn = values.Pop();
                if (fn is Fn function)
                    values.Push(fn);
                else if (fn is Future future)
                    values.Push(future.Wait());
                else
                    throw new Error($"cannot wait on {fn.Type}.", context);
            }
            else if (type == TokenType.Match)
            {
                var value = On([node.Children[0]], context);
                var exprs = node.Children[1].Children.Split(TokenType.Comma);
                var match = Obj.None;

                foreach (var expr in exprs)
                {
                    var splited = expr.Split(TokenType.Colon);
                    var conditions = splited[0].Split(TokenType.Or);
                    var result = On(splited[1], context);

                    foreach (var condition in conditions)
                    {
                        if (condition is { Count: 1 } && IsType(condition[0].Value) && value.Is(new Obj(condition[0].Value)).As<Bool>(out var b) && b.Value)
                            match = result;
                        else if (condition is { Count: 1 } && condition[0].Value == "_" || On(condition, context).Eq(value).As<Bool>().Value)
                            match = result;
                        else continue;
                        
                        break;
                    }

                    if (!match.IsNone())
                        break;
                }

                values.Push(match.Unwrap(context));

                bool IsType(string s) => Global.Class.ContainsKey(s); 
            }
            else if (type.IsBinaryOperator())
            {
                try
                {
                    var right = values.Pop();
                    var left = values.Pop();
                    var value = type switch
                    {
                        TokenType.Plus => left.Add(right),
                        TokenType.Minus => left.Sub(right),
                        TokenType.Asterisk => left.Mul(right),
                        TokenType.Slash => left.Div(right),
                        TokenType.DoubleSlash => left.IDiv(right),
                        TokenType.Percent => left.Mod(right),
                        TokenType.DoubleAsterisk => left.Pow(right),
                        TokenType.BAnd => left.BAnd(right),
                        TokenType.BOr => left.BOr(right),
                        TokenType.BXor => left.BXor(right),
                        TokenType.LeftShift => left.LShift(right),
                        TokenType.RightShift => left.RShift(right),
                        TokenType.Equal => left.Eq(right),
                        TokenType.Unequal => left.NEq(right),
                        TokenType.LessOrEqual => left.LtOrEq(right),
                        TokenType.GreaterOrEqual => left.GtOrEq(right),
                        TokenType.LessThan => left.Lt(right),
                        TokenType.GreaterThan => left.Gt(right),
                        TokenType.And => left.And(right),
                        TokenType.Or => left.Or(right),
                        TokenType.Xor => left.Xor(right),
                        TokenType.In => right.In(left),
                        TokenType.Is => left.Is(right.IsType() ? new Obj(right.Type[2..^2]) : right),
                        _ => throw new Error($"binary operator {type} is not implemented.", context)
                    };

                    values.Push(value.Unwrap(context));

                }
                catch (InvalidOperationException)
                {
                    throw new Error("invalid expression", context);
                }
            }
            else if (type.IsUnaryOperator())
            {
                var right = values.Pop();
                var index = type == TokenType.Indexer || type == TokenType.Slicer ?
                    Convert.ToIndex(node, context) : new Tup([], []);

                var value = type switch
                {
                    TokenType.Positive => right.Pos(),
                    TokenType.Negative => right.Neg(),
                    TokenType.Indexer => right.GetItem(index[0]),
                    TokenType.Slicer => right.Slicer(index[0].IsNone() ? new Int(0) : index[0].As<Int>(),
                                                     index[1].IsNone() ? new Int(-1) : index[1].As<Int>(),
                                                     index.Count == 3 ? index[2].As<Int>() : new Int(1)),
                    TokenType.BNot => right.BNot(),
                    TokenType.Not => right.Not(),
                    TokenType.Spread => right.Spread(),
                    _ => throw new Error($"unary operator {type} is not implemented.", context)
                };

                values.Push(value.Unwrap(context));
            }
        }

        return values.Count == 1 ? values.Pop() : values.Count == 0 ? Obj.None : throw new Error("invalid expression", context);
    }

    private static List<Node> GetPostfix(List<Node> nodes)
    {
        var operators = new Stack<Node>();
        var postfix = new List<Node>();

        foreach (var node in nodes)
        {
            if (node.Type.IsOperator())
            {
                while (operators.Count > 0 && operators.Peek().Type.IsOperator() && 
                       operators.Peek().Type.GetPrecedence() <= node.Type.GetPrecedence())                
                    postfix.Add(operators.Pop());
                operators.Push(node);
            }           
            else postfix.Add(node);
        }

        postfix.AddRange(operators);

        return postfix;
    }

}