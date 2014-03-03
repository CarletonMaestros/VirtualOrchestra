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
    public partial class SongSelectWindow : Window
    {
        public string songFile;
        public string songName;

        public SongSelectWindow()
        {
            InitializeComponent();
        }

        public void Clean()
        {
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\AlbumArt.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "";
            SongName.Content = "Pick a Song!!";
            YearText.Content = "";
        }

        public void SongLoop(object sender, RoutedEventArgs e)
        {
            SongPreview.Position = TimeSpan.Zero;
            SongPreview.Play();
        }

        public void ListBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Radiohead.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Radiohead";
            SongName.Content = "Paranoid Android";
            YearText.Content = "1997";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.mid";
            songName = "Radiohead: Paranoid Android";
        }

        private void ListBoxItem_Selected_2(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\prayer.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Madonna.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Madonna";
            SongName.Content = "Like a Prayer";
            YearText.Content = "1989";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\prayer.mid";
            songName = "Madonna: Like a Prayer";
        }

        private void ListBoxItem_Selected_3(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\fall.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Vivaldi.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Antonio Vivaldi";
            SongName.Content = "L'autunno";
            YearText.Content = "1725";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\fall.mid";
            songName = "Vivaldi: Autumn";
        }

        private void ListBoxItem_Selected_4(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\sym5.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Beethoven.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Ludwig van Beethoven";
            SongName.Content = "Symphony No. 5";
            YearText.Content = "1808";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\sym5.mid";
            songName = "Beethoven: Fifth Symphony";
        }

        private void ListBoxItem_Selected_5(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\mtking.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\MountainKing.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Edvard Grieg";
            SongName.Content = "In the Hall of the Mountain King";
            YearText.Content = "1876";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\mtking.mid";
            songName = "Grieg: In the Hall of the Mountain King";
        }

        private void ListBoxItem_Selected_6(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\daft.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Daftpunk.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Daft Punk";
            SongName.Content = "Get Lucky";
            YearText.Content = "2013";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\daft.mid";
            songName = "Daft Punk: Get Lucky";
        }

        private void ListBoxItem_Selected_7(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\h.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Beatles.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "The Beatles";
            SongName.Content = "Hard Day's Night";
            YearText.Content = "1964";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\h.mid";
            songName = "The Beatles: Hard Day's Night";
        }

        private void ListBoxItem_Selected_8(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\rockamadeus.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\falco.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Falco";
            SongName.Content = "Rock Me Amadeus";
            YearText.Content = "1985";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\rockamadeus.mid";
            songName = "Falco: Rock Me Amadeus";
        }

        private void ListBoxItem_Selected_9(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\always.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\unicorn.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Erasure";
            SongName.Content = "Always";
            YearText.Content = "1994";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\always.mid";
            songName = "Erasure: Always";
        }

        private void ListBoxItem_Selected_10(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\airgstr.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Bach.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Air on a G String";
            SongName.Content = "Johann Sebastian Bach";
            YearText.Content = "1730";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\airgstr.mid";
            songName = "Bach: Air on a G String";
        }

        private void ListBoxItem_Selected_11(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\newworld.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Dvorak.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Antonin Dvorak";
            SongName.Content = "From the New World";
            YearText.Content = "1893";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\newworld.mid";
            songName = "Dvorak: From the New World";
        }

        private void ListBoxItem_Selected_12(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\swan.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Tchaikovsky.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Pyotr Ilyich Tchaikovsky";
            SongName.Content = "Swan Lake Prelude";
            YearText.Content = "1875";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\swan.mid";
            songName = "Tchaikovsky: Swan Lake Prelude";
        }

        private void ListBoxItem_Selected_13(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\rhapsody.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Queen.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Queen";
            SongName.Content = "Bohemian Rhapsody";
            YearText.Content = "1975";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\rhapsody.mid";
            songName = "Queen: Bohemian Rhapsody";
        }

        private void ListBoxItem_Selected_14(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\doves.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Prince.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Prince";
            SongName.Content = "When Doves Cry";
            YearText.Content = "1984";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\doves.mid";
            songName = "Prince: When Doves Cry";
        }

        private void ListBoxItem_Selected_15(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\canheat.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Jamiroquai.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Jamiroquai";
            SongName.Content = "Canned Heat";
            YearText.Content = "1999";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\canheat.mid";
            songName = "Jamiroquai: Canned Heat";
        }

        private void ListBoxItem_Selected_16(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\flash.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Stones.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "The Rolling Stones";
            SongName.Content = "Jumpin' Jack Flash";
            YearText.Content = "1968";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\flash.mid";
            songName = "The Rolling Stones: Jumpin' Jack Flash";
        }

        private void ListBoxItem_Selected_17(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\heyya.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Outkast.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "OutKast";
            SongName.Content = "Hey Ya!";
            YearText.Content = "2003";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\heyya.mid";
            songName = "OutKast: Hey Ya!";
        }

        private void ListBoxItem_Selected_19(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\life.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Sinatra.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Frank Sinatra";
            SongName.Content = "That's Life";
            YearText.Content = "1966";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\life.mid";
            songName = "Frank Sinatra: That's Life";
        }

        private void ListBoxItem_Selected_20(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\mansworld.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Brown.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "James Brown";
            SongName.Content = "It's a Man's Man's Man's World";
            YearText.Content = "1966";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\mansworld.mid";
            songName = "James Brown: It's a Man's Man's Man's World";
        }

        private void ListBoxItem_Selected_21(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\stairway.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\LedZep.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Led Zeppelin";
            SongName.Content = "Stairway to Heaven";
            YearText.Content = "1971";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\stairway.mid";
            songName = "Led Zeppelin: Stairway to Heaven";
        }

        private void ListBoxItem_Selected_22(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\superstition.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Wonder.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Stevie Wonder";
            SongName.Content = "Superstition";
            YearText.Content = "1972";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\superstition.mid";
            songName = "Stevie Wonder: Superstition";
        }

        private void ListBoxItem_Selected_23(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            SongPreview.Source = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\misery.wav");
            SongPreview.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Paramore.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Paramore";
            SongName.Content = "Misery Business";
            YearText.Content = "2007";
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\misery.mid";
            songName = "Paramore: Misery Business";
        }

        private void LeftHandMode(object sender, RoutedEventArgs e)
        {
            Gestures.rightHandVolume = false;
        }

        private void RightHandMode(object sender, RoutedEventArgs e)
        {
            Gestures.rightHandVolume = true;
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            App.PlaySong(songFile, songName, false);
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            SongPreview.Stop();
            App.ShowStartScreen();
        }

    }
}