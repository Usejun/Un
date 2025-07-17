using Un;
using Un.Object;
using Un.Object.Primitive;

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
        catch (Exception e)
        {
            if (Context.InBlock("try"))
            {
                throw new Error(e.Message, Context);
            }
            else
            {
                var error = new Error(e.Message, Context); 
                Context.Scope["log"].Call(new ([new Str(error.ToString())], [""]));
            }
        }
        finally
        {
            Free();
            Defer();
        }

        if ((returned?.Type == "skip" || returned?.Type == "break") && Context.CurrentBlock != "loop")
            throw new Panic($"'{returned?.Type}' keyword can only be used inside a loop");

        return returned;
    }

    private void Free()
    {
        foreach (var obj in Context.Usings)
        {
            obj.Exit();
        }
    }

    private void Defer()
    {
        foreach (var nodes in Context.Defers)
        {
            var parser = new Parser(new Context(Context.Scope, new("defer", []), []));
            parser.Parse(nodes);
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
            Context = new(scope, new UnFile(name, File.ReadAllLines(allPath)), [])
        };
    }

    public static Runner Load(Context context) => new()
    {
        Context = context
    };
}
