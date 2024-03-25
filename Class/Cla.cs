using Un.Object;

namespace Un.Class
{
    public class Cla(string className) : Obj
    {
        public string className = className;

        public Dictionary<string, Obj> Properties = [];
        public Dictionary<string, Func<Obj>> Methodes = [];
    }
}
