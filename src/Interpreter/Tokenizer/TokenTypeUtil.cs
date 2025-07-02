namespace Un;

public static class TokenTypeUtil
{
    public static bool IsOperator(this TokenType type) => type switch
    {
        // 산술 연산자
        TokenType.Plus or TokenType.Minus or TokenType.Asterisk or TokenType.Slash or TokenType.Percent or
        TokenType.DoubleAsterisk or TokenType.DoubleSlash => true,

        // 비트 연산자
        TokenType.BAnd or TokenType.BOr or TokenType.BXor or TokenType.BNot or TokenType.LeftShift or TokenType.RightShift => true,

        // 비교 연산자
        TokenType.Equal or TokenType.Unequal or TokenType.LessOrEqual or TokenType.GreaterOrEqual or TokenType.LessThan or
        TokenType.GreaterThan => true,

        // 논리 연산자
        TokenType.And or TokenType.Or or TokenType.Xor or TokenType.Not => true,

        // 대입 연산자
        TokenType.Assign or TokenType.PlusAssign or TokenType.MinusAssign or TokenType.AsteriskAssign or
        TokenType.SlashAssign or TokenType.PercentAssign or TokenType.BAndAssign or TokenType.BOrAssign or
        TokenType.BXorAssign or TokenType.QuestionAssign or TokenType.DoubleQuestionAssign or
        TokenType.RightShiftAssign or TokenType.LeftShiftAssign => true,

        // 기타 연산자 
        TokenType.DoubleQuestion or TokenType.Question or TokenType.Indexer or TokenType.Slicer or TokenType.Spread or
        TokenType.Positive or TokenType.Negative or TokenType.QuestionDot or TokenType.In or TokenType.Is or TokenType.Go or
        TokenType.Wait or TokenType.Call => true,
        _ => false,
    };

    public static bool IsBinaryOperator(this TokenType type) => type switch
    {
        TokenType.Plus or TokenType.Minus or TokenType.Asterisk or TokenType.Slash or TokenType.Percent or
        TokenType.BAnd or TokenType.BOr or TokenType.BXor or TokenType.DoubleAsterisk or TokenType.DoubleSlash or
        TokenType.LeftShift or TokenType.RightShift or TokenType.And or TokenType.Or or TokenType.Xor => true,

        TokenType.Equal or TokenType.Unequal or TokenType.LessOrEqual or TokenType.GreaterOrEqual => true,

        TokenType.LessThan or TokenType.GreaterThan or TokenType.In or TokenType.Is => true,

        _ => false
    };

    public static bool IsUnary(this TokenType type) => type switch
    {
        TokenType.Plus or TokenType.Minus or TokenType.BNot or TokenType.Not or TokenType.Indexer or TokenType.Slicer or TokenType.Asterisk => true,
        _ => false
    };

    public static bool IsUnaryOperator(this TokenType type) => type switch
    {
        TokenType.Positive or TokenType.Negative or TokenType.BNot or TokenType.Not or TokenType.Indexer or TokenType.Slicer or TokenType.Spread => true,
        _ => false
    };

    public static bool IsAssignmentOperator(this TokenType type) => type switch
    {
        TokenType.Assign or TokenType.PlusAssign or TokenType.MinusAssign or TokenType.AsteriskAssign or
        TokenType.SlashAssign or TokenType.PercentAssign or TokenType.DoubleAsteriskAssign or
        TokenType.BAndAssign or TokenType.BOrAssign or TokenType.BXorAssign or
        TokenType.LeftShiftAssign or TokenType.RightShiftAssign => true,
        _ => false
    };

    public static bool IsIdentifier(this TokenType type) => type switch
    {
        TokenType.Identifier or TokenType.Indexer or TokenType.Slicer or TokenType.Property or TokenType.NullableProperty or TokenType.Call or
        TokenType.Integer or TokenType.Float or TokenType.String or TokenType.FString or TokenType.Boolean or TokenType.List or TokenType.Dict or
        TokenType.Set => true,
        _ => false
    };

    public static bool IsLiteral(this TokenType type) => type switch
    {
        TokenType.Integer or TokenType.Float or TokenType.String or TokenType.FString or TokenType.Boolean or
        TokenType.List or TokenType.Tuple or TokenType.Dict or TokenType.Set => true,
        _ => false
    };

    public static bool IsLeftBracket(this TokenType type) => type switch
    {
        TokenType.LParen or TokenType.LBrace or TokenType.LBrack => true,
        _ => false
    };

    public static bool IsRightBracket(this TokenType type) => type switch
    {
        TokenType.RParen or TokenType.RBrace or TokenType.RBrack => true,
        _ => false
    };

    public static bool IsShortCircuitOperator(this TokenType type) => type switch
    {
        TokenType.And or TokenType.Or or TokenType.Xor => true,
        _ => false,
    };

    public static TokenType GetCloser(this TokenType type) => type switch
    {
        TokenType.LParen => TokenType.RParen,
        TokenType.LBrace => TokenType.RBrace,
        TokenType.LBrack => TokenType.RBrack,
        _ => throw new Panic("invalid token type for closer")
    };

    public static TokenType GetOpener(this TokenType type) => type switch
    {
        TokenType.RParen => TokenType.LParen,
        TokenType.RBrace => TokenType.LBrace,
        TokenType.RBrack => TokenType.LBrack,
        _ => throw new Panic("invalid token type for open")
    };

    public static int GetPrecedence(this TokenType type) => type switch
    {
        TokenType.LParen => -1,
        TokenType.Indexer or TokenType.Slicer or TokenType.NullableProperty or TokenType.Property => 0,
        TokenType.Go or TokenType.Wait => 1,
        TokenType.Call => 2,
        TokenType.BNot or TokenType.Positive or TokenType.Negative or TokenType.Spread => 3,
        TokenType.Asterisk or TokenType.Slash or TokenType.DoubleSlash or TokenType.Percent => 4,
        TokenType.Plus or TokenType.Minus => 5,
        TokenType.LeftShift or TokenType.RightShift => 6,
        TokenType.BAnd => 7,
        TokenType.BXor => 8,
        TokenType.BOr => 9,
        TokenType.DoubleAsterisk => 10,

        TokenType.In or TokenType.Is or TokenType.Equal or TokenType.Unequal or TokenType.LessThan or TokenType.LessOrEqual or
        TokenType.GreaterThan or TokenType.GreaterOrEqual => 11,

        TokenType.Assign or TokenType.PlusAssign or TokenType.MinusAssign or TokenType.AsteriskAssign or TokenType.SlashAssign or
        TokenType.DoubleSlashAssign or TokenType.DoubleAsteriskAssign or TokenType.PercentAssign or TokenType.BAndAssign or
        TokenType.BOrAssign or TokenType.BXorAssign or TokenType.LeftShiftAssign or TokenType.RightShiftAssign => 12,

        TokenType.Not => 13,

        TokenType.And => 14,

        TokenType.Xor => 15,

        TokenType.Or => 16,

        TokenType.DoubleQuestion => 17,

        TokenType.RParen => 18,
        _ => 0,
    };
}