using HLAE_Panorama_Tool.Custom_Controls;
using HLAE_Panorama_Tool.Core.DiffGenerators;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
    /// Interaction logic for ExportDiff.xaml
    /// </summary>
    public partial class ExportDiff : Window
    {
        #region Initialization
        public ExportDiff()
        {
            Differences = new List<DiffViewer>();

            InitializeComponent();
            InitializeSnapshots();
        }

        private void InitializeSnapshots()
        {
            if(Directory.Exists("snapshots"))
            {
                List<string> SnapShots = new List<string>(Directory.GetFiles("snapshots", "*.pss", SearchOption.TopDirectoryOnly).Select(System.IO.Path.GetFileNameWithoutExtension));

                foreach (string snapshot in SnapShots)
                    SnapshotBox.Items.Insert(SnapshotBox.Items.Count - 2, snapshot);
            }

            if (SnapshotBox.Items.Count > 2)
            {
                SnapshotBoxDiv.Visibility = Visibility.Visible;
                SnapshotBox.SelectedIndex = 0;
                PopulateDiffList(Directory.GetCurrentDirectory() + "\\snapshots\\"
                    + (string)SnapshotBox.SelectedValue + ".pss");
            }
        }
        #endregion

        #region Event Handlers
        private void DiffViewer_Click(object sender, RoutedEventArgs e)
        {
            DiffViewer caller = (DiffViewer)sender;

            if (caller.IsChecked)
                DiffEntrySetState(caller, false);
            else
                DiffEntrySetState(caller, true);
        }

        private void IncludeAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (DiffViewer entry in Differences)
                DiffEntrySetState(entry, true);
        }

        private void ExcludeAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (DiffViewer entry in Differences)
                DiffEntrySetState(entry, false);
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            foreach (DiffViewer entry in Differences)
                DiffEntrySetState(entry, !entry.IsChecked);
        }

        private void SnapshotBox_DropDownClosed(object sender, EventArgs e)
        {
            int ItemCount = SnapshotBox.Items.Count;
            if(SnapshotBox.SelectedIndex == ItemCount - 1) // browse selected
            {
                OpenFileDialog ofdg = new OpenFileDialog();
                ofdg.Filter = "Panorama Screenshot|*.pss";
                ofdg.InitialDirectory = Directory.GetCurrentDirectory() + "\\snapshots";
                if(ofdg.ShowDialog() == true)
                {
                    PopulateDiffList(ofdg.FileName);
                }
                SnapshotBox.SelectedIndex = -1;
            }
            else if(SnapshotBox.SelectedIndex >= 0) // local snapshot
            {
                PopulateDiffList(Directory.GetCurrentDirectory() + "\\snapshots\\"
                    + (string)SnapshotBox.SelectedValue + ".pss");
            }
        }

        private void PopulateDiffList(string Snapshot)
        {
            List<DifferenceObject> RealDifferences = new List<DifferenceObject>();

            ZipArchive SnapArch = ZipFile.OpenRead(Snapshot);

            foreach (DifferenceObject diffFile in RealDifferences)
            {
                TextBlock dFileTB = new TextBlock();
                dFileTB.TextAlignment = TextAlignment.Center;
                dFileTB.HorizontalAlignment = HorizontalAlignment.Stretch;
                dFileTB.VerticalAlignment = VerticalAlignment.Stretch;
                dFileTB.VerticalAlignment = VerticalAlignment.Center;
                dFileTB.Text = System.IO.Path.GetFileName(diffFile.Filename);
                dFileTB.FontFamily = (FontFamily)FindResource("LatoRegular");
                dFileTB.FontSize = 18;
                dFileTB.Margin = new Thickness(10, 0, 10, 0);
                dFileTB.Foreground = (Brush)FindResource("OffWhiteBrush");
                dFileTB.Background = (Brush)FindResource("BackgroundDarkBrush");
                DiffEntries.Children.Add(dFileTB);

                foreach (Difference diff in diffFile.Differences)
                {
                    DiffViewer diffViewer = new DiffViewer()
                    {
                        Difference = diff,
                        InfoWidth = 200,
                        MinHeight = 60,
                        Margin = new Thickness(3),
                        Foreground = (Brush)FindResource("OffWhiteBrush"),
                        InfoBackground = (Brush)FindResource("BackgroundDarkBrush"),
                        BorderBrush = (Brush)FindResource("HighlightGreyBrush"),
                        Cursor = Cursors.Hand,
                        BorderThickness = new Thickness(2)
                    };
                    diffViewer.Click += DiffViewer_Click;
                    DiffEntries.Children.Add(diffViewer);
                    Differences.Add(diffViewer);
                }
            }
        }

        /// <summary>
        /// Sets diff entrys state and updates it's visual style.
        /// </summary>
        /// <param name="obj">The DiffViewer entry.</param>
        /// <param name="state">The state, true or false.</param>
        private void DiffEntrySetState(DiffViewer obj, bool state)
        {
            if(state)
            {
                obj.IsChecked = true;
                obj.BorderBrush = (Brush)FindResource("HighlightGreyBrush");
                obj.BorderThickness = new Thickness(2);
            }
            else
            {
                obj.IsChecked = false;
                obj.BorderBrush = (Brush)FindResource("BackgroundDarkBrush");
                obj.BorderThickness = new Thickness(2);
            }
        }
        #endregion

        #region Properties
        private List<DiffViewer> Differences;
        #endregion
    }
}
