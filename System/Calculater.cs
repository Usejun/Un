using Un.Collections;
using Un.Data;

namespace Un
{
    public class Calculator
    {
        public static List<Token> Postfix(List<Token> expression)
        {
            Stack<Token> postfixStack = [];
            List<Token> postfix = [];

            foreach (var token in expression)
            {
                if (Token.IsOperator(token))
                {
                    if (token.type == Token.Type.RParen)
                    {
                        while (postfixStack.TryPop(out var v) && v.type != Token.Type.LParen)
                            postfix.Add(v);
                    }
                    else
                    {
                        while (postfixStack.TryPeek(out var v) && Token.Priority[v.type] <= Token.Priority[token.type] && v.type != Token.Type.LParen)
                            postfix.Add(postfixStack.Pop());
                        postfixStack.Push(token);
                    }
                }
                else postfix.Add(token);
            }

            postfix.AddRange([.. postfixStack]);

            return postfix;
        }

        public static Obj Calculate(List<Token> expression, Dictionary<string, Obj> properties)
        {
            if (expression.Count == 0) return Obj.None;

            Stack<Obj> calculateStack = [];
            List<Token> postfix = Postfix(expression);

            for (int i = 0; i < postfix.Count; i++)
            {
                Token token = postfix[i];

                if (Process.TryGetStaticClass(token, out var staticCla) && token.type == Token.Type.Variable)
                {
                    calculateStack.Push(staticCla);
                }
                else if (Process.TryGetClass(token, out var cla) && token.type == Token.Type.Function)
                {
                    if (calculateStack.TryPop(out var obj) && obj is Iter args)
                        calculateStack.Push(cla.Clone().Init(args));
                    else
                        calculateStack.Push(new NativeFun(token.value, -1, cla.Init));
                }
                else if (properties.TryGetValue(token.value, out var local))
                {
                    if (local is Fun fun && calculateStack.TryPop(out var obj) && obj is Iter args)
                        local = fun.Clone().Call(args);

                    calculateStack.Push(local);
                }
                else if (Process.TryGetProperty(token, out var global))
                {
                    if (global is Fun fun && calculateStack.TryPop(out var obj) && obj is Iter args)
                        global = fun.Clone().Call(args);

                    calculateStack.Push(global);
                }
                else if (Token.IsOperator(token))
                {
                    Obj a = calculateStack.Pop(), b;
                    if (token.type == Token.Type.Indexer)
                    {
                        b = Obj.Convert(token.value, properties);

                        calculateStack.Push(a.GetItem(new Iter([b])));
                    }
                    else if (token.type == Token.Type.Slicer)
                    {
                        var index = token.value.Split(':');

                        calculateStack.Push(a.Slice([Obj.Convert(index[0], properties), Obj.Convert(index[1], properties)]));
                    }
                    else if (token.type == Token.Type.Property)
                    {
                        b = a.Get(token.value);

                        if (b is Fun fun)
                            b = fun.Call(calculateStack.Pop().CIter().ExtendInsert(a, 0));

                        calculateStack.Push(b);
                    }
                    else if (token.type == Token.Type.Bang || token.type == Token.Type.Not)
                    {
                        calculateStack.Push(new Bool(!a.CBool().value));
                    }
                    else if (token.type == Token.Type.BNot)
                    {
                        calculateStack.Push(a.BNot());
                    }
                    else if (token.type == Token.Type.In)
                    {
                        calculateStack.Push(a.CIter().Contains(calculateStack.Pop()));
                    }
                    else if (token.type == Token.Type.Plus && calculateStack.Count == 0)
                    {
                        calculateStack.Push(a.CInt().Mul(new Int(1)));
                    }
                    else if (token.type == Token.Type.Minus && calculateStack.Count == 0)
                    {
                        calculateStack.Push(a.CInt().Mul(new Int(-1)));
                    }
                    else
                    {
                        b = calculateStack.Pop();

                        Obj c = token.type switch
                        {
                            Token.Type.Plus => b.Add(a),
                            Token.Type.Minus => b.Sub(a),
                            Token.Type.Asterisk => b.Mul(a),
                            Token.Type.DoubleAsterisk => b.Pow(a),
                            Token.Type.Slash => b.Div(a),
                            Token.Type.DoubleSlash => b.IDiv(a),
                            Token.Type.Percent => b.Mod(a),
                            Token.Type.And => b.And(a),
                            Token.Type.Or => b.Or(a),
                            Token.Type.Xor => b.Xor(a),
                            Token.Type.BAnd => b.BAnd(a),
                            Token.Type.BOr => b.BOr(a),
                            Token.Type.BXor => b.BXor(a),
                            Token.Type.Equal => b.Equals(a),
                            Token.Type.Unequal => b.Unequals(a),
                            Token.Type.GreaterOrEqual => b.GreaterOrEquals(a),
                            Token.Type.LessOrEqual => b.LessOrEquals(a),
                            Token.Type.GreaterThen => b.GreaterThen(a),
                            Token.Type.LessThen => b.LessThen(a),
                            Token.Type.Method => ((Fun)b.Get(token.value)).Call(a.CIter().Insert(b, 0)),
                            _ => throw new InvalidOperationException()
                        };

                        calculateStack.Push(c);
                    }
                }
                else
                {
                    calculateStack.Push(Obj.Convert(token.value, properties));
                }
            }

            return calculateStack.TryPop(out var v) ? v : Obj.None;
        }
    }
}
