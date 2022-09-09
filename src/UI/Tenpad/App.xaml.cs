using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Tools;
using Tenpad.Core;
using Window = HandyControl.Controls.Window;

namespace Tenpad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IoC.BuildServiceProvider();

            var win = new MainWindow();
            var m = IoC.Resolve<MainViewModel>();
            m.Init();
            win.DataContext = m;
            Current.MainWindow = win;
            Current.MainWindow.Show();
        }
    }
}
