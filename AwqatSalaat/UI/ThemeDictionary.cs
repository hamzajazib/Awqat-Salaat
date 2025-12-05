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

        public ThemeDictionary() : base()
        {
            Source = BrushesUri;
            MergedDictionaries.Add(new ResourceDictionary { Source = StylesUri });
            ThemeManager.Changed += ThemeSource_Changed;
            ThemeSource_Changed();
        }

        public static ResourceDictionary GetAccentDictionary(ThemeKey theme, string accent)
        {
            return AccentsDictionary[$"{accent}{theme}"] as ResourceDictionary;
        }

        private void ThemeSource_Changed()
        {
            var dictionary = new ResourceDictionary() { Source = Sources[ThemeManager.Current] };

            foreach (var key in dictionary.Keys)
            {
                this[key] = dictionary[key];
            }

            dictionary = GetAccentDictionary(ThemeManager.Current, ThemeManager.Accent);

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
