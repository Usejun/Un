namespace Un;

using TokenInfo = (Token token, TokenType type);

public class Lexer()
{
    private List<Token> tokens;
    private int index = 0;

    private List<Node> Lex()
    {
        index = 0;
        List<Node> nodes = [];

        while (!IsEnd())
        {
            (var token, var type) = Next();

            if (type.IsLeftBracket())
            {                
                var (start, end) = GetBracketRange(type);
                var children = new Lexer().Lex(tokens[(start + 1)..end]);

                nodes.Add(new Node(type switch
                {
                    TokenType.LBrack => IsSlicer(start, end) ? "indexer" : "list",
                    TokenType.LBrace => IsSet(start, end) ? "set" : "dict",
                    TokenType.LParen => IsIdentifier() ? "call" : "tuple",
                    _ => throw new Panic($"invalid left bracket type {type}")
                }, type switch
                {
                    TokenType.LBrack => IsSlicer(start, end) ? TokenType.Indexer : TokenType.List,
                    TokenType.LBrace => IsSet(start, end) ? TokenType.Set : TokenType.Dict,
                    TokenType.LParen => IsIdentifier() ? TokenType.Call : TokenType.Tuple,
                    _ => throw new Panic($"invalid left bracket type {type}")
                })
                {
                    Children = children,
                });

                index = end + 1;
            }
            else if (type.IsUnary())
            {
                nodes.Add(new Node(token.Value, IsUnary() ? type switch
                {
                    TokenType.Plus => TokenType.Positive,
                    TokenType.Minus => TokenType.Negative,
                    TokenType.Not => TokenType.Not,
                    TokenType.Asterisk => TokenType.Spread,
                    TokenType.DoubleAsterisk => TokenType.DictSpread,
                    _ => throw new Panic($"invalid unary operator type {type}")
                } : type));
            }
            else if (type == TokenType.Dot)
            {
                if (!IsIdentifier())
                    throw new Panic("expected identifier after dot");

                (var property, _) = Next();

                nodes.Add(new Node(property.Value, TokenType.Property));
            }
            else if (type == TokenType.Func)
            {
                var lexed = new Lexer().Lex(tokens[index..]);

                if (lexed.Count < 2)
                    throw new Panic("expected function body after func keyword");

                if (lexed[0].Type == TokenType.Tuple && lexed[1].Type == TokenType.Return)
                {
                    var splited = lexed.Split(TokenType.Comma);
                    nodes.Add(new Node("fn", TokenType.Func)
                    {
                        Children = splited[0]
                    });

                    for (int i = 1; i < splited.Count; i++)
                    {
                        nodes.Add(new Node(",", TokenType.Comma));
                        nodes.AddRange(splited[i]);
                    }
                }
                else if (lexed[0].Type == TokenType.Identifier && lexed[1].Type == TokenType.Call)
                {
                    nodes.Add(new Node(lexed[0].Value, TokenType.Func)
                    {
                        Children = [lexed[1]]
                    });
                }
                else throw new Panic("expected function body after func keyword");

                break;
            }
            else nodes.Add(new Node(token.Value, type));
        }

        return nodes;

        bool IsIdentifier() => nodes.Count > 0 && nodes[^1].Type.IsIdentifier();

        bool IsUnary() => (nodes.Count == 0 || nodes[^1].Type.IsOperator() || nodes[^1].Type == TokenType.Comma) && index < tokens.Count ;
    }

    public List<Node> Lex(List<Token> tokens)
    {
        this.tokens = tokens;
        return Lex();
    }


    #region Node Reader
    private TokenInfo Next()
    {
        if (IsEnd())
            throw new Panic("end of tokens reached");

        var token = tokens[index];
        var type = token.Type;

        index++;

        return (token, type);
    }
    private TokenInfo Peek(int index)
    {
        if (IsEnd(index))
            throw new Panic("end of tokens reached");

        var token = tokens[index];
        var type = token.Type;

        return (token, type);
    }
    #endregion

    #region Is
    private bool IsSlicer(int start, int end)
    {
        int depth = 0, colon = 0;

        for (int i = start; i < end; i++)
        {
            (_, var type) = Peek(i);
            depth = type == TokenType.LBrack ? depth + 1 :
                    type == TokenType.RBrack ? depth - 1 :
                    depth;
            if (depth == 1 && type == TokenType.Colon)
                colon++;
        }

        if (colon > 2)
            throw new Panic("too many colons in slicer");

        return colon > 0 && colon < 3;
    }
    private bool IsSet(int start, int end)
    {
        int depth = 0;

        for (int i = start; i < end; i++)
        {
            (_, var type) = Peek(i);
            depth = type == TokenType.LBrace ? depth + 1 :
                    type == TokenType.RBrace ? depth - 1 :
                    depth;
            if (depth == 1 && type == TokenType.Colon)
                return false;
        }

        return true;
    }
    private bool IsEnd() => index >= tokens.Count;
    private bool IsEnd(int idx) => idx >= tokens.Count;
    #endregion

    private (int start, int end) GetBracketRange(TokenType opener)
    {
        int start = index-1, end = index-1, depth = 0;
        var closer = opener.GetCloser();

        while (end < tokens.Count)
        {
            var (_, type) = Peek(end);
            if (type.IsRightBracket())
                depth--;
            else if (type.IsLeftBracket())
                depth++;

            if (depth == 0)
                break;

            end++;
        }

        if (depth > 0)
            throw new Panic($"unmatched {closer} at index {end}");
        return (start, end);
    }
}