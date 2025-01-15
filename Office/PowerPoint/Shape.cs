using Microsoft.Office.Interop.PowerPoint;

namespace Un.Office.PowerPoint;

public class Shape : Ref<Microsoft.Office.Interop.PowerPoint.Shape>
{
    public Shape() : base("shape", null) { }
    
    public Shape(Microsoft.Office.Interop.PowerPoint.Shape shape) : base("shape", shape) { }

    public override void Init()
    {
        field.Set("height", new NativeFun("height", -1, args =>
        {
            if (args[0] is not Shape self)
                throw new ValueError("invalid arguments");            
            if (args.Count == 2)
            {
                if (args[1] is Int i) self.Value.Height = i.Value;
                else if (args[1] is Float f) self.Value.Height = (float)f.Value;
                else throw new ValueError("invalid arguments");
            }            

            return new Float(self.Value.Height);
        }));
        field.Set("width", new NativeFun("width", -1, args =>
        {
            if (args[0] is not Shape self)
                throw new ValueError("invalid arguments");
            if (args.Count == 2)
            {
                if (args[1] is Int i) self.Value.Width = i.Value;
                else if (args[1] is Float f) self.Value.Width = (float)f.Value;
                else throw new ValueError("invalid arguments");
            }

            return new Float(self.Value.Width);
        }));
        field.Set("visible", new NativeFun("visible", 2, args =>
        {
            if (args[0] is not Shape self || args[1] is not Bool condition)
                throw new ValueError("invalid arguments");            

            self.Value.Visible = condition.Value ? Microsoft.Office.Core.MsoTriState.msoTrue : Microsoft.Office.Core.MsoTriState.msoFalse;

            return None;
        }));
    }

    public override Obj Copy() => this;

    public override Obj Clone() => new Shape(Value);
}
