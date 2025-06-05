using Un.Object.Primitive;

namespace Un.Object;

public class Enu(string type) : Obj(type)
{
    public List<Str> Constants { get; set; }
}
