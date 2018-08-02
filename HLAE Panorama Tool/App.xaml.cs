using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HLAE_Panorama_Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if(!File.Exists(HLAE_Panorama_Tool.Properties.Settings.Default.HLAEBinary)
                || !File.Exists(HLAE_Panorama_Tool.Properties.Settings.Default.CSGOBinary)
                || !File.Exists(HLAE_Panorama_Tool.Properties.Settings.Default.SEVZBinary))
            {
                Setup SetupWindow = new Setup();
                SetupWindow.Show();
            }
            else
            {
                MainWindow MainWnd = new MainWindow();
                MainWnd.Show();
            }
        }
    }
}
