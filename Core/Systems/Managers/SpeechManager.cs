namespace Wayfarer.Systems.Managers
{
    public class SpeechManager
    {
        public static string TextPartAudioPath(string txtKey, int partIndex)
        {
            // TODO: query audio PATH from Lang.db and return it as res:// path
            return "";
        }

        public static string Text(string txtKey, int partIndex)
        {
            // TODO: query a string from Lang.db with PART value of /part/ and return it as final string
            return "";
        }

        public static int TextPartCount(string txtKey)
        {
            // TODO: query a PART_TOTAL count of a txtKey sequence
            return 0;
        }
    }
}