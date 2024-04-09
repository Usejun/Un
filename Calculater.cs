using Un.Object;
using Un.Function;

namespace Un
{
    public class Calculator
    {
        public readonly static Dictionary<Token.Type, int> Operator = new()
        {
            { Token.Type.Assign, 0 }, { Token.Type.RParen, 0 }, { Token.Type.Equal,  0 }, { Token.Type.Unequal, 0 },
            { Token.Type.LessOrEqual, 0 }, { Token.Type.LessThen, 0 }, { Token.Type.GreaterOrEqual, 0 }, { Token.Type.GreaterThen, 0 },
            { Token.Type.Plus, 1 }, { Token.Type.Minus, 1 }, { Token.Type.Percent, 1 }, { Token.Type.Bang, 1 },
            { Token.Type.Asterisk, 2 }, { Token.Type.Slash, 2 }, { Token.Type.DoubleSlash, 2 },
            { Token.Type.Indexer, 2 }, { Token.Type.Property, 2 },
            { Token.Type.Function, 3 }, { Token.Type.Method, 3 },
            { Token.Type.LParen, 4 },
        };

        private readonly Stack<Token> postfixStack = [];
        private readonly Stack<Obj> calculateStack = [];

        public List<Token> Postfix(List<Token> expression)
        {
            postfixStack.Clear();
            List<Token> postfix = [];

            foreach (var token in expression)
            {
                if (Operator.TryGetValue(token.tokenType, out int value))
                {
                    if (token.tokenType == Token.Type.RParen)
                    {
                        while (postfixStack.TryPop(out var v) && v.tokenType != Token.Type.LParen)
                            postfix.Add(v);
                    }
                    else
                    {
                        while (postfixStack.TryPeek(out var v) && Operator[v.tokenType] >= value && v.tokenType != Token.Type.LParen)
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

                if (IsOperator(token))
                {
                    if (IsSoloOperator(token))
                    {
                        Obj a = calculateStack.Pop(), b;
                        if (token.tokenType == Token.Type.Indexer)
                        {
                            b = Obj.Convert(token.value, properties);

                            calculateStack.Push(a.GetByIndex(b));
                        }
                        else if (token.tokenType == Token.Type.Property)
                        {
                            b = a.Get(token.value);

                            if (b is Fun fun)
                                b = fun.Call(Iter.Arg(a, calculateStack.Pop()));

                            calculateStack.Push(b);
                        }
                        else if (token.tokenType == Token.Type.Bang)
                        {
                            if (a is Bool bo)
                                calculateStack.Push(new Bool(!bo.value));
                            else
                                throw new InvalidOperationException("It is not boolean type.");
                        }
                        else throw new InvalidOperationException();
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
                            Token.Type.Method => ((Fun)b.Get(token.value)).Call(Iter.Arg(b, a)),
                            _ => throw new InvalidOperationException()
                        };

                        calculateStack.Push(c);
                    }
                }
                else if (Process.TryGetClass(token, out var cla))
                {
                    calculateStack.Push(cla.Init(calculateStack.TryPop(out var value) ? value : Obj.None));
                }
                else if (Process.TryGetStaticClass(token, out var staticCla))
                {
                    calculateStack.Push(staticCla);
                }
                else if (properties.TryGetValue(token.value, out var local))
                {
                    if (local is Fun fun)
                        local = fun.Call(calculateStack.TryPop(out var obj) ? obj : Obj.None);

                    calculateStack.Push(local);
                }
                else if (Process.TryGetProperty(token, out var global))
                {
                    if (global is Fun fun)
                        global = fun.Call(calculateStack.TryPop(out var obj) ? obj : Obj.None);
                    
                    calculateStack.Push(global);
                }
                else
                {
                    calculateStack.Push(Obj.Convert(token.value, properties));
                }
            }

            return calculateStack.TryPop(out var v) ? v : Obj.None;
        }

        public static bool IsOperator(Token token) => IsOperator(token.tokenType);

        public static bool IsOperator(Token.Type type) => type switch
        {
            >= Token.Type.Assign and <= Token.Type.RParen => true,
            _ => false
        };

        public static bool IsOperator(char chr) => IsOperator(Token.GetType(chr));

        public static bool IsOperator(string str) => IsOperator(Token.GetType(str));

        public static bool IsSoloOperator(Token token) => IsSoloOperator(token.tokenType);

        public static bool IsSoloOperator(Token.Type type) => type switch
        {
            Token.Type.Bang or Token.Type.Indexer or Token.Type.Property => true,
            _ => false,
        };

        public static bool IsSoloOperator(char chr) => IsSoloOperator(Token.GetType(chr));

        public static bool IsSoloOperator(string str) => IsSoloOperator(Token.GetType(str));

        public static bool IsBasicOperator(Token.Type type) => type switch
        {
            >= Token.Type.Assign and <= Token.Type.Unequal => true,
            _ => false,
        };

        public static bool IsBasicOperator(char chr) => IsBasicOperator(Token.GetType(chr));

        public static bool IsBasicOperator(string str) => IsBasicOperator(Token.GetType(str));
    }
}
