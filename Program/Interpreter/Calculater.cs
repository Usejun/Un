﻿using Un.Interpreter;

namespace Un;

public class Calculator
{
    private static List<Token> Postfix(List<Token> expression)
    {
        Stack<Token> postfixStack = [];
        List<Token> postfix = [];

        foreach (var token in expression)
        {
            if (Token.IsOperator(token.type))
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
            
            if (Process.TryGetStaticClass(token, out var sc))
            {
                calculateStack.Push(sc);
            }
            else if (IsCallable(token.type))
            {
                var (name, args, call) = Fun.Split(token.value, field);

                if (token.type == Token.Type.Method)
                {
                    Obj a = Collections.Tuple.Split(calculateStack.Pop());

                    calculateStack.Push(call ? Fun.Method(a, name, args, new()) : a.Get(name));
                }
                else if (token.type == Token.Type.NullableMethod)
                {
                    Obj a = Collections.Tuple.Split(calculateStack.Pop());

                    if (!a.HasProperty(name))                    
                        calculateStack.Push(Obj.None);                    
                    else                    
                        calculateStack.Push(call ? Fun.Method(a, name, args, new()) : a.Get(name));                    
                }
                else if (Process.TryGetClass(name, out var cla))
                {
                    calculateStack.Push(call ? cla.Clone().Init(args, new()) : new NativeFun(Literals.Init, 0, field => cla.Init(args, new()), []));
                }
                else if (field.Get(name, out var local))
                {
                    if (!local.As<Fun>(out var fun))
                        throw new SyntaxError($"{name} is not function.");  

                    calculateStack.Push(call ? fun.Clone().Call(args, new()) : fun);
                }
                else if (Process.TryGetGlobalProperty(name, out var global))
                {
                    if (!global.As<Fun>(out var fun))
                        throw new SyntaxError($"{name} is not function.");  

                    calculateStack.Push(call ? fun.Clone().Call(args, new()) : fun);
                }
                else throw new KeyError($"{name} function does not exist.");
            }
            else if (Token.IsOperator(token.type))
            {
                if (calculateStack.Count == 0)
                    throw new SyntaxError("invalid expression.");

                Obj a = Collections.Tuple.Split(calculateStack.Pop());
                Obj b = Collections.Tuple.Split(Token.IsSoloOperator(token.type) ? Obj.None : calculateStack.Pop());
                Obj c = token.type switch
                {
                    Token.Type.Indexer => a.GetItem(Obj.Parse(token.value, field), field),
                    Token.Type.Slicer => a.Slice(Obj.Parse(token.value.Split(Literals.Colon, 1)[0], field), 
                                                 Obj.Parse(token.value.Split(Literals.Colon, 2)[1], field), field),
                    Token.Type.Property => a.Get(token.value),
                    Token.Type.NullableProperty => a.HasProperty(token.value) ? a.Get(token.value) : Obj.None,
                    Token.Type.Not => new Bool(!a.CBool().Value),
                    Token.Type.BNot => a.BNot(),
                    Token.Type.DoubleQuestion => Obj.IsNone(b) ? a : b,
                    Token.Type.In => a.CList().Contains(b),
                    Token.Type.Is => new Bool(b.ClassName == (a.As<Fun>(out var fun) ? fun.Name : a.ClassName)),
                    Token.Type.And => b.CBool().Value ? a.CBool() : new Bool(false),
                    Token.Type.Or => !b.CBool().Value ? a.CBool() : new Bool(true),
                    Token.Type.Plus => b.Add(a, field),
                    Token.Type.Minus => b.Sub(a, field),
                    Token.Type.Asterisk => b.Mul(a, field),
                    Token.Type.DoubleAsterisk => b.Pow(a, field),
                    Token.Type.Slash => b.Div(a, field),
                    Token.Type.DoubleSlash => b.IDiv(a, field),
                    Token.Type.Percent => b.Mod(a, field),
                    Token.Type.BAnd => b.BAnd(a, field),
                    Token.Type.BOr => b.BOr(a, field),
                    Token.Type.BXor => b.BXor(a, field),
                    Token.Type.Equal => b.Eq(a, field),
                    Token.Type.Unequal => b.Ueq(a, field),
                    Token.Type.GreaterOrEqual => b.Goe(a, field),
                    Token.Type.LessOrEqual => b.Loe(a, field),
                    Token.Type.GreaterThen => b.Gt(a, field),
                    Token.Type.LessThen => b.Lt(a, field),
                    _ => throw new OperatorError("unknown operator.")
                };;

                calculateStack.Push(c);
            }                
            else calculateStack.Push(Collections.Tuple.Split(Obj.Parse(token.value, field)));
        }

        if (calculateStack.Count == 1)
            return calculateStack.Pop();
        else if (calculateStack.Count == 0)
            return Obj.None;
        else
            throw new SyntaxError("invalid expression.");
    }

    public static Obj On(List<Token> expression, Field field)
    {
        Obj value;
        List<Token> buffer = [];
        Token.Type prev = Token.Type.None;
        Bool condiotion = Bool.False;

        foreach (var token in expression)
        {
            if (!Token.IsConditional(token.type))
            {
                buffer.Add(token);
                continue;
            }

            value = Calculate(buffer, field);
            buffer.Clear();

            switch (prev)
            {
                case Token.Type.And:
                    if (!value.CBool().Value)
                        return Bool.False;
                    break;
                case Token.Type.Or:
                    if (!value.CBool().Value)
                        return condiotion;
                    break;
                case Token.Type.Xor:
                    return new Bool(condiotion != value.CBool());
                default:
                    switch (token.type)
                    {
                        case Token.Type.And:
                            if (!value.CBool().Value)
                                return Bool.False;
                            break;
                        case Token.Type.Or:
                            if (value.CBool().Value)
                                return Bool.True;
                            break;
                        case Token.Type.Xor:
                            break;
                        default:
                            break;
                    }
                    break;
            }

            condiotion = value.CBool();
            prev = token.type;

        }

        value = Calculate(buffer, field);

        switch (prev)
        {
            case Token.Type.And:
                if (!value.CBool().Value)
                    return Bool.False;
                break;
            case Token.Type.Or:
                if (!value.CBool().Value)
                    return condiotion;
                break;
            case Token.Type.Xor:
                return new Bool(condiotion != value.CBool());
            default:
                break;
        }

        return value;
    }

    public static Obj All(string str, Field field) => On(Lexer.Lex(Tokenizer.Tokenize(str), field), field);

    public static Obj All(List<Token> tokens, Field field) => On(Lexer.Lex(tokens, field), field);

    private static bool IsCallable(Token.Type type) => type switch
    {
        Token.Type.Function or Token.Type.Method or Token.Type.NullableMethod => true,
        _ => false
    };
}
