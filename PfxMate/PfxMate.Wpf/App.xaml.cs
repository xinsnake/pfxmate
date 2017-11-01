using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;

namespace PfxMate.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                new MainWindow().Show();
                return;
            }

            var action = e.Args[0].ToString();
            
            if (action != "Open" || e.Args.Length == 1)
            {
                MessageBox.Show("Invalid Operation " + action, "Error");
                Environment.Exit(12);
            }

            string certPath = "", certPass = "";

            if (e.Args.Length >= 2)
            {
                certPath = e.Args[1].ToString();
            }

            if (e.Args.Length >= 3)
            {
                certPass = e.Args[2].ToString();
            }

            var wnd = new MainWindow(certPath, certPass);
            wnd.Show();
        }
    }
}
