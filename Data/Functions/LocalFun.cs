using Un.Interpreter;

namespace Un.Data;

public class LocalFun : Fun
{    
    protected string[] code = [];
    protected int nesting = 0;

    public LocalFun(string name) : base(name) { }

    public LocalFun(string name, string[] code) : base(name)
    {
        int tab = 0, white = 0;

        foreach (char c in code[0])
        {
            if (c == Literals.Tab) tab++;
            else if (c == Literals.Space) white++;
            else break;
        }

        nesting = Math.Max(tab, white / 4) + 1;
        
        var args = Parameter(code[0]);

        Args = [];

        if (!args.IsArgument())
            throw new SyntaxError("invalid arguments position");

        for (int i = 0; i < args.Count; i++)
            Args.Add(args.Names[i], args[i]);

        if (args.Count != 0 && args.Names[^1].StartsWith(Literals.Asterisk))
        {
            IsDynamic = true;
            Args[Args.Len - 1] = (args.Names[^1][1..], args[^1]); 
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
