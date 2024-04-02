namespace Un
{
    public static class Util
    {
        public static void Write<T>(T[] values)
        {
            Console.WriteLine(string.Join(" ", values));
        }
    }
}
