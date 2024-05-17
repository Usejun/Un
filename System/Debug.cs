namespace Un;

public static class Debug
{
    public static void Assert(string massage) => throw new AssertError(massage);

    public static void Assert(bool condition, string massage)
    {
        if (condition) Assert(massage);
    }
}