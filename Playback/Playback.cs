using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Orchestra.Playback
{
    class Playback
    {
        static Dictionary<String,float> RowDict(string[] row, string[] headers)
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

        static void ReadCSV(string filename)
        {
            // Parse CSV
            var rows = File.ReadAllLines(filename)
                       .Select(r => r.Split(';')[0].Split(','));

            // Read headers
            var headers = rows.First();

            // Read data
            foreach (var row in rows.Skip(1))
            {
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

                // Trigger skeleton update event
                Dispatch.TriggerSkeletonMoved(skel);
            }
        }

        static void Main(string[] args)
        {
            Gestures.Load();
            ReadCSV(@"C:\Users\admin\Desktop\Dance.csv");
            Console.ReadLine();
        }
    }
}