namespace Un;

public abstract class BaseError(string message) : Exception(message)
{
    public override string ToString()
    {
        StringBuffer result = new();

        Process.TryGetGlobalProperty(Literals.Name, out var v);

        result.AppendLine();
        result.AppendLine($"   File <{v.CStr().Value}>, line [{Process.Line + 1}]");
        result.AppendLine($"      {Process.Code[Process.Line].Trim()}");
        result.AppendLine($"      {new string('^', Process.Code[Process.Line].Trim().Length)}");
        result.AppendLine($"{GetType().Name} : {Message}");
        result.AppendLine();
        result.AppendLine($"   Call Stack [{Process.CallStack.Count}]");

        while (Process.CallStack.TryPop(out var info))
            result.AppendLine($"      line [{info.Line}] : {Process.Code[info.Line].Trim()}");

        return result.ToString();
    }
}

public class AssertError : BaseError
{
    public AssertError() : base("assert exception") { }
    public AssertError(string message) : base(message) { }
}

public class IndexError : BaseError
{
    public IndexError() : base("invalid index") { }
    public IndexError(string message) : base(message) { }
}

public class KeyError : BaseError
{
    public KeyError() : base("invalid key") { }
    public KeyError(string message) : base(message) { }
}

public class ValueError : BaseError
{
    public ValueError() : base("invalid value") { }
    public ValueError(string message) : base(message) { }
}

public class ClassError : BaseError
{
    public ClassError() : base("invalid class") { }
    public ClassError(string message) : base(message) { }
}

public class NoneError : BaseError
{
    public NoneError() : base("invalid none") { }
    public NoneError(string message) : base(message) { }

    public static void IsNull(object obj)
    {
        if (obj is null) throw new NoneError();
    }

    public static void IsNull(object obj, string message)
    {
        if (obj is null) throw new NoneError(message);
    }
}

public class FileError : BaseError
{
    public FileError() : base("invalid file") { }
    public FileError(string message) : base(message) { }
}

public class TypeError : BaseError
{
    public TypeError() : base("invalid type") { }
    public TypeError(string message) : base(message) { }
}

public class SyntaxError : BaseError
{
    public SyntaxError() : base("invalid syntax") { }
    public SyntaxError(string message) : base(message) { }
}

public class OperatorError : BaseError
{
    public OperatorError() : base("invalid operator") { }
    public OperatorError(string message) : base(message) { }
}

public class ArgumentError : BaseError
{ 
    public ArgumentError() : base("invalid argumennt") { }
    public ArgumentError(string message) : base(message) { }
}

public class PropertyError : BaseError
{
    public PropertyError() : base("occur error in property") { }
    public PropertyError(string message) : base(message) { }
}

public class DivideByZeroError : BaseError
{
    public DivideByZeroError() : base("divide by zero") { }
    public DivideByZeroError(string message) : base(message) { }
}