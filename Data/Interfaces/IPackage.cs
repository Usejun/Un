namespace Un.Data;

public interface IPackage
{
    public string Name { get; }    

    public virtual IEnumerable<Fun> Import() => [];

    public virtual IEnumerable<Obj> Include() => [];
}
