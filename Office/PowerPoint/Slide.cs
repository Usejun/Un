namespace Un.Office.PowerPoint;

public class Slide : Ref<Microsoft.Office.Interop.PowerPoint.Slide>
{
    public Slide() : base("slide", null) { }

    public Slide(Microsoft.Office.Interop.PowerPoint.Slide slide) : base("slide", slide) { }

    public override void Init()
    {
        field.Set("", new NativeFun("", 1, args =>
        {
            if (args[0] is not Slide self)
                throw new ValueError("invalid arguments");          

            return None;
        }));
    }

    public override Obj Copy() => this;

    public override Obj Clone() => new Slide(Value);
}
