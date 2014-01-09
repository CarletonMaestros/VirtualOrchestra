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


namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///         Image flute = new Image();


    public partial class MainWindow : Window
    {
        private Dictionary<int, int[]> instDict;
        private HashSet<int> instruments;
        private Sequence seq1;
        private Sequencer seqr1;
        private OutputDevice outDevice;
        private Storyboard johnStoryboard;
        private Storyboard paulStoryboard;
        private Storyboard georgeStoryboard;
        private DoubleAnimation myDoubleAnimation;
        private int outDeviceID = 0;
        int[] instpos = new int[16];

        public MainWindow()
        {
            InitializeComponent();
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            instDict = new Dictionary<int, int[]>();
            PreProcessInstruments(instDict);
            
            myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 1.0;
            myDoubleAnimation.To = 0.2;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(.2));
            myDoubleAnimation.AutoReverse = true;
            myDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //johnStoryboard = new Storyboard();
            //johnStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, johnImage.Name);
            //Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Image.OpacityProperty));
            //johnStoryboard.Begin(this, true);
            //johnStoryboard.Pause(this);

            //paulStoryboard = new Storyboard();
            //paulStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, paulImage.Name);
            //Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Image.OpacityProperty));
            //paulStoryboard.Begin(this, true);
            //paulStoryboard.Pause(this);

            //georgeStoryboard = new Storyboard();
            //georgeStoryboard.Children.Add(myDoubleAnimation);
            //Storyboard.SetTargetName(myDoubleAnimation, georgeImage.Name);
            //Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Image.OpacityProperty));
            //georgeStoryboard.Begin(this, true);
            //georgeStoryboard.Pause(this);
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
            //int[] instDictVal = new int[] {1, 2, 3, 4, 5};
            instDict.Add(0, new int[] {24, 35});
            instDict.Add(1, new int[] {25});
            instruments = new HashSet<int>();
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


                //One way to approach setting a image's source here:
                var uriSource = new Uri("piano.jpg", UriKind.Relative); //whaaa, it's not throwing errors, but it's like this isn't the right path to an instrument file because the image isn't showing up.
                Console.WriteLine(uriSource);
                imgToPopulate.Source = new BitmapImage(uriSource);


                //And another approach:
                //Image testImage = new Image();
                //BitmapImage bitmapImage = new BitmapImage();
                //testImage.Width = bitmapImage.DecodePixelWidth = 80;
                //bitmapImage.UriSource = new Uri(testImage.BaseUri, "Images/myimage.png");

                //testImage.Source = 
                counter += 1;
            }

        }


        private void StartTicks(Dictionary<int, List<int[]>> ticksDict)
        {
            //Start stopwatch to keep track of when events sh0uld happen (coming from dispatch)
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.seq1 = new Sanford.Multimedia.Midi.Sequence();
            this.seqr1 = new Sanford.Multimedia.Midi.Sequencer();
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