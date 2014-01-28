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
    public partial class GUIWindow : Window
    {
        private Dictionary<int, Dictionary<string, List<int[]>>> instChangesDict;
        private List<int> instruments;
        private int currTick = 0;
        private ChannelHandler[] channels = new ChannelHandler[16];


        // private float beatsPerSecond = 2; //will be set dynamically

        //private int beatsPerMeasure = 4;
        private int lastPopulate = -100000; //tick value where most recent piano roll population occurred, starts low to trigger piano roll population
        private int noteHeightResolution = 48; //number of notes that fit vertically into the canvas

        //private LinearGradientBrush[] colorByChannel = new LinearGradientBrush[16] { LinearGradientBrush(.Aqua, Brushes.Beige, Brushes.Blue, Brushes.BlueViolet, Brushes.Brown, Brushes.Chartreuse, Brushes.Crimson, Brushes.Red, Brushes.Salmon, Brushes.Silver, Brushes.Yellow, Brushes.Turquoise, Brushes.Violet, Brushes.Black, Brushes.Aquamarine, Brushes.Orange };

        private SolidColorBrush[] colorByChannel = new SolidColorBrush[16] { Brushes.Aqua, Brushes.RoyalBlue, Brushes.Blue, Brushes.BlueViolet, Brushes.Brown, Brushes.Chartreuse, Brushes.Crimson, Brushes.Red, Brushes.Salmon, Brushes.Silver, Brushes.Yellow, Brushes.White, Brushes.Violet, Brushes.Black, Brushes.Aquamarine, Brushes.Orange };

        private int ticksPerBeat;
        private int beatsPerMeasure;
        private Dictionary<int, List<int[]>> eventsAtTicksDict;

        public GUIWindow()
        {
            InitializeComponent();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //entries are [instr, pitch, velocity, duration, channel]

            Dispatch.SongLoaded += SongLoaded;
            Dispatch.TickInfo += TickTriggered;
            Dispatch.VolumeChanged += VolumeChanged;

        }

        public void VolumeChanged(float time, float volume)
        {
            Console.WriteLine(volume);
            VolumeGauge.Opacity = (volume);
            VolumeGauge.Height = volume * 478;
        }

        private void SongLoaded(SongData song)
        {
            for (int i = 0; i < 16; i++)
            {
                var squareNumber = "square" + (i+1).ToString(); //get the correct image to populate's name. I changed the image names to what's on that sheet of paper.
                //Console.WriteLine(squareNumber); 
                object item = FindName(squareNumber); // turn its name from a string into the Image
                Image imgToPopulate = (Image)item;

                ChannelHandler temp = new ChannelHandler();
                channels[i] = temp;
                channels[i].channelImage = imgToPopulate;
            }
            ticksPerBeat = song.ppq;
            beatsPerMeasure = song.beatsPerMeasure;
            eventsAtTicksDict = song.eventsAtTicksDict;
            PianoRoll.ClipToBounds = true;
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
        }

        private void PreProcessInstruments(Dictionary<int, int[]> instDict)
        {

            instruments = new List<int>();
            foreach (KeyValuePair<int, int[]> instrument in instDict)
            {
                foreach (int inst in instrument.Value)
                {
                    instruments.Add(inst);
                    //Console.WriteLine("Adding: {0}", inst);
                }
            }
            //Console.WriteLine("Final list: {0} ", instruments);
            int counter = 1;
            foreach (int inst in instruments)
            {
                var squareNumber = "square" + counter.ToString(); //get the correct image to populate's name. I changed the image names to what's on that sheet of paper.
                //Console.WriteLine(squareNumber); 
                object item = FindName(squareNumber); // turn its name from a string into the Image
                Image imgToPopulate = (Image)item;

                var uriString = @"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\" + (InstrumentEnumerator)inst + ".jpg";
                //var uriString = @"C:\Users\Rachel\My Documents\GitHub\VirtualOrchestra\GUI\Resources\" + instName + ".jpg";
                Console.WriteLine(uriString);
                BitmapImage bitIm = new BitmapImage();
                bitIm.BeginInit();
                bitIm.UriSource = new Uri(uriString);
                bitIm.EndInit();
                imgToPopulate.Source = bitIm;

                counter += 1;
            }
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
                        double height = PianoRoll.ActualHeight / noteHeightResolution;
                        double width = note[3] / TicksPerPixel;
                        if (note[0] < 0) { note[0] = 0; }
                        Rectangle noteRect = new Rectangle { Width = width, Height = height, Fill = colorByChannel[note[4] % 16], Opacity = note[2] / 127d, Stroke = Brushes.Black, StrokeThickness = 2 };
                        noteRect.Tag = ticks;
                        double xPos = ((int)noteRect.Tag - currTick) / TicksPerPixel;
                        double yPos = PianoRoll.ActualHeight - (((note[1] + 12) % noteHeightResolution) / (double)noteHeightResolution * PianoRoll.ActualHeight); //high notes are low Y, because pixel numbering starts at top left
                        Canvas.SetLeft(noteRect, xPos);
                        Canvas.SetTop(noteRect, yPos);
                        PianoRoll.Children.Add(noteRect);
                        channels[note[4]].EventData.Add(new int[] { ticks, note[3], note[0], note[2]});
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

                            var uriString = @"C:\Users\admin\Desktop\VirtualOrchestra\GUI\Resources\" + (InstrumentEnumerator)note[2] + ".jpg";
                            //var uriString = @"C:\Users\Rachel\My Documents\GitHub\VirtualOrchestra\GUI\Resources\" + instName + ".jpg";
                            Console.WriteLine(uriString);
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

            return correctedVelocity * percentRemaining;
        }

        private double calculateNewPos(Rectangle child)
        {
            double pixelPosition = ((int)child.Tag - currTick) / TicksPerPixel;
            return pixelPosition;
        }

        //private void StartStopwatch()
        //{ 
        //    Timer aTimer = new System.Timers.Timer(10);
        //    aTimer.Elapsed += new ElapsedEventHandler(TestPlayback);
        //    aTimer.Enabled = true;
        //}

        private void TickTriggered(int tick)
        {
            currTick = tick;
            Dispatcher.Invoke((Action)(() => UpdateRectangles()));
            Dispatcher.Invoke((Action)(() => UpdateChannelPictures()));
            //UpdateRectangles();
            //TestPlayback(currTick);
            PopulatePianoRoll();
        }

        private void TestPlayback(int currTick)
        {
            ///

            ///
            if (instChangesDict.ContainsKey(currTick))
            {

                Console.WriteLine("\nCurrent tick:  {0}", currTick);
                System.Console.WriteLine("Turning On: ");
                foreach (int thing in instChangesDict[currTick]["TurnOn"][0])
                {
                    Console.WriteLine(thing);
                    Console.WriteLine(instChangesDict[currTick]["TurnOn"][thing].ToString());
                }

                System.Console.WriteLine("Turning Off: ");
                if (instChangesDict[currTick].ContainsKey("TurnOff"))
                {
                    foreach (int thing in instChangesDict[currTick]["TurnOff"][0])
                    {
                        Console.WriteLine(thing);
                        Console.WriteLine(instChangesDict[currTick]["TurnOn"][thing].ToString());
                    }
                }
            }

        }



        //    private Dictionary<int, Dictionary<string, List<int[]>>> MakeInstChangesDict()
        //    {
        //        //We should prolly break this into 2 functions. I'm just psuedocode vomiting. 
        //        //Start stopwatch to keep track of when events sh0uld happen (coming from dispatch)
        //        // Dictionary is like:
        //        // {AbsoluteTick1: [[instrument, pitch, velocity, note duration],[instrument, pitch, velocity, duration, channel]], AbsoluteTick2: [[...]...]}


        //        Dictionary<int, List<int[]>> instPlayingDict = new Dictionary<int, List<int[]>>(); // {Instrument: [all the ticks it plays at]}
        //        Dictionary<int, Dictionary<string, List<int[]>>> instChangesDict = new Dictionary<int, Dictionary<string, List<int[]>>>();

        //        foreach (KeyValuePair<int, List<int[]>> tick in eventsAtTicksDict)
        //        {
        //            foreach (int[] note in tick.Value) // Instrument notes kind of...
        //            {

        //                int instrument = note[0];

        //                if (!instPlayingDict.ContainsKey(instrument)) // if the instrument's not in the dictionary yet
        //                {
        //                    List<int[]> newInstList = new List<int[]>(); //make a list for its ticks
        //                    instPlayingDict.Add(note[0], newInstList); // put it in the dictionary
        //                }
        //                int[] tempArr = new int[] {tick.Key, (tick.Key + note[3]), note[4]};
        //                instPlayingDict[note[0]].Add(tempArr);

        //            }

        //        }
        //        // ok so we now have every tick in which an instrument plays


        //        foreach (KeyValuePair<int, List<int[]>> inst in instPlayingDict) //key is instrument, value is list of ticks they play at
        //        {
        //            int length = inst.Value.Count - 1;
        //            //always have a turnOn at the insttrument's first note
        //            if (!instChangesDict.ContainsKey(inst.Value[0][0]))
        //            {
        //                Dictionary<String, List<int[]>> subDict0 = new Dictionary<String, List<int[]>>();
        //                List<int[]> newInstList0 = new List<int[]>();
        //                subDict0.Add("TurnOn", newInstList0);
        //                instChangesDict.Add(inst.Value[0][0], subDict0);
        //            }
        //            instChangesDict[inst.Value[0][0]]["TurnOn"].Add(new int[] {inst.Key, inst.Value[0][2]});
        //            for (int i = 0; i < length; i++)
        //            {

        //                if ((inst.Value[i + 1][0] - inst.Value[i][1]) > 1000)
        //                {
        //                    if (!instChangesDict.ContainsKey(inst.Value[i + 1][0])) //if the dict doesnt have the tick as a key yet
        //                    {
        //                        Dictionary<String, List<int[]>> subDict = new Dictionary<String, List<int[]>>();
        //                        List<int[]> newInstList = new List<int[]>();
        //                        List<int[]> newInstList2 = new List<int[]>();
        //                        subDict.Add("TurnOn", newInstList);
        //                        subDict.Add("TurnOff", newInstList2);
        //                        instChangesDict.Add(inst.Value[i + 1][0], subDict); // put it in the dictionary
        //                    }
        //                    if (!instChangesDict.ContainsKey(inst.Value[i][1])) //if the dict doesnt have the tick as a key yet
        //                    {
        //                        Dictionary<String, List<int[]>> subDict2 = new Dictionary<String, List<int[]>>();
        //                        List<int[]> newInstList3 = new List<int[]>();
        //                        List<int[]> newInstList4 = new List<int[]>();
        //                        subDict2.Add("TurnOn", newInstList3);
        //                        subDict2.Add("TurnOff", newInstList4);
        //                        instChangesDict.Add(inst.Value[i][1], subDict2);
        //                    }
        //                    instChangesDict[inst.Value[i + 1][0]]["TurnOn"].Add(new int[] { inst.Key, inst.Value[0][2] });
        //                    instChangesDict[inst.Value[i][1]]["TurnOff"].Add(new int[] { inst.Key, inst.Value[0][2] });
        //                }
        //            }
        //        }
        //        return instChangesDict;
        //    }
        //}
    }
}