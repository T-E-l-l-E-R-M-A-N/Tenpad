using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MuvUi.Controls
{
    public class Window : System.Windows.Window
    {
        #region Private Fields

        private Panel MainView;

        private ButtonBase PART_WindowControlBox_CloseButton;
        private Button PART_WindowControlBox_ExpandButton;
        private ButtonBase PART_WindowControlBox_MinimizeButton;

        private Border PART_Window_NonClientAreaBox;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty NonClientAreaContentProperty = DependencyProperty.Register(
            "NonClientAreaContent", typeof(FrameworkElement), typeof(Window), new PropertyMetadata(default(FrameworkElement)));

        #endregion

        #region Public Properties
        public FrameworkElement NonClientAreaContent
        {
            get => (FrameworkElement)GetValue(NonClientAreaContentProperty);
            set => SetValue(NonClientAreaContentProperty, value);
        }

        #endregion

        #region Static Constructor

        static Window()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        }

        #endregion

        #region Constructor

        public Window()
        {
            AllowsTransparency = false;
            WindowStyle = WindowStyle.None;
        }

        #endregion

        #region Overrided Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_Window_NonClientAreaBox = GetTemplateChild(nameof(PART_Window_NonClientAreaBox)) as Border;

            PART_WindowControlBox_CloseButton = GetTemplateChild(nameof(PART_WindowControlBox_CloseButton)) as Button;
            PART_WindowControlBox_ExpandButton = GetTemplateChild(nameof(PART_WindowControlBox_ExpandButton)) as Button;
            PART_WindowControlBox_MinimizeButton = GetTemplateChild(nameof(PART_WindowControlBox_MinimizeButton)) as Button;
            MainView = GetTemplateChild("MainView") as Panel;

            PART_Window_NonClientAreaBox.MouseLeftButtonDown += PART_Window_NonClientAreaBoxOnMouseLeftButtonDown;

            PART_WindowControlBox_CloseButton.Click += PART_WindowControlBox_CloseButtonOnClick;
            PART_WindowControlBox_ExpandButton.Click += PART_WindowControlBox_ExpandButtonOnClick;
            PART_WindowControlBox_MinimizeButton.Click += PART_WindowControlBox_MinimizeButtonOnClick;

            StateChanged += OnStateChanged;
        }

        #endregion

        #region Private Events Handlers

        private void PART_Window_NonClientAreaBoxOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_WindowControlBox_CloseButtonOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PART_WindowControlBox_ExpandButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MainView.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                WindowState = WindowState.Maximized;
                MainView.Margin = new Thickness(6,6,6,6);
            }
        }

        private void PART_WindowControlBox_MinimizeButtonOnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MainView.Margin = new Thickness(6, 6, 6, 6);
                PART_WindowControlBox_ExpandButton.Content = "";
            }
            else
            {
                MainView.Margin = new Thickness(0, 0, 0, 0);
                PART_WindowControlBox_ExpandButton.Content = "";
            }
        }
        #endregion
    }
}
