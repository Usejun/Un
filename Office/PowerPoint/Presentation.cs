using Microsoft.Office.Interop.PowerPoint;

namespace Un.Office.PowerPoint;

public class Presentation : Ref<Microsoft.Office.Interop.PowerPoint.Presentation>
{
    private Application app = null;

    public Presentation() : base("powerpoint", null) { }

    public override Obj Init(Map args)
    {
        if (args.Count == 0) throw new ClassError("initialize error");
        else if (args.Count == 1)
        {
            app = new();

            if (args[0] is not Str path)
                throw new ClassError("initialize error");

            Value = app.Presentations.Open(path.Value);
        }
        else if (args.Count == 2)
        {
            app = new();

            if (args[0] is not Str path || args[1] is not Str type)
                throw new ClassError("initialize error");

            if (type.Value.Equals("c", StringComparison.CurrentCultureIgnoreCase))
                using (File.Create(path.Value)) { }

            Value = app.Presentations.Open(path.Value);
        }
        else throw new ClassError("initialize error");

        return this;
    }

    public override void Init()
    {
        field.Set("slides", new NativeFun("slides", 1, args =>
        {
            if (args[0] is not Presentation self)
                throw new ValueError("invaild arguments");

            return new Slides(self.Value.Slides);
        }));
        field.Set("insert_file", new NativeFun("insert_file", 3, args =>
        {
            if (args[0] is not Presentation self || args[1] is not Int index || args[1] is not Str path)
                throw new ValueError("invaild arguments");

            Value.Slides.InsertFromFile(path.Value, index.Value < 0 ? (int)(Value.Slides.Count + index.Value + 1) : (int)index.Value);

            return this;
        }));
    }

    public override Obj Entry()
    {
        return None;
    }

    public override Obj Exit()
    {
        System.Threading.Tasks.Task.Run(Value.Save);
        Value.Close();
        app.Quit();

        return None;
    }

    public override Obj Copy() => this;

    public override Obj Clone() => new Presentation()
    {
        Value = Value,
    };
}
