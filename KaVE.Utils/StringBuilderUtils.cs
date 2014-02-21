using System.Text;

namespace KaVE.Utils
{
    public static class StringBuilderUtils
    {
        public static void AppendIf(this StringBuilder identifier, bool condition, string stringToAppend)
        {
            if (condition)
            {
                identifier.Append(stringToAppend);
            }
        }
    }
}
