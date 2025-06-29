namespace Un;

public class Error(string message, Context context, string header = "Error") : Exception(message)
{
    public override string ToString() =>
$"""

    <{context.File.Name}>, line [{context.File.Index}] 
        {context.File.Code[context.File.Line].code}
        {new string('^', context.File.Code[context.File.Line].code.Length)}
{header} : {Message}
""";
}
