using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Orchestra.Recorder
{
    class Recorder
    {
        static KinectSensor sensor;
        static Stopwatch stopwatch;
        static StreamWriter file;

        static bool InitKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                    break;
                }
            }

            if (sensor == null) return false;

            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += SkeletonFrameReady;
            sensor.Start();
            return true;
        }

        static void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get skeletons from Kinect
            Skeleton[] skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            // Push event to the rest of the system
            foreach (Skeleton skel in skeletons)
            {
                if (skel.TrackingState == SkeletonTrackingState.Tracked)
                {
                    SkeletonMoved(skel);
                    break;
                }
            }
        }

        static void SkeletonMoved(Skeleton skel)
        {
            // Jank-ass shit
            if (stopwatch == null)
            {
                Console.WriteLine(@"Found skeleton! Recording to Desktop\Dance.csv");

                // Start timer
                stopwatch = Stopwatch.StartNew();

                // Open output file
                file = new System.IO.StreamWriter(@"C:\Users\admin\Desktop\Dance.csv");

                // Write headers
                file.Write("Time");
                foreach (Joint joint in skel.Joints)
                {
                    JointType t = joint.JointType;
                    file.Write(", " + t + ".X, " + t + ".Y, " + t + ".Z");
                }
                file.WriteLine();
            }

            // Write row
            file.Write(stopwatch.ElapsedMilliseconds/1000d);
            foreach (Joint joint in skel.Joints)
            {
                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    SkeletonPoint p = joint.Position;
                    file.Write(", {0}, {1}, {2}", p.X.ToString("#.##"), p.Y.ToString("#.##"), p.Z.ToString("#.##"));
                }
                else
                {
                    file.Write(", , , ");
                }
            }
            file.WriteLine();
            file.Flush();
        }

        static void Main(string[] args)
        {
            if (!InitKinect()) return;
            Console.WriteLine("Press <Enter> to quit.\n");
            Console.WriteLine("Looking for skeleton...");
            Console.ReadLine();
            sensor.SkeletonFrameReady -= SkeletonFrameReady;
            Thread.Sleep(100);
        }
    }
}
