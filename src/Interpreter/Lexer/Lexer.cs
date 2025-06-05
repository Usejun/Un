namespace Un;

using TokenInfo = (Token token, TokenType type);

public class Lexer
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
                var closer = type.GetCloser();
                var (start, end) = GetBracketRange(closer);
                var children = new Lexer().Lex(tokens[start..end]);

                nodes.Add(new Node(type switch
                {
                    TokenType.LBrack => IsSlicer(start, end) ? "indexer" : "list",
                    TokenType.LBrace => IsSet(start, end) ? "set" : "dict",
                    TokenType.LParen => IsIdentifier() ? "call" : "tuple",
                    _ => throw new Error($"invalid left bracket type {type}")
                }, type switch
                {
                    TokenType.LBrack => IsSlicer(start, end) ? TokenType.Indexer : TokenType.List,
                    TokenType.LBrace => IsSet(start, end) ? TokenType.Set : TokenType.Dict,
                    TokenType.LParen =>  IsIdentifier() ? TokenType.Call : TokenType.Tuple,
                    _ => throw new Error($"invalid left bracket type {type}")
                })
                {
                    Children = children,
                });

                index = end + 1;
            }
            else if (type == TokenType.BNot)
            {
                if (nodes.Count != 0 && nodes[^1].Type == TokenType.Call)
                    nodes[^1].Type = TokenType.AsyncCall;
                else
                    nodes.Add(new Node(token.Value, type));
            }
            else if (type == TokenType.Question)
            {
                if (nodes.Count != 0 && nodes[^1].Type == TokenType.Call)
                    nodes[^1].Type = TokenType.TryCall;
                else
                    nodes.Add(new Node(token.Value, type));
            }
            else if (type.IsUnaryOperator())
            {
                nodes.Add(new Node(token.Value, IsUnaryOperator() ? type switch
                {
                    TokenType.Plus => TokenType.Positive,
                    TokenType.Minus => TokenType.Negative,
                    TokenType.Not => TokenType.Not,
                    TokenType.Asterisk => TokenType.Spread,
                    TokenType.DoubleAsterisk => TokenType.DictSpread,
                    _ => throw new Error($"invalid unary operator type {type}")
                } : type));
            }
            else if (type == TokenType.Dot)
            {
                if (!IsIdentifier())
                    throw new Error("expected identifier after dot");

                (var property, _) = Next();

                nodes.Add(new Node(property.Value, TokenType.Property));
            }
            else if (type == TokenType.Func)
            {
                var value = Peek(index);

                if (value.type == TokenType.Identifier)
                {
                    var args = Peek(index + 1);
                    if (args.type == TokenType.LBrack)
                    {
                        nodes.Add(new Node("fn", TokenType.Func)
                        {
                            Children = [
                                new Node(value.token.Value, TokenType.Identifier),
                                new Node(args.token.Value, TokenType.Tuple)],
                        });
                        index += 2;
                    }
                    else
                        throw new Error("invalid function definition");
                }
                else if (value.type == TokenType.Tuple)
                {
                    nodes.Add(new Node("fn", TokenType.Func)
                    {
                        Children = [new Node(value.token.Value, TokenType.Tuple)],
                    });
                    index++;
                }
                else
                    throw new Error("invalid function definition");

                value = Next();

                if (value.type == TokenType.Return)
                    break;
                else
                    throw new Error("expected return type after '->'");
            }
            else nodes.Add(new Node(token.Value, type));
        }

        return nodes;

        bool IsIdentifier() => nodes.Count > 0 && nodes[^1].Type.IsIdentifier();

        bool IsUnaryOperator() => (nodes.Count == 0 || nodes[^1].Type.IsOperator()) && index < tokens.Count && tokens[index].Type.IsIdentifier();
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
            throw new Error("end of tokens reached");

        var token = tokens[index];
        var type = token.Type;

        index++;

        return (token, type);
    }
    private TokenInfo Peek(int index)
    {
        if (IsEnd(index))
            throw new Error("end of tokens reached");

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
            throw new Error("too many colons in slicer");

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

    private (int start, int end) GetBracketRange(TokenType closer)
    {
        int start = index, end = index + 1, depth = 1;

        while (end < tokens.Count && depth > 0)
        {
            var (_, type) = Peek(end);
            if (type == closer)
                depth--;
            else if (type.IsLeftBracket())
                depth++;

            if (depth == 0)
                break;

            end++;
        }

        if (depth > 0)
            throw new Error($"unmatched {closer} at index {end}");
        return (start, end);
    }
}