using Un;

Runner runner = null!;

try
{
    if (args.Length < 2)
        throw new Panic("not enough arguments");
    

    Global.Init(args[0]);
    runner = Runner.Load(args[1], Global.GetGlobalScope());
    runner.Run();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    if (runner is not null && runner.Context.BlockStackTrace.Length > 1)
    {
        Console.WriteLine("    trace:");

        var blockStack = runner.Context.BlockStackTrace;
        foreach (var block in blockStack.Length > 11 ? blockStack.Take(10).Skip(1).Reverse() : blockStack.Reverse().Skip(1))
            Console.WriteLine($"\t{block.Code.Trim()}:[{block.Line + 1}] ({block.Type})");
        if (blockStack.Length > 10)
            Console.WriteLine("\t... (truncated)");
    }

}