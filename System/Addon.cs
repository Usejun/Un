namespace Un
{
    public static class Addon
    {
        public static void Assert(bool condition, string massage)
        {
            if (condition) throw new AssertException(massage); 
        }
    }
}
