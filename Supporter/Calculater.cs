﻿using Un.Object;
using Un.Object.Value;
using Un.Object.Reference;
using Un.Object.Function;

namespace Un.Supporter
{
    public class Calculator
    {
        public static List<Token> Postfix(List<Token> expression)
        {
            Stack<Token> postfixStack = [];
            List<Token> postfix = [];

            foreach (var token in expression)
            {
                if (Token.Operator.TryGetValue(token.type, out int value))
                {
                    if (token.type == Token.Type.RParen)
                    {
                        while (postfixStack.TryPop(out var v) && v.type != Token.Type.LParen)
                            postfix.Add(v);
                    }
                    else
                    {
                        while (postfixStack.TryPeek(out var v) && Token.Operator[v.type] >= value && v.type != Token.Type.LParen)
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

                if (Token.IsOperator(token))
                {
                    if (Token.IsSoloOperator(token))
                    {
                        Obj a = calculateStack.Pop(), b;
                        if (token.type == Token.Type.Indexer)
                        {
                            b = Obj.Convert(token.value, properties);

                            calculateStack.Push(a.GetItem(new Iter([b])));
                        }
                        else if (token.type == Token.Type.Property)
                        {
                            b = a.Get(token.value);

                            if (b is Fun fun)
                                b = fun.Call(calculateStack.Pop().CIter().ExtendInsert(a, 0));

                            calculateStack.Push(b);
                        }
                        else if (token.type == Token.Type.Bang)
                        {                        
                            calculateStack.Push(new Bool(!a.CBool().value));
                        }
                        else if (token.type == Token.Type.In)
                        {
                            calculateStack.Push(a.CIter().Contains(calculateStack.Pop()));
                        }
                        else throw new InvalidOperationException();
                    }
                    else
                    {
                        Obj a = calculateStack.Pop(), b = calculateStack.Pop();

                        Obj c = token.type switch
                        {
                            Token.Type.Plus => b.Add(a),
                            Token.Type.Minus => b.Sub(a),
                            Token.Type.Asterisk => b.Mul(a),
                            Token.Type.Slash => b.Div(a),
                            Token.Type.DoubleSlash => b.IDiv(a),
                            Token.Type.Percent => b.Mod(a),
                            Token.Type.And => b.And(a),
                            Token.Type.Or => b.Or(a),
                            Token.Type.Caret => b.Xor(a),
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
                else if (Process.TryGetClass(token, out var cla))
                {
                    if (calculateStack.TryPop(out var obj) && obj is Iter args)
                        calculateStack.Push(cla.Clone().Init(args));
                    else
                        calculateStack.Push(new NativeFun(token.value, cla.Init));
                }
                else if (Process.TryGetStaticClass(token, out var staticCla))
                {
                    calculateStack.Push(staticCla);
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
                else
                {
                    calculateStack.Push(Obj.Convert(token.value, properties));
                }
            }

            return calculateStack.TryPop(out var v) ? v : Obj.None;
        }
    }
}
