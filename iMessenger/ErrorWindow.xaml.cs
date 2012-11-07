using System;
using System.Windows;
using System.Windows.Threading;

namespace iMessenger
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        public ErrorWindow()
        {
            InitializeComponent();
        }

        public void ShowError(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            Visibility = Visibility.Visible;
            Exception exception = args.Exception;
            ErrorMessage.Content = "Error: " + exception.Message;
            args.Handled = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
