    using Un.Object;

namespace Un
{
    public class Calculator
    {
        Dictionary<Token.Type, int> Operator = Process.Operator;

        Stack<Token> postfixStack = [];
        Stack<Obj> calculateStack = [];

        public List<Token> Postfix(List<Token> expression)
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

        public Obj Calculate(List<Token> expression)
        {
            calculateStack.Clear();
            List<Token> postfix = Postfix(expression);

            for (int i = 0; i < postfix.Count; i++)
            {
                Token token = postfix[i];

                if (Process.IsFunction(token.value))
                    calculateStack.Push(Process.Function[token.value](calculateStack.TryPop(out var p) ? p : Obj.None));
                else if (Process.IsVariable(token.value))
                    calculateStack.Push(Process.Variable[token.value]);
                else if (Process.IsOperator(token.value))
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
                        _ => throw new ObjException("operator Error")
                    };

                    calculateStack.Push(c);
                }
                else
                    calculateStack.Push(Obj.Convert(token.value));
            }

            return calculateStack.TryPop(out var v) ? v : Obj.None;
        }
    }
}
