namespace Un.Data;

public class Fun : Obj
{
    public string name = "";
    public List<string> args = [];
    public string[] code = [];
    public int nesting = 0;

    public Fun() : base("func") { }

    public Fun(string name, string[] code) : base("func")
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
            args.Add(tokens[i].value);
        }

        this.name = name;
        this.code = code;
    }

    public virtual Obj Call(Iter args)
    {
        Field field = new(this.field);

        for (int i = 0; i < this.args.Count; i++)
            field.Set(this.args[i], args[i]);

        return Process.Interpret(name, code, field, [], line:1, nesting:nesting);
    }

    public override Fun Clone()
    {
        return new()
        {
            name = name,
            code = code,
            nesting = nesting,
            args = args,
            field = new(field)
        };
    }

    public override Str CStr() => new(name);

    public override int GetHashCode() => name.GetHashCode();
}
