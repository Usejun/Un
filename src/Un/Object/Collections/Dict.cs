using Un.Object.Primitive;
using Un.Object.Function;

namespace Un.Object.Collections;

public class Dict(Dictionary<Obj, Obj> value) : Ref<Dictionary<Obj, Obj>>(value, "dict")
{
    public Dict() : this([]) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Dict(),
        _ => throw new Error($"invaild '{Type}' initialize"),
    };

    public override Obj Len() => new Int(Value.Count);

    public override Obj GetItem(Obj key) => Value.TryGetValue(key, out var value) ? value : throw new Error($"key '{key.ToStr().Value}' not found in dictionary");

    public override void SetItem(Obj key, Obj value)
    {
        Value[key] = value;
    }

    public override Obj In(Obj obj) => obj switch
    {
        Dict dict => new Bool(Overlap(dict)),
        _ => throw new Error($"cannot check if '{obj.Type}' is in '{Type}'"),
    };

    public override Obj Copy() => this;

    public override Obj Clone() => new Dict(new Dictionary<Obj, Obj>(Value));

    public override Str ToStr() => new($"{{{string.Join(", ", Value.Select(x => $"{x.Key.ToStr().Value}: {x.Value.ToStr().Value}"))}}}");

    public override List ToList() => new([.. Value.Keys.Zip(Value.Values).Select(x => new Tup([x.First, x.Second], ["key", "value"]))]);

    public override Tup ToTuple() => new([.. Value.Keys.Zip(Value.Values).Select(x => new Tup([x.First, x.Second], ["key", "value"]))], []);

    public override Iters Iter() => new([.. Value.Keys.Zip(Value.Values).Select(x => new Tup([x.First, x.Second], ["key", "value"]))]);

    private bool Overlap(Dict dict)
    {
        foreach (var (key, value) in dict.Value)
        {
            if (!Value.TryGetValue(key, out var v) || !v.Eq(value).Value)
                return false;
        }
        return true;
    }

    public override Attributes GetOriginal() => new()
    {
        { "add", new NFn()
            {
                Name = "add",
                Args = [
                    new Arg("key") { IsEssential = true },
                    new Arg("value") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    self.Value.Add(args["key"], args["value"]);
                    return self;
                }
            }
        },
        { "remove", new NFn()
            {
                Name = "remove",
                Args = [
                    new Arg("key") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    return new Bool(self.Value.Remove(args["key"]));
                }
            }
        },
        { "get", new NFn()
            {
                Name = "get",
                Args = [
                    new Arg("key") { IsEssential = true },
                    new Arg("default") { IsOptional = true, DefaultValue = None }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    return self.Value.TryGetValue(args["key"], out var value) ? value : args["default"];
                }
            }
        },
        { "contains_key", new NFn()
            {
                Name = "contains_key",
                Args = [
                    new Arg("key") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    return new Bool(self.Value.ContainsKey(args["key"]));
                }
            }
        },
        { "contains_value", new NFn()
            {
                Name = "contains_value",
                Args = [
                    new Arg("value") { IsEssential = true }
                ],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    return new Bool(self.Value.ContainsValue(args["value"]));
                }
            }
        },
        { "clear", new NFn()
            {
                Name = "clear",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    self.Value.Clear();
                    return Obj.None;
                }
            }
        },
        { "keys", new NFn()
            {
                Name = "keys",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    return new List([.. self.Value.Keys]);
                }
            }
        },
        { "values", new NFn()
            {
                Name = "values",
                Args = [],
                Func = (args) =>
                {
                    if (!args["self"].As<Dict>(out var self))
                        throw new Error("invalid argument");

                    return new List([.. self.Value.Values]);
                }
            }
        }
    };


}