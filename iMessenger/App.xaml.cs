using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Windows;

namespace iMessenger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();
            Core core = new Core(window);
            window.Core = core;
            window.Show();
            // Handle all unhandled exceptions
            Dispatcher.UnhandledException += core.OnErrorRaised;
            Dispatcher.UnhandledException += window.OnErrorWindowOpened;
        }
    }
}

