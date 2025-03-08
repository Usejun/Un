namespace Un.Interpreter;

public static class Tokenizer
{
    public static List<Token> Tokenize(string code)
    {
        List<Token> tokens = [];
        int index = 0;

        while (index < code.Length)
        {
            SkipWhitespace(code);
            if (index >= code.Length) break;
            else if (Token.IsComment(code[index])) tokens.Add(new($"{code[index++]}", Token.Type.Comment));
            else if (Token.IsString(code[index])) tokens.Add(String(code));
            else if (char.IsLetter(code[index]) || code[index] == Literals.CUnderbar) tokens.Add(Keyword(code));
            else if (char.IsDigit(code[index])) tokens.Add(Number(code));
            else if (Token.IsOperator(code[index])) tokens.Add(Operator(code));
            else if (Token.IsSymbol(code[index])) tokens.Add(new(code[index++]));
            else throw new SyntaxError();
        }

        return tokens;

        void SkipWhitespace(string code)
        {
            int length = code.Length;
            while (index < length && char.IsWhiteSpace(code[index]))
                index++;
        }

        Token String(string code)
        {
            bool closed = false;
            StringBuffer str = new($"{code[index++]}");

            while (!closed && index < code.Length)
            {
                bool isEscape = false;

                if (code[index] != Literals.Escape)
                    str.Append(code[index++]);                
                else if(index + 1 < code.Length && Token.Escape.TryGetValue(code[index + 1], out var c))
                {
                    isEscape = true;
                    str.Append(c);         
                    index += 2;
                }
                else throw new SyntaxError("invalid string syntax");       

                if (!isEscape && str[^1] == str[0]) closed = true;
            }

            return closed ? new(str.ToString(), Token.Type.String) : throw new SyntaxError("invalid string syntax");
        }

        Token Number(string code)
        {
            StringBuffer str = new();

            Take(Token.IsDigit);

            if (OutOfRange()) return new(str.ToString(), Token.Type.Integer);

            if (code[index] == Literals.CDot)
            {
                str.Append(code[index++]);
                Take(Token.IsDigit);

                if (code.Length <= index)
                    return new(str.ToString(), Token.Type.Float);
                else if (code[index] == Literals.e || code[index] == Literals.E)                
                {
                    str.Append(code[index++]);
                    if (code[index] == Literals.CPlus || code[index] == Literals.CMinus)
                        str.Append(code[index++]);                
                }
                Take(Token.IsDigit);

                if (str[^1] == Literals.e || str[^1] == Literals.E)
                    throw new SyntaxError("invalid float syntax");

                return new(str.ToString(), Token.Type.Float);
            }
            else if (code[index] == Literals.e || code[index] == Literals.E)
            {
                str.Append(code[index++]);

                if (code[index] == Literals.CPlus || code[index] == Literals.CMinus)
                    str.Append(code[index++]);

                Take(Token.IsDigit);

                return new(str.ToString(), Token.Type.Float);
            }
            else if (str.Length == 1 && str.Equals(Literals.CZero))
            {
                str.Clear();

                int baseNumber = code[index++] switch 
                {
                    Literals.b => 2,
                    Literals.o => 8,
                    Literals.x => 16,
                    _ => -1
                };

                if (baseNumber == -1)
                    throw new SyntaxError("invalid base-by-integer");

                
                Take(baseNumber switch
                {
                    2 => Token.IsBinaryDigit,
                    8 => Token.IsOctalDigit,
                    16 => Token.IsHexDigit,
                    _ => throw new SyntaxError("invalid base-by-integer"),
                });

                return new($"{Convert.ToInt64(str.ToString(), baseNumber)}", Token.Type.Integer);
            }

            return new(str.ToString(), Token.Type.Integer);

            bool OutOfRange() => index >= code.Length;

            void Take(Func<char, bool> conditions)
            {
                //str.Append(code[index++]);
                while (!OutOfRange() && conditions(code[index]))
                {
                    if (code[index] == Literals.CUnderbar)
                        index++;
                    else str.Append(code[index++]);
                }
            }
        }

        Token Operator(string code)
        {
            bool TryOperator(string type, out Token token)
            {
                token = Token.None;

                for (int i = 0; i < type.Length; i++)
                    if (code[index + i] != type[i])
                        return false;

                index += type.Length;
                token = new Token(type);
                return true;
            }

            foreach (var o in Token.Union)
                if (TryOperator(o, out var t))
                    return t;

            return new(code[index++]);
        }

        Token Keyword(string code)
        {
            StringBuffer str = new();

            while (index < code.Length && (char.IsLetterOrDigit(code[index]) || code[index] == Literals.CUnderbar))
                str.Append(code[index++]);

            var keyword = str.ToString();   

            if (Process.TryGetGlobalProperty(keyword, out var property) && property is Fun)
                return new(keyword, Token.Type.Function);
            if (Token.GetType(keyword) != Token.Type.None)
                return new(keyword.ToString());

            return new Token(keyword, Token.Type.Variable);
        }
    }

    public static bool IsBody(string code, int nesting)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrEmpty(code)) return true;

        bool a = true, b = true;

        for (int i = 0; a && i < nesting; i++)
            if (code.Length > i && code[i] != Literals.Tab)
                a = false;

        for (int i = 0; b && i < 4 * nesting; i++)
            if (code.Length > i && code[i] != Literals.Space)
                b = false;

        return a || b;
    }
}
