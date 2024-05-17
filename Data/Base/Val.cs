namespace Un.Data;

public abstract class Val<T>(string type, T value) : Obj(type)
    where T : IComparable<T>
{
    public T value = value;

    public override Str CStr() => new($"{value}");

    public override Iter CIter() => new([Clone()]);

    public override Bool Equals(Obj obj) => new(obj is Val<T> v && value.CompareTo(v.value) == 0);

    public override Bool LessThen(Obj obj) => new(obj is Val<T> v && value.CompareTo(v.value) < 0);

    public override int GetHashCode() => value.GetHashCode();
}
