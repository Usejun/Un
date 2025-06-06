using Un.Object;
using Un.Object.Primitive;
using Un.Object.Collections;
using Un.Object.Function;

namespace Un;

public static class Executer
{
    public static Obj On(List<Node> nodes, Scope scope)
    {        
        var postfix = GetPostfix(nodes);
        var values = new Stack<Obj>();

        foreach (var node in postfix)
        {
            var type = node.Type;

            if (type.IsLiteral())
            {
                var value = Convert.Auto(node, scope);

                values.Push(value switch
                {
                    Tup t => t.Count == 1 && (t.Name.Length == 0 || string.IsNullOrEmpty(t.Name[0])) ? t[0] : t,
                    _ => value,
                });
            }
            else if (type == TokenType.Call)
            {
                var value = values.Pop();
                var args = Convert.ToTuple(node, scope);

                if (value.Type.EndsWith(".__init__"))
                    values.Push(Global.Class[value.Type.Split('.')[0]].Init(args));
                else
                    values.Push(value.As<Fn>().Call(args));
            }
            else if (type == TokenType.Identifier)
            {
                if (scope.TryGetValue(node.Value, out Obj? value))
                    values.Push(value);
                else if (Global.TryGetGlobalVariable(node.Value, out value))
                    values.Push(value);
                else if (Global.IsClass(node.Value))
                    values.Push(new Obj($"{node.Value}.__init__"));
                else
                    throw new Error($"undefined variable: {node.Value}.");
            }
            else if (type == TokenType.Property)
            {
                var obj = values.Pop();
                var prop = node.Value;
                values.Push(obj.GetAttr(prop));
            }
            else if (type == TokenType.NullableProperty)
            {
                var obj = values.Pop();
                var prop = node.Value;
                values.Push(obj.Has(prop) ? obj.GetAttr(prop) : Obj.None);
            }
            else if (type == TokenType.Func)
            {
                Fn fn = node.Value == "fn"
                ? new PFn()
                {
                    Name = node.Value,
                    Args = Fn.GetArgs(node.Children[0].Children, scope),
                    Nodes = node.Children[2..],
                    Closure = scope,
                }
                : new LFn()
                {
                    Name = node.Value,
                    Args = Fn.GetArgs(node.Children[0].Children, scope),
                    Body = Global.File.GetBody(),
                    Closure = scope,
                };

                if (node.Value != "fn")
                    scope[node.Value] = fn;

                values.Push(fn);
            }
            else if (type == TokenType.Go)
            {
                var fn = values.Pop();
                if (fn is not Fn function)
                    throw new Error($"cannot call {fn.Type} as a function.");

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
                if (fn is not Fn function)
                    throw new Error($"cannot call {fn.Type} as a function.");

                values.Push(new WFn()
                {
                    Name = function.Name,
                    Func = function,
                    Args = function.Args,
                    Closure = function.Closure,
                });
            }
            else if (type.IsBinaryOperator())
            {
                var right = values.Pop();
                var left = values.Pop();
                values.Push(type switch
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
                    TokenType.Is => left.Is(right),
                    _ => throw new Error($"binary operator {type} is not implemented.")
                });
            }
            else if (type.IsUnaryOperator())
            {
                var right = values.Pop();
                var index = type == TokenType.Indexer || type == TokenType.Slicer ?
                    Convert.ToIndex(node, scope) : new Tup([], []);

                values.Push(type switch
                {
                    TokenType.Positive => right.Pos(),
                    TokenType.Negative => right.Neg(),
                    TokenType.Indexer => right.GetItem(index[0]),
                    TokenType.Slicer => right.Slicer(index[0].IsNone() ? new Int(0) : index[0].As<Int>(),
                                                     index[1].IsNone() ? new Int(-1) : index[1].As<Int>(),
                                                     index.Count == 3 ? index[2].As<Int>() : new Int(1)),
                    TokenType.BNot => right.BNot(),
                    TokenType.Not => right.Not(),
                    _ => throw new Error($"unary operator {type} is not implemented.")
                });
            }
        }

        return values.Count == 1 ? values.Pop() : values.Count == 0 ? Obj.None : throw new Error("invalid expression");
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