using System.ComponentModel;
using System.Windows;

namespace AwqatSalaat.Helpers
{
    internal static class Designer
    {
        private static readonly DependencyObject _depObj = new DependencyObject();

        public static bool IsInDesignMode() => DesignerProperties.GetIsInDesignMode(_depObj);
    }
}
