using Un.Object;

namespace Un.Function
{
    public abstract class Importable : Obj
    {
        public abstract Dictionary<string, Fun> Methods();
    }
}
