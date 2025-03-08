namespace Un;

public class CallInfo(string name, int line, Field variable)
{
    public string Name => name;
    public int Line => line;
    public Field Variable => variable;
}
