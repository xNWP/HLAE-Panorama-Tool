using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HLAE_Panorama_Tool
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void CSGO_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "Executable Files|*.exe";
            OpenFileDialog.InitialDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Counter-Strike Global Offensive";

            if (OpenFileDialog.ShowDialog() == true)
            {
                this.CSGOBinaryText.Text = OpenFileDialog.FileName;

                Properties.Settings.Default.Save();
            }
        }

        private void HLAE_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "Executable Files|*.exe";

            if (OpenFileDialog.ShowDialog() == true)
            {
                this.HLAEBinaryText.Text = OpenFileDialog.FileName;

                Properties.Settings.Default.Save();
            }
        }

        private void SevZ_Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog OpenFileDialog = new OpenFileDialog();
            OpenFileDialog.Filter = "Executable Files|*.exe";
            OpenFileDialog.InitialDirectory = "C:\\Program Files\\7-Zip";

            if (OpenFileDialog.ShowDialog() == true)
            {
                this.SEVZBinaryText.Text = OpenFileDialog.FileName;

                Properties.Settings.Default.Save();
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();

            if (!File.Exists(Properties.Settings.Default.HLAEBinary)
                || !File.Exists(Properties.Settings.Default.CSGOBinary)
                || !File.Exists(Properties.Settings.Default.SEVZBinary))
            {
                MessageBox.Show("Please fill in all the paths with valid binaries.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MainWindow MainWnd = new MainWindow();
                this.Close();
                MainWnd.Show();
            }
        }

        private void Escape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (TB.Text != "")
            {
                if (File.Exists(TB.Text))
                    TB.BorderBrush = Brushes.Green;
                else
                    TB.BorderBrush = Brushes.Red;
            }
            else
                TB.BorderBrush = new SolidColorBrush(Color.FromRgb(0xAB, 0xAd, 0xB3));
        }
    }
}
