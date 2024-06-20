namespace Un;

public class Calculator
{
    private static List<Token> Postfix(List<Token> expression)
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

    private static Obj Calculate(List<Token> expression, Field field)
    {
        if (expression.Count == 0) return Obj.None;

        Stack<Obj> calculateStack = [];
        List<Token> postfix = Postfix(expression);

        for (int i = 0; i < postfix.Count; i++)
         {
            Token token = postfix[i];
            
            if (Process.TryGetStaticClass(token, out var staticCla))
            {
                calculateStack.Push(staticCla);
            }
            else if (token.type == Token.Type.Function || token.type == Token.Type.Method)
            {
                var index = token.Value.IndexOf(Literals.FunctionSep);
                var name = token.Value[..index];
                var args = new Collections.Tuple(new Map(token.Value[(index + 1)..], field));               
                var isCall = args.Count >= 0; 

                if (token.type == Token.Type.Method)
                {
                    Obj a = calculateStack.Pop(), b = a.Get(name);

                    if (a.Get(name) is not Fun fun) throw new SyntaxError();

                    calculateStack.Push(isCall ? fun.Call(new([a, ..args])) : fun);
                }
                else if (Process.TryGetClass(name, out var cla))
                {
                    calculateStack.Push(isCall ? cla.Clone().Init(args) : new NativeFun(name, -1, cla.Init));
                }
                else if (field.Get(name, out var local))
                {
                    if (local is not Fun fun) throw new SyntaxError();

                    calculateStack.Push(isCall ? fun.Clone().Call(args) : fun);
                }
                else if (Process.TryGetGlobalProperty(name, out var global))
                {
                    if (global is not Fun fun) throw new SyntaxError();

                    calculateStack.Push(isCall ? fun.Clone().Call(args) : fun);
                }
                else throw new SyntaxError();
            }
            else if (Token.IsOperator(token))
            {
                Obj a = calculateStack.Pop(), b;
                a = a is Collections.Tuple at && at.Count == 1 ? at[0] : a;                

                if (token.type == Token.Type.Indexer)
                {
                    b = Obj.Convert(token.Value, field);
                    b = b is Collections.Tuple bt && bt.Count == 1 ? bt[0] : b;

                    calculateStack.Push(a.GetItem(new List([b])));
                }
                else if (token.type == Token.Type.Slicer)
                {
                    var index = token.Value.Split(':');

                    calculateStack.Push(a.Slice([Obj.Convert(index[0], field), Obj.Convert(index[1], field)]));
                }
                else if (token.type == Token.Type.Property)
                {
                    b = a.Get(token.Value);

                    calculateStack.Push(b);
                }
                else if (token.type == Token.Type.Not)
                {
                    calculateStack.Push(new Bool(!a.CBool().Value));
                }
                else if (token.type == Token.Type.BNot)
                {
                    calculateStack.Push(a.BNot());
                }
                else if (token.type == Token.Type.In)
                {
                    b = calculateStack.Pop();
                    b = b is Collections.Tuple bt && bt.Count == 1 ? bt[0] : b;

                    calculateStack.Push(a.CList().Contains(b));
                }
                else if (token.type == Token.Type.Is)
                {
                    b = calculateStack.Pop();
                    b = b is Collections.Tuple bt && bt.Count == 1 ? bt[0] : b;

                    calculateStack.Push(new Bool(b.ClassName == (a is Fun fun ? fun.Name : a.ClassName)));
                }
                else if (token.type == Token.Type.And)
                {
                    b = calculateStack.Pop();
                    b = b is Collections.Tuple bt && bt.Count == 1 ? bt[0] : b;

                    calculateStack.Push(b.CBool().Value ? a.CBool() : new Bool(false));
                }
                else if (token.type == Token.Type.Or)
                {
                    b = calculateStack.Pop();
                    b = b is Collections.Tuple bt && bt.Count == 1 ? bt[0] : b;

                    calculateStack.Push(!b.CBool().Value ? a.CBool() : new Bool(true));
                }
                else
                {
                    b = calculateStack.Pop();
                    b = b is Collections.Tuple bt && bt.Count == 1 ? bt[0] : b;

                    Obj c = token.type switch
                    {                        
                        Token.Type.Plus => b.Add(a),
                        Token.Type.Minus => b.Sub(a),
                        Token.Type.Asterisk => b.Mul(a),
                        Token.Type.DoubleAsterisk => b.Pow(a),
                        Token.Type.Slash => b.Div(a),
                        Token.Type.DoubleSlash => b.IDiv(a),
                        Token.Type.Percent => b.Mod(a),
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
                        _ => throw new OperatorError()
                    };

                    calculateStack.Push(c);
                }
            }                
            else calculateStack.Push(Obj.Convert(token.Value, field));
        }

        return calculateStack.TryPop(out var v) ? v : Obj.None;
    }

    public static Obj On(List<Token> expression, Field field)
    {        
        List<Token> buffer = [];
        Token.Type prev = Token.Type.None;

        foreach (var token in expression)
        {
            if (token.type == Token.Type.And)
            {
                Obj value = Calculate(buffer, field);
                buffer.Clear();

                if (prev != Token.Type.None) 
                    return new Bool(value.CBool().Value);

                if (!value.CBool().Value) return new Bool(false);

                prev = token.type;
            }
            else if (token.type == Token.Type.Or)
            {
                Obj value = Calculate(buffer, field);
                buffer.Clear();

                if (prev != Token.Type.None)
                    return new Bool(value.CBool().Value);

                if (value.CBool().Value) return new Bool(true);

                prev = token.type;
            }
            else buffer.Add(token);
        }

        return buffer.Count != 0 ? Calculate(buffer, field) : Obj.None;
    }

    public static Obj All(string str, Field field) => On(Lexer.Lex(Tokenizer.Tokenize(str), field), field);

    public static Obj All(List<Token> tokens, Field field) => On(Lexer.Lex(tokens, field), field);
}
