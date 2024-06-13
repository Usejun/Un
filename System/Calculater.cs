﻿namespace Un;

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

    public static Obj Calculate(List<Token> expression, Field field)
    {
        if (expression.Count == 0) return Obj.None;

        Stack<Obj> calculateStack = [];
        List<Token> postfix = Postfix(expression);

         for (int i = 0; i < postfix.Count; i++)
         {
            Token token = postfix[i];

            if (token.type == Token.Type.Variable && Process.TryGetStaticClass(token, out var staticCla))
            {
                calculateStack.Push(staticCla);
            }
            else if ((token.type == Token.Type.Variable || token.type == Token.Type.Function) && Process.TryGetClass(token, out var cla))
            {
                if (token.type == Token.Type.Function && calculateStack.TryPop(out var obj) && obj is List args)
                    calculateStack.Push(cla.Clone().Init(args));
                else
                    calculateStack.Push(new NativeFun(token.Value, -1, cla.Init));
            }
            else if (token.type == Token.Type.Function)
            {
                if (field.Get(token.Value, out var local))
                {
                    if (local is Fun fun && calculateStack.TryPeek(out var a) && a is List args)
                        local = fun.Clone().Call(calculateStack.Pop().CList());

                    calculateStack.Push(local);
                }
                else if (Process.TryGetGlobalProperty(token, out var global))
                {
                    if (global is Fun fun && calculateStack.TryPeek(out var a) && a is List args)
                        global = fun.Clone().Call(calculateStack.Pop().CList());

                    calculateStack.Push(global);
                }
                else throw new SyntaxError();
            }
            else if (Token.IsOperator(token))
            {
                Obj a = calculateStack.Pop(), b;
                if (token.type == Token.Type.Indexer)
                {
                    b = Obj.Convert(token.Value, field);

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

                    if (b is Fun fun)
                        b = fun.Call(calculateStack.Pop().CList().ExtendInsert(a, 0));

                    calculateStack.Push(b);
                }
                else if (token.type == Token.Type.Bang || token.type == Token.Type.Not)
                {
                    calculateStack.Push(new Bool(!a.CBool().Value));
                }
                else if (token.type == Token.Type.BNot)
                {
                    calculateStack.Push(a.BNot());
                }
                else if (token.type == Token.Type.In)
                {
                    calculateStack.Push(a.CList().Contains(calculateStack.Pop()));
                }
                else if (token.type == Token.Type.Is)
                {
                    b = calculateStack.Pop();

                    if (a is Fun fun)
                        calculateStack.Push(new Bool(b.ClassName == fun.name));
                    else
                        calculateStack.Push(new Bool(b.ClassName == a.ClassName));
                }
                else if (token.type == Token.Type.And)
                {
                    b = calculateStack.Pop();

                    calculateStack.Push(b.CBool().Value ? a.CBool() : new Bool(false));
                }
                else if (token.type == Token.Type.Or)
                {
                    b = calculateStack.Pop();

                    calculateStack.Push(!b.CBool().Value ? a.CBool() : new Bool(true));
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
                        Token.Type.Method => ((Fun)b.Get(token.Value)).Call(b.Type().Value == "obj" ? a.CList() : a.CList().Insert(b, 0)),
                        _ => throw new OperatorError()
                    };

                    calculateStack.Push(c);
                }
            }                
            else
            {
                calculateStack.Push(Obj.Convert(token.Value, field));
            }
        }

        return calculateStack.TryPop(out var v) ? v : Obj.None;
    }
}
