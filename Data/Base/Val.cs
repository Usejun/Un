namespace Un.Data;

public abstract class Val<T>(string type, T value) : Obj(type)
    where T : IComparable<T>
{
    public T Value { get; protected set; } = value;

    public override Str CStr() => new($"{Value}");

    public override List CList() => new([Clone()]);

    public override Bool Equals(Obj obj) => new(obj is Val<T> v && Value.CompareTo(v.Value) == 0);

    public override Bool LessThen(Obj obj) => new(obj is Val<T> v && Value.CompareTo(v.Value) < 0);

    public override int GetHashCode() => Value.GetHashCode();
}
