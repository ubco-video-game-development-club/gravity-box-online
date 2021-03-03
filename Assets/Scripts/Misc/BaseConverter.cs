using System.Text;

public static class BaseConverter
{
    private const char ZERO = '0';
    private const char TEN = 'a';
    private const char THIRTY_SIX = 'A';

    public static string ToBase(long n, long b)
    {
        StringBuilder sb = new StringBuilder();

        while(n > 0)
        {
            long p = n % b;
            char digit;

            if(p < 10) digit = (char)(ZERO + p);
            else if(p < 36) digit = (char)(TEN + (p - 10));
            else if(p < 82) digit = (char)(THIRTY_SIX + (p - 36));
            else return null;

            sb.Insert(0, digit);

            n /= b;
        }

        return sb.ToString();
    }
}
