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
            songName = "Radiohead: Paranoid Android";
            //songFile = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample MIDIs\r.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Madonna.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\prayer.mid";
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
            ScarMon.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Skrillex.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\s.mid";
            songName = "Skrillex: Scary Monsters and Nice Sprites";
        }

        private void ScarMonLoop(object sender, RoutedEventArgs e)
        {
            ScarMon.Position = TimeSpan.Zero;
            ScarMon.Play();
        }

        private void ListBoxItem_Selected_4(object sender, RoutedEventArgs e)
        {
            StopAllMusic();
            Sym5.Play();
            BitmapImage newIm = new BitmapImage();
            newIm.BeginInit();
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Beethoven.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\sym5.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\MountainKing.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\mtking.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Daftpunk.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\daft.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Beatles.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\h.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\falco.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\rockamadeus.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\unicorn.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\always.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Bach.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\airgstr.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Dvorak.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\newworld.mid";
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
            newIm.UriSource = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\Tchaikovsky.jpg");
            newIm.EndInit();
            PreviewImage.Source = newIm;
            songFile = @"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\swan.mid";
            songName = "Tchaikovsky: Swan Lake Prelude";
        }

        private void SwanLakeLoop(object sender, RoutedEventArgs e)
        {
            SwanLake.Position = TimeSpan.Zero;
            SwanLake.Play();
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
            App.PlaySong(songFile, songName);
        }
    }
}