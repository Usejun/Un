using Un.Object;
using Un.Class;

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

        public Obj Calculate(List<Token> expression, Dictionary<string, Obj> variable)
        {
            calculateStack.Clear();
            List<Token> postfix = Postfix(expression);

            for (int i = 0; i < postfix.Count; i++)
            {
                Token token = postfix[i]; 
                
                if (variable.TryGetValue(token.value, out var value))
                {
                    calculateStack.Push(value);
                }
                else if (Process.IsGlobalVariable(token))
                {
                    calculateStack.Push(Process.Variable[token.value]);
                }
                else if (Process.IsClass(token))
                {
                    calculateStack.Push(Process.GetClass(token.value));
                }
                else if (Process.IsFunc(token))
                {
                    calculateStack.Push(Process.GetFunc(token.value).Call(calculateStack.TryPop(out var obj) ? obj : Obj.None));
                }
                else if (Process.IsOperator(token))
                {
                    if (Process.IsSoloOperator(token))
                    {
                        Obj a = calculateStack.Pop(), b = Obj.None;
                        if (token.tokenType == Token.Type.Indexer)
                        {
                            b = Obj.Convert(token.value, variable);
                            if (a is Iter iter && b is Int index1)
                                calculateStack.Push(iter[index1]);
                            else if (a is Str str && b is Int index2)
                                calculateStack.Push(str[index2]);
                            else
                                throw new ObjException("Operator Error");
                        }
                        else if (token.tokenType == Token.Type.Pointer)
                        {
                            if (a is Cla cla)
                                calculateStack.Push(cla.Get(new(token.value)));
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
                            Token.Type.Equal => new Bool(b.CompareTo(a) == 0),
                            Token.Type.Unequal => new Bool(b.CompareTo(a) != 0),
                            Token.Type.GreaterOrEqual => new Bool(b.CompareTo(a) <= 0),
                            Token.Type.LessOrEqual => new Bool(b.CompareTo(a) >= 0),
                            Token.Type.GreaterThen => new Bool(b.CompareTo(a) < 0),
                            Token.Type.LessThen => new Bool(b.CompareTo(a) > 0),
                            _ => throw new ObjException("Operator Error")
                        };

                        calculateStack.Push(c);
                    }
                }
                else
                {
                    calculateStack.Push(Obj.Convert(token.value, variable));
                }
            }

            return calculateStack.TryPop(out var v) ? v : Obj.None;
        }
    }
}
