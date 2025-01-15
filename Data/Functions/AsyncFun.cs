using Un.Interpreter;

namespace Un.Data;

public class AsyncFun : LocalFun
{
    public AsyncFun(string name) : base(name) { }

    public AsyncFun(string name, string[] code) : base(name)
    {
        int tab = 0, white = 0;

        foreach (char c in code[0])
        {
            if (c == '\t') tab++;
            else if (c == ' ') white++;
            else break;
        }

        nesting = Math.Max(tab, white / 4) + 1;

        var tokens = Tokenizer.Tokenize(code[0]);
        bool isDefault = false;
        int i = 4;

        while (i < tokens.Count)
        {
            var token = tokens[i];

            if (token.type == Token.Type.RParen) break;
            else if (token.type == Token.Type.Comma)
            {
                i++;
                continue;
            }
            else if (token.type == Token.Type.Asterisk)
            {
                isDefault = true;
                IsDynamic = true;
                args.Add(tokens[i + 1].Value);
                i++;
            }
            else if (!IsDynamic && tokens[i + 1].type == Token.Type.Assign)
            {
                isDefault = true;
                args.Add(token.Value);

                int j = i + 2, depth = 0;
                List<Token> buffer = [];

                while (j < tokens.Count)
                {
                    if (depth == 0 && (tokens[j].type == Token.Type.Comma || tokens[j].type == Token.Type.RParen)) break;
                    if (tokens[j].type == Token.Type.LParen || tokens[j].type == Token.Type.LBrack) ++depth;
                    if (tokens[j].type == Token.Type.RParen || tokens[j].type == Token.Type.RBrack) --depth;
                    buffer.Add(tokens[j++]);
                }

                field.Set(token.Value, Convert(Token.String(buffer), new()));
                i = j;
            }
            else if (!isDefault && !IsDynamic)
            {
                Length++;
                args.Add(token.Value);
            }
            else throw new SyntaxError("invalid argrements");
            i++;
        }

        Name = name;
        this.code = code;
    }

    public override Obj Call(Collections.Tuple args, Field field)
    {
        Field local = new(this.field);

        for (int i = 0; i < this.args.Count; i++)
            local.Set(this.args[i], args[i]);

        return new Task(new Task<Obj>(() => {
            Parser sub = new(code, new(local), line:1, nesting: nesting);

            while (sub.TryInterpret()) { };

            return sub.ReturnValue ?? None;
        }));
    }

    public override AsyncFun Clone()
    {
        return new(Name)
        {
            code = code,
            nesting = nesting,
            args = args,
            field = new(field)
        };
    }
}
