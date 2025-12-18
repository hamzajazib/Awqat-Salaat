using AwqatSalaat.Configurations;
using AwqatSalaat.Helpers;
using AwqatSalaat.Properties;
using Serilog;
using System;

namespace AwqatSalaat.UI
{
    public class ThemeChangedEventArgs : EventArgs
    {
        public bool ButtonThemeChanged { get; }
        public bool GeneralThemeChanged { get; }
        public bool AccentChanged { get; }

        public ThemeChangedEventArgs(bool buttonThemeChanged = false, bool generalThemeChanged = false, bool accentChanged = false)
        {
            if (!(buttonThemeChanged || generalThemeChanged || accentChanged))
            {
                throw new InvalidOperationException();
            }

            ButtonThemeChanged = buttonThemeChanged;
            GeneralThemeChanged = generalThemeChanged;
            AccentChanged = accentChanged;
        }
    }

    public static class ThemeManager
    {
        private static ThemeKey _generalTheme = ThemeKey.Dark;
        private static ThemeKey _buttonTheme = ThemeKey.Dark;
        private static string _accent = "Gold";

        public static ThemeKey GeneralTheme
        {
            get => _generalTheme;
            private set
            {
                if (_generalTheme == value) return;

                _generalTheme = value;
                Changed?.Invoke(null, new ThemeChangedEventArgs(generalThemeChanged: true));
            }
        }

        public static ThemeKey ButtonTheme
        {
            get => _buttonTheme;
            private set
            {
                if (_buttonTheme == value) return;

                _buttonTheme = value;
                Changed?.Invoke(null, new ThemeChangedEventArgs(buttonThemeChanged: true));
            }
        }

        public static string Accent
        {
            get => _accent;
            private set
            {
                if (_accent == value) return;

                _accent = value;
                Changed?.Invoke(null, new ThemeChangedEventArgs(accentChanged: true));
            }
        }

        public static event EventHandler<ThemeChangedEventArgs> Changed;

        static ThemeManager()
        {
            if (!Designer.IsInDesignMode())
            {
                SyncButtonWithSystemTheme();
                SetGeneralTheme(Settings.Default.Theme);
            }

            Accent = Settings.Default.ThemeAccent.ToString();
            Settings.Realtime.PropertyChanged += Realtime_PropertyChanged;
        }

        private static void Realtime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Theme))
            {
                var value = Settings.Realtime.Theme;

                SetGeneralTheme(value);
            }
            else if (e.PropertyName == nameof(Settings.ThemeAccent))
            {
                Accent = Settings.Realtime.ThemeAccent.ToString();
            }
        }

        public static void ToggleTheme()
        {
            SetGeneralTheme(_generalTheme == ThemeKey.Dark ? ThemeKey.Light : ThemeKey.Dark);
        }

        public static void SetGeneralTheme(ThemeKey theme)
        {
            if (theme == ThemeKey.Auto)
            {
                SyncGeneralWithAppsTheme();
            }
            else
            {
                Log.Information($"Setting general theme: {theme}");
                GeneralTheme = theme;
            }
        }

        public static void SyncButtonWithSystemTheme()
        {
            ThemeKey theme;

            if (SystemInfos.IsAccentColorOnTaskBar() == true)
            {
                // When accent color is used, we have to figure out the theme based on the color
                var accent = SystemInfos.GetAccentColor();
                bool colorIsDark = (5 * accent.g + 2 * accent.r + accent.b) <= 8 * 200;
                theme = colorIsDark ? ThemeKey.Dark : ThemeKey.Light;
                Log.Information($"Accent color on taskbar: R={accent.r}, G={accent.g}, B={accent.b}");
            }
            else
            {
                // We use "system theme" instead of "apps theme" because the taskbar uses the former
                theme = SystemInfos.IsLightThemeUsed() == true ? ThemeKey.Light : ThemeKey.Dark;
            }

            Log.Information($"Setting button theme: {theme}");

            ButtonTheme = theme;
        }

        public static void SyncGeneralWithAppsTheme()
        {
            if (Settings.Realtime.Theme == ThemeKey.Auto)
            {
                ThemeKey theme = SystemInfos.IsAppsLightThemeUsed() == true ? ThemeKey.Light : ThemeKey.Dark;

                Log.Information($"Setting general theme (Auto): {theme}");

                GeneralTheme = theme;
            }
        }
    }
}
