using System.Text;

namespace Un.Data
{
    public class Object : Obj
    {
        public Object() : base("obj") { }

        public override Obj Init(List args)
        {
            return this;
        }

        public override Obj Get(string str)
        {
            if (field.Get(str, out var v)) return v;
            else return None;
        }

        public override void Set(string str, Obj Value)
        {
            field.Set(str, Value);
        }

        public override Str CStr()
        {
            StringBuilder sb = new();

            ToStr(this, 0);

            return new($"{sb}");

            void ToStr(Object obj, int depth)
            {
                sb.Append(new string(' ', 4*depth));
                sb.AppendLine("{");

                foreach (var key in obj.field.Keys)
                {
                    sb.Append(new string(' ', 4 * (depth + 1)));
                    sb.Append($"{key}:{obj.field[key].Type().Value}, ");

                    if (obj.field[key] is Object o)
                    {
                        sb.AppendLine();
                        ToStr(o, 1);                        
                    }
                    else sb.AppendLine($"{obj.field[key].CStr().Value}");
                }

                sb.Append(new string(' ', 4 * depth));
                sb.AppendLine("}");
            }
        }

        public override Obj Clone() => new Object() { field = new(field) };

        public override Obj Copy() => this;
    }
}
