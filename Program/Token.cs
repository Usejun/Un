namespace Un;

public class Token
{
    public static Token None => new(Literals.None, Type.None);

    public readonly static Dictionary<string, Type> Types = new() 
    {
        { Literals.Assign, Type.Assign },
        { Literals.PlusAssign, Type.PlusAssign },
        { Literals.MinusAssign, Type.MinusAssign },
        { Literals.AsteriskAssign, Type.AsteriskAssign },
        { Literals.DoubleAsteriskAssign, Type.DoubleAsteriskAssign },
        { Literals.SlashAssign, Type.SlashAssign },
        { Literals.DoubleSlashAssign, Type.DoubleSlashAssign },
        { Literals.PercentAssign, Type.PercentAssign },
        { Literals.BAndAssign, Type.BAndAssign },
        { Literals.BOrAssign, Type.BOrAssign },
        { Literals.BXorAssign, Type.BXorAssign },
        { Literals.LeftShiftAssign, Type.LeftShiftAssign },
        { Literals.RightShiftAssign, Type.RightShiftAssign },
        { Literals.Plus, Type.Plus },
        { Literals.Minus, Type.Minus },
        { Literals.Bang, Type.Bang },
        { Literals.Asterisk, Type.Asterisk },
        { Literals.DoubleAsterisk, Type.DoubleAsterisk },
        { Literals.Slash, Type.Slash },
        { Literals.Percent, Type.Percent },
        { Literals.Amperand, Type.BAnd },
        { Literals.Pipe, Type.BOr },
        { Literals.Caret, Type.BXor },
        { Literals.Tilde, Type.BNot },
        { Literals.And, Type.And },
        { Literals.Or, Type.Or },
        { Literals.Xor, Type.Xor },
        { Literals.Not, Type.Not },
        { Literals.LeftShift, Type.LeftShift },
        { Literals.RightShift, Type.RightShift },
        { Literals.DoubleSlash, Type.DoubleSlash },
        { Literals.LessOrEqual, Type.LessOrEqual },
        { Literals.GreaterOrEqual, Type.GreaterOrEqual },
        { Literals.LessThen, Type.LessThen },
        { Literals.GreaterThen, Type.GreaterThen },
        { Literals.Equal, Type.Equal },
        { Literals.Unequal, Type.Unequal },
        { Literals.LBrack, Type.LBrack },
        { Literals.RBrack, Type.RBrack },
        { Literals.LBrace, Type.LBrace },
        { Literals.RBrace, Type.RBrace },
        { Literals.LParen, Type.LParen },
        { Literals.RParen, Type.RParen },
        { Literals.Question, Type.Question },
        { Literals.DoubleQuestion, Type.DoubleQuestion },
        { Literals.QuestionDot, Type.QuestionDot },
        { Literals.Dot, Type.Dot },
        { Literals.Comma, Type.Comma },
        { Literals.Colon, Type.Colon },
        { Literals.Func, Type.Func },
        { Literals.True, Type.True },
        { Literals.False, Type.False },
        { Literals.If, Type.If },
        { Literals.ElIf, Type.ElIf },
        { Literals.Else, Type.Else },
        { Literals.For, Type.For },
        { Literals.While, Type.While },
        { Literals.Break, Type.Break },
        { Literals.Continue, Type.Continue },
        { Literals.Import, Type.Import },
        { Literals.In, Type.In },
        { Literals.Is, Type.Is },
        { Literals.Return, Type.Return },
        { Literals.Comment, Type.Comment },
        { Literals.Class, Type.Class },
        { Literals.Enum, Type.Enum },
        { Literals.Arrow, Type.Lambda },
        { Literals.Fn, Type.Func },
        { Literals.Using, Type.Using },
        { Literals.As, Type.As },
        { Literals.Async, Type.Async },
        { Literals.Await, Type.Await },
        { Literals.Del, Type.Del },
        { Literals.Try, Type.Try },
        { Literals.Catch, Type.Catch },
        { Literals.Fin, Type.Fin },
        { Literals.Throw, Type.Throw },
    };

    public readonly static List<string> Union = new()
    {
        Literals.PlusAssign,
        Literals.MinusAssign,
        Literals.AsteriskAssign,
        Literals.DoubleAsteriskAssign,
        Literals.SlashAssign,
        Literals.DoubleSlashAssign,
        Literals.PercentAssign,
        Literals.BAndAssign,
        Literals.BOrAssign,
        Literals.BXorAssign,
        Literals.LeftShiftAssign,
        Literals.RightShiftAssign,
        Literals.DoubleAsterisk,
        Literals.LeftShift,
        Literals.RightShift,
        Literals.DoubleSlash,
        Literals.LessOrEqual,
        Literals.GreaterOrEqual,
        Literals.Equal,
        Literals.Unequal,
        Literals.Arrow,
        Literals.QuestionDot,
        Literals.DoubleQuestion,
    };

    public static Dictionary<Type, int> Priority = new()
    {
        {Type.LParen, -1},

        {Type.Indexer, 1}, {Type.Slicer, 1}, {Type.Function, 1}, {Type.Property, 1}, {Type.Method, 1}, {Type.NullableMethod, 1}, {Type.NullableProperty, 1},

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

        {Type.DoubleQuestion, 16},

        {Type.RParen, 17},
    };

    public enum Type
    {
        None,

        RParen, 

