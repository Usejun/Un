using Un.Function;
using Un.Object;
using Un.Class;

namespace Un
{
    public static class Process
    {
        public static string File = "";

        public static string Path = "";

        public static string[] Code = [];

        public static Interpreter Main = new([], Variable, Func);

        public static Dictionary<string, Cla> Class = [];     

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
            { Token.Type.Asterisk, 2 }, { Token.Type.Slash, 2 }, { Token.Type.DoubleSlash, 2 },
            { Token.Type.Indexer, 2 }, { Token.Type.Pointer, 2 },
            { Token.Type.Function, 3 }, 
            { Token.Type.LParen, 4 },
        };

        public static Dictionary<string, Obj> Variable = [];

        public static Dictionary<string, Fun> Func = [];

        public static void Initialize(string path, string file)
        {            
            Path = path;
            File = file;

            using StreamReader r = new(new FileStream($"{path}\\{file}", FileMode.Open));

            Code = r.ReadToEnd().Split('\n');
        }

        public static void Run()
        {
            Main.code = Code;
            Main.variable = Variable;
            Main.method = Func;

            while (Main.TryInterpret()) ;            
        }

        public static Fun GetFunc(string name)
        {
            if (Func.TryGetValue(name, out var func))
                return func.Clone();
            throw new ObjException("Get function Error");
        }

        public static Cla GetClass(string name)
        {
            if (Class.TryGetValue(name, out var cla))
                return cla.Clone();
            throw new ObjException("Get Class Error");
        }

        public static void Import(Importable importable)
        {
            foreach (var item in importable.Methods())
                Func.Add(item.Key, item.Value);
        }

        public static void Import(string file)
        {
            using StreamReader r = new(new FileStream($"{Path}\\{file}", FileMode.Open));

            Interpreter interpreter = new(r.ReadToEnd().Split('\n'), [], []);

            while (interpreter.TryInterpret());
        }

        public static bool IsGlobalVariable(string str) => Variable.ContainsKey(str);

        public static bool IsGlobalVariable(Token token) => IsGlobalVariable(token.value);

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
            Token.Type.Bang or Token.Type.Indexer or Token.Type.Pointer => true,
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

        public static bool IsFunc(Token token) => IsFunc(token.value);

        public static bool IsFunc(string str) => Func.ContainsKey(str);

        public static bool IsLoop(Token token) => IsLoop(token.value);

        public static bool IsLoop(string str) => Loop.ContainsKey(str);

        public static bool IsControl(Token token) => IsControl(token.value);

        public static bool IsControl(string str) => Control.ContainsKey(str);

        public static bool IsClass(Token token) => IsClass(token.value);

        public static bool IsClass(string str) => Class.ContainsKey(str);

    }
}
