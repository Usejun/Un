using Un.Object;

namespace Un;

public class Err(string message) : Obj
{
    public string Message => message;
}