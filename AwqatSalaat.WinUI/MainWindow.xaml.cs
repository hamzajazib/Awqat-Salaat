using AwqatSalaat.WinUI.Xaml;

namespace AwqatSalaat.WinUI
{
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            this.InitializeComponent();
            widget.DisplayModeChanged += Widget_DisplayModeChanged;
        }

        private void Widget_DisplayModeChanged(Views.DisplayMode displayMode)
        {
            if (displayMode is Views.DisplayMode.Compact or Views.DisplayMode.CompactNoCountdown)
            {
                widget.Width = 62;
            }
            else
            {
                widget.Width = 118;
            }
        }
    }
}
