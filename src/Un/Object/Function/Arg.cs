namespace Un.Object.Function;

public class Arg(string name) : Obj
{
    public string Name { get; set; } = name;
    public new string Type { get; set; } = "any";

    public bool IsEssential { get; set; }
    public bool IsOptional { get; set; }
    public bool IsPositional { get; set; }
    public bool IsKeyword { get; set; }

    public Obj? DefaultValue { get; set; }
}
