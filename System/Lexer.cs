namespace Un;

public static class Lexer
{
    public static List<Token> Lex(List<Token> tokens, Dictionary<string, Obj> properties)
    {
         List<Token> analyzedTokens = [];

        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i].type == Token.Type.LBrack)
            {
                int j = i + 1, depth = 1;
                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LBrack)
                        depth++;
                    if (tokens[j].type == Token.Type.RBrack)
                        depth--;
                    if (depth == 0) break;
                    j++;
                }

                if (analyzedTokens.Count > 0 &&
                   (analyzedTokens[^1].type == Token.Type.Variable ||
                    analyzedTokens[^1].type == Token.Type.Iterator ||
                    analyzedTokens[^1].type == Token.Type.String ||
                    analyzedTokens[^1].type == Token.Type.Indexer ||
                    analyzedTokens[^1].type == Token.Type.Property))
                {
                    int colon = -1;

                    for (int k = i; k < j; k++)
                        if (tokens[k].type == Token.Type.Colon)
                            colon = k;

                    if (colon > 0)
                    {
                        Obj start = i + 1 == colon ? new Int(0) : Calculator.Calculate(Lex(tokens[(i + 1)..colon], properties), properties);
                        Obj end = colon + 1 == j ? new Int(-1) : Calculator.Calculate(Lex(tokens[(colon + 1)..j], properties), properties);

                        if (start is not Int || end is not Int) throw new SyntaxError();

                        analyzedTokens.Add(new Token($"{start.CStr().value}:{end.CStr().value}", Token.Type.Slicer));
                    } 
                    else
                    {
                        Obj index = Calculator.Calculate(Lex(tokens[(i + 1)..j], properties), properties);

                        if (index is Str)
                            analyzedTokens.Add(new Token($"\"{index.CStr().value}\"", Token.Type.Indexer));
                        else
                            analyzedTokens.Add(new Token(index.CStr().value, Token.Type.Indexer));
                    }
                }
                else
                {
                    string value = "[";
                    depth = 1;

                    j = i + 1;

                    while (j < tokens.Count)
                    {
                        if (tokens[j].type == Token.Type.LBrack)
                            depth++;
                        if (tokens[j].type == Token.Type.RBrack)
                            depth--;
                        if (depth <= 0) break;
                        value += Token.IsOperator(tokens[j]) ? $" {tokens[j++].value} " : tokens[j++].value;
                    }

                    value += "]";

                    analyzedTokens.Add(new(value, Token.Type.Iterator));
                }

                i = j;
            }
            else if (tokens[i].type == Token.Type.Dot)
            {
                if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Class)
                    analyzedTokens[^1].type = Token.Type.Variable;

                analyzedTokens.Add(new(tokens[i + 1].value, Token.Type.Property));
                i++;
                if (tokens.Count > i + 1 && tokens[i + 1].type == Token.Type.LParen)
                    analyzedTokens[^1].type = Token.Type.Method;
            }
            else if (tokens[i].type == Token.Type.Lambda)
            {
                string keyword = "lambda ";
                string arg = "";
                string code = "";

                while (analyzedTokens[^1].type == Token.Type.Variable)
                {                       
                    arg += analyzedTokens[^1].value;
                    analyzedTokens.RemoveAt(analyzedTokens.Count - 1);

                    if (analyzedTokens.Count > 0 && analyzedTokens[^1].type == Token.Type.Comma)
                    {
                        arg += ",";
                        analyzedTokens.RemoveAt(analyzedTokens.Count - 1);
                    }
                    else break;
                }

                int j = i, depth = 0;

                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LParen)
                        depth++;
                    if (tokens[j].type == Token.Type.RParen)
                        depth--;
                    code += tokens[j].value;

                    if (depth <= 0 && tokens[j].type == Token.Type.RParen) break;
                    j++;
                }

                analyzedTokens.Add(new Token(keyword + arg + code, Token.Type.Lambda));

                i = j;
            }
            else if (tokens[i].type == Token.Type.LBrace)
            {
                int j = i + 1, depth = 1;
                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LBrace)
                        depth++;
                    if (tokens[j].type == Token.Type.RBrace)
                        depth--;
                    if (depth == 0) break;
                    j++;
                }

                if (i + 1 == j)
                    tokens.Add(new Token("{}", Token.Type.Dictionary));
                else
                {
                    string value = "";

                    for (int k = i; k <= j; k++)
                        value += tokens[k].value;

                    tokens.Add(new Token(value, Token.Type.Dictionary));
                }

                i = j;
            }
            else if (tokens[i].type == Token.Type.LParen)
            {
                if (analyzedTokens.Count == 0 ||
                   (analyzedTokens[^1].type != Token.Type.Function && 
                    analyzedTokens[^1].type != Token.Type.Method &&
                    analyzedTokens[^1].type != Token.Type.Class))
                {
                    analyzedTokens.Add(tokens[i]);
                    continue;
                }

                if (analyzedTokens[^1].type == Token.Type.Class)
                    analyzedTokens[^1].type = Token.Type.Function;

                string value = "[";
                int j = i + 1, depth = 1;

                while (j < tokens.Count)
                {
                    if (tokens[j].type == Token.Type.LParen)
                        depth++;
                    if (tokens[j].type == Token.Type.RParen)
                        depth--;
                    if (depth <= 0) break;
                    value += Token.IsOperator(tokens[j]) ? $" {tokens[j++].value} " : tokens[j++].value;
                }

                value += "]";

                analyzedTokens.Add(new(value, Token.Type.Iterator));

                i = j;
            }
            else analyzedTokens.Add(tokens[i]);

            if (analyzedTokens[^1].type == Token.Type.Variable && Process.IsClass(tokens[i]))
                analyzedTokens[^1].type = Token.Type.Class;

            if (analyzedTokens[^1].type == Token.Type.Variable && properties.TryGetValue(tokens[i].value, out var f) && f is Fun)
                analyzedTokens[^1].type = Token.Type.Function;
        }

        return analyzedTokens;
    }
}
