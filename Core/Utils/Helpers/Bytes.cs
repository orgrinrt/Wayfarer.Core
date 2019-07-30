namespace Wayfarer.Core.Utils.Helpers
{
    public class Bytes
    {
        public static int Combine(byte b1, byte b2)
        {
            int combined = b1 << 8 | b2;
            return combined;
        }
    }
}