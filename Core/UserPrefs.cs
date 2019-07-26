using Godot;

namespace Wayfarer.Core
{
    public class UserPrefs
    {
        [Export()] private Resource _general;
        [Export()] private Resource _gameplay;
        [Export()] private Resource _locale;
        [Export()] private Resource _video;
        [Export()] private Resource _audio;
        [Export()] private Resource _controls;

        public object GetUserPref(UserPrefCategory category, string prefKey)
        {
            // TODO:
            return null;
        }
    }
}