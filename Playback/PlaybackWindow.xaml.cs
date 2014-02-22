using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

using Microsoft.Kinect;

namespace Orchestra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PlaybackWindow : Window
    {
        MainWindow gui = new MainWindow();
        static SynchronizationContext mainThread;

        public PlaybackWindow()
        {
            mainThread = SynchronizationContext.Current;
            InitializeComponent();
            gui.Show();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Hide this window
            Hide();

            // Load components
            Gestures.Load();
            MIDI.Load();

            // Start reading once the song is loaded
            new Thread(ReadCSV).Start();
        }

        static async void ReadCSV()
        {
            string filename = @"C:\Users\Rachel\Documents\GitHub\VirtualOrchestra\Sample Dances\TempoGesture1.csv";

            // Parse CSV
            var rows = File.ReadAllLines(filename)
                       .Select(r => r.Split(';')[0].Split(','));

            // Read headers
            var headers = rows.First();

            // Read data
            foreach (var row in rows.Skip(1))
            {
                // Delay
                Thread.Sleep(1000/30);

                // Read skeleton
                var skel = new Skeleton();
                var d = RowDict(row, headers);
                foreach (JointType type in Enum.GetValues(typeof(JointType)))
                {
                    var name = Enum.GetName(typeof(JointType), type);
                    var point = new SkeletonPoint();
                    point.X = d[name + ".X"];
                    point.Y = d[name + ".Y"];
                    point.Z = d[name + ".Z"];
                    var joint = skel.Joints[type];
                    joint.Position = point;
                    joint.TrackingState = point.X == float.NaN ? JointTrackingState.NotTracked : JointTrackingState.Tracked;
                    skel.Joints[type] = joint;
                }

                // Trigger skeleton update event in main thread
                mainThread.Send(state=>Dispatch.TriggerSkeletonMoved(skel), null);
            }
        }

        static Dictionary<String, float> RowDict(string[] row, string[] headers)
        {
            var dict = new Dictionary<String, float>();
            for (var i = 0; i < row.Length; ++i)
            {
                float x;
                bool valid = float.TryParse(row[i], out x);
                dict[headers[i]] = valid ? x : float.NaN;
            }
            return dict;
        }
    }
}
