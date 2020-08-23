using Xamarin.Essentials;

namespace CommonsLib_APP.Settings
{
    /// <summary>
    /// Defines the settings for the Global App Tint.
    /// </summary>
    public static class GlobalTintSettings
    {
        private const string SettingsNamespace = "global_tint_settings";
        
        /// <summary>
        /// Whether or not to use global custom tint while loading app screens.
        /// </summary>
        public static bool UseCustomTint
        {
            get => Preferences.Get(nameof(UseCustomTint), false, SettingsNamespace);
            set => Preferences.Set(nameof(UseCustomTint), value, SettingsNamespace);
        }
        
        public static string ToolbarBackgroundColor
        {
            get => Preferences.Get(nameof(ToolbarBackgroundColor), "#315981", SettingsNamespace);
            set => Preferences.Set(nameof(ToolbarBackgroundColor), value, SettingsNamespace);
        }

        public static string ToolbarTitleColor
        {
            get => Preferences.Get(nameof(ToolbarTitleColor), "#FFFFFF", SettingsNamespace);
            set => Preferences.Set(nameof(ToolbarTitleColor), value, SettingsNamespace);
        }
        
        public static string SystemStatusColor
        {
            get => Preferences.Get(nameof(SystemStatusColor), "#1B3147", SettingsNamespace);
            set => Preferences.Set(nameof(SystemStatusColor), value, SettingsNamespace);
        }

        public static void Clear()
        {
            Preferences.Clear(SettingsNamespace);
        }
        
    }
}