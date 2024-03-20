using Un.Object;

namespace Un
{
    public class Token
    {
        public enum Type
        {
            None,

            Assign,
            Plus,
            Minus,
            Bang,
            Asterisk,
            Slash,
            Percent,
            DoubleSlash,

            LessOrEqual,
            GreaterOrEqual,
            LessThen,
            GreaterThen,

            Equal,
            Unequal,

            Comma,

            LParen,
            RParen,

            LBrack,
            RBrack,

            Func,
            True,
            False,
            If,
            ElIf,
            Else,
            For,
            While,
            Import,
            Return,
            Comment,

            Variable,
            Function,
            Integer,
            Float,
            String,
            Boolean,
            Iterator
        }

        public Token(char c)
        {
            value = $"{c}";
            tokenType = GetType(c);
        }

        public Token(string s)
        {
            value = s;
            tokenType = GetType(s);
        }

        public Token(Obj obj)
        {
            value = obj.ToString();
            tokenType = GetType(obj);
        }

        public Token(string s, Type type)
        {
            value = s;
            tokenType = type;
        }

        public Type tokenType;
        public string value;

        public static Type GetType(char chr) => chr switch
        {
            '=' => Type.Assign,
            '+' => Type.Plus,
            '-' => Type.Minus,
            '!' => Type.Bang,
            '*' => Type.Asterisk,
            '/' => Type.Slash,
            '%' => Type.Percent,
            '>' => Type.LessThen,
            '<' => Type.GreaterThen,
            ',' => Type.Comma,
            '(' => Type.LParen,
            ')' => Type.RParen,
            '[' => Type.LBrack,
            ']' => Type.RBrack,
            '#' => Type.Comment,
            _ => Type.None
        };

        public static Type GetType(string str) => str switch
        {
            ">=" => Type.LessOrEqual,
            "<=" => Type.GreaterOrEqual,
            "==" => Type.Equal,
            "!=" => Type.Unequal,
            "//" => Type.DoubleSlash,
            "fn" => Type.Func,
            "true" => Type.True,
            "false" => Type.False,
            "if" => Type.If,
            "elif" => Type.ElIf,
            "else" => Type.Else,
            "for" => Type.For,
            "while" => Type.While,
            "return" => Type.Return,
            "import" => Type.Import,
            _ => Type.None
        };

        public static Type GetType(Obj obj)
        {
            if (obj is Int) return Type.Integer;
            if (obj is Float) return Type.Float;
            if (obj is Str) return Type.String;
            if (obj is Bool) return Type.Boolean;
            if (obj is Iter) return Type.Iterator;
            return Type.None;
        }
    }
}
