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
using Sanford.Multimedia.Midi;
using ImageUtils;
using System.Diagnostics;
using System.Timers;


namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///         Image flute = new Image();


    public partial class MainWindow : Window
    {
        private Dictionary<int, int[]> instDict;
        private Dictionary<int, int[]> ticksDict;
        private Dictionary<int, Dictionary<string, List<int>>> instChangesDict;
        private List<int> instruments;
        private Sequence seq1;
        private Sequencer seqr1;
        private OutputDevice outDevice;
        private Storyboard SB1;
        private Storyboard SB2;
        private Storyboard SB3;
        private Storyboard SB4;
        private Storyboard SB5;
        private Storyboard SB6;
        private Storyboard SB7;
        private Storyboard SB8;
        private Storyboard SB9;
        private Storyboard SB10;
        private Storyboard SB11;
        private Storyboard SB12;
        private Storyboard SB13;
        private Storyboard SB14;
        private Storyboard SB15;
        private Storyboard SB16;
        private int outDeviceID = 0;
        private int currTick = 0;
        int[] instpos = new int[16];

        private float beatsPerSecond = 2; //will be set dynamically

        public MainWindow()
        {
            InitializeComponent();
        }


        public void WindowLoaded(object sender, RoutedEventArgs e)
        {           
            instDict = new Dictionary<int, int[]>();
            PreProcessInstruments(instDict);


            //entries are [instr, pitch, velocity, duration]
            Dictionary<int, int[][]> ticksDict = new Dictionary<int, int[][]>();
            int[][] jaggedArray = {
       
            new int[] {2, 2, 3, 4},
            new int[] {3, 6, 7, 8},
            new int[] {5, 10, 11, 12},
            };
            ticksDict.Add(1, jaggedArray);
            int[][] jaggedArray2 = {
       
            new int[] {2, 12, 13, 14},
            new int[] {3, 16, 17, 18},
            new int[] {5, 110, 111, 112},
            };
            ticksDict.Add(5, jaggedArray2);
            int[][] jaggedArray3 = {
       
            new int[] {2, 12, 13, 14},
            new int[] {3, 16, 17, 18},
            new int[] {5, 110, 111, 112},
            };
            ticksDict.Add(2000, jaggedArray3);
            int[][] jaggedArray5 = {
       
            new int[] {2, 12, 13, 14},
            new int[] {3, 16, 17, 18},
            new int[] {5, 110, 111, 112},
            };
            ticksDict.Add(2400, jaggedArray5);            
            int[][] jaggedArray4 = {
       
            new int[] {2, 12, 13, 14},
            new int[] {3, 16, 17, 18},
            new int[] {5, 110, 111, 112},
            };
            ticksDict.Add(3500, jaggedArray4);
            instChangesDict = MakeInstChangesDict(ticksDict);




            //instChangesDict = new Dictionary<int, Dictionary<string, int[]>>();

            //myDoubleAnimation = new DoubleAnimation();
            //myDoubleAnimation.From = 1.0;
            //myDoubleAnimation.To = 0.2;
            //myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.2));
            //myDoubleAnimation.AutoReverse = true;
            //myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //FIX THIS LATER


            
            //curStoryboard = new Storyboard();
            //curStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, square1.Name);
            //Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Image.OpacityProperty));
            //curStoryboard.Begin(this, true);
            //curStoryboard.Pause(this);

        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            seqr1.Stop();
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var x = seq1.GetLength();
            seqr1.Sequence = seq1;
            seqr1.Start();
        }

        private void PreProcessInstruments(Dictionary<int, int[]> instDict)
        {
            instDict.Add(0, new int[] {34, 23, 75, 5, 112, 33, 84, 120, 112, 1, 85, 85, 52, 52, 115});
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
                Console.WriteLine(squareNumber); 
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

            GeneratePianoRoll();
            // CompositionTarget.Rendering += UpdateRectangles;
        }

        private void GeneratePianoRoll()
        {
            Rectangle rect = new Rectangle { Width = 400, Height = 400, Fill = Brushes.Red };
            Point newPoint = new Point(100,0);
            Canvas.SetTop(rect, newPoint.Y);
            Canvas.SetLeft(rect, newPoint.X);

            PianoRoll.ClipToBounds = true;
            PianoRoll.Children.Add(rect);
        }

        protected void UpdateRectangles(object sender, EventArgs e)
        {
            Rectangle rect = new Rectangle { Width = 400, Height = 400, Fill = Brushes.Red }; ;
            Point newPoint = new Point(0, 0);
            Canvas.SetTop(rect, newPoint.Y);
            Canvas.SetLeft(rect, newPoint.X);

        }

        private void StartStopwatch()
        { 
            Timer aTimer = new System.Timers.Timer(10);
            aTimer.Elapsed += new ElapsedEventHandler(TestPlayback);
            aTimer.Enabled = true;


        }

        private void TestPlayback(object source, ElapsedEventArgs e)
        {
            currTick++;
            if (instChangesDict.ContainsKey(currTick))
            {

                Console.WriteLine("\nCurrent tick:  {0}", currTick);
                System.Console.WriteLine("Turning On: ");
                foreach (int thing in instChangesDict[currTick]["TurnOn"])
                {
                    Console.WriteLine(thing);
                    //Console.WriteLine(instChangesDict[currTick]["TurnOn"][i].ToString());
                }

                System.Console.WriteLine("Turning Off: ");
                if (instChangesDict[currTick].ContainsKey("TurnOff"))
                {
                    foreach (int thing in instChangesDict[currTick]["TurnOff"])
                    {
                        Console.WriteLine(thing);
                        //Console.WriteLine(instChangesDict[currTick]["TurnOn"][i].ToString());
                    }
                }
            }
            
        }
        



        private Dictionary<int, Dictionary<string, List<int>>> MakeInstChangesDict(Dictionary<int, int[][]> ticksDict)
        {
            //We should prolly break this into 2 functions. I'm just psuedocode vomiting. 
            //Start stopwatch to keep track of when events sh0uld happen (coming from dispatch)
            // Dictionary is like:
            // {AbsoluteTick1: [[instrument, pitch, velocity, note duration],[instrument, pitch, velocity, duration]], AbsoluteTick2: [[...]...]}


            Dictionary<int, List<int[]>> instPlayingDict = new Dictionary<int, List<int[]>>(); // {Instrument: [all the ticks it plays at]}
            Dictionary<int, Dictionary<string, List<int>>> instChangesDict = new Dictionary<int, Dictionary<string, List<int>>>();

            foreach (KeyValuePair<int, int[][]> tick in ticksDict)
            {
                foreach (int[] note in tick.Value) // Instrument notes kind of...
                {

                    int instrument = note[0];

                    if (!instPlayingDict.ContainsKey(instrument)) // if the instrument's not in the dictionary yet
                    {
                        List<int[]> newInstList = new List<int[]>(); //make a list for its ticks
                        instPlayingDict.Add(note[0], newInstList); // put it in the dictionary
                    }
                    int[] tempArr = new int[] {tick.Key, (tick.Key + note[3])};
                    instPlayingDict[note[0]].Add(tempArr);

                }

            }
            // ok so we now have every tick in which an instrument plays


            foreach (KeyValuePair<int, List<int[]>> inst in instPlayingDict) //key is instrument, value is list of ticks they play at
            {
                int length = inst.Value.Count - 1;
                //always have a turnOn at the insttrument's first note
                if (!instChangesDict.ContainsKey(inst.Value[0][0]))
                {
                    Dictionary<String, List<int>> subDict0 = new Dictionary<String, List<int>>();
                    List<int> newInstList0 = new List<int>();
                    subDict0.Add("TurnOn", newInstList0);
                    instChangesDict.Add(inst.Value[0][0], subDict0);
                }
                instChangesDict[inst.Value[0][0]]["TurnOn"].Add(inst.Key);
                for (int i = 0; i < length; i++)
                {

                    if ((inst.Value[i + 1][0] - inst.Value[i][1]) > 1000)
                    {
                        if (!instChangesDict.ContainsKey(inst.Value[i + 1][0])) //if the dict doesnt have the tick as a key yet
                        {
                            Dictionary<String, List<int>> subDict = new Dictionary<String, List<int>>();
                            List<int> newInstList = new List<int>();
                            List<int> newInstList2 = new List<int>();
                            subDict.Add("TurnOn", newInstList);
                            subDict.Add("TurnOff", newInstList2);
                            instChangesDict.Add(inst.Value[i + 1][0], subDict); // put it in the dictionary
                        }
                        if (!instChangesDict.ContainsKey(inst.Value[i][1])) //if the dict doesnt have the tick as a key yet
                        {
                            Dictionary<String, List<int>> subDict2 = new Dictionary<String, List<int>>();
                            List<int> newInstList3 = new List<int>();
                            List<int> newInstList4 = new List<int>();
                            subDict2.Add("TurnOn", newInstList3);
                            subDict2.Add("TurnOff", newInstList4);
                            instChangesDict.Add(inst.Value[i][1], subDict2);
                        }
                        instChangesDict[inst.Value[i + 1][0]]["TurnOn"].Add(inst.Key);
                        instChangesDict[inst.Value[i][1]]["TurnOff"].Add(inst.Key);
                    }
                }
            }

            

            return instChangesDict;
        }

       



        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            StartStopwatch();
            this.seq1 = new Sanford.Multimedia.Midi.Sequence();
            this.seqr1 = new Sanford.Multimedia.Midi.Sequencer();

            //seq1.LoadAsync(@"C:\Users\Rachel\My Documents\GitHub\VirtualOrchestra\Sample MIDIs\r.mid");
            seq1.LoadAsync(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.mid");

            //sequencer1.Stop() followed by sequencer1.Continue could be used to handle changing tempo
            //also, perhaps sequencer1.position could be used (ticks)
            //sequence1.GetLength()

            if (OutputDevice.DeviceCount == 0)
            {
                Console.WriteLine("No MIDI output devices available.");
            }
            else
            {
                try
                {
                    outDevice = new OutputDevice(outDeviceID);
                    seq1.LoadCompleted += HandleLoadCompleted;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message, "Error!");
                }
            }
            seqr1.ChannelMessagePlayed += HandleChannelMessagePlayed;
            seqr1.Chased += HandleChased;
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            outDevice.Send(e.Message);            
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }
    }
}