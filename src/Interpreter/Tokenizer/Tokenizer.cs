namespace Un;

public class Tokenizer()
{
    private UnFile code;
    private List<Token> tokens = [];

    public List<Token> Tokenize(UnFile code)
    {
        this.code = code;

        tokens.Clear();

        while (!code.EOF && !code.EOL)
        {
            Token token;
            var peek = Peek();

            if (peek == '\n') break;
            else if (char.IsWhiteSpace(peek))
            {
                Read();
                continue;
            }
            else if (IsComment(peek))
            {
                code.Move(0, code.Line + 1);
                break;
            }
            else if (IsString(peek)) token = GetStringToken();
            else if (IsFString(peek)) token = GetFStringToken();
            else if (IsNumber(peek)) token = GetNumberToken();
            else if (IsLetter(peek)) token = GetIdentifierToken();
            else if (IsOperator(peek)) token = GetOperatorToken();
            else if (IsBracket(peek)) token = GetBracketTokens(Read());
            else token = new Token($"{peek}", TokenType.Error);

            if (token.Type == TokenType.Error)
                throw new Panic("unexpected token: " + token.Value);

            tokens.Add(token);
        }

        return tokens;
    }

    #region Reader
    private char Peek() => code.Peek();
    private char Read() => code.Read();
    private string ReadInt()
    {
        string buffer = "";
        while (!code.EOF && !code.EOL && char.IsAsciiDigit(Peek()))
            buffer += Read();
        return buffer;
    }
    private bool TryReadInt(out string buffer)
    {
        buffer = "";
        while (!code.EOF && !code.EOL && char.IsAsciiDigit(Peek()))
            buffer += Read();
        return buffer != "";
    }
    private string ReadRange(params char[] chars)
    {
        string buffer = "";
        while (!code.EOF && !code.EOL && Now(chars))
            buffer += Read();
        return buffer;
    }
    private string ReadRange(Func<char, bool> func)
    {
        string buffer = "";
        while (!code.EOF && !code.EOL && func(Peek()))
            buffer += Read();
        return buffer;
    }

    private bool Now(params char[] chars) 
    {
        foreach (var chr in chars)
            if (!code.EOF && !code.EOL && Peek() == chr)
                return true;
        return false;            
    }
    #endregion

    #region Get Token
    private Token GetStringToken()
    {
        List<char> buffer = [Read()];
        var next = Read();
        var isMultiLine = false;

        if (!IsEnd() && next == buffer[0] && next == Peek())
        {
            isMultiLine = true;
            buffer.Add(next);
            buffer.Add(Read());
        }
        else buffer.Add(next);
        
        if (!isMultiLine && buffer[0] == buffer[^1])
            return new Token(string.Join("", buffer[1..^1]), TokenType.String);

        while (!code.EOF)
        {
            buffer.Add(Read());
            if (buffer[^1] == '\\')
                buffer[^1] = Read() switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '\\' => '\\',
                    '\'' => '\'',
                    '"' => '"',
                    '`' => '`',
                    _ => throw new Panic("unexpected escape character: " + buffer[^1])
                };

            if (code.EOL && isMultiLine)
                buffer.Add('\n');

            if (buffer[0] == buffer[^1])
            {
                if (!isMultiLine)
                    return new Token(string.Join("", buffer[1..^1]), TokenType.String);
                next = Read();
                if (!IsEnd() && buffer[0] == next && next == Peek())
                {
                    Read();
                    return new Token(string.Join("", buffer[3..^1]), TokenType.String);
                }
            }
        }

        return new Token("unclosed string literal", TokenType.Error);
    } 
    private Token GetFStringToken()
    {
        Token str = GetStringToken();
        if (str.Type == TokenType.Error)
            return str;

        return new Token(str.Value, TokenType.FString);
    }
    private Token GetNumberToken()
    {
        string buffer = ReadInt();
        var type = TokenType.Integer;

        if (code.EOF)
            return new Token(buffer, type);
        else if (IsFormation())
        {
            var peek = Peek();

            buffer += Read();
            buffer += peek switch
            {
                'x' or 'X' => ReadRange(char.IsAsciiHexDigit),
                'o' or 'O' => ReadRange('0', '1', '2', '3', '4', '5', '6', '7'),
                'b' or 'B' => ReadRange('0', '1'),
                _ => throw new Panic("unexpected token: " + peek)
            };
            
            return new Token($"{System.Convert.ToInt64(buffer, buffer[..2] switch
            {
                "0x" => 16,
                "0o" => 8,
                "0b" => 2,
                _ => 10
            })}", type);
        }
        else if (Now('.'))
        {
            buffer += Read();
            if (!TryReadInt(out var number))
            {
                code.Move(code.Index - 1, code.Line);
                return new Token(buffer[..^1], TokenType.Integer);
            }
            buffer += number;
            type = TokenType.Float;
        }                       

        if (!code.EOF && !code.EOL &&Now('e', 'E'))
        {
            buffer += Read();
            if (!code.EOF && !code.EOL &&Now('+', '-'))
                buffer += Read();

            if (!TryReadInt(out var number))
                return new Token("unexpected token: " + Peek(), TokenType.Error);

            buffer += number;
            type = TokenType.Float;
        }         

        return new Token(buffer, type);        

        bool IsFormation() => buffer == "0" && !code.EOF && !code.EOL &&Now('x', 'X', 'b', 'B', 'o', 'O');
    }
    private Token GetIdentifierToken()
    {
        string buffer = new(Read(), 1);

        while (!code.EOF && !code.EOL &&(IsLetter(Peek()) || IsNumber(Peek())))
            buffer += Read();
        
        return new Token(buffer, buffer switch
        {
            "true" => TokenType.Boolean,
            "false" => TokenType.Boolean,
            "none" => TokenType.None,
            "if" => TokenType.If,
            "elif" => TokenType.ElIf,
            "else" => TokenType.Else,
            "for" => TokenType.For,
            "while" => TokenType.While,
            "break" => TokenType.Break,
            "skip" => TokenType.Skip,
            "use" => TokenType.Use,
            "using" => TokenType.Using,
            "class" => TokenType.Class,
            "enum" => TokenType.Enum,
            "fn" => TokenType.Func,
            "in" => TokenType.In,
            "is" => TokenType.Is,
            "try" => TokenType.Try,
            "and" => TokenType.And,
            "or" => TokenType.Or,
            "not" => TokenType.Not,
            "xor" => TokenType.Xor,
            "match" => TokenType.Match,
            "as" => TokenType.As,
            "go" => TokenType.Go,
            "wait" => TokenType.Wait,
            "defer" => TokenType.Defer,
            _ => TokenType.Identifier
        });
    }
    private Token GetOperatorToken()
    {
        var chr = Read();

        switch (chr)
        {
            case '?':
                if (Now('.'))
                    return new Token($"{chr}{Read()}", TokenType.QuestionDot);
                return new Token($"{chr}", TokenType.Question);
            case '|':
                if (Now('='))
                    return new Token($"{chr}{Read()}", TokenType.BOrAssign);
                return new Token($"{chr}", TokenType.BOr);       
            case '-':
                if (Now('>'))
                    return new Token($"{chr}{Read()}", TokenType.Return);
                if (Now('='))
                    return new Token($"{chr}{Read()}", TokenType.MinusAssign);
                return new Token($"{chr}", TokenType.Minus);
            case '=':
            case '+':
            case '%':
            case '^':
            case '&':
            case '!':
                if (Now('='))
                    return new Token($"{chr}{Read()}", chr switch
                    {
                        '+' => TokenType.PlusAssign,
                        '-' => TokenType.MinusAssign,
                        '%' => TokenType.PercentAssign,
                        '^' => TokenType.BXorAssign,
                        '&' => TokenType.BAndAssign,
                        '|' => TokenType.BOrAssign,                        
                        '!' => TokenType.Unequal,
                        '=' => TokenType.Equal,
                        _ => TokenType.Error
                    });
                return new Token($"{chr}", chr switch
                    {
                        '+' => TokenType.Plus,
                        '-' => TokenType.Minus,
                        '%' => TokenType.Percent,
                        '^' => TokenType.BXor,
                        '&' => TokenType.BAnd,
                        '|' => TokenType.BOr,
                        '!' => TokenType.BNot,
                        '=' => TokenType.Assign,
                        _ => TokenType.Error
                    });
            case '/':
            case '<':
            case '>':            
            case '*':
                if (Now(chr))
                {
                    var c = Read();
                    if (Now('='))
                        return new Token($"{chr}{c}{Read()}", chr switch
                        {
                            '/' => TokenType.DoubleSlashAssign,
                            '<' => TokenType.LeftShiftAssign,
                            '>' => TokenType.RightShiftAssign,
                            '?' => TokenType.DoubleQuestionAssign,
                            '*' => TokenType.DoubleAsteriskAssign,
                            _ => TokenType.Error
                        });
                    return new Token(new(c, 2), chr switch
                    {
                        '/' => TokenType.DoubleSlash,
                        '<' => TokenType.LeftShift,
                        '>' => TokenType.RightShift,
                        '?' => TokenType.DoubleQuestion,
                        '*' => TokenType.DoubleAsterisk,
                        _ => TokenType.Error
                    });
                }
                if (Now('='))
                    return new Token($"{chr}{Read()}", chr switch
                        {
                            '/' => TokenType.SlashAssign,
                            '<' => TokenType.LessOrEqual,
                            '>' => TokenType.GreaterOrEqual,
                            '?' => TokenType.QuestionAssign,
                            '*' => TokenType.AsteriskAssign,
                            _ => TokenType.Error
                        });
                return new Token($"{chr}", chr switch
                        {
                            '/' => TokenType.Slash,
                            '<' => TokenType.LessThan,
                            '>' => TokenType.GreaterThan,
                            '?' => TokenType.Question,
                            '*' => TokenType.Asterisk,
                            _ => TokenType.Error
                        });
            case '(':
            case ')':
            case '{':
            case '}':
            case '[':
            case ']':
            case ':':
            case ',':
            case '.':
            case '@':
                return new Token($"{chr}", chr switch
                {
                    '(' => TokenType.LParen,
                    ')' => TokenType.RParen,
                    '{' => TokenType.LBrace,
                    '}' => TokenType.RBrace,
                    '[' => TokenType.LBrack,
                    ']' => TokenType.RBrack,
                    ':' => TokenType.Colon,
                    ',' => TokenType.Comma,
                    '.' => TokenType.Dot,
                    '@' => TokenType.At,
                    _ => TokenType.Error
                }); 
            default:
                break;
        }    
        
        return new Token($"{chr}", TokenType.Error);
    }
    private Token GetBracketTokens(char bracket)
    {        
        tokens.Add(new Token($"{bracket}", bracket switch
        {
            '(' => TokenType.LParen,
            '{' => TokenType.LBrace,
            '[' => TokenType.LBrack,
            _ => TokenType.Error
        }));

        char closer = bracket switch
        {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            _ => ' ',
        };

        while (!code.EOF)
        {
            Token token;
            var peek = Peek();
            
            if (peek == closer)
                break;

            if (char.IsWhiteSpace(peek))
            {
                Read();
                continue;
            }
            else if (IsComment(peek)) break;
            else if (IsString(peek)) token = GetStringToken();
            else if (IsFString(peek)) token = GetFStringToken();
            else if (IsNumber(peek)) token = GetNumberToken();
            else if (IsLetter(peek)) token = GetIdentifierToken();
            else if (IsOperator(peek)) token = GetOperatorToken();
            else if (IsBracket(peek)) token = GetBracketTokens(Read());
            else token = new Token($"{peek}", TokenType.Error);

            if (token.Type == TokenType.Error)
                return token;

            tokens.Add(token);

            if (code.EOL)
            {
                code.Move(0, code.Line + 1);
                if (code.EOF)
                    throw new Panic("unclosed bracket: " + bracket);
            }
        }

        var end = Read();

        return new Token($"{end}", end switch
        {
            ')' => TokenType.RParen,
            ']' => TokenType.RBrack,
            '}' => TokenType.RBrace,
            _ => TokenType.Error
        });
    }
    #endregion

    #region Is
    private bool IsEnd() => code.EOF && code.EOL;
    private bool IsComment(char c) => c == '#';
    private bool IsString(char c) => c == '\'' || c == '"';
    private bool IsFString(char c) => c == '`';
    private bool IsNumber(char c) => char.IsAsciiDigit(c);
    private bool IsLetter(char c) => char.IsLetter(c) || c == '_';
    private bool IsBracket(char c) => c switch
    {
        '{' or '[' or '(' => true,
        _ => false,
    };
    private bool IsOperator(char c) => c switch
    {
        '!' or '@' or '%' or '^' or '&' or '*' or '-' or '+' or '=' or  ':' or '*' or
        '~' or '&' or '|' or '<' or '>' or '?' or ',' or '.' or '/' or '}' or ')' or ']' => true,
        _ => false
    };
    #endregion
}