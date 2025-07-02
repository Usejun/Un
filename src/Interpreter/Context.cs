using Un.Object.Collections;

namespace Un;

public class Context(Scope scope, UnFile file)
{
    public Scope Scope { get; set; } = scope;
    public UnFile File { get; set; } = file;

    public string? CurrentBlock => blockStack.TryPeek(out var top) ? top : null;

    private readonly Stack<string> blockStack = new();

    /// <summary>
    /// 현재 블록 스택에 블록을 추가합니다.
    /// </summary>
    public void EnterBlock(string type) => blockStack.Push(type);

    /// <summary>
    /// 가장 마지막에 들어간 블록을 제거합니다.
    /// </summary>
    public void ExitBlock()
    {
        if (blockStack.Count > 0)
            blockStack.Pop();
    }

    /// <summary>
    /// 현재 어떤 블록 안에 있는지 검사합니다.
    /// </summary>
    public bool InBlock(string type) => CurrentBlock == type;


    /// <summary>
    /// 현재 루프 안인지 확인합니다.
    /// </summary>
    public bool InLoop => InBlock("loop");

    /// <summary>
    /// 현재 try-catch 안인지 확인합니다.
    /// </summary>
    public bool InTry => InBlock("try");

    /// <summary>
    /// 현재 function 안인지 확인합니다.
    /// </summary>
    public bool InFunction => InBlock("fn");

    /// <summary>
    /// 현재 모든 블록 스택 반환 (디버깅 용도)
    /// </summary>
    public string[] BlockStackTrace => [..blockStack];
}
