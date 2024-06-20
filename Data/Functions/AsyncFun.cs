namespace Un.Data;

public class AsyncFun : LocalFun
{
    public AsyncFun(string name) : base(name) { }

    public AsyncFun(string name, string[] code) : base(name)
    {
        int tab = 0, white = 0;

        for (int i = 0; i < code[0].Length; i++)
            if (code[0][i] == '\t') tab++;
            else break;

        for (int i = 0; i < code[0].Length; i++)
            if (code[0][i] == ' ') white++;
            else break;

        nesting = Math.Max(tab, white / 4) + 1;

        var tokens = Tokenizer.Tokenize(code[0]);

        for (int i = 4; i < tokens.Count; i++)
        {
            if (tokens[i].type == Token.Type.RParen) break;
            if (tokens[i].type == Token.Type.Comma) continue;
            args.Add(tokens[i].Value);
        }

        Name = name;
        this.code = code;
    }

    public override Obj Call(Collections.Tuple args)
    {
        Field field = new(this.field);

        for (int i = 0; i < this.args.Count; i++)
            field.Set(this.args[i], args[i]);

        return new Task(new Task<Obj>(() => {
            Parser sub = new(code, new(field), line:1, nesting: nesting);

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