        DoubleQuestion,

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
        Property, Method, NullableProperty, NullableMethod,
        Function, Arguments,

        LParen,
        LBrace, RBrace,
        LBrack, RBrack, 
        
        Question, Dot, 
        
        Comma, Colon, QuestionDot,

        Func, Class, Enum,
        True, False,
        If, ElIf, Else,
        For, While,
        Break, Continue, Return,
        Import, Using, As,
        Comment,

        Variable,

        Integer, Float, String, Boolean, 
        List, Dictionary, Lambda, Tuple,

        Async, Await,

        Del,

        Try, Catch, Fin, Throw,
    }

    public Type type;
    public string Value;

    public Token(char c)
    {
        Value = $"{c}";
        type = GetType(Value);
    }

    public Token(string s)
    {
        Value = s;
        type = GetType(s);
    }

    public Token(Obj obj)
    {
        Value = obj.CStr().Value;
        type = GetType(obj);
    }

    public Token(string s, Type type)
    {
        Value = s;
        this.type = type;
    }

    public override string ToString() => $"{type} : {Value}";

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
        if (obj is List) return Type.List;
        if (obj is Dict) return Type.Dictionary;
        if (obj is Collections.Tuple) return Type.Tuple;
        return Type.None;
    }


    public static bool IsOperator(Type type) => type switch
    {
        > Type.None and < Type.Comma => true,
        _ => false
    };

    public static bool IsOperator(string str) => IsOperator(GetType(str));

    public static bool IsOperator(char chr) => IsOperator(GetType(chr));


    public static bool IsSoloOperator(Type type) => type switch
    {
        Type.Indexer or Type.Slicer or Type.Property or Type.Not or Type.BNot or Type.NullableProperty => true,
        _ => false
    };


    public static bool IsBasicOperator(Type type) => type switch
    {
        > Type.None and < Type.Indexer => true,
        > Type.Function and < Type.Dot => true,
        _ => false
    };

    public static bool IsBasicOperator(string str) => IsBasicOperator(GetType(str));

    public static bool IsBasicOperator(char chr) => IsBasicOperator(GetType(chr));


    public static bool IsLoop(Type type) => type switch
    {
        Type.For or Type.While => true,
        _ => false,
    };

    public static bool IsLoop(string str) => IsLoop(GetType(str));


    public static bool IsControl(Type type) => type switch
    {
        Type.If or Type.ElIf or Type.Else => true,
        _ => false,
    };

    public static bool IsControl(string str) => IsControl(GetType(str));


    public static bool IsComment(Type type) => type switch
    {
        Type.Comment => true,
        _ => false,
    };

    public static bool IsComment(string str) => str == Literals.Comment;

    public static bool IsComment(char chr) => chr == Literals.CComment;


    public static bool IsAssigns(Type type) => type switch
    {
        >= Type.Assign and <= Type.RightShiftAssign => true,
        _ => false
    };


    public static bool IsSymbol(Type type) => type switch
    {
        Type.Comma or Type.Colon => true,
        _ => false,
    };

    public static bool IsSymbol(char chr) => IsSymbol(GetType(chr));


    public static bool IsLiteral(Type type) => type switch
    {
        >= Type.Integer and <= Type.Lambda => true,
        _ => false
    };


    public static bool IsConditional(Type type) => type switch
    {
        Type.And or Type.Or or Type.Xor => true,
        _ => false
    };



    public static bool IsString(char chr) => chr switch
    {
        Literals.Single or Literals.Double or Literals.Backtick => true,
        _ => false 
    };

    public static string String(List<Token> tokens)
    {
        var s = "";

        foreach (var token in tokens)
            s += token.type switch
            {
                Type.Method => Literals.Dot + token.Value,
                Type.Property => Literals.Dot + token.Value,
                Type.And or Type.Or or Type.Xor => $" {token.Value} ",
                _ => token.Value
            };
        return s;
    }

    public static bool IsBinaryDigit(char chr) => chr >= Literals.CZero && chr - Literals.CZero < 2;

    public static bool IsOctalDigit(char chr) => chr >= Literals.CZero && chr - Literals.CZero < 8;

    public static bool IsHexDigit(char chr) => chr switch
    {
        >= '0' and <= '9' => true,
        >= 'a' and <= 'f' => true,
        >= 'A' and <= 'F' => true,
        _ => false
    };


    public static Type Pair(Type type) => type switch
    {
        Type.LParen => Type.RParen,
        Type.LBrack => Type.RBrack,
        Type.LBrace => Type.RBrace,
        _ => Type.None
    };

    public static int IndexOfPair(List<Token> tokens, Type type, int start = 0)
    {
        int j = start, depth = 0;

        while (j < tokens.Count)
        {
            if (tokens[j].type == type)
                depth++;
            if (tokens[j].type == Pair(type))
                depth--;
            if (depth == 0) return j;
            j++;
        }

        return depth == 0 ? j : -1;
    }

    public static int IndexOf(List<Token> tokens, Func<Type, bool> codition, int start = 0)
    {
        for (int i = start; i < tokens.Count; i++)
            if (codition(tokens[i].type)) return i;
        return -1;
    }

    public static int IndexOf(List<Token> tokens, Type type, int start = 0)
    {
        for (int i = start; i < tokens.Count; i++)
            if (tokens[i].type == type) return i;
        return -1;
    }
}
