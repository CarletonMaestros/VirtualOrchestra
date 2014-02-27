using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using ImageUtils;
using System.Diagnostics;
using System.Timers;


namespace Orchestra
{
    public partial class MainWindow : Window
    {
        private int currTick = 0;
        private ChannelHandler[] channels = new ChannelHandler[16];


        // private float beatsPerSecond = 2; //will be set dynamically

        //private int beatsPerMeasure = 4;
        private int lastPopulate = -100000; //tick value where most recent piano roll population occurred, starts low to trigger piano roll population
        private int noteHeightResolution = 60; //number of notes that fit vertically into the canvas

        //private LinearGradientBrush[] colorByChannel = new LinearGradientBrush[16] { LinearGradientBrush(.Aqua, Brushes.Beige, Brushes.Blue, Brushes.BlueViolet, Brushes.Brown, Brushes.Chartreuse, Brushes.Crimson, Brushes.Red, Brushes.Salmon, Brushes.Silver, Brushes.Yellow, Brushes.Turquoise, Brushes.Violet, Brushes.Black, Brushes.Aquamarine, Brushes.Orange };

        private SolidColorBrush[] colorByChannel = new SolidColorBrush[16] { Brushes.Aqua, Brushes.RoyalBlue, Brushes.Blue, Brushes.BlueViolet, Brushes.Brown, Brushes.Chartreuse, Brushes.Crimson, Brushes.Red, Brushes.Salmon, Brushes.Silver, Brushes.Yellow, Brushes.White, Brushes.Violet, Brushes.Black, Brushes.Aquamarine, Brushes.Orange };

        private int ticksPerBeat;
        private int beatsPerMeasure;
        private Dictionary<int, List<int[]>> eventsAtTicksDict;
        private SongData curSong;
        private string curSongName;
        private string curSongFile;
        private int beat;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //entries are [instr, pitch, velocity, duration, channel]

            Dispatch.Beat += Beat;
            Dispatch.SongLoaded += SongLoaded;
            Dispatch.TickInfo += TickTriggered;
            Dispatch.VolumeChanged += VolumeChanged;

        }

        private void Beat(float time, int beat)
        {
            this.beat++;
        }

        public void VolumeChanged(float time, float volume)
        {
            VolumeGauge.Height = volume * PianoRoll.ActualHeight;
        }

        private void SongLoaded(SongData song, string songName, string songFile)
        {
            curSong = song;
            curSongName = songName;
            curSongFile = songFile;
            beat = 0;

            ticksPerBeat = 0;
            beatsPerMeasure = 0;
            eventsAtTicksDict = new Dictionary<int, List<int[]>>();

            SongName.Content = songName;

            lastPopulate = -100000000;

            List<Rectangle> markedChildren = new List<Rectangle>();
            foreach (Rectangle child in PianoRoll.Children)
            {
                markedChildren.Add(child);
            }
            foreach (Rectangle child in markedChildren)
            {
                PianoRoll.Children.Remove(child);
            }

            for (int i = 0; i < 16; i++)
            {
                var squareNumber = "square" + (i + 1).ToString(); //get the correct image to populate's name. I changed the image names to what's on that sheet of paper.
                var rectNumber = "rect" + (i + 1).ToString(); //get the correct rect to populate's name. I changed the rect names to what's on that sheet of paper.
                //Console.WriteLine(squareNumber); 

                object rectangle = FindName(rectNumber);
                Canvas rect = (Canvas)rectangle;
                rect.Background = colorByChannel[i];
                rect.Opacity = .6;

                object item = FindName(squareNumber); // turn its name from a string into the Image

                Image imgToPopulate = (Image)item;

                imgToPopulate.Source = null;

                ChannelHandler temp = new ChannelHandler();
                channels[i] = temp;
                channels[i].channelImage = imgToPopulate;
                channels[i].prevInst = -1;
            }
            ticksPerBeat = song.ppq;
            beatsPerMeasure = song.beatsPerMeasure;
            eventsAtTicksDict = song.eventsAtTicksDict;
            PianoRoll.ClipToBounds = true;
            VolumeGauge.ClipToBounds = true;

            Dispatch.TriggerLock(false);
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
        }

        private void PopulatePianoRoll()
        {
            if (currTick - lastPopulate > ticksPerBeat * beatsPerMeasure)
            {
                nextNoteBatch();
            }
        }

