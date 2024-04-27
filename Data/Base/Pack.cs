

namespace Un.Data
{
    public class Pack(string packageName) : Obj
    {
        protected string packageName = packageName;

        public virtual IEnumerable<Fun> Import() => [];

        public virtual IEnumerable<Obj> Include() => [];
    }
}
