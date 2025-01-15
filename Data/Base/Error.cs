namespace Un.Data;

public class Error : Ref<Exception>
{
    public Error() : base("error", new NoneError())
    {
    }

    public Error(Exception value) : base(value.GetType().Name, value)
    {
    }

    public override void Init()
    {
        field.Set("message", new NativeFun("message", 0, (args, field) =>
        {
            if (!field[Literals.Self].Is(Literals.Error))
                throw new ArgumentError();

            return new Str(Value.Message);
        }));
    }

    public override Str CStr()
    {
        string result = "";

        Process.TryGetGlobalProperty(Literals.Name, out var v);

        result += "\n";
        result += $"   File <{v.CStr().Value}>, line [{Process.Line + 1}]\n";
        result += $"      {Process.Code[Process.Line].Trim()}\n";
        result += $"      {new string('^', Process.Code[Process.Line].Trim().Length)}\n";
        result += $"{ClassName} : {Value.Message}";

        return new(result);
    }

    public static List<(string, NativeFun)> Import() =>
    [
        (typeof(IndexError).Name, Make<IndexError>()),
        (typeof(KeyError).Name, Make<KeyError>()),
        (typeof(ValueError).Name, Make<ValueError>()),
        (typeof(ClassError).Name, Make<ClassError>()),
        (typeof(NoneError).Name, Make<NoneError>()),
        (typeof(FileError).Name, Make<FileError>()),
        (typeof(TypeError).Name, Make<TypeError>()),
        (typeof(SyntaxError).Name, Make<SyntaxError>()),
        (typeof(OperatorError).Name, Make<OperatorError>()),
        (typeof(ArgumentError).Name, Make<ArgumentError>()),
        (typeof(DivideByZeroError).Name, Make<DivideByZeroError>()),
    ]; 

    private static NativeFun Make<T>()
        where T : Exception, new()
    => new(nameof(T), -1, (args, field) =>
    {
        if (args.Count == 0)
            return new Error(new T());
        else if (args.Count == 1)
            return new Error((T)Activator.CreateInstance(typeof(T), args[0].CStr().Value));
        throw new ArgumentError();
    });
    
}
