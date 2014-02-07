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

namespace Orchestra
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class SongSelectWindow : Window
    {
        public string songFile;

        public SongSelectWindow()
        {
            InitializeComponent();
        }

        public void ListBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            ParaAnd.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            //newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Radiohead.jpg");
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Radiohead.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.mid";
            //songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\r.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ParaAndLoop(object sender, RoutedEventArgs e)
        {
            ParaAnd.Position = TimeSpan.Zero;
            ParaAnd.Play();
        }

        private void ListBoxItem_Selected_2(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Madonna.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\m.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ListBoxItem_Selected_3(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            ScarMon.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Skrillex.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\s.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ScarMonLoop(object sender, RoutedEventArgs e)
        {
            ScarMon.Position = TimeSpan.Zero;
            ScarMon.Play();
        }

        private void ListBoxItem_Selected_4(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Beethoven.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\b2.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ListBoxItem_Selected_5(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\MountainKing.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\g.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ListBoxItem_Selected_6(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Daftpunk.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\d.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ListBoxItem_Selected_7(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Beatles.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\b.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void ListBoxItem_Selected_8(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Kanye.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\k.mid";
            Dispatch.TriggerSongSelected(songFile); 
        }

        private void StopAllMusic()
        {
            bool parAndPause = ParaAnd.CanPause;
            if (parAndPause == true)
            {
                ParaAnd.Stop();
            }
            bool scarMonPause = ScarMon.CanPause;
            if (scarMonPause == true)
            {
                ScarMon.Stop();
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var guiWindow = new GUIWindow();
            guiWindow.Show();
            this.Close();
        }
    }
}
