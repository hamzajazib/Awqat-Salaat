using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AwqatSalaat.WinUI.Controls
{
    internal class ComboBoxEx : ComboBox
    {
        public ComboBoxEx() : base()
        {
            // Workaround for a bug https://github.com/microsoft/microsoft-ui-xaml/issues/4035
            RegisterPropertyChangedCallback(ItemsSourceProperty, OnItemsSourceChanged);

            DefaultStyleKey = typeof(ComboBoxEx);
        }

        private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (!string.IsNullOrEmpty(SelectedValuePath))
            {
                var path = SelectedValuePath;
                SelectedValuePath = null;
                SelectedValuePath = path;
            }
        }
    }
}
