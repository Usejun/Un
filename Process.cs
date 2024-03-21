using Un.Object;

namespace Un
{
    public static class Process
    {
        public static string[] Code = [];

        public static Dictionary<string, Func<Obj, Obj>> Function = new()
        {
            {"write", Std.Write}, {"writeln", Std.Writeln}, {"readln", Std.Readln},
            {"int", Std.Int}, {"float", Std.Float}, {"str", Std.Str}, {"type", Std.Type},
            {"func", Std.Func}, {"len", Std.Len}, {"iter", Std.Iter}, {"range", Std.Range}
        };

        public static Dictionary<string, Token.Type> Control = new()
        {
            {"if", Token.Type.If}, {"elif", Token.Type.ElIf}, {"else", Token.Type.Else}
        };

        public static Dictionary<string, Token.Type> Loop = new()
        {
            {"for", Token.Type.For}, {"while", Token.Type.While}
        };

        public static Dictionary<Token.Type, int> Operator = new()
        {
            { Token.Type.Assign, 0 }, { Token.Type.RParen, 0 }, { Token.Type.Equal,  0 }, { Token.Type.Unequal, 0 },
            { Token.Type.LessOrEqual, 0 }, { Token.Type.LessThen, 0 }, { Token.Type.GreaterOrEqual, 0 }, { Token.Type.GreaterThen, 0 },
            { Token.Type.Plus, 1 }, { Token.Type.Minus, 1 }, { Token.Type.Percent, 1 }, { Token.Type.Bang, 1 },
            { Token.Type.Asterisk, 2 }, { Token.Type.Slash, 2 }, { Token.Type.DoubleSlash, 2 }, { Token.Type.Variable, 2 },
            { Token.Type.Indexer, 2 },
            { Token.Type.Function, 3 }, 
            { Token.Type.LParen, 4 },
        };

        public static Dictionary<string, Obj> Variable = [];

        public static Dictionary<string, int> Func = [];

        public static bool IsVariable(string str) => Variable.ContainsKey(str);

        public static bool IsVariable(Token token) => IsVariable(token.value);

        public static bool IsOperator(Token token) => IsOperator(token.tokenType);

        public static bool IsOperator(Token.Type type) => type switch
        {
            >= Token.Type.Assign and <= Token.Type.RParen => true, 
            _ => false
        };

        public static bool IsOperator(string str) => str switch
        {
            "+" or "-" or "*" or "/" or "%" or "//" or "=" or
            "(" or ")" or ">" or "<" or "!" or ">=" or "==" or
            "<=" or "!=" => true,
            _ => false,
        };

        public static bool IsOperator(char chr) => chr switch
        {
            '+' or '-' or '*' or '/' or '%' or '=' or '(' or ')' or
            '>' or '<' or '!' or '[' or ']' => true,
            _ => false,
        };

        public static bool IsSoloOperator(Token token) => IsSoloOperator(token.tokenType);

        public static bool IsSoloOperator(Token.Type type) => type switch
        {
            Token.Type.Bang or Token.Type.Indexer => true,
            _ => false,
        };

        public static bool IsSoloOperator(char chr) => chr switch
        {
            '!' => true,
            _ => false,
        };

        public static bool IsSoloOperator(string str) => str switch
        {
            "!" => true,
            _ => false,
        };

        public static bool IsFunction(Token token) => IsFunction(token.value);

        public static bool IsFunction(string str) => Function.ContainsKey(str);

        public static bool IsFunc(Token token) => IsFunc(token.value);

        public static bool IsFunc(string str) => Func.ContainsKey(str);

        public static bool IsLoop(Token token) => IsLoop(token.value);

        public static bool IsLoop(string str) => Loop.ContainsKey(str);

        public static bool IsControl(Token token) => IsControl(token.value);

        public static bool IsControl(string str) => Control.ContainsKey(str);

    }
}
