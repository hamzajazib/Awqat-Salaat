using System;
using System.Windows;

namespace AwqatSalaat.UI
{
    public class ThemeDictionaryDesignTime : ResourceDictionary
    {
        private static readonly Uri DarkUri = new Uri("/AwqatSalaat;component/UI/Themes/Dark.xaml", UriKind.RelativeOrAbsolute);
        private static readonly Uri StylesUri = new Uri("/AwqatSalaat;component/UI/Themes/Styles.xaml", UriKind.RelativeOrAbsolute);
        private static readonly Uri BrushesUri = new Uri("/AwqatSalaat;component/UI/Themes/Brushes.xaml", UriKind.RelativeOrAbsolute);
        private static readonly ResourceDictionary AccentsDictionary = new ResourceDictionary()
        {
            Source = new Uri("/AwqatSalaat;component/UI/Themes/Accents.xaml", UriKind.RelativeOrAbsolute)
        };

        public ThemeDictionaryDesignTime() : base()
        {
            if (!Helpers.Designer.IsInDesignMode())
            {
                return;
            }

            MergedDictionaries.Add(new ResourceDictionary { Source = BrushesUri });
            MergedDictionaries.Add(new ResourceDictionary { Source = StylesUri });

            var dictionary = new ResourceDictionary() { Source = DarkUri };

            foreach (var key in dictionary.Keys)
            {
                this[key] = dictionary[key];
            }

            dictionary = AccentsDictionary[$"GoldDark"] as ResourceDictionary;

            foreach (var key in dictionary.Keys)
            {
                this[key] = dictionary[key];
            }
        }
    }
}
