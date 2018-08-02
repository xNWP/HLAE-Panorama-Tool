using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HLAE_Panorama_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // to prevent maximize sizing over taskbar
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            InitializeComponent();

            // CreatePanoramaMyZip Worker
            CreatePanoramaMyZip = new BackgroundWorker();
            CreatePanoramaMyZip.DoWork += new DoWorkEventHandler(CreatePanoramaMyZip_Work);

            // StatusBar
            StatusBarWorker = new BackgroundWorker();
            StatusBarWorker.WorkerReportsProgress = true;
            StatusBarWorker.DoWork += new DoWorkEventHandler(StatusBarWorker_Work);
            StatusBarWorker.ProgressChanged += new ProgressChangedEventHandler(StatusBar_Update);

            StatusBarPulse = new BackgroundWorker();
            StatusBarPulse.WorkerReportsProgress = true;
            StatusBarPulse.DoWork += new DoWorkEventHandler(StatusBarPulse_Work);
            StatusBarPulse.ProgressChanged += new ProgressChangedEventHandler(StatusBarPulse_Update);

            // Create Snapshot
            CreateSnapshot = new BackgroundWorker();
            CreateSnapshot.DoWork += new DoWorkEventHandler(CreateSnapshot_Work);
        }

        /// <summary>
        /// Pass snapshot file to function.
        /// </summary>
        private void CreateSnapshot_Work(object sender, DoWorkEventArgs e)
        {
            string CSGODir = StripFileFromString(Properties.Settings.Default.CSGOBinary);
            string SnapFile = (string)e.Argument;

            ZipFile.CreateFromDirectory(CSGODir + "\\csgo\\panorama", SnapFile, CompressionLevel.Optimal, false);
        }

        private void StatusBarPulse_Update(object sender, ProgressChangedEventArgs e)
        {
            if(e.ProgressPercentage == 0)
            {
                DoubleAnimation Out = new DoubleAnimation(25, TimeSpan.FromSeconds(1));
                Out.DecelerationRatio = 0.25;
                Out.AccelerationRatio = 0.75;
                DoubleAnimation OutOpac = new DoubleAnimation(0.5, TimeSpan.FromSeconds(1));
                OutOpac.DecelerationRatio = 0.5;
                OutOpac.AccelerationRatio = 0.5;
                StatusBar.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, Out);
                StatusBar.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, OutOpac);
            }
            else if(e.ProgressPercentage == 1)
            {
                DoubleAnimation In = new DoubleAnimation(10, TimeSpan.FromSeconds(1));
                In.DecelerationRatio = 0.75;
                In.AccelerationRatio = 0.25;
                DoubleAnimation InOpac = new DoubleAnimation(0.3, TimeSpan.FromSeconds(1));
                InOpac.DecelerationRatio = 0.5;
                InOpac.AccelerationRatio = 0.5;
                StatusBar.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, In);
                StatusBar.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, InOpac);
            }
            else if(e.ProgressPercentage == 2)
            {
                DoubleAnimation Out = new DoubleAnimation(25, TimeSpan.FromMilliseconds(500));
                Out.DecelerationRatio = 0.25;
                Out.AccelerationRatio = 0.75;
                DoubleAnimation OutOpac = new DoubleAnimation(0.5, TimeSpan.FromMilliseconds(500));
                OutOpac.DecelerationRatio = 0.5;
                OutOpac.AccelerationRatio = 0.5;
                StatusBar.Effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, Out);
                StatusBar.Effect.BeginAnimation(DropShadowEffect.OpacityProperty, OutOpac);
                ColorAnimation ToGreen = new ColorAnimation(Color.FromRgb(0x3b, 0xf9, 0x31), TimeSpan.FromMilliseconds(750));
                StatusBar.Effect.BeginAnimation(DropShadowEffect.ColorProperty, ToGreen);
                FullWindow.Effect.BeginAnimation(DropShadowEffect.ColorProperty, ToGreen);
            }
        }

        /// <summary>
        /// Pass BackgroundWorker as an argument to change
        /// status colour.
        /// </summary>
        private void StatusBarPulse_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            BackgroundWorker WaitOn = (BackgroundWorker)e.Argument;

            while (WaitOn.IsBusy)
            {
                worker.ReportProgress(0); // out
                Thread.Sleep(1000);
                worker.ReportProgress(1); // in
                Thread.Sleep(1000);
            }
            worker.ReportProgress(2); // Green
        }

        private void StatusBar_Update(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0 && StatusGrid.Visibility != Visibility.Visible)
            {
                DoubleAnimation FadeIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(250));
                FadeIn.AccelerationRatio = 0.5;
                FadeIn.DecelerationRatio = 0.5;
                StatusGrid.Visibility = Visibility.Visible;
                StatusGrid.BeginAnimation(Grid.OpacityProperty, FadeIn);
                ColorAnimation ToOrange = new ColorAnimation(Color.FromRgb(0xd8, 0x8c, 0x41), TimeSpan.FromSeconds(1));
                FullWindow.Effect.BeginAnimation(DropShadowEffect.ColorProperty, ToOrange);
            }
            else if (e.ProgressPercentage == 1)
            {
                DoubleAnimation FadeOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(250));
                FadeOut.AccelerationRatio = 0.5;
                FadeOut.DecelerationRatio = 0.5;
                StatusGrid.BeginAnimation(Grid.OpacityProperty, FadeOut);
                ColorAnimation ToDefault = new ColorAnimation(Color.FromRgb(0x00, 0xC1, 0xEC), TimeSpan.FromSeconds(1));
                FullWindow.Effect.BeginAnimation(DropShadowEffect.ColorProperty, ToDefault);
            }
            else if (e.ProgressPercentage == 2)
            {
                StatusGrid.Visibility = Visibility.Collapsed;

                System.Windows.Media.Effects.DropShadowEffect dropShadowEffect = new System.Windows.Media.Effects.DropShadowEffect();
                dropShadowEffect.Color = Color.FromRgb(0xd8, 0x8c, 0x41);
                dropShadowEffect.ShadowDepth = 0;
                StatusBar.Effect = dropShadowEffect;
            }
        }

        /// <summary>
        /// Pass BackgroundWorker as an argument to wait
        /// on for status bar to dissappear.
        /// </summary>
        private void StatusBarWorker_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            BackgroundWorker WaitOn = (BackgroundWorker)e.Argument;

            worker.ReportProgress(0); // fade in
            while (WaitOn.IsBusy)
            {
                Thread.Sleep(300);
            }

            Thread.Sleep(2500);
            worker.ReportProgress(1); // fade out
            Thread.Sleep(500);
            worker.ReportProgress(2); // collapse / reset
        }

        private void CreatePanoramaMyZip_Work(object sender, DoWorkEventArgs e)
        {
            string HLAEDir = StripFileFromString(Properties.Settings.Default.HLAEBinary);
            string PanoZip = HLAEDir + "\\panorama.org.zip";
            string CSGODir = StripFileFromString(Properties.Settings.Default.CSGOBinary);
            string TempDir = System.IO.Path.GetTempPath() +
                System.IO.Path.GetRandomFileName() + "_HLAEPanoramaTool";

            Directory.CreateDirectory(TempDir + "\\panorama");

            using (ZipArchive Arch = ZipFile.Open(PanoZip, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in Arch.Entries)
                {
                    Directory.CreateDirectory(StripFileFromString(CSGODir + "\\csgo\\" + entry.FullName));
                    entry.ExtractToFile(CSGODir + "\\csgo\\" + entry.FullName, true);

                    if (entry.FullName == "panorama\\panorama.cfg")
                        entry.ExtractToFile(TempDir + "\\panorama\\panorama.cfg");
                }
            }

            if (File.Exists(HLAEDir + "\\panorama.my.zip"))
                File.Delete(HLAEDir + "\\panorama.my.zip");

            // Creation of .my.zip needs to be done with 7-zip as ZipFile does
            // not support Copy (no compression).
            ProcessStartInfo PrcStInfo = new ProcessStartInfo(Properties.Settings.Default.SEVZBinary,
                    "a -tzip -mx0 -mcp=20127 -mcl=off -mcu=off -mtc=off -- " + HLAEDir + "\\panorama.my.zip " +
                    TempDir + "\\panorama");
            PrcStInfo.CreateNoWindow = true;
            PrcStInfo.UseShellExecute = true;

            Process SevZipProc = new Process();
            SevZipProc.StartInfo = PrcStInfo;
            SevZipProc.Start();
            SevZipProc.WaitForExit();
        }

        private string StripFileFromString(string FullPath)
        {
            string path = FullPath;
            for(int i = path.Length - 1; i >= 0; i--)
            {
                if(path[i] == '\\' || path[i] == '/')
                {
                    path = path.Substring(0, i);
                    break;
                }
            }
            return path;
        }

        private void Minimize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void Escape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // WINDOW EFFECT
            this.Effect.SetValue(DropShadowEffect.OpacityProperty, 0.0);
        }

        private void Window_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // WINDOW EFFECT
            this.Effect.SetValue(DropShadowEffect.OpacityProperty, 0.5);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = new Thickness(0.0);
                this.MainGrid.Margin = new Thickness(7.0);
            }
            else
            {
                this.BorderThickness = new Thickness(3.0);
                this.MainGrid.Margin = new Thickness(0.0);
            }
        }

        private void pano_org_zip_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Properties.Settings.Default.HLAEBinary,
                "-csgoLauncher -csgoExe \"" + Properties.Settings.Default.CSGOBinary +
                "\" -gfxEnabled true -noGui -autoStart -gfxWidth 1280 -gfxHeight 720 -gfxFull false -avoidVac true " +
                "-customLaunchOptions " + "\"-console -panorama -afxDetourPanorama\"");
        }

        private void pano_my_zip_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("This will overwrite any existing modifications to the Panorama UI, would you like to continue?",
                "About To Overwrite Panorama UI", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
                == MessageBoxResult.Yes)
            {
                CreatePanoramaMyZip.RunWorkerAsync();
                StatusBarText.Content = "Generating panorama.my.zip";
                StatusBarWorker.RunWorkerAsync(CreatePanoramaMyZip);
                StatusBarPulse.RunWorkerAsync(CreatePanoramaMyZip);
            }
        }

        private void snapshot_btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.AddExtension = true;
            SFD.Filter = "Panorama Screenshot|*.pss";
            SFD.InitialDirectory = Directory.GetCurrentDirectory() + "\\snapshots";
            SFD.OverwritePrompt = true;
            SFD.FileName = "Snapshot-" + DateTime.Today.Year + "-" +
                DateTime.Today.Month + "-" + DateTime.Today.Day;

            if(SFD.ShowDialog() == true)
            {
                if (File.Exists(SFD.FileName))
                    File.Delete(SFD.FileName);
                CreateSnapshot.RunWorkerAsync(SFD.FileName);
                StatusBarText.Content = "Generating snapshot " + SFD.SafeFileName;
                StatusBarWorker.RunWorkerAsync(CreateSnapshot);
                StatusBarPulse.RunWorkerAsync(CreateSnapshot);
            }
        }

        private void export_diff_btn_Click(object sender, RoutedEventArgs e)
        {
            ExportDiff eWin = new ExportDiff();
            eWin.ShowDialog();
        }

        // Class Properties
        BackgroundWorker CreatePanoramaMyZip;
        BackgroundWorker StatusBarWorker;
        BackgroundWorker StatusBarPulse;
        BackgroundWorker CreateSnapshot;
    }
}
