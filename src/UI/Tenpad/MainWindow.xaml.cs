using System;
using System.Windows;
using System.Windows.Media;

namespace Tenpad
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WindowControlBox_MinButton.Click += WindowControlBox_MinButtonOnClick;
            WindowControlBox_ExpButton.Click += WindowControlBox_ExpButtonOnClick;
            WindowControlBox_CloseButton.Click += WindowControlBox_CloseButtonOnClick;
            StateChanged += OnStateChanged;
        }

        private void WindowControlBox_MinButtonOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void WindowControlBox_ExpButtonOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            if (WindowState == WindowState.Maximized)
            {
                ExpButtonIcon.Source = Application.Current.FindResource("WinCtlBoxRestoreIcon") as DrawingImage;
            }
            else
            {
                ExpButtonIcon.Source = Application.Current.FindResource("WinCtlBoxExpandIcon") as DrawingImage;
            }
        }

        private void WindowControlBox_CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            LayoutGrid.Margin = WindowState == WindowState.Maximized
                ? new Thickness(6,6,6,6)
                : new Thickness(0, 0, 0, 0);
        }
    }
}
