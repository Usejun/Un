using Un.Function;
using Un.Object;

namespace Un.Package
{
    public class Pack(string packageName) : Obj
    { 
        protected string packageName = packageName;

        public virtual IEnumerable<Fun> Import() => [];
    }
}
