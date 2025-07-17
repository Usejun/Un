using Un.Object;

namespace Un;

public class Context(Scope scope, UnFile file, Stack<string> blockStack)
{
    public Scope Scope { get; set; } = scope;
    public UnFile File { get; set; } = file;
    public Stack<string> BlockStack = blockStack;
    public List<Node> Annotations { get; set; } = [];
    public Stack<List<Node>> Defers { get; set; } = [];
    public Stack<Obj> Usings { get; set; } = [];

    public string? CurrentBlock => BlockStack.TryPeek(out var top) ? top : null;


    public void EnterBlock(string type) => BlockStack.Push(type);

    public void ExitBlock()
    {
        if (BlockStack.Count > 0)
            BlockStack.Pop();
    }

    public bool InBlock(string type) => BlockStack.Count > 0 && BlockStack.Contains(type);

    public string[] BlockStackTrace => [.. BlockStack];
}
