namespace Un
{
    public static class Extension
    {
        public static int Digit(this int n)
        {
            int digit = 0;

            while (n != 0)
            {
                n /= 10;
                digit++;
            }

            return digit;
        }
    }
}
