using AwqatSalaat.WinUI.Helpers;
using Microsoft.UI.Xaml;

namespace AwqatSalaat.WinUI.Xaml
{
    public class WindowEx : Window
    {
        public WindowEx() : base()
        {
            ThemeHelper.ThemeChanged += ThemeHelper_ThemeChanged;
            this.Activated += WindowEx_Activated;
            this.Closed += WindowEx_Closed;
        }

        private void WindowEx_Closed(object sender, WindowEventArgs args)
        {
            ThemeHelper.ThemeChanged -= ThemeHelper_ThemeChanged;
        }

        private void WindowEx_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState != WindowActivationState.Deactivated)
            {
                this.Activated -= WindowEx_Activated;
                ThemeHelper_ThemeChanged();
            }
        }

        private void ThemeHelper_ThemeChanged()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (Content is FrameworkElement frameworkElement)
                {
                    frameworkElement.RequestedTheme = ThemeHelper.GeneralTheme;
                }
            });
        }
    }
}
