using Un.Object.Function;
using Un.Object.Primitive;

namespace Un.Object.Collections;

public class Set(HashSet<Obj> value) : Ref<HashSet<Obj>>(value, "set")
{
    public Set() : this([]) { }

    public override Obj Init(Tup args) => new Set([..args.ToList()]);

    public override Obj Add(Obj other)
    {
        if (other is not Set otherSet)
            throw new Error($"unsupported operand type(s) for +: 'set' and '{other.Type}'");
        return new Set([.. Value.Union(otherSet.Value)]);
    }

    public override Obj Sub(Obj other)
    {
        if (other is not Set otherSet)
            throw new Error($"unsupported operand type(s) for -: 'set' and '{other.Type}'");
        return new Set([.. Value.Except(otherSet.Value)]);
    }

    public override Obj BXor(Obj other)
    {
        if (other is not Set otherSet)
            throw new Error($"unsupported operand type(s) for ^: 'set' and '{other.Type}'");
        return new Set([.. Value.Intersect(otherSet.Value)]);
    }

    public override Obj Len() => new Int(Value.Count);

    public override Obj GetItem(Obj key) => Value.TryGetValue(key, out var value) ? value : throw new Error($"key {key.ToStr().Value} not found in set");

    public override void SetItem(Obj key, Obj value)
    {
        if (Value.Contains(key))
            throw new Error($"key {key.ToStr().Value} already exists in set");
        Value.Add(key);
    }

    public override Obj Copy() => this;

    public override Obj Clone() => new Set([.. Value]);

    public override Str ToStr() => new($"{{{string.Join(", ", Value.Select(x => x.ToStr().Value))}}}");

    public override Spread Spread() => new([.. Value]);

    public override Attributes GetOriginal() => new()
    {
        { "add", new NFn
            {
                Name = "add",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    return new Bool(self.Value.Add(args["value"]));
                }
            }
        },
        { "remove", new NFn
            {
                Name = "remove",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    return new Bool(self.Value.Remove(args["value"]));
                }
            }
        },
        { "contains", new NFn
            {
                Name = "contains",
                Args = [new Arg("value") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    return new Bool(self.Value.Contains(args["value"]));
                }
            }
        },
        { "clear", new NFn
            {
                Name = "clear",
                Args = [],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    self.Value.Clear();
                    return None;
                }
            }
        },
        { "clone", new NFn
            {
                Name = "clone",
                Args = [],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    return self.Clone();
                }
            }
        },
        { "union", new NFn
            {
                Name = "union",
                Args = [new Arg("other") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    if (!args["other"].As<Set>(out var other))
                        throw new Error("invalid argument: other");
                    return self.Add(other);
                }
            }
        },
        { "intersect", new NFn
            {
                Name = "intersect",
                Args = [new Arg("other") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    if (!args["other"].As<Set>(out var other))
                        throw new Error("invalid argument: other");
                    return self.Sub(other);
                }
            }
        },
        { "difference", new NFn
            {
                Name = "difference",
                Args = [new Arg("other") { IsEssential = true }],
                Func = args =>
                {
                    if (!args["self"].As<Set>(out var self))
                        throw new Error("invalid argument: self");
                    if (!args["other"].As<Set>(out var other))
                        throw new Error("invalid argument: other");
                    return self.BXor(other);
                }
            }
        }
    };
}