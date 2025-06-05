namespace Un;

public class Token(string value, TokenType type)
{
    public static Token Error = new("error", TokenType.Error);
    public static Token None = new("none", TokenType.None);

    public string Value = value;
    public TokenType Type = type; 

    public override string ToString() => $"Token: {Value}, Type: {Type}";
}