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
using Sanford.Multimedia.Midi;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///         Image flute = new Image();


    public partial class MainWindow : Window
    {
        private Sequence seq1;
        //private Sequence seq2; //for preprocessing
        private Sequencer seqr1;
        //private Sequencer seqr2; //for preprocessing
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        //private bool donePreProcessing = false;

        
        int[] instpos = new int[16];
        //int[] positions = new int[256];



        public MainWindow()
        {
            InitializeComponent();
            
        }

        public void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            seqr1.Stop();
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var x = seq1.GetLength();
            seqr1.Sequence = seq1;
            //Preprocess();
            seqr1.Start();
        }
        //private void HandlePlayingCompleted(object sender, EventArgs e)
        //{
        //    donePreProcessing = true;
        //}
        //private void Preprocess()
        //{
        //    seq2 = seq1;
        //    seqr2 = seqr1;
        //    seqr2.Start();
        //    while (!donePreProcessing)
        //    {

        //        //seq2 = new Sanford.Multimedia.Midi.Sequence();
        //        //seqr2 = new Sanford.Multimedia.Midi.Sequencer();
        //        //seqr2.Sequence = seq2;
        //        //seq2.LoadAsync(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\g.mid");
                
        //        seqr2.clock.Tempo = 1000;
        //        seqr2.ChannelMessagePlayed += PreProcessChannelMessage;
        //        seqr2.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                
        //    }
        //    seqr2.Stop();
        //}
        
        //private void PreProcessChannelMessage(object sender, ChannelMessageEventArgs e)
        //{
        //    seqr2.clock.Tempo = 1000;
        //    if (e.Message.Command == ChannelCommand.ProgramChange)
        //    {
        //        positions[e.Message.Data1+1] = 1;
        //    }
        //}

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.seq1 = new Sanford.Multimedia.Midi.Sequence();
            this.seqr1 = new Sanford.Multimedia.Midi.Sequencer();
            seq1.LoadAsync(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\g.mid");

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
                //Console.Write("Changing Midi Channel ");
                //Console.Write(e.Message.MidiChannel);
                //Console.Write(" to ");
                //Console.WriteLine((GeneralMidiInstrument)e.Message.Data1);
                instpos[e.Message.MidiChannel] = e.Message.Data1;
            }
            if (e.Message.Command == ChannelCommand.NoteOn) 
            {
                //Console.Write("Playing note on ");
                //Console.WriteLine((GeneralMidiInstrument)instpos[e.Message.MidiChannel]);
                if (instpos[e.Message.MidiChannel] == 73)
                {
  

                    this.Dispatcher.Invoke((Action)(() =>
                    {

                        Image myImage = new Image();
                        myImage.Width = 200;

                        BitmapImage myBitmapImage = new BitmapImage();

                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = new Uri(@"C:\Users\Admin\Desktop\VirtualOrchestra\GUI\Resources\flute.jpg");

                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();

                        InstrumentImage.Source = myBitmapImage;
                        //Application.Current.MainWindow.Background = new SolidColorBrush(Colors.DarkOrchid);
                    }));
                }

                if (instpos[e.Message.MidiChannel] == 71)
                {


                    this.Dispatcher.Invoke((Action)(() =>
                    {

                        Image myImage = new Image();
                        myImage.Width = 200;

                        BitmapImage myBitmapImage = new BitmapImage();

                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = new Uri(@"C:\Users\Admin\Desktop\VirtualOrchestra\GUI\Resources\clarinet.jpg");

                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();

                        InstrumentImage2.Source = myBitmapImage;
                        //Application.Current.MainWindow.Background = new SolidColorBrush(Colors.DarkOrchid);
                    }));
                }
                if (instpos[e.Message.MidiChannel] == 60)
                {


                    this.Dispatcher.Invoke((Action)(() =>
                    {

                        Image myImage = new Image();
                        myImage.Width = 200;

                        BitmapImage myBitmapImage = new BitmapImage();

                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = new Uri(@"C:\Users\Admin\Desktop\VirtualOrchestra\GUI\Resources\frenchhorn.jpg");

                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();

                        InstrumentImage3.Source = myBitmapImage;
                        //Application.Current.MainWindow.Background = new SolidColorBrush(Colors.DarkOrchid);
                    }));
                }
                if (instpos[e.Message.MidiChannel] == 48)
                {


                    this.Dispatcher.Invoke((Action)(() =>
                    {

                        Image myImage = new Image();
                        myImage.Width = 200;

                        BitmapImage myBitmapImage = new BitmapImage();

                        myBitmapImage.BeginInit();
                        myBitmapImage.UriSource = new Uri(@"C:\Users\Admin\Desktop\VirtualOrchestra\GUI\Resources\violin.jpg");

                        myBitmapImage.DecodePixelWidth = 200;
                        myBitmapImage.EndInit();

                        InstrumentImage4.Source = myBitmapImage;
                        //Application.Current.MainWindow.Background = new SolidColorBrush(Colors.DarkOrchid);
                    }));
                }
            }
            
            if (e.Message.Command == ChannelCommand.NoteOff)
            {
                //Console.Write("Stopping note on ");
                //Console.WriteLine((GeneralMidiInstrument)instpos[e.Message.MidiChannel]);
                this.Dispatcher.Invoke((Action)(() =>
                {

                    Image myImage = new Image();
                    myImage.Width = 200;


                    BitmapImage myBitmapImage = new BitmapImage();

                    myBitmapImage.BeginInit();
                    myBitmapImage.UriSource = new Uri(@"C:\Users\Admin\Desktop\VirtualOrchestra\GUI\Resources\whitesquare.jpg");

                    myBitmapImage.DecodePixelWidth = 200;
                    myBitmapImage.EndInit();
                    InstrumentImage.Source = myBitmapImage;
                    InstrumentImage2.Source = myBitmapImage;
                    InstrumentImage3.Source = myBitmapImage;
                    InstrumentImage4.Source = myBitmapImage;
                    //Application.Current.MainWindow.Background = new SolidColorBrush(Colors.Azure);
                }));
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
