using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private Regedit regedit;

        public Options()
        {
            InitializeComponent();
            this.regedit = new Regedit(Registry.CurrentUser, "SOFTWARE\\Webzen\\Mu\\Config");
        }

        private static void ShowErrorMessage(Exception e)
        {
            MessageBoxResult result = MessageBox.Show(e.Message);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

            if (regedit.Read("Resolution") == string.Empty || regedit.Read("Resolution") == null)
            {
                regedit.Write("Resolution", 0);
            }

            if (regedit.Read("WindowMode") == string.Empty || regedit.Read("WindowMode") == null)
            {
                regedit.Write("WindowMode", 0);
            }

            if (regedit.Read("SoundOnOFF") == string.Empty || regedit.Read("SoundOnOFF") == null)
            {
                regedit.Write("SoundOnOFF", 1);
            }

            if (regedit.Read("MusicOnOFF") == string.Empty || regedit.Read("MusicOnOFF") == null)
            {
                regedit.Write("MusicOnOFF", 1);
            }

            if (regedit.Read("VolumeLevel") == string.Empty || regedit.Read("VolumeLevel") == null)
            {
                regedit.Write("VolumeLevel", 9);
            }


            this.volumeSlider.IsSnapToTickEnabled = true;
            this.accountTextBox.MaxLength = 10;
            this.cBoxResolution.Items.Add("640x480 [4:3]");
            this.cBoxResolution.Items.Add("800x600 [4:3]");
            this.cBoxResolution.Items.Add("1024x768 [4:3]");
            this.cBoxResolution.Items.Add("1280x1024 [5:4]");
            this.cBoxResolution.Items.Add("1600x1280 [5:4]");
            this.cBoxResolution.Items.Add("1366x768 [16:9]");
            this.setControls();

        }

        private void accountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Z ]", "");
            textboxSender.SelectionStart = cursorPosition;
        }

        private void setControls()
        {
            this.cBoxResolution.SelectedIndex = int.Parse(regedit.Read("Resolution"));

            bool windowMode = Convert.ToBoolean(int.Parse(regedit.Read("WindowMode")));
            if (windowMode)
            {
                this.windowModeYes.IsChecked = true;
            }
            else
            {
                this.windowModeNo.IsChecked = true;
            }

            this.soundCheckBox.IsChecked = Convert.ToBoolean(int.Parse(regedit.Read("SoundOnOFF")));
            this.musicCheckBox.IsChecked = Convert.ToBoolean(int.Parse(regedit.Read("MusicOnOFF")));
            this.volumeSlider.Value = int.Parse(regedit.Read("VolumeLevel"));
            this.accountTextBox.Text = regedit.Read("ID");
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            regedit.Write("Resolution", this.cBoxResolution.SelectedIndex);
            int wMode = 0;
            if (this.windowModeYes.IsChecked == true)
            {
                wMode = 1;
            }
            regedit.Write("WindowMode", wMode);
            regedit.Write("SoundOnOFF", Convert.ToInt32(this.soundCheckBox.IsChecked));
            regedit.Write("MusicOnOFF", Convert.ToInt32(this.musicCheckBox.IsChecked));
            regedit.Write("VolumeLevel", this.volumeSlider.Value);
            regedit.Write("ID", this.accountTextBox.Text);

            this.Close();
        }

        private void repairButton_Click(object sender, RoutedEventArgs e)
        {
            regedit.Write("LangSelection", "Eng");
            regedit.Write("Resolution", 0);
            regedit.Write("WindowMode", 0);
            regedit.Write("SoundOnOFF", 1);
            regedit.Write("MusicOnOFF", 1);
            regedit.Write("VolumeLevel", 9);
            this.setControls();
        }
    }
}
