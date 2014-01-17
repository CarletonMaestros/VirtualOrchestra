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
        int[] instpos = new int[16];


        public MainWindow()
        {
            InitializeComponent();
        }


        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            instDict = new Dictionary<int, int[]>();
            //PreProcessInstruments(instDict);


            
            Dictionary<int, int[][]> ticksDict = new Dictionary<int, int[][]>();
            int[][] jaggedArray = {
       
            new int[] {1, 2, 3, 4},
            new int[] {5, 6, 7, 8},
            new int[] {9, 10, 11, 12},
            };
            ticksDict.Add(0, jaggedArray);
            int[][] jaggedArray2 = {
       
            new int[] {11, 12, 13, 14},
            new int[] {15, 16, 17, 18},
            new int[] {19, 110, 111, 112},
            };
            ticksDict.Add(5, jaggedArray2);
            int[][] jaggedArray3 = {
       
            new int[] {11, 12, 13, 14},
            new int[] {15, 16, 17, 18},
            new int[] {19, 110, 111, 112},
            };
            ticksDict.Add(5000, jaggedArray3);
            int[][] jaggedArray4 = {
       
            new int[] {11, 12, 13, 14},
            new int[] {15, 16, 17, 18},
            new int[] {19, 110, 111, 112},
            };
            ticksDict.Add(12000, jaggedArray4);
            MakeInstChangesDict(ticksDict);


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

        }


        private void MakeInstChangesDict(Dictionary<int, int[][]> ticksDict)
        {
            //We should prolly break this into 2 functions. I'm just psuedocode vomiting. 
            //Start stopwatch to keep track of when events sh0uld happen (coming from dispatch)
            // Dictionary is like:
            // {AbsoluteTick1: [[instrument, pitch, velocity, note duration],[instrument, pitch, velocity, duration]], AbsoluteTick2: [[...]...]}
            Console.WriteLine("at makeinstchangedict");

            Dictionary<int, List<int>> instPlayingDict = new Dictionary<int, List<int>>(); // {Instrument: [all the ticks it plays at]}
            Dictionary<int, Dictionary<string, List<int>>> instChangesDict = new Dictionary<int, Dictionary<string, List<int>>>();

            foreach (KeyValuePair<int, int[][]> tick in ticksDict)
            {
                foreach (int[] note in tick.Value) // Instrument notes kind of...
                {

                    int instrument = note[0];

                    if (!instPlayingDict.ContainsKey(instrument)) // if the instrument's not in the dictionary yet
                    {
                        List<int> newInstList = new List<int>(); //make a list for its ticks
                        instPlayingDict.Add(note[0], newInstList); // put it in the dictionary
                    }

                    instPlayingDict[note[0]].Add(tick.Key);

                }

            }
            // ok so we now have every tick in which an instrument plays


            foreach (KeyValuePair<int, List<int>> inst in instPlayingDict) //key is instrument, value is list of ticks they play at
            {
                int length = inst.Value.Count - 1;
                for (int i = 0; i < length; i++)
                {
                    Console.WriteLine(inst.Value[i + 1]);
                    Console.WriteLine(inst.Value[i]);
                    if (inst.Value[i + 1] - inst.Value[i] > 1000)
                    {
                        if (!instChangesDict.ContainsKey(inst.Value[i + 1])) //if the dict doesnt have the tick as a key yet
                        {
                            Dictionary<String, List<int>> subDict = new Dictionary<String, List<int>>();
                            List<int> newInstList = new List<int>();
                            subDict.Add("TurnOn", newInstList);
                            subDict.Add("TurnOff", newInstList);
                            instChangesDict.Add(inst.Value[i + 1], subDict); // put it in the dictionary
                        }
                        if (!instChangesDict.ContainsKey(inst.Value[i])) //if the dict doesnt have the tick as a key yet
                        {
                            Dictionary<String, List<int>> subDict2 = new Dictionary<String, List<int>>();
                            List<int> newInstList2 = new List<int>();
                            subDict2.Add("TurnOn", newInstList2);
                            subDict2.Add("TurnOff", newInstList2);
                            instChangesDict.Add(inst.Value[i], subDict2);
                        }
                        Dictionary<String, List<int>> tempDict = instChangesDict[inst.Value[i + 1]];
                        tempDict["TurnOn"].Add(inst.Key);
                        //instChangesDict[inst.Value[i + 1]]["TurnOn"].Add(inst.Key);
                        instChangesDict[inst.Value[i]]["TurnOff"].Add(inst.Key);
                    }
                }
            }


            foreach (KeyValuePair<int, Dictionary<string, List<int>>> inst in instChangesDict)
            {

                Console.WriteLine("Key:");

                Console.WriteLine(inst.Key);
                Console.WriteLine("Value:");

            }
        }
            //    foreach (int[] note in tick.Value) // Instrument notes kind of...
            //    {
            //        currentInstrument = note[0];
            //        if (noteLengthDict[currentInstrument][0] == null) { //if there's no value for a start time
            //            noteLengthDict[currentInstrument][0] = tick.Key; //setting the start time for an instrument
            //        }
            //        noteLengthDict[currentInstrument][1] = tick.Key + note[3]; //end time is startTime+duration
            //    }
            //    foreach (KeyValuePair<int, int[]> instrument in noteLengthDict)
            //    {
            //        int startTick = instChangesDict[instrument.Value[0]]; 
            //        if (tick.Key > (instrument.Value[1] + 1000))
            //        {
            //            int value = 0;
            //            // WILL CREATE NEW DICTIONARIES WHEN IT DOESNT HAVE TO
            //            if (instChangesDict.TryGetValue(tick.Key, out value)) //if the startTime isn't a key in the dictionary yet                          
            //            {
            //                instChangesDict[instrument.Value[0]] = new Dictionary<string, int>();
            //            }
            //            instChangesDict[instrument.Value[0]]["TurnOn"] = instrument.Key;
            //            instChangesDict[instrument.Value[1]] = new Dictionary<string,int>();
            //            instChangesDict[instrument.Value[1]]["TurnOff"] = instrument.Key;
            //        }
            //    }
            //}
        


            // for each tick in ticksDict.keys():
            //   for each instrument in each of those ticks:
            //       instrumentOff = tick
            //       while the instrument shows up in ticksDict[tick]: ticksDict[tick+1000]:
            //            instrumentOff = last tick at which instrument has a note
            //currentTick = given by MIDI via dispath
            // Leave instrument lit up for at least note duration. 
            // tick = 60000 / (BPM * PPQ), so say avg tick = 2.5 millisec
            // Check for occurrence of same instrument within key values of AbsoluteTickX + 1000 
            // (2.5 second window for instrument to come back in)
            
            //for all the instruments in tickDict at a given tick
            //{   
            //    set instrument image transparency to 0


                
            //}
            //  If tickStopwatch = currentTime + timeToPlay, increase the instrument image's transparency:
            //ImageTransparency.ChangeOpacity(imgToPopulate, 10);

                    

        //}
       



        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
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
            if (e.Message.Command == ChannelCommand.ProgramChange)
            {
                Console.Write("Changing Midi Channel ");
                Console.Write(e.Message.MidiChannel);
                Console.Write(" to ");
                Console.WriteLine((GeneralMidiInstrument)e.Message.Data1);
                instpos[e.Message.MidiChannel] = e.Message.Data1;
            }
            if (e.Message.Command == ChannelCommand.NoteOn)
            {
                Console.Write("Playing note on ");
                Console.WriteLine((GeneralMidiInstrument)instpos[e.Message.MidiChannel]);
                //    if (instpos[e.Message.MidiChannel] == 24)
                //    {

                //        this.Dispatcher.Invoke((Action)(() =>
                //        {
                //            if (johnStoryboard.GetIsPaused(this) == true)
                //            {
                //                johnStoryboard.Resume(this);
                //            }
                //        }));
                //    }

                //    if (instpos[e.Message.MidiChannel] == 35)
                //    {
                //        this.Dispatcher.Invoke((Action)(() =>
                //        {
                //            if (paulStoryboard.GetIsPaused(this) == true)
                //            {
                //                paulStoryboard.Resume(this);
                //            }
                //        }));
                //    }
                //    if (instpos[e.Message.MidiChannel] == 25)
                //    {
                //        this.Dispatcher.Invoke((Action)(() =>
                //        {
                //            if (georgeStoryboard.GetIsPaused(this) == true)
                //            {
                //                georgeStoryboard.Resume(this);
                //            }
                //        }));
                //    }
                //}
            }
            if (e.Message.Command == ChannelCommand.NoteOff)
            {
                Console.Write("Stopping note on ");
                Console.WriteLine((GeneralMidiInstrument)instpos[e.Message.MidiChannel]);
            //    if (instpos[e.Message.MidiChannel] == 24)
            //    {
            //        this.Dispatcher.Invoke((Action)(() =>
            //        {
            //            if (johnStoryboard.GetIsPaused(this) == false)
            //            {
            //                johnStoryboard.Pause(this);
            //            }
            //        }));
            //    }
            //    if (instpos[e.Message.MidiChannel] == 35)
            //    {
            //        this.Dispatcher.Invoke((Action)(() =>
            //        {
            //            if (paulStoryboard.GetIsPaused(this) == false)
            //            {
            //                paulStoryboard.Pause(this);
            //            }
            //        }));
            //    }
            //    if (instpos[e.Message.MidiChannel] == 25)
            //    {
            //        this.Dispatcher.Invoke((Action)(() =>
            //        {
            //            if (georgeStoryboard.GetIsPaused(this) == false)
            //            {
            //                georgeStoryboard.Pause(this);
            //            }
            //        }));
            //    }
            //}
            }
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