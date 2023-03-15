using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
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

namespace MapVariantIDSwitcher.Windows
{
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        public Preferences()
        {
            InitializeComponent();
        }

        private void SetMapVariantPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                DefaultMapVariantPathTextBlock.Text = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.DefaultMapVariantPath = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void SetModMapPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                DefaultModMapPathTextBlock.Text = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.DefaultModMapPath = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void SetOutputFolderPathButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true)
            {
                DefaultOutputFolderPathTextBlock.Text = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.DefaultOutputPath = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }
    }
}
