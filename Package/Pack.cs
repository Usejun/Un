using Un.Object;
using Un.Object.Function;

namespace Un.Package
{
    public class Pack(string packageName) : Obj
    { 
        protected string packageName = packageName;

        public virtual IEnumerable<Fun> Import() => [];
    }
}
