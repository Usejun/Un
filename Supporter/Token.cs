using Un.Object;

namespace Un.Supporter
{
    public class Token
    {
        public readonly static Dictionary<Type, int> Operator = new()
        {
            { Type.Assign, 0 }, { Type.RParen, 0 }, { Type.Equal,  0 }, { Type.Unequal, 0 },
            { Type.LessOrEqual, 0 }, { Type.LessThen, 0 }, { Type.GreaterOrEqual, 0 }, { Type.GreaterThen, 0 },
            { Type.And, 0 }, { Type.Or, 0 }, { Type.Caret , 0 },
            { Type.Plus, 1 }, { Type.Minus, 1 }, { Type.Percent, 1 }, { Type.Bang, 1 },
            { Type.Asterisk, 2 }, { Type.Slash, 2 }, { Type.DoubleSlash, 2 },
            { Type.Indexer, 3 }, { Type.Property, 3 },
            { Type.Function, 4 }, { Type.Method, 4 },
            { Type.LParen, 5 },
        };

        public readonly static Dictionary<string, Type> Control = new()
        {
            {"if", Type.If}, {"elif", Type.ElIf}, {"else", Type.Else}
        };

        public readonly static Dictionary<string, Type> Loop = new()
        {
            {"for", Type.For}, {"while", Type.While}
        };

        public static Dictionary<string, Type> Types = [];

        public enum Type
        {
            None,

            Assign,
            PlusAssign,
            MinusAssign,
            AsteriskAssign,
            SlashAssign,
            DoubleSlashAssign,
            PercentAssign,

            Plus,
            Minus,
            Bang,
            Asterisk,
            Slash,
            Percent,
            And,
            Or,
            Caret,
            DoubleSlash,

            LessOrEqual,
            GreaterOrEqual,
            LessThen,
            GreaterThen,

            Equal,
            Unequal,

            Indexer,
            Property,
            Method,

            LBrack,
            RBrack,

            LParen,
            RParen,

            Dot,
            Comma,

            Func,
            True,
            False,
            If,
            ElIf,
            Else,
            For,
            While,
            Import,
            In,
            Return,
            Comment,

            Variable,
            Function,
            Integer,
            Float,
            String,
            Boolean,
            Iterator,
            Class,
        }

        public Token(char c)
        {
            value = $"{c}";
            type = GetType(c);
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

        public Type type;
        public string value;

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
            >= Type.Assign and <= Type.RParen => true,
            _ => false
        };

        public static bool IsOperator(string str) => IsOperator(GetType(str));

        public static bool IsOperator(char chr) => IsOperator(GetType(chr));


        public static bool IsSoloOperator(Token token) => IsSoloOperator(token.type);

        public static bool IsSoloOperator(Type type) => type switch
        {
            Type.Bang or Type.Indexer or Type.Property or Type.Function => true,
            _ => false,
        };

        public static bool IsSoloOperator(string str) => IsSoloOperator(GetType(str));

        public static bool IsSoloOperator(char chr) => IsSoloOperator(GetType(chr));


        public static bool IsBasicOperator(Token token) => IsBasicOperator(token.type);

        public static bool IsBasicOperator(Type type) => type switch
        {
            >= Type.Assign and <= Type.Unequal => true,
            _ => false,
        };

        public static bool IsBasicOperator(string str) => IsBasicOperator(GetType(str));

        public static bool IsBasicOperator(char chr) => IsBasicOperator(GetType(chr));


        public static bool IsLoop(Token token) => Loop.ContainsKey(token.value);

        public static bool IsLoop(Type type) => type switch
        {
            Type.For or Type.While => true,
            _ => false,
        };

        public static bool IsLoop(string str) => Loop.ContainsKey(str);


        public static bool IsControl(Token token) => Control.ContainsKey(token.value);

        public static bool IsControl(Type type) => type switch
        {
            Type.If or Type.ElIf or Type.Else => true,
            _ => false,
        };

        public static bool IsControl(string str) => Control.ContainsKey(str);


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
            >= Type.Assign and <= Type.PercentAssign => true,
            _ => false
        };

    }
}
