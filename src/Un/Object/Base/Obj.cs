using Un.Object.Collections;
using Un.Object.Primitive;

namespace Un.Object;

public class Obj(string type) : IComparable<Obj>
{
    public static Obj Error = new("error");
    public static Obj None = new("none");
    public static Obj Null = new("null");

    public virtual string Type { get; protected set; } = type;
    public virtual HashSet<string> Types { get; set; } = [];
    public virtual Obj Self { get; set; } = None;
    public virtual Obj Super { get; set; } = None;
    public virtual Attributes Members { get; set; } = [];

    public Obj() : this("obj") {}

    public virtual Obj Init(Tup args)
    {
        if (TryMethod("__init__", out _, args)) { }
        else if (Super is not null && !Super.IsNone())
            Super.Init(args);
        else
            return new Err($"unsupported operand type(s) for __init__: '{Type}'");
        return this;
    }

    public virtual Obj Add(Obj other)
    {
        if (TryMethod("__add__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Add(other) : new Err($"unsupported operand type(s) for +: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Sub(Obj other)
    {
        if (TryMethod("__sub__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Sub(other) : new Err($"unsupported operand type(s) for -: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Mul(Obj other)
    {
        if (TryMethod("__mul__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Mul(other) : new Err($"unsupported operand type(s) for *: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Div(Obj other)
    {
        if (TryMethod("__div__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Div(other) : new Err($"unsupported operand type(s) for /: '{Type}' and '{other.Type}'");
    }

    public virtual Obj IDiv(Obj other)
    {
        if (TryMethod("__idiv__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.IDiv(other) : new Err($"unsupported operand type(s) for //: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Mod(Obj other)
    {
        if (TryMethod("__mod__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Mod(other) : new Err($"unsupported operand type(s) for %: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Pow(Obj other)
    {
        if (TryMethod("__pow__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Pow(other) : new Err($"unsupported operand type(s) for **: '{Type}' and '{other.Type}'");
    }

    public virtual Obj And(Obj other)
    {
        if (ToBool().As<Bool>().Value) return other;
        return other;
    }

    public virtual Obj Or(Obj other)
    {
        if (ToBool().As<Bool>().Value) return this;
        return other;
    }

    public virtual Obj Xor(Obj other)
    {
        if (ToBool().As<Bool>().Value ^ other.ToBool().As<Bool>().Value) return new Bool(true);
        return new Bool(false);
    }

    public virtual Obj Not() => new Bool(!ToBool().As<Bool>().Value);

    public virtual Obj BAnd(Obj other)
    {
        if (TryMethod("__band__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BAnd(other) : new Err($"unsupported operand type(s) for &: '{Type}' and '{other.Type}'");
    }

    public virtual Obj BOr(Obj other)
    {
        if (TryMethod("__bor__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BOr(other) : new Err($"unsupported operand type(s) for |: '{Type}' and '{other.Type}'");
    }

    public virtual Obj BXor(Obj other)
    {
        if (TryMethod("__bxor__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BXor(other) : new Err($"unsupported operand type(s) for ^: '{Type}' and '{other.Type}'");
    }

    public virtual Obj BNot()
    {
        if (TryMethod("__bnot__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BNot() : new Err($"unsupported operand type(s) for ~: '{Type}'");
    }

    public virtual Obj LShift(Obj other)
    {
        if (TryMethod("__lsh__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.LShift(other) : new Err($"unsupported operand type(s) for <<: '{Type}' and '{other.Type}'");
    }

    public virtual Obj RShift(Obj other)
    {
        if (TryMethod("__rsh__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.RShift(other) : new Err($"unsupported operand type(s) for >>: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Eq(Obj other)
    {
        if (TryMethod("__eq__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Eq(other) : new Err($"unsupported operand type(s) for ==: '{Type}' and '{other.Type}'");
    }

    public virtual Obj NEq(Obj other) => new Bool(!Eq(other).As<Bool>().Value);

    public virtual Obj Lt(Obj other)
    {
        if (TryMethod("__lt__", out Obj? value, new([other], ["other"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Lt(other) : new Err($"unsupported operand type(s) for <: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Gt(Obj other) => new Bool(!other.Lt(other).As<Bool>().Value && !other.Eq(other).As<Bool>().Value);

    public virtual Obj LtOrEq(Obj other) => new Bool(other.Lt(other).As<Bool>().Value || other.Eq(other).As<Bool>().Value);

    public virtual Obj GtOrEq(Obj other) => new Bool(!other.Lt(other).As<Bool>().Value);

    public virtual Obj Slicer(Int to, Int from, Int step)
    {
        List list = [];
        long a = to.Value;
        long b = from.Value == -1 ? Len().As<Int>().Value : from.Value;

        do
        {
            list.Append(GetItem(new Int(a)));
            a += step.Value;
        } while (a < b);

        return list;
    }

    public virtual Obj Call(Tup args)
    {
        if (TryMethod("__call__", out Obj? value, args))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Call(args) : new Err($"unsupported operand type(s) for (): '{Type}'");
    }

    public virtual Obj Len()
    {
        if (TryMethod("__len__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Len() : new Err("unsupported operand type(s) for len()");
    }

    public virtual Obj Hash()
    {
        if (TryMethod("__hash__", out Obj? value, []))
            return value.ToInt();
        return Super is not null && !Super.IsNone() ? Super.Hash() : new Err("cannot hashable object");
    }

    public virtual void SetAttr(string name, Obj value)
    {
        if (TryMethod("__setattr__", out _, new([new Str(name), value], ["key", "value"])))
            return;
        if (Super is not null && !Super.IsNone())
            Super.SetAttr(name, value);

        Members[name] = value;
    }

    public virtual Obj GetAttr(string name)
    {
        if (TryMethod("__getattr__", out Obj? value, new([new Str(name)], []))) {}
        else if (Global.TryGetOriginalValue(Type, name, out value)) {}
        else if (Members.TryGetValue(name, out value)) {}
        else if (Super is not null && !Super.IsNone())
            value = Super.GetAttr(name);
        else return new Err($"'{Type}' object has no attribute '{name}'");

        value.Self = this;
        value.Super = Super;
        return value;
    }

    public virtual void SetItem(Obj key, Obj value)
    {
        if (TryMethod("__setitem__", out _, new([key, value], ["key", "value"])))
            return;
        else if (Super is not null && !Super.IsNone())
            Super.SetItem(key, value);
        else
            throw new Panic($"unsupported operand type(s) for [] = 'value': '{Type}'");
    }

    public virtual Obj GetItem(Obj key)
    {
        if (TryMethod("__getitem__", out Obj? value, new([key], ["key"])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.GetItem(key) : new Err($"unsupported operand type(s) for []: '{Type}'");
    }

    public virtual Obj Pos()
    {
        if (TryMethod("__pos__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Pos() : new Err("cannot positiveable object");
    }

    public virtual Obj Neg()
    {
        if (TryMethod("__neg__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Neg() : new Err("cannot negativeable object");
    }

    public virtual Obj Spread()
    {
        if (TryMethod("__spread__", out Obj? value, []) && value.As<Spread>(out var spread))
            return spread;
        return Super is not null && !Super.IsNone() ? Super.Spread() : new Err("cannot spreadable object");
    }

    public virtual Obj Is(Obj obj)
    {
        if (TryMethod("__is__", out Obj? value, new([obj], ["obj"])))
            return value;

        foreach (var type in Types)
            if (type == obj.Type)
                return new Bool(true);

        return obj.Type == Type ? new Bool(true) : Super is not null && !Super.IsNone() ? Super.Is(obj) : new Err("cannot compare types");
    }

    public virtual Obj In(Obj obj)
    {
        if (TryMethod("__in__", out Obj? value, new([obj], [])))
            return value;
        return Super is not null && !Super.IsNone() ? Super.In(obj) : new Err("cannot find object");
    }

    public virtual Obj ToInt()
    {
        if (TryMethod("__int__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.ToInt() : new Err("cannot intable object");
    }

    public virtual Obj ToFloat()
    {
        if (TryMethod("__tuple__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.ToFloat() : new Err("cannot floatable object");
    }

    public virtual Obj ToStr()
    {
        if (TryMethod("__str__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() && Super.Has("__str__") ? Super.ToStr() : new Str($"{Type}");
    }

    public virtual Obj ToBool()
    {
        if (TryMethod("__bool__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.ToBool() : new Err("cannot boolable object");
    }

    public virtual Obj ToList()
    {
        if (TryMethod("__list__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.ToList() : new Err("cannot listable object");
    }

    public virtual Obj ToTuple()
    {
        if (TryMethod("__tuple__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.ToTuple() : new Err("cannot tupleable object");
    }

    public virtual Obj Entry()
    {
        if (TryMethod("__entry__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Entry() : new Err("cannot entryable object");
    }

    public virtual Obj Exit()
    {
        if (TryMethod("__exit__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Exit() : new Err("cannot exitable object");
    }

    public virtual Obj Iter()
    {
        if (TryMethod("__iter__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Iter() : new Err("cannot iterable object");
    }

    public virtual Obj Next()
    {
        if (TryMethod("__next__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Next() : new Err("cannot iterable object");
    }

    public virtual Obj Copy()
    {
        if (TryMethod("__copy__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Copy() : this;
    }

    public virtual Obj Clone()
    {
        if (TryMethod("__clone__", out Obj? value, []))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Clone() : new Err("cannot cloneable object");
    }

    public bool As<T>(out T value) where T : Obj
    {
        if (this is T obj)
        {
            value = obj;
            return true;
        }

        value = null!;
        return false;
    }

    public bool As<T, U>(out Obj value) where T : Obj where U : Obj
    {
        if (this is T obj1)
        {
            value = obj1;
            return true;
        }
        if (this is U obj2)
        {
            value = obj2;
            return true;
        }

        value = null!;
        return false;
    }


    public T As<T>() where T : Obj
    {
        if (this is T obj)
            return obj;

        throw new Panic($"cannot cast {Type} to {typeof(T).Name}");
    }

    public Obj As<T, U>() where T : Obj where U : Obj
    {
        if (this is T obj1)
            return obj1;
        if (this is U obj2)
            return obj2;

       return new Err($"cannot cast {Type} to {typeof(T).Name} or {typeof(U).Name}");
    }

    public T As<T>(string message) where T : Obj
    {
        if (this is T obj)
            return obj;

        throw new Panic(message);
    }

    public bool Ok(out Err e)
    {
        if (this is Err err)
        {
            e = err;        
            return false;
        }

        e = new Err("");
        return true;
    }

    public Obj Unwrap(Context context)
    {
        if (this is Err e)        
            throw new Error(e.Message, context);
        return this;
    }

    public T Unwrap<T>(Context context)
        where T : Obj
    {
        if (this is Err e)
            throw new Error(e.Message, context);
        if (this is not T t)
            throw new Error($"cannot cast {Type} to {typeof(T).Name}", context);
        return t;
    }

    private bool TryMethod(string name, out Obj value, Tup args)
    {
        if (Members.TryGetValue(name, out Obj? method))
        {
            method.Self = this;
            method.Super = Super;
            value = method.Call(args);
            return true;
        }
        value = null!;
        return false;
    }

    public bool IsNone() => Type == "none";

    public bool IsType() => Type.StartsWith("__") && Type.EndsWith("__");

    public bool Has(string name)
    {
        if (Members.ContainsKey(name))
            return true;
        if (Super is not null && !Super.IsNone())
            return Super.Has(name);
        return false;
    }

    public virtual Attributes GetOriginal() => [];

    public override bool Equals(object? other) => other switch
    {
        Obj o => Eq(o).As<Bool>().Value,
        _ => false,
    };
    
    public int CompareTo(Obj? other)
    {
        if (other == null) return 0;
        if (Eq(other).As<Bool>().Value) return 0;
        if (Lt(other).As<Bool>().Value) return -1;
        if (Gt(other).As<Bool>().Value) return 1;
        throw new Panic("types that are not comparable to each other.");
    }
}
