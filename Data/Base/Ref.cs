﻿namespace Un.Data;

public abstract class Ref<T>(string type, T value) : Obj(type)
{
    public T value = value;

    public override Str CStr() => new($"{value}");

    public override Iter CIter() => new([Copy()]);

    public override int GetHashCode() => value is null ? 0 : value.GetHashCode();

    public override Obj Copy() => this;
}
