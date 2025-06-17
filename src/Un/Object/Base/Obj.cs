using Un.Object.Collections;
using Un.Object.Function;
using Un.Object.Primitive;

namespace Un.Object;

public class Obj(string type) : IComparable<Obj>
{
    public static Obj Error = new("error");
    public static Obj None = new("none");
    public static Obj Null = new("null");

    public virtual string Type { get; protected set; } = type;
    public virtual Obj Self { get; protected set; } = None;
    public virtual Obj Super { get; protected set; } = None;
    public virtual Attributes Members { get; set; } = [];

    public Obj() : this("obj") {}

    public virtual Obj Init(Tup args)
    {
        if (TryMethod("__init__", out Obj? value))
            return value.Call([this, .. args]);
        return Super is not null && !Super.IsNone() ? Super.Init(args) : throw new Error($"unsupported operand type(s) for __init__: '{Type}'");
    }

    public virtual Obj Add(Obj other)
    {
        if (TryMethod("__add__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Add(other) : throw new Error($"unsupported operand type(s) for +: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Sub(Obj other)
    {
        if (TryMethod("__sub__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Sub(other) : throw new Error($"unsupported operand type(s) for -: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Mul(Obj other)
    {
        if (TryMethod("__mul__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Mul(other) : throw new Error($"unsupported operand type(s) for *: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Div(Obj other)
    {
        if (TryMethod("__div__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Div(other) : throw new Error($"unsupported operand type(s) for /: '{Type}' and '{other.Type}'");
    }

    public virtual Obj IDiv(Obj other)
    {
        if (TryMethod("__idiv__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.IDiv(other) : throw new Error($"unsupported operand type(s) for //: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Mod(Obj other)
    {
        if (TryMethod("__mod__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Mod(other) : throw new Error($"unsupported operand type(s) for %: '{Type}' and '{other.Type}'");
    }

    public virtual Obj Pow(Obj other)
    {
        if (TryMethod("__pow__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Pow(other) : throw new Error($"unsupported operand type(s) for **: '{Type}' and '{other.Type}'");
    }

    public virtual Obj And(Obj other)
    {
        if (ToBool().Value) return other;
        return other;
    }

    public virtual Obj Or(Obj other)
    {
        if (ToBool().Value) return this;
        return other;
    }

    public virtual Obj Xor(Obj other)
    {
        if (ToBool().Value ^ other.ToBool().Value) return new Bool(true);
        return new Bool(false);
    }

    public virtual Obj Not() => new Bool(!ToBool().Value);

    public virtual Obj BAnd(Obj other)
    {
        if (TryMethod("__band__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BAnd(other) : throw new Error($"unsupported operand type(s) for &: '{Type}' and '{other.Type}'");
    }

    public virtual Obj BOr(Obj other)
    {
        if (TryMethod("__bor__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BOr(other) : throw new Error($"unsupported operand type(s) for |: '{Type}' and '{other.Type}'");
    }

    public virtual Obj BXor(Obj other)
    {
        if (TryMethod("__bxor__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BXor(other) : throw new Error($"unsupported operand type(s) for ^: '{Type}' and '{other.Type}'");
    }

    public virtual Obj BNot()
    {
        if (TryMethod("__bnot__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.BNot() : throw new Error($"unsupported operand type(s) for ~: '{Type}'");
    }

    public virtual Obj LShift(Obj other)
    {
        if (TryMethod("__lsh__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.LShift(other) : throw new Error($"unsupported operand type(s) for <<: '{Type}' and '{other.Type}'");
    }

    public virtual Obj RShift(Obj other)
    {
        if (TryMethod("__rsh__", out Obj? value, other))
            return value;
        return Super is not null && !Super.IsNone() ? Super.RShift(other) : throw new Error($"unsupported operand type(s) for >>: '{Type}' and '{other.Type}'");
    }

    public virtual Bool Eq(Obj other)
    {
        if (TryMethod("__eq__", out Obj? value, other))
            return value.ToBool();
        return Super is not null && !Super.IsNone() ? Super.Eq(other) : throw new Error($"unsupported operand type(s) for ==: '{Type}' and '{other.Type}'");
    }

    public virtual Bool NEq(Obj other) => new(!Eq(other).Value);

    public virtual Bool Lt(Obj other)
    {
        if (TryMethod("__lt__", out Obj? value))
            return value.Call([this, other]).ToBool();
        return Super is not null && !Super.IsNone() ? Super.Lt(other) : throw new Error($"unsupported operand type(s) for <: '{Type}' and '{other.Type}'");
    }

    public virtual Bool Gt(Obj other) => new(!other.Lt(other).Value && !other.Eq(other).Value);

    public virtual Bool LtOrEq(Obj other) => new(other.Lt(other).Value || other.Eq(other).Value);

    public virtual Bool GtOrEq(Obj other) => new(!other.Lt(other).Value);

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
        if (TryMethod("__call__", out Obj? value))
            return value.Call([this, .. args]);
        return Super is not null && !Super.IsNone() ? Super.Call(args) : throw new Error($"unsupported operand type(s) for (): '{Type}'");
    }

    public virtual Obj Len()
    {
        if (TryMethod("__len__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Len() : throw new Error("unsupported operand type(s) for len()");
    }

    public virtual Int Hash()
    {
        if (TryMethod("__hash__", out Obj? value))
            return value.ToInt();
        return Super is not null && !Super.IsNone() ? Super.Hash() : throw new Error("Cannot hashable object");
    }

    public virtual void SetAttr(string name, Obj value)
    {
        if (TryMethod("__setattr__", out _, new Str(name), value))
            return;
        if (Super is not null && !Super.IsNone())
            Super.SetAttr(name, value);

        Members[name] = value;
    }

    public virtual Obj GetAttr(string name)
    {
        if (TryMethod("__getattr__", out Obj? value, new Str(name))) {}
        else if (Global.TryGetOriginalValue(Type, name, out value)) {}
        else if (Members.TryGetValue(name, out value)) {}
        else if (Super is not null && !Super.IsNone())
            value = Super.GetAttr(name);
        else throw new Error($"'{Type}' object has no attribute '{name}'");

        value.Self = this;
        return value;
    }

    public virtual void SetItem(Obj key, Obj value)
    {
        if (TryMethod("__setitem__", out _, key, value))
            return;
        else if (Super is not null && !Super.IsNone())
            Super.SetItem(key, value);
        else
            throw new Error($"unsupported operand type(s) for [] = 'value': '{Type}'");
    }

    public virtual Obj GetItem(Obj key)
    {
        if (TryMethod("__getitem__", out Obj? value, key))
            return value;
        return Super is not null && !Super.IsNone() ? Super.GetItem(key) : throw new Error($"unsupported operand type(s) for []: '{Type}'");
    }

    public virtual Obj Pos()
    {
        if (TryMethod("__pos__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Pos() : throw new Error("Cannot positiveable object");
    }

    public virtual Obj Neg()
    {
        if (TryMethod("__neg__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Neg() : throw new Error("Cannot negativeable object");
    }

    public virtual Obj Spread()
    {
        if (TryMethod("__spread__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Spread() : throw new Error("Cannot spreadable object");
    }

    public virtual Obj Is(Obj obj)
    {
        if (TryMethod("__is__", out Obj? value, obj))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Is(obj) : new Bool(Type == obj.Type);
    }

    public virtual Obj In(Obj obj)
    {
        if (TryMethod("__in__", out Obj? value, obj))
            return value;
        return Super is not null && !Super.IsNone() ? Super.In(obj) : throw new Error("Cannot inable object");
    }

    public virtual Int ToInt()
    {
        if (TryMethod("__int__", out Obj? value))
            return value.ToInt();
        return Super is not null && !Super.IsNone() ? Super.ToInt() : throw new Error("Cannot intable object");
    }

    public virtual Float ToFloat()
    {
        if (TryMethod("__tuple__", out Obj? value))
            return value.ToFloat();
        return Super is not null && !Super.IsNone() ? Super.ToFloat() : throw new Error("Cannot floatable object");
    }

    public virtual Str ToStr()
    {
        if (TryMethod("__str__", out Obj? value))
            return value.ToStr();
        return Super is not null && !Super.IsNone() ? Super.ToStr() : new Str($"{Type}");
    }

    public virtual Bool ToBool()
    {
        if (TryMethod("__bool__", out Obj? value))
            return value.ToBool();
        return Super is not null && !Super.IsNone() ? Super.ToBool() : throw new Error("Cannot boolable object");
    }

    public virtual List ToList()
    {
        if (TryMethod("__list__", out Obj? value))
            return value.ToList();
        return Super is not null && !Super.IsNone() ? Super.ToList() : throw new Error("Cannot listable object");
    }

    public virtual Tup ToTuple()
    {
        if (TryMethod("__tuple__", out Obj? value))
            return value.ToTuple();
        return Super is not null && !Super.IsNone() ? Super.ToTuple() : throw new Error("Cannot tupleable object");
    }

    public virtual Obj Entry()
    {
        if (TryMethod("__entry__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Entry() : throw new Error("Cannot entryable object");
    }

    public virtual Obj Exit()
    {
        if (TryMethod("__exit__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Exit() : throw new Error("Cannot exitable object");
    }

    public virtual Iters Iter()
    {
        if (TryMethod("__iter__", out Obj? value))
            return value.Iter();
        return Super is not null && !Super.IsNone() ? Super.Iter() : throw new Error("Cannot iterable object");
    }

    public virtual Obj Next()
    {
        if (TryMethod("__next__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Next() : throw new Error("Cannot iterable object");
    }

    public virtual Obj Copy()
    {
        if (TryMethod("__copy__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Copy() : this;
    }

    public virtual Obj Clone()
    {
        if (TryMethod("__clone__", out Obj? value))
            return value;
        return Super is not null && !Super.IsNone() ? Super.Clone() : throw new Error("Cannot cloneable object");
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

        throw new Error($"cannot cast {Type} to {typeof(T).Name}");
    }

    public Obj As<T, U>() where T : Obj where U : Obj
    {
        if (this is T obj1)
            return obj1;
        if (this is U obj2)
            return obj2;

        throw new Error($"cannot cast {Type} to {typeof(T).Name} or {typeof(U).Name}");
    }

    public T As<T>(string message) where T : Obj
    {
        if (this is T obj)
            return obj;

        throw new Error(message);
    }

    private bool TryMethod(string name, out Obj value, params Obj[] args)
    {
        if (Members.TryGetValue(name, out Obj? method))
        {
            value = method.Call([this, .. args]);
            return true;
        }
        value = null!;
        return false;
    }

    public bool IsNone() => Type == "none";

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
        Obj o => Eq(o).Value,
        _ => false,
    };
    
    public int CompareTo(Obj? other)
    {
        if (other == null) return 0;
        if (Eq(other).Value) return 0;
        if (Lt(other).Value) return -1;
        if (Gt(other).Value) return 1;
        throw new Error("types that are not comparable to each other.");
    }
}
