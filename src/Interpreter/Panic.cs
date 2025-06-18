namespace Un;

public class Panic(string message) : Exception(message)
{
    public override string ToString() =>
$"""

{GetType().Name} : {Message}
""";
}
