using System.Text;

namespace Un.Data
{
    public class Object : Obj
    {
        public Object() : base("obj") { }

        public override Obj Init(Iter args)
        {
            return this;
        }

        public override Obj Get(string str)
        {
            if (field.Get(str, out var v)) return v;
            else return None;
        }

        public override void Set(string str, Obj value)
        {
            field.Set(str, value);
        }

        public override Str CStr()
        {
            StringBuilder sb = new();

            sb.AppendLine("{");

            foreach (var key in field.Keys)
                sb.AppendLine($"    {key} = {field[key].CStr().value} : {field[key].Type().value},");            
            sb.AppendLine("}");

            return new($"{sb}");
        }

        public override Obj Clone() => new Object() { field = new(field) };

        public override Obj Copy() => this;
    }
}
