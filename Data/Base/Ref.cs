namespace Un.Data;

public abstract class Ref<T>(string type, T value) : Obj(type)
{
    public T Value { get; protected set; } = value;

    public override Str CStr() => new($"{Value}");

    public override List CList() => new([Copy()]);

    public override int GetHashCode() => Value is null ? 0 : Value.GetHashCode();

    public override Obj Copy() => this;
}
