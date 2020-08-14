using Xamarin.Essentials;

namespace CommonsLib_APP.Settings
{
    /// <summary>
    /// Defines the settings for the Global App Tint.
    /// </summary>
    public static class GlobalTintSettings
    {
        /// <summary>
        /// Whether or not to use global custom tint while loading app screens.
        /// </summary>
        public static bool UseCustomTint
        {
            get => Preferences.Get(nameof(UseCustomTint), false);
            set => Preferences.Set(nameof(UseCustomTint), value);
        }
        
        public static string ToolbarBackgroundColor
        {
            get => Preferences.Get(nameof(ToolbarBackgroundColor), "#315981");
            set => Preferences.Set(nameof(ToolbarBackgroundColor), value);
        }

        public static string ToolbarTitleColor
        {
            get => Preferences.Get(nameof(ToolbarTitleColor), "#FFFFFF");
            set => Preferences.Set(nameof(ToolbarTitleColor), value);
        }
        
        public static string SystemStatusColor
        {
            get => Preferences.Get(nameof(SystemStatusColor), "#1B3147");
            set => Preferences.Set(nameof(SystemStatusColor), value);
        }
        
    }
}