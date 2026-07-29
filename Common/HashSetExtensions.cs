using System.Collections.Generic;
using System.Text;

public static class HashSetExtensions
{
    public static string AllToString(this HashSet<string> hashset)
    {
        lock (hashset)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in hashset)
            {
                sb.Append($"{item}, ");
            }

            return sb.ToString();
        }
    }

    public static string AllToString(this HashSet<ushort> hashset)
    {
        lock (hashset)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in hashset)
            {
                sb.Append($"{item}, ");
            }

            return sb.ToString();
        }
    }
}