namespace Un;

public class Error(string message) : Exception(message)
{
    public override string ToString() =>
$"""

    <{Global.File.Name}>, line [{Global.File.Index}] 
        {Global.File.Code[Global.File.Line].code}
        {new string('^', Global.File.Code.Count)}
{GetType().Name} : {Message}
""";
}