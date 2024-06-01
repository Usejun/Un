namespace Un;

public class Token
{
    public static Token None => new("None", Type.None);

    public static Dictionary<string, Type> Types = [];

    public static List<string> Union = [];

    public static Dictionary<Type, int> Priority = new()
    {
        {Type.LParen, -1},

        {Type.Indexer, 1}, {Type.Slicer, 1}, {Type.Function, 1}, {Type.Property, 1}, {Type.Method, 1},

        {Type.DoubleAsterisk, 2},

        {Type.BNot, 3},

        {Type.Asterisk, 4}, {Type.Slash, 4}, {Type.DoubleSlash, 4}, {Type.Percent, 4},

        {Type.Plus, 5}, {Type.Minus, 5},

        {Type.LeftShift, 6}, {Type.RightShift, 6},

        {Type.BAnd, 7},

        {Type.BXor, 8},

        {Type.BOr, 9},

        {Type.In, 10}, {Type.Is, 10}, {Type.Equal, 10}, {Type.Unequal, 10}, {Type.LessThen, 10}, {Type.LessOrEqual, 10},
        {Type.GreaterThen, 10}, {Type.GreaterOrEqual, 10},

        {Type.Assign, 11}, {Type.PlusAssign, 11}, {Type.MinusAssign, 11}, {Type.AsteriskAssign, 11}, {Type.SlashAssign, 11},
        {Type.DoubleSlashAssign, 11}, {Type.DoubleAsteriskAssign, 11}, {Type.PercentAssign, 11}, {Type.BAndAssign, 11},
        {Type.BOrAssign, 11}, {Type.BXorAssign, 11}, {Type.LeftShiftAssign, 11}, {Type.RightShiftAssign, 11},

        {Type.Not, 12},

        {Type.And, 13},

        {Type.Xor, 14},

        {Type.Or, 15},

        {Type.RParen, 16},
    };

    public enum Type
    {
        None,

        RParen, 

        Assign, PlusAssign, MinusAssign,
        AsteriskAssign, DoubleAsteriskAssign,
        SlashAssign, DoubleSlashAssign,
        PercentAssign,
        BAndAssign, BOrAssign, BXorAssign, 
        LeftShiftAssign, RightShiftAssign,

        Or, And, Xor, Not,

        In, Is,

        Equal, Unequal,
        LessOrEqual, GreaterOrEqual,
        LessThen, GreaterThen,

        BOr, BXor, BAnd,

        LeftShift, RightShift,

        Plus, Minus,

        Asterisk, 
        Slash, DoubleSlash,
        Percent,

        BNot, Bang,

        DoubleAsterisk,

        Indexer, Slicer,
        Property, Method,
        Function,

        LParen,
        LBrace, RBrace,
        LBrack, RBrack,

        Dot, Comma,Colon,

        Func, Class, Enum,
        True, False,
        If, ElIf, Else,
        For, While,
        Break, Continue, Return,
        Import, Using,
        Comment,

        Variable,

        Integer, Float, String, Boolean, Iterator, Dictionary, Lambda,
    }

    public Type type;
    public string value;

    public Token(char c)
    {
        value = $"{c}";
        type = GetType(value);
    }

    public Token(string s)
    {
        value = s;
        type = GetType(s);
    }

    public Token(Obj obj)
    {
        value = obj.CStr().value;
        type = GetType(obj);
    }

    public Token(string s, Type type)
    {
        value = s;
        this.type = type;
    }

    public override string ToString() => $"{type} : {value}";

    public static Type GetType(char chr) => GetType($"{chr}");

    public static Type GetType(string str)
    {
        if (Types.TryGetValue(str, out var type)) return type;
        return Type.None;
    }

    public static Type GetType(Obj obj)
    {
        if (obj is Int) return Type.Integer;
        if (obj is Float) return Type.Float;
        if (obj is Str) return Type.String;
        if (obj is Bool) return Type.Boolean;
        if (obj is Iter) return Type.Iterator;
        return Type.None;
    }


    public static bool IsOperator(Token token) => IsOperator(token.type);

    public static bool IsOperator(Type type) => type switch
    {
        > Type.None and < Type.Dot => true,
        _ => false
    };

    public static bool IsOperator(string str) => IsOperator(GetType(str));

    public static bool IsOperator(char chr) => IsOperator(GetType(chr));


    public static bool IsBasicOperator(Token token) => IsBasicOperator(token.type);

    public static bool IsBasicOperator(Type type) => type switch
    {
        > Type.None and < Type.Indexer => true,
        > Type.Function and < Type.Dot => true,
        _ => false
    };

    public static bool IsBasicOperator(string str) => IsBasicOperator(GetType(str));

    public static bool IsBasicOperator(char chr) => IsBasicOperator(GetType(chr));


    public static bool IsLoop(Token token) => IsLoop(token.type);

    public static bool IsLoop(Type type) => type switch
    {
        Type.For or Type.While => true,
        _ => false,
    };

    public static bool IsLoop(string str) => IsLoop(GetType(str));


    public static bool IsControl(Token token) => IsControl(token.type);

    public static bool IsControl(Type type) => type switch
    {
        Type.If or Type.ElIf or Type.Else => true,
        _ => false,
    };

    public static bool IsControl(string str) => IsControl(GetType(str));


    public static bool IsComment(Token token) => token.type switch
    {
        Type.Comment => true,
        _ => false,
    };

    public static bool IsComment(Type type) => type switch
    {
        Type.Comment => true,
        _ => false,
    };

    public static bool IsComment(string str) => IsComment(GetType(str));

    public static bool IsComment(char chr) => IsComment(GetType(chr));


    public static bool IsAssigns(Token token) => IsAssigns(token.type);

    public static bool IsAssigns(Type type) => type switch
    {
        >= Type.Assign and <= Type.RightShiftAssign => true,
        _ => false
    };


    public static bool IsSymbol(Token token) => IsSymbol(token.type);

    public static bool IsSymbol(Type type) => type switch
    {
        Type.Comma or Type.Dot or Type.Colon => true,
        _ => false,
    };

    public static bool IsSymbol(char chr) => IsSymbol(GetType(chr));


    public static bool IsLiteral(Token token) => IsLiteral(token.type);

    public static bool IsLiteral(Type type) => type switch
    {
        >= Type.Integer and <= Type.Lambda => true,
        _ => false
    };   
    
    
    public static bool IsString(char chr) => chr switch
    {
        '\'' or '"' or '`' => true,
        _ => false 
    };


}
