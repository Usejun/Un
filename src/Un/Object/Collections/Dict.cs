using Un.Object.Primitive;
using Un.Object.Function;
using Un.Object.Iter;

namespace Un.Object.Collections;

public class Dict(Dictionary<Obj, Obj> value) : Ref<Dictionary<Obj, Obj>>(value, "dict")
{
    public Dict() : this([]) { }

    public override Obj Init(Tup args) => args switch
    {
        { Count: 0 } => new Dict(),
        _ => new Err($"invaild '{Type}' initialize"),
    };

    public override Int Len() => new(Value.Count);

    public override Obj GetItem(Obj key) => Value.TryGetValue(key, out var value) ? value : new Err($"key '{key.ToStr().As<Str>().Value}' not found in dictionary");

    public override void SetItem(Obj key, Obj value)
    {
        Value[key] = value;
    }

    public override Obj In(Obj obj) => obj switch
    {
        Dict dict => new Bool(Overlap(dict)),
        _ => new Err($"cannot check if '{obj.Type}' is in '{Type}'"),
    };

    public override Obj Copy() => this;

    public override Obj Clone() => new Dict(new Dictionary<Obj, Obj>(Value));

    public override Str ToStr() => new($"{{{string.Join(", ", Value.Select(x => $"{x.Key.ToStr().As<Str>().Value}: {x.Value.ToStr().As<Str>().Value}"))}}}");

    public override List ToList() => new([.. Value.Keys.Zip(Value.Values).Select(x => new Tup([x.First, x.Second], ["key", "value"]))]);

    public override Tup ToTuple() => new([.. Value.Keys.Zip(Value.Values).Select(x => new Tup([x.First, x.Second], ["key", "value"]))], []);

    public override Iters Iter() => new([.. Value.Keys.Zip(Value.Values).Select(x => new Tup([x.First, x.Second], ["key", "value"]))]);

    public override Spread Spread() => new([.. Value.Select(i => new Tup([i.Key, i.Value], ["key", "value"]))]);

    private bool Overlap(Dict dict)
    {
        foreach (var (key, value) in dict.Value)
        {
            if (!Value.TryGetValue(key, out var v) || !v.Eq(value).As<Bool>().Value)
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
                        return new Err("invalid argument");

                    self.Value.Add(args["key"], args["value"]);
                    return None;
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
                        return new Err("invalid argument");

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
                        return new Err("invalid argument");

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
                        return new Err("invalid argument");

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
                        return new Err("invalid argument");

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
                        return new Err("invalid argument");

                    self.Value.Clear();
                    return None;
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
                        return new Err("invalid argument");

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
                        return new Err("invalid argument");

                    return new List([.. self.Value.Values]);
                }
            }
        }
    };


}