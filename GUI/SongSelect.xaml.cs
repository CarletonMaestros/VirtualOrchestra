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
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\AlbumArt.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "";
            SongName.Content = "Pick a Song!";
            YearText.Content = "";
        }

        public void ListBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            ParaAnd.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Radiohead.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Radiohead";
            SongName.Content = "Paranoid Android";
            YearText.Content = "1997";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\r.mid";
            songName = "Radiohead: Paranoid Android";
        }

        private void ParaAndLoop(object sender, RoutedEventArgs e)
        {
            ParaAnd.Position = TimeSpan.Zero;
            ParaAnd.Play();
        }

        private void ListBoxItem_Selected_2(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Prayer.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Madonna.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Madonna";
            SongName.Content = "Like a Prayer";
            YearText.Content = "1989";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\prayer.mid";
            songName = "Madonna: Like a Prayer";
        }

        private void PrayerLoop(object sender, RoutedEventArgs e)
        {
            Prayer.Position = TimeSpan.Zero;
            Prayer.Play();
        }

        private void ListBoxItem_Selected_3(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Fall.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Vivaldi.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Antonio Vivaldi";
            SongName.Content = "L'autunno";
            YearText.Content = "1725";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\fall.mid";
            songName = "Vivaldi: Autumn";
        }

        private void FallLoop(object sender, RoutedEventArgs e)
        {
            Fall.Position = TimeSpan.Zero;
            Fall.Play();
        }

        private void ListBoxItem_Selected_4(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Sym5.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Beethoven.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Ludwig van Beethoven";
            SongName.Content = "Symphony No. 5";
            YearText.Content = "1808";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\sym5.mid";
            songName = "Beethoven: Fifth Symphony";
        }

        private void Sym5Loop(object sender, RoutedEventArgs e)
        {
            Sym5.Position = TimeSpan.Zero;
            Sym5.Play();
        }

        private void ListBoxItem_Selected_5(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            MtKing.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\MountainKing.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Edvard Grieg";
            SongName.Content = "In the Hall of the Mountain King";
            YearText.Content = "1876";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\mtking.mid";
            songName = "Greig: In the Hall of the Mountain King";
        }

        private void MtKingLoop(object sender, RoutedEventArgs e)
        {
            MtKing.Position = TimeSpan.Zero;
            MtKing.Play();
        }

        private void ListBoxItem_Selected_6(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            GetLucky.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Daftpunk.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Daft Punk";
            SongName.Content = "Get Lucky";
            YearText.Content = "2013";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\daft.mid";
            songName = "Daft Punk: Get Lucky";
        }

        private void GetLuckyLoop(object sender, RoutedEventArgs e)
        {
            GetLucky.Position = TimeSpan.Zero;
            GetLucky.Play();
        }

        private void ListBoxItem_Selected_7(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            HardDay.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Beatles.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "The Beatles";
            SongName.Content = "Hard Day's Night";
            YearText.Content = "1964";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\h.mid";
            songName = "The Beatles: Hard Day's Night";
        }

        private void HardDayLoop(object sender, RoutedEventArgs e)
        {
            HardDay.Position = TimeSpan.Zero;
            HardDay.Play();
        }

        private void ListBoxItem_Selected_8(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            RockAmadeus.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\falco.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Falco";
            SongName.Content = "Rock Me Amadeus";
            YearText.Content = "1985";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\rockamadeus.mid";
            songName = "Falco: Rock Me Amadeus";
        }

        private void RockAmadeusLoop(object sender, RoutedEventArgs e)
        {
            RockAmadeus.Position = TimeSpan.Zero;
            RockAmadeus.Play();
        }

        private void ListBoxItem_Selected_9(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Always.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\unicorn.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Erasure";
            SongName.Content = "Always";
            YearText.Content = "1994";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\always.mid";
            songName = "Erasure: Always";
        }

        private void AlwaysLoop(object sender, RoutedEventArgs e)
        {
            Always.Position = TimeSpan.Zero;
            Always.Play();
        }

        private void ListBoxItem_Selected_10(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            AirG.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Bach.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Johann Sebastian Bach";
            SongName.Content = "Air on a G String";
            YearText.Content = "1730";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\airgstr.mid";
            songName = "Bach: Air on a G String";
        }

        private void AirGLoop(object sender, RoutedEventArgs e)
        {
            AirG.Position = TimeSpan.Zero;
            AirG.Play();
        }

        private void ListBoxItem_Selected_11(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            NewWorld.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Dvorak.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Antonin Dvorak";
            SongName.Content = "From the New World";
            YearText.Content = "1893";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\newworld.mid";
            songName = "Dvorak: From the New World";
        }

        private void NewWorldLoop(object sender, RoutedEventArgs e)
        {
            NewWorld.Position = TimeSpan.Zero;
            NewWorld.Play();
        }

        private void ListBoxItem_Selected_12(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            SwanLake.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Tchaikovsky.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Pyotr Ilyich Tchaikovsky";
            SongName.Content = "Swan Lake Prelude";
            YearText.Content = "1875";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\swan.mid";
            songName = "Tchaikovsky: Swan Lake Prelude";
        }

        private void SwanLakeLoop(object sender, RoutedEventArgs e)
        {
            SwanLake.Position = TimeSpan.Zero;
            SwanLake.Play();
        }

        private void ListBoxItem_Selected_13(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Bohemian.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Queen.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Queen";
            SongName.Content = "Bohemian Rhapsody";
            YearText.Content = "1975";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\rhapsody.mid";
            songName = "Queen: Bohemian Rhapsody";
        }

        private void BohemianLoop(object sender, RoutedEventArgs e)
        {
            Bohemian.Position = TimeSpan.Zero;
            Bohemian.Play();
        }

        private void ListBoxItem_Selected_14(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Doves.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Prince.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Prince";
            SongName.Content = "When Doves Cry";
            YearText.Content = "1984";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\doves.mid";
            songName = "Prince: When Doves Cry";
        }

        private void DovesLoop(object sender, RoutedEventArgs e)
        {
            Doves.Position = TimeSpan.Zero;
            Doves.Play();
        }

        private void ListBoxItem_Selected_15(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            CanHeat.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Jamiroquai.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Jamiroquai";
            SongName.Content = "Canned Heat";
            YearText.Content = "1999";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\canheat.mid";
            songName = "Jamiroquai: Canned Heat";
        }

        private void CanHeatLoop(object sender, RoutedEventArgs e)
        {
            CanHeat.Position = TimeSpan.Zero;
            CanHeat.Play();
        }

        private void ListBoxItem_Selected_16(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Flash.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Stones.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "The Rolling Stones";
            SongName.Content = "Jumpin' Jack Flash";
            YearText.Content = "1968";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\flash.mid";
            songName = "The Rolling Stones: Jumpin' Jack Flash";
        }

        private void FlashLoop(object sender, RoutedEventArgs e)
        {
            Flash.Position = TimeSpan.Zero;
            Flash.Play();
        }

        private void ListBoxItem_Selected_17(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            HeyYa.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Outkast.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "OutKast";
            SongName.Content = "Hey Ya!";
            YearText.Content = "2003";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\heyya.mid";
            songName = "OutKast: Hey Ya!";
        }

        private void HeyYaLoop(object sender, RoutedEventArgs e)
        {
            HeyYa.Position = TimeSpan.Zero;
            HeyYa.Play();
        }

        private void ListBoxItem_Selected_19(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Life.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Sinatra.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Frank Sinatra";
            SongName.Content = "That's Life";
            YearText.Content = "1966";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\life.mid";
            songName = "Sinatra: That's Life";
        }

        private void LifeLoop(object sender, RoutedEventArgs e)
        {
            Life.Position = TimeSpan.Zero;
            Life.Play();
        }

        private void ListBoxItem_Selected_20(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            MansWorld.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Brown.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "James Brown";
            SongName.Content = "It's a Man's Man's Man's World";
            YearText.Content = "1966";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\mansworld.mid";
            songName = "Brown: It's a Man's Man's Man's World";
        }

        private void MansWorldLoop(object sender, RoutedEventArgs e)
        {
            MansWorld.Position = TimeSpan.Zero;
            MansWorld.Play();
        }

        private void ListBoxItem_Selected_21(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Stairway.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\LedZep.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Led Zeppelin";
            SongName.Content = "Stairway to Heaven";
            YearText.Content = "1971";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\stairway.mid";
            songName = "Led Zeppelin: Stairway to Heaven";
        }

        private void StairwayLoop(object sender, RoutedEventArgs e)
        {
            Stairway.Position = TimeSpan.Zero;
            Stairway.Play();
        }

        private void ListBoxItem_Selected_22(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Superstition.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\Wonder.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            ArtistName.Content = "Stevie Wonder";
            SongName.Content = "Superstition";
            YearText.Content = "1972";
            songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\superstition.mid";
            songName = "Wonder: Superstition";
        }

        private void SuperstitionLoop(object sender, RoutedEventArgs e)
        {
            Superstition.Position = TimeSpan.Zero;
            Superstition.Play();
        }

        public void StopAllMusic()
        {
            bool parAndPause = ParaAnd.CanPause;
            if (parAndPause == true)
            {
                ParaAnd.Stop();
            }
            bool fallPause = Fall.CanPause;
            if (fallPause == true)
            {
                Fall.Stop();
            }
            bool prayerPause = Prayer.CanPause;
            if (prayerPause == true)
            {
                Prayer.Stop();
            }
            bool hardDayPause = HardDay.CanPause;
            if (hardDayPause == true)
            {
                HardDay.Stop();
            }
            bool sym5Pause = Sym5.CanPause;
            if (sym5Pause == true)
            {
                Sym5.Stop();
            }
            bool rockAmadeusPause = RockAmadeus.CanPause;
            if (rockAmadeusPause == true)
            {
                RockAmadeus.Stop();
            }
            bool getLuckyPause = GetLucky.CanPause;
            if (getLuckyPause == true)
            {
                GetLucky.Stop();
            }
            bool mtKingPause = MtKing.CanPause;
            if (mtKingPause == true)
            {
                MtKing.Stop();
            }
            bool alwaysPause = Always.CanPause;
            if (alwaysPause == true)
            {
                Always.Stop();
            }
            bool airGPause = AirG.CanPause;
            if (airGPause == true)
            {
                AirG.Stop();
            }
            bool newWorldPause = NewWorld.CanPause;
            if (newWorldPause == true)
            {
                NewWorld.Stop();
            }
            bool swanLakePause = SwanLake.CanPause;
            if (swanLakePause == true)
            {
                SwanLake.Stop();
            }
            bool bohemianPause = Bohemian.CanPause;
            if (bohemianPause == true)
            {
                Bohemian.Stop();
            }
            bool dovesPause = Doves.CanPause;
            if (dovesPause == true)
            {
                Doves.Stop();
            }
            bool canHeatPause = CanHeat.CanPause;
            if (canHeatPause == true)
            {
                CanHeat.Stop();
            }
            bool flashPause = Flash.CanPause;
            if (flashPause == true)
            {
                Flash.Stop();
            }
            bool heyYaPause = HeyYa.CanPause;
            if (heyYaPause == true)
            {
                HeyYa.Stop();
            }
            bool lifePause = Life.CanPause;
            if (lifePause == true)
            {
                Life.Stop();
            }
            bool mansWorldPause = MansWorld.CanPause;
            if (mansWorldPause == true)
            {
                MansWorld.Stop();
            }
            bool stairwayPause = Stairway.CanPause;
            if (stairwayPause == true)
            {
                Stairway.Stop();
            }
            bool superstitionPause = Superstition.CanPause;
            if (superstitionPause == true)
            {
                Superstition.Stop();
            }
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
            StopAllMusic();
            App.PlaySong(songFile, songName, false);
        }

        private void MenuButtonClick(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            App.ShowStartScreen();
        }

    }
}