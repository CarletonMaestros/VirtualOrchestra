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
using System.Diagnostics;


namespace Orchestra
{
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : Window
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
            Process.GetCurrentProcess().Kill();
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            SongSelectWindow songSelect = new SongSelectWindow(false, this);
            songSelect.Show();
        }

        private void TutorialButtonClick(object sender, RoutedEventArgs e)
        {
            TutorialWindow tutorial = new TutorialWindow();
            tutorial.Show();
        }

        private void TutorialClick(object sender, RoutedEventArgs e)
        {
            TutorialWindow tutorialWindow = new TutorialWindow();
            tutorialWindow.Show();
        }
        //Things weren't working, added this
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SongSelectWindow songSelect = new SongSelectWindow(false, this);
            songSelect.Show();
        }
    }
}