        private void nextNoteBatch()
        {
            for (int ticks = currTick; ticks < currTick + ticksPerBeat * (beatsPerMeasure * 2); ticks++)
            {
                if (eventsAtTicksDict.ContainsKey(ticks))
                {
                    foreach (int[] note in eventsAtTicksDict[ticks])
                    {
                        //entries are [instr, pitch, velocity, duration, channel]
                        double height = PianoRoll.ActualHeight / noteHeightResolution;
                        double width = note[3] / TicksPerPixel;
                        if (note[0] < 0) { note[0] = 0; }
                        Rectangle noteRect = new Rectangle { Width = width, Height = height, Fill = colorByChannel[note[4] % 16], Opacity = note[2] / 127d, Stroke = Brushes.Black, StrokeThickness = 2 };
                        noteRect.Tag = ticks;
                        double xPos = ((int)noteRect.Tag - currTick) / TicksPerPixel;
                        double yPos = PianoRoll.ActualHeight - ((note[1] % noteHeightResolution) / (double)noteHeightResolution * PianoRoll.ActualHeight); //high notes are low Y, because pixel numbering starts at top left
                        Canvas.SetLeft(noteRect, xPos);
                        Canvas.SetTop(noteRect, yPos);
                        PianoRoll.Children.Add(noteRect);
                        if (note[4] == 9) //drums on track 10
                        {
                            note[0] = 128;
                        }
                        //entries are [ticks, duration, instr, velocity]

                        channels[note[4]].EventData.Add(new int[] { ticks, note[3], note[0], note[2] });
                    }
                    eventsAtTicksDict.Remove(ticks);
                }
            }
            lastPopulate = currTick;
        }

        public int TicksInPianoRoll
        {
            get { return ticksPerBeat * beatsPerMeasure; }
        }

        public double TicksPerPixel
        {
            get { return TicksInPianoRoll / PianoRoll.ActualWidth; }
        }

        private void UpdateLeadIn()
        {
            if (beat == 1) StartBeat.Content = "And...";
            else if (1 < beat && beat < 6) StartBeat.Content = beat-1;
            else StartBeat.Content = "";
        }

        protected void UpdateRectangles()
        {
            List<Rectangle> markedChildren = new List<Rectangle>();
            foreach (Rectangle child in PianoRoll.Children)
            {
                double newPos = calculateNewPos(child);
                Canvas.SetLeft(child, newPos);
                if (newPos + child.ActualWidth < 0)
                {
                    markedChildren.Add(child);
                }
            }
            foreach (Rectangle child in markedChildren)
            {
                PianoRoll.Children.Remove(child);
            }
        }

        protected void UpdateChannelPictures()
        {

            foreach (ChannelHandler channel in channels)
            {
                bool escapedAtContinue = false;
                List<int[]> markedChildren = new List<int[]>();
                foreach (int[] note in channel.EventData)
                {
                    if ((note[0] + note[1]) < currTick)
                    {
                        markedChildren.Add(note);
                        escapedAtContinue = true;
                        continue;
                    }
                    if ((note[0] < currTick) && (currTick < (note[0] + note[1])))
                    {
                        escapedAtContinue = false;
                        if (note[2] != channel.prevInst)
                        {
                            channel.prevInst = note[2];

                            //entries are [tick, duration, instr, velocity]
                            //var uriString = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\" + (InstrumentEnumerator)note[2] + ".png";
                            var uriString = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\GUI\Resources\" + (InstrumentEnumerator)note[2] + ".jpg";
                            //Console.WriteLine(uriString);
                            BitmapImage bitIm = new BitmapImage();
                            bitIm.BeginInit();
                            bitIm.UriSource = new Uri(uriString);
                            bitIm.EndInit();
                            channel.channelImage.Source = bitIm;
                            channel.channelImage.Opacity = calcOpacity(note[3], ((note[0] + note[1]) - currTick) / (double)note[1]);
                            break;
                        }
                        else
                        {
                            channel.channelImage.Opacity = calcOpacity(note[3], ((note[0] + note[1]) - currTick) / (double)note[1]);
                            break;
                        }
                    }

                    else
                    {
                        escapedAtContinue = false;
                        channel.channelImage.Opacity = 0;
                        break;
                    }
                }
                if (escapedAtContinue) { channel.channelImage.Opacity = 0; }
                foreach (int[] child in markedChildren)
                {
                    channel.EventData.Remove(child);
                }
            }
        }

        private double calcOpacity(int velocity, double percentRemaining)
        {
            double correctedVelocity = velocity / 127d;
            //double correctedVelocity = 1d;

            return correctedVelocity * percentRemaining;
        }

        private double calculateNewPos(Rectangle child)
        {
            double pixelPosition = ((int)child.Tag - currTick) / TicksPerPixel;
            return pixelPosition;
        }

        private void TickTriggered(int tick)
        {
            currTick = tick;
            Dispatcher.Invoke((Action)(() => UpdateLeadIn()));
            Dispatcher.Invoke((Action)(() => UpdateRectangles()));
            Dispatcher.Invoke((Action)(() => UpdateChannelPictures()));
            PopulatePianoRoll();
        }

        private void EndSongButtonClick(object sender, RoutedEventArgs e)
        {
            StartBeat.Content = "";
            SongName.Content = "Loading Song...";
            Dispatch.TriggerStop();
            App.SelectSong();
        }

        private void RestartButtonClick(object sender, RoutedEventArgs e)
        {
            StartBeat.Content = "";
            SongName.Content = "Loading Song...";
            Dispatch.TriggerStop();
            App.PlaySong(curSongFile, curSongName, false);
        }

        private void QuitButtonClick(object sender, RoutedEventArgs e)
        {
            StartBeat.Content = "";
            SongName.Content = "Loading Song...";
            Dispatch.TriggerStop();
            App.ShowStartScreen();
        }
    }
}