using System.Globalization;

namespace Konpanion
{
    internal static class Utilities
    {
        public static string f2s(float x)
        {
            return x.ToString("0.00", CultureInfo.InvariantCulture);
        }
        public static float s2f(string v)
        {
            return float.Parse(v, CultureInfo.InvariantCulture);
        }
        public static int s2i(string s)
        {
            return int.Parse(s, CultureInfo.InvariantCulture);
        }

        public static string i2s(int i)
        {
            return i.ToString(CultureInfo.InvariantCulture);
        }
    }
}
