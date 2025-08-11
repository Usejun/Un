using Un;

Global.Init(args[0]);
Runner runner = Runner.Load(args[1], Global.GetScope());


try
{
    runner.Run();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
    Console.WriteLine("    trace:");
    foreach (var block in runner.Context.BlockStackTrace)
        Console.WriteLine($"\t{block.Code.Trim()}:[{block.Line}] ({block.Type})");

}