using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Orchestra
{
    public class Kinect
    {
        private static KinectSensor sensor;

        public static void Load()
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                    break;
                }
            }

            if (null != sensor)
            {
                // Subscribe to skeleton stream
                sensor.SkeletonStream.Enable();
                sensor.SkeletonFrameReady += SkeletonFrameReady;

                // Start the sensor
                try { sensor.Start(); }
                catch (IOException) { sensor = null; }
            }
        }

        private static void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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

            // Dispatch the first skeleton
            foreach (Skeleton skel in skeletons)
            {
                if (skel.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Dispatch.TriggerSkeletonMoved(skel);
                    break;
                }
            }
        }
    }
}
