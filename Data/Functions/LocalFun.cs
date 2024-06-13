namespace Un.Data;

public class LocalFun : Fun
{
    public List<string> args = [];
    public string[] code = [];
    public int nesting = 0;

    public LocalFun(string name) : base(name) { }

    public LocalFun(string name, string[] code) : base(name)
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

        for (int i = 3; i < tokens.Count; i++)
        {
            if (tokens[i].type == Token.Type.RParen) break;
            if (tokens[i].type == Token.Type.Comma) continue;
            args.Add(tokens[i].Value);
        }

        this.name = name;
        this.code = code;
    }

    public override Obj Call(List args)
    {
        Field field = new(this.field);

        for (int i = 0; i < this.args.Count; i++)
            field.Set(this.args[i], args[i]);

        return Process.Interpret(name, code, field, [], line: 1, nesting: nesting);
    }

    public override LocalFun Clone()
    {
        return new(name)
        {            
            code = code,
            nesting = nesting,
            args = args,
            field = new(field)
        };
    }
}
