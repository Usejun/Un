namespace Un;

public class Error(string message, Context context, string header = "Error") : Exception(message)
{
    public string Header { get; } = header;
    private string code = context.File.Code[context.File.Line].code.Trim();

    public override string ToString() =>
$"""
    <{context.File.Name}>, line [{context.File.Index}] 
        {code}
        {new string('^', code.Length)}
{header} : {Message}
""";
}
