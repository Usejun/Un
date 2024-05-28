using System.Text;

namespace Un;

public abstract class Error : Exception
{
    public Error(string message) : base(message) { }

    public override string ToString()
    {
        StringBuilder result = new();

        result.AppendLine();
        result.AppendLine($"   File <{Process.Public["__name__"].CStr().value}>, line [{Process.Line + 1}]");
        result.AppendLine($"      {Process.Code[Process.Line].Trim()}");
        result.AppendLine($"      {new string('^', Process.Code[Process.Line].Trim().Length)}");
        result.Append    ($"{GetType().Name} : {Message}");

        return result.ToString();
    }
}

public class AssertError : Error
{
    public AssertError() : base("assert exception") { }
    public AssertError(string message) : base(message) { }
}

public class IndexError : Error
{
    public IndexError() : base("invalid index") { }
    public IndexError(string message) : base(message) { }
}

public class KeyError : Error
{
    public KeyError() : base("invalid key") { }
    public KeyError(string message) : base(message) { }
}

public class ValueError : Error
{
    public ValueError() : base("invalid value") { }
    public ValueError(string message) : base(message) { }
}

public class ClassError : Error
{
    public ClassError() : base("invalid class") { }
    public ClassError(string message) : base(message) { }
}

public class NoneError : Error
{
    public NoneError() : base("invalid None") { }
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

public class FileError : Error
{
    public FileError() : base("invalid file") { }
    public FileError(string message) : base(message) { }
}

public class TypeError : Error
{
    public TypeError() : base("invalid type") { }
    public TypeError(string message) : base(message) { }
}

public class SyntaxError : Error
{
    public SyntaxError() : base("invalid syntax") { }
    public SyntaxError(string message) : base(message) { }
}

public class OperatorError : Error
{
    public OperatorError() : base("invalid operator") { }
    public OperatorError(string message) : base(message) { }
}