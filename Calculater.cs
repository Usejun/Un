using Un.Object;
using Un.Function;

namespace Un
{
    public class Calculator
    {
        Stack<Token> postfixStack = [];
        Stack<Obj> calculateStack = [];

        public List<Token> Postfix(List<Token> expression)
        {
            postfixStack.Clear();
            List<Token> postfix = [];

            foreach (var token in expression)
            {
                if (Process.Operator.TryGetValue(token.tokenType, out int value))
                {
                    if (token.tokenType == Token.Type.RParen)
                    {
                        while (postfixStack.TryPop(out var v) && v.tokenType != Token.Type.LParen)
                            postfix.Add(v);
                    }
                    else
                    {
                        while (postfixStack.TryPeek(out var v) && Process.Operator[v.tokenType] >= value && v.tokenType != Token.Type.LParen)
                            postfix.Add(postfixStack.Pop());
                        postfixStack.Push(token);
                    }
                }
                else postfix.Add(token);
            }

            postfix.AddRange([.. postfixStack]);

            return postfix;
        }

        public Obj Calculate(List<Token> expression, Dictionary<string, Obj> properties)
        {
            calculateStack.Clear();

            List<Token> postfix = Postfix(expression);

            for (int i = 0; i < postfix.Count; i++)
            {
                Token token = postfix[i];

                if (Process.IsOperator(token))
                {
                    if (Process.IsSoloOperator(token))
                    {
                        Obj a = calculateStack.Pop(), b = Obj.None;
                        if (token.tokenType == Token.Type.Indexer)
                        {
                            b = Obj.Convert(token.value, properties);
                            calculateStack.Push(a.GetByIndex(b));
                        }
                        else if (token.tokenType == Token.Type.Pointer)
                        {
                            b = a.Get(token.value);

                            if (b is Fun fun)
                                calculateStack.Push(fun.Call(new Iter([a, calculateStack.TryPop(out var result) ? result : Obj.None])));
                            else
                                calculateStack.Push(b);
                        }
                        else if (token.tokenType == Token.Type.Bang)
                        {
                            if (a is Bool bo)
                                calculateStack.Push(new Bool(!bo.value));
                            else
                                throw new ObjException("Operator Error");
                        }
                        else throw new ObjException("Operator Error");
                    }
                    else
                    {
                        Obj a = calculateStack.Pop(), b = calculateStack.Pop();

                        Obj c = token.tokenType switch
                        {
                            Token.Type.Plus => b.Add(a),
                            Token.Type.Minus => b.Sub(a),
                            Token.Type.Asterisk => b.Mul(a),
                            Token.Type.Slash => b.Div(a),
                            Token.Type.DoubleSlash => b.IDiv(a),
                            Token.Type.Percent => b.Mod(a),
                            Token.Type.Equal => new Bool(a.Comp(b).value == 0),
                            Token.Type.Unequal => new Bool(a.Comp(b).value != 0),
                            Token.Type.GreaterOrEqual => new Bool(a.Comp(b).value <= 0),
                            Token.Type.LessOrEqual => new Bool(a.Comp(b).value >= 0),
                            Token.Type.GreaterThen => new Bool(a.Comp(b).value < 0),
                            Token.Type.LessThen => new Bool(a.Comp(b).value > 0),
                            Token.Type.Method => (b.Get(token.value) as Fun).Call(new Iter([b, a])),
                            _ => throw new ObjException("Operator Error")
                        };

                        calculateStack.Push(c);
                    }
                }
                else if (Process.IsClass(token))
                {
                    calculateStack.Push(Process.GetClass(token.value).Init(calculateStack.TryPop(out var value) ? value : Obj.None));
                }
                else if (Process.IsStaticClass(token))
                {
                    calculateStack.Push(Process.GetStaticClass(token.value));
                }
                else if (properties.TryGetValue(token.value, out var value1))
                {
                    if (value1 is Fun fun)
                        calculateStack.Push(fun.Call(calculateStack.TryPop(out var obj) ? obj : Obj.None));
                    else
                        calculateStack.Push(value1);
                }
                else if (Process.IsGlobalVariable(token))
                {
                    Obj value = Process.GetProperty(token.value);
                    if (value is Fun fun)
                        calculateStack.Push(fun.Call(calculateStack.TryPop(out var obj) ? obj : Obj.None));
                    else
                        calculateStack.Push(value);
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
