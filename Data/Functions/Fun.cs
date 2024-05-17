namespace Un.Data;

public class Fun : Obj
{
    public string name = "";
    public List<string> args = [];
    public string[] code = [];
    public int nesting = 0;

    public Fun() : base("func") { }

    public Fun(string[] code) : base("func")
    {
        this.code = code;
        List<Token> tokens = Tokenizer.Tokenize(code[0]);
        int i = 3;
        name = tokens[1].value;

        try
        {
            while (tokens[i].type != Token.Type.RParen)
            {
                if (tokens[i].type != Token.Type.Comma)
                    args.Add(tokens[i].value);
                i++;
            }

            foreach (var chr in code[0])
                if (chr != '\t') break;
                else nesting++;
        }
        catch
        {
            throw new SyntaxError("invalid function syntax");
        }
    }

    public virtual Obj Call(Iter args)
    {
        Parser interpreter = new(code, properties, line: 1, nesting: nesting + 1);

        for (int i = 0; i < this.args.Count; i++)
            properties.Add(this.args[i], args[i]);

        while (interpreter.TryInterpret()) ;

        properties.Clear();

        return interpreter.ReturnValue ?? None;
    }

    public override Fun Clone()
    {
        return new()
        {
            name = name,
            code = code,
            nesting = nesting,
            args = args,
            properties = []
        };
    }

    public override Str CStr() => new(name);

    public override int GetHashCode() => name.GetHashCode();
}
