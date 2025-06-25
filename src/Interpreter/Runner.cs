using Un;
using Un.Object;
using Un.Object.Collections;

public class Runner()
{
    public Context Context { get; private set; }

    public Obj Run()
    {
        Obj returned = Obj.None;
        try
        {
            var tokenizer = new Tokenizer();
            var lexer = new Lexer();
            var parser = new Parser(Context);

            while (!Context.File.EOF && parser.ReturnValue is null)
            {
                var tokens = tokenizer.Tokenize(Context.File);
                var nodes = lexer.Lex(tokens);
                returned = parser.Parse(nodes);

                if (Context.File.EOL)
                    Context.File.Move(0, Context.File.Line + 1);
            }

            returned = parser.ReturnValue;

        }
        finally
        {
            Free();
        }

        return returned;
    }

    private void Free()
    {
        if (Context.Scope.TryGetValue("__using__", out var usings))
        {
            foreach (var obj in usings.As<List>())
                obj.Exit();
        }
    }

    public static Runner Load(string file, Scope scope, string path = "/src/")
    {
        var topPath = Path.Combine("/workspaces/Un/" + path);
        var allPath = Path.Combine(topPath, file);
        var name = file[..^3];

        if (!Path.Exists(allPath))
            throw new Panic($"file {file} not found in {topPath}");

        return new()
        {
            Context = new(scope,  new UnFile(name, File.ReadAllLines(allPath)))
        };
    }

    public static Runner Load(string name, string[] body, Scope scope) => new()
    {
        Context = (scope,  new UnFile(name, body))
    };
}
