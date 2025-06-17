using Un.Object.Collections;

namespace Un.Object.Function;

public class LFn : Fn
{
    public string[] Body { get; set; }

    public override Obj Call(Tup args)
    {
        var scope = new Scope();
        Bind(scope, args);
        var file = new UnFile(Name, Body);
        var tokenizer = new Tokenizer();
        var lexer = new Lexer();
        var parser = new Parser(scope);
        var returned = None;

        while (!file.EOF)
        {
            var tokens = tokenizer.Tokenize(file);
            var nodes = lexer.Lex(tokens);
            returned = parser.Parse(nodes);

            if (file.EOL)
                file.Move(0, file.Line + 1);
        }

        if (scope.TryGetValue("__using__", out var usings))
            foreach (var obj in usings.ToList())
                obj.Exit();

        return returned;
    }
}