using Godot;

namespace Wayfarer.Core.Utils.Localization
{
    public class Lang
    {
        public static string Locale => GetCurrLocale();
        
        public static string Get(string key)
        {
            // wrapper for the Godot's Tr system
            if (TranslationServer.Translate(key) != string.Empty)
            {
                return TranslationServer.Translate(key);
            }
            return key;
        }

        public static void SetLocale(string localeKey)
        {
            TranslationServer.SetLocale(localeKey);
        }

        public static string GetCurrLocale()
        {
            return TranslationServer.GetLocale();
        }
    }
}