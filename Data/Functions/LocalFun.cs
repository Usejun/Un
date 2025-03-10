using Un.Interpreter;

namespace Un.Data;

public class LocalFun : Fun
{    
    protected string[] code = [];
    protected int nesting = 0;

    public LocalFun(string name) : base(name) { }

    public LocalFun(string name, string[] code, int i = 3) : base(name)
    {
        int tab = 0, white = 0;

        foreach (char c in code[0])
        {
            if (c == Literals.Tab) tab++;
            else if (c == Literals.Space) white++;
            else break;
        }

        nesting = Math.Max(tab, white / 4) + 1;
        Args = [];

        var tokens = Tokenizer.Tokenize(code[0]);
        bool isDefault = false;

        while (i < tokens.Count)
        {
            var token = tokens[i];

            if (token.type == Token.Type.RParen) break;
            else if (token.type == Token.Type.Comma) { }
            else if (token.type == Token.Type.Asterisk)
            {
                isDefault = true;
                IsDynamic = true;
                Args.Add((tokens[i + 1].value.ToString(), new List()));
                i++;
            }
            else if (!IsDynamic && tokens[i + 1].type == Token.Type.Assign)
            {
                isDefault = true;

                int j = i + 2, depth = 0;
                List<Token> buffer = [];

                while (j < tokens.Count)
                {
                    if (depth == 0 && (tokens[j].type == Token.Type.Comma || tokens[j].type == Token.Type.RParen)) break;
                    if (tokens[j].type == Token.Type.LParen || tokens[j].type == Token.Type.LBrack) ++depth;
                    if (tokens[j].type == Token.Type.RParen || tokens[j].type == Token.Type.RBrack) --depth;
                    buffer.Add(tokens[j++]);
                }

                var value = Parse(Token.String(buffer), new());

                field.Set(token.value, value);
                Args.Add((token.value.ToString(), value));
                i = j;
            }
            else if (!isDefault && !IsDynamic)
            {
                Length++;
                Args.Add((token.value.ToString(), null!));
            }
            else throw new SyntaxError("invalid argrements");
            i++;
        }

        Name = name;
        this.code = code;
    }

    public override Obj Call(Field field)
    {
        Field local = new(this.field);
        local.Merge(field);

        return Process.Interpret(Name, code, local, [], line: 1, nesting: nesting);
    }

    public override LocalFun Clone()
    {
        return new(Name)
        {            
            IsDynamic = IsDynamic,
            code = code,
            nesting = nesting,
            Args = Args,
            field = new(field)
        };
    }
}
