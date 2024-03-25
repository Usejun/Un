using Un.Object;
using Un.Function;

namespace Un
{
    public static class Calculator
    {
        static Dictionary<Token.Type, int> Operator = Process.Operator;
        
        static Stack<Token> postfixStack = [];
        static Stack<Obj> calculateStack = [];

        public static List<Token> Postfix(List<Token> expression)
        {
            postfixStack.Clear();
            List<Token> postfix = [];

            foreach (var token in expression)
            {
                if (Process.Operator.ContainsKey(token.tokenType))
                {
                    if (token.tokenType == Token.Type.RParen)
                    {
                        while (postfixStack.TryPop(out var v) && v.tokenType != Token.Type.LParen)
                            postfix.Add(v);
                    }
                    else
                    {
                        while (postfixStack.TryPeek(out var v) && Operator[v.tokenType] >= Operator[token.tokenType] && v.tokenType != Token.Type.LParen)
                            postfix.Add(postfixStack.Pop());
                        postfixStack.Push(token);
                    }
                }
                else postfix.Add(token);
            }

            postfix.AddRange([.. postfixStack]);

            return postfix;
        }

        public static Obj Calculate(List<Token> expression)
        {
            calculateStack.Clear();
            List<Token> postfix = Postfix(expression);

            for (int i = 0; i < postfix.Count; i++)
            {
                Token token = postfix[i];

                if (Process.IsFunc(token))
                {
                    calculateStack.Push(Process.Func[token.value].Call(calculateStack.TryPop(out var obj) ? obj : Obj.None));
                }
                else if (Process.IsVariable(token))
                {
                    calculateStack.Push(Process.Variable[token.value]);
                }
                else if (Process.IsOperator(token))
                {
                    if (Process.IsSoloOperator(token))
                    {
                        Obj a = calculateStack.Pop(), b = Obj.None;
                        if (token.tokenType == Token.Type.Indexer)
                        {
                            b = Obj.Convert(token.value);
                            if (a is Iter iter && b is Int index1)
                                calculateStack.Push(iter[index1]);
                            else if (a is Str str && b is Int index2)
                                calculateStack.Push(str[index2]);
                            else
                                throw new ObjException("Operator Error");
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
                    calculateStack.Push(Obj.Convert(token.value));
                }
            }

            return calculateStack.TryPop(out var v) ? v : Obj.None;
        }

        public static bool TryInt(this long l, out int i)
        {
            i = 0;
            if (l < int.MinValue || l > int.MaxValue) 
                return false;
            i = (int)l;
            return true;
        }
    }
}
