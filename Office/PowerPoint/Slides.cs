namespace Un.Office.PowerPoint;

public class Slides : Ref<Microsoft.Office.Interop.PowerPoint.Slides>
{
    public Slides() : base("slides", null) { }

    public Slides(Microsoft.Office.Interop.PowerPoint.Slides slides) : base("slides", slides) { }

    public override Obj Copy() => this;

    public override Obj Clone() => new Slides(Value);
}
