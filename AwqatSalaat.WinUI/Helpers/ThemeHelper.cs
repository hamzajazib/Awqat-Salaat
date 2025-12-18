using AwqatSalaat.Configurations;
using AwqatSalaat.Helpers;
using AwqatSalaat.Properties;
using Microsoft.UI.Xaml;
using Serilog;
using System;
using Windows.UI.ViewManagement;

namespace AwqatSalaat.WinUI.Helpers
{
    internal static class ThemeHelper
    {
        private static readonly UISettings s_uiSettings = new UISettings();

        public static ElementTheme GeneralTheme { get; private set; }
        public static ElementTheme ButtonTheme { get; private set; }

        public static event Action ThemeChanged;

        static ThemeHelper()
        {
            Settings.Realtime.PropertyChanged += Realtime_PropertyChanged;
            s_uiSettings.ColorValuesChanged += UISettings_ColorValuesChanged;

            OnThemeChanged(Settings.Realtime.Theme);
        }

        private static void Realtime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Theme))
            {
                var theme = Settings.Realtime.Theme;
                OnThemeChanged(theme);
            }
            else if (e.PropertyName == nameof(Settings.ThemeAccent))
            {
                var accent = Settings.Realtime.ThemeAccent;
                ((App)Application.Current).OverrideAccentColor(accent.ToString());
            }
        }

        private static void UISettings_ColorValuesChanged(UISettings sender, object args)
        {
            OnThemeChanged(Settings.Realtime.Theme);
        }

        private static void OnThemeChanged(ThemeKey themeKey)
        {
            ButtonTheme = GetButtonTheme();
            GeneralTheme = GetElementTheme(themeKey);

            ThemeChanged?.Invoke();
        }

        private static ElementTheme GetButtonTheme()
        {
            if (SystemInfos.IsAccentColorOnTaskBar() == true)
            {
                // When accent color is used, we have to figure out the theme based on the color
                var accent = s_uiSettings.GetColorValue(UIColorType.Accent);
                Log.Information($"Accent color on taskbar: R={accent.R}, G={accent.G}, B={accent.B}");
                bool colorIsDark = (5 * accent.G + 2 * accent.R + accent.B) <= 8 * 200;
                return colorIsDark ? ElementTheme.Dark : ElementTheme.Light;
            }
            else
            {
                // We use "system theme" instead of "apps theme" because the taskbar uses the former
                return SystemInfos.IsLightThemeUsed() == true ? ElementTheme.Light : ElementTheme.Dark;
            }
        }

        public static void ReloadElementTheme(FrameworkElement element, ElementTheme startTheme)
        {
            if (element.RequestedTheme == ElementTheme.Dark)
                element.RequestedTheme = ElementTheme.Light;
            else if (element.RequestedTheme == ElementTheme.Light)
                element.RequestedTheme = ElementTheme.Default;
            else if (element.RequestedTheme == ElementTheme.Default)
                element.RequestedTheme = ElementTheme.Dark;

            if (element.RequestedTheme != startTheme)
                ReloadElementTheme(element, startTheme);
        }

        public static ElementTheme GetElementTheme(ThemeKey key) => key switch
        {
            ThemeKey.Auto => ElementTheme.Default,
            ThemeKey.Dark => ElementTheme.Dark,
            ThemeKey.Light => ElementTheme.Light,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
