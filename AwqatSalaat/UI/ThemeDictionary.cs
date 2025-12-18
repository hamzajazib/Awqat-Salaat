using AwqatSalaat.Configurations;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AwqatSalaat.UI
{
    public class ThemeDictionary : ResourceDictionary
    {
        private static readonly Dictionary<ThemeKey, Uri> Sources = new Dictionary<ThemeKey, Uri>()
        {
            [ThemeKey.Dark] = new Uri("/AwqatSalaat;component/UI/Themes/Dark.xaml", UriKind.RelativeOrAbsolute),
            [ThemeKey.Light] = new Uri("/AwqatSalaat;component/UI/Themes/Light.xaml", UriKind.RelativeOrAbsolute)
        };
        private static readonly Uri StylesUri = new Uri("/AwqatSalaat;component/UI/Themes/Styles.xaml", UriKind.RelativeOrAbsolute);
        private static readonly Uri BrushesUri = new Uri("/AwqatSalaat;component/UI/Themes/Brushes.xaml", UriKind.RelativeOrAbsolute);
        private static readonly ResourceDictionary AccentsDictionary = new ResourceDictionary()
        {
            Source = new Uri("/AwqatSalaat;component/UI/Themes/Accents.xaml", UriKind.RelativeOrAbsolute)
        };

        private bool _applyToButton;

        public bool ApplyToButton
        {
            get => _applyToButton;
            set
            {
                if (_applyToButton != value)
                {
                    _applyToButton = value;
                    ThemeSource_Changed(null, null);
                }
            }
        }

        public ThemeDictionary() : base()
        {
            Source = BrushesUri;
            MergedDictionaries.Add(new ResourceDictionary { Source = StylesUri });
            ThemeManager.Changed += ThemeSource_Changed;
            ThemeSource_Changed(null, null);
        }

        public static ResourceDictionary GetAccentDictionary(ThemeKey theme, string accent)
        {
            return AccentsDictionary[$"{accent}{theme}"] as ResourceDictionary;
        }

        private void ThemeSource_Changed(object sender, ThemeChangedEventArgs args)
        {
            var themeKey = ApplyToButton ? ThemeManager.ButtonTheme : ThemeManager.GeneralTheme;
            var dictionary = new ResourceDictionary() { Source = Sources[themeKey] };

            foreach (var key in dictionary.Keys)
            {
                this[key] = dictionary[key];
            }

            dictionary = GetAccentDictionary(themeKey, ThemeManager.Accent);

            foreach (var key in dictionary.Keys)
            {
                this[key] = dictionary[key];
            }
        }

        ~ThemeDictionary()
        {
            ThemeManager.Changed -= ThemeSource_Changed;
        }
    }
}
