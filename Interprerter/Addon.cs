namespace Un
{
    public static class Addon
    {
        public static bool TryInt(this long l, out int i)
        {
            i = 0;
            if (l < int.MinValue || l > int.MaxValue)
                return false;
            i = (int)l;
            return true;
        }
    }
}
