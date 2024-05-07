using Un.Collections;
using Un.Data;

namespace Un
{
    public class Token
    {
        public static Token None => new("None", Type.None);

        public static Dictionary<string, Type> Types = [];

        public static List<string> UnionOper = [];

        public enum Type
        {
            None,

            RParen,
            Assign,
            PlusAssign,
            MinusAssign,
            AsteriskAssign,
            DoubleAsteriskAssign,
            SlashAssign,
            DoubleSlashAssign,
            PercentAssign,
            BAndAssign,
            BOrAssign,
            BXorAssign, 
            LeftShiftAssign,
            RightShiftAssign,

            Or,
            And,
            Xor,
            Not,

            In,

            Equal,
            Unequal,
            LessOrEqual,
            GreaterOrEqual,
            LessThen,
            GreaterThen,

            BOr,
            BXor,
            BAnd,

            LeftShift,
            RightShift,

            Plus,
            Minus,

            Asterisk,
            Slash,
            DoubleSlash,
            Percent,

            BNot,
            Bang,

            DoubleAsterisk,

            Indexer,
            Slicer,
            Property,
            Method,
            Function,

            LParen,

            LBrack,
            RBrack,

            Dot,
            Comma,
            Colon,

            Func,
            True,
            False,
            If,
            ElIf,
            Else,
            For,
            While,
            Break,
            Continue,
            Import,
            Return,
            Comment,

            Variable,
            Integer,
            Float,
            String,
            Boolean,
            Iterator,
            Class,
            Lambda,
            Using,
        }

        public Type type;
        public string value;

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

    }
}
