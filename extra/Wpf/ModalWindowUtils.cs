using System.Windows;
using Control = System.Windows.Controls.Control;
using TextBox = System.Windows.Controls.TextBox;

namespace Ara3D.Utils.Wpf
{
    public static class ModalWindowUtils
    {
        public static Window CreateWindow(float width, float height, Control control)
        {
            var r = new Window
            {
                Width = width,
                Height = height,
                Content = control
            };
            r.Show();
            return r;
        }

        public static Window CreateTextBlockWindow(string text)
            => CreateWindow(600, 600, new TextBox
            {
                Text = text, 
                AcceptsReturn = true, 
                TextWrapping = TextWrapping.Wrap
            });
    
    }
}
