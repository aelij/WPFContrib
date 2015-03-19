using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Avalon.Windows.Controls;

namespace WpfContribTest.Pages
{
    /// <summary>
    /// Interaction logic for InlineDialogs.xaml
    /// </summary>
    public partial class InlineDialogs : Page
    {
        private readonly Random _rng = new Random();

        public InlineDialogs()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InlineModalDialog
            {
                Owner = this,
                Content = new DialogSampleContent(),
                Width = _rng.Next(100, (int)ActualWidth),
                Height = _rng.Next(100, (int)ActualHeight),
            };
            dialog.InputBindings.Add(new KeyBinding {Key = Key.Escape, Command = InlineModalDialog.CloseCommand});
            dialog.Show();
        }
    }

    internal class DialogSampleContent
    {
    }
}
