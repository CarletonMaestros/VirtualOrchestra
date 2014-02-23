using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Kinect;

namespace Orchestra
{
    public class TempoVolume
    {
        String seeking;
        SkeletonPoint prevOne;
        SkeletonPoint prevTwo;
        SkeletonPoint rightHand;
        SkeletonPoint rightHip;
        float threshold;
        int stillFramesCount;
        int framesInFirstBeat;
        double volume = 64;
        float newVolume = 64;
        float prevBeat;
        List<float> xValues = new List<float>();

        public TempoVolume()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;
            seeking = "STILL";
            stillFramesCount = 0;
            framesInFirstBeat = 0;
        }

        public void print(float a) { Console.WriteLine(a); }

        public void print(int a) { Console.WriteLine(a); }

        public void print(string a) { Console.WriteLine(a); }

        public void print(double a) { Console.WriteLine(a); }

        public void Unload() { Dispatch.SkeletonMoved -= this.SkeletonMoved; }

        void SongLoaded(SongData song, string songName, string songFile) 
        { 
            Dispatch.TriggerVolumeChanged(0.5f); 
            rightHip.X = 0; 
        }

        void SkeletonMoved(float time, Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                rightHand = skel.Joints[JointType.HandRight].Position;
                if (rightHip.X == 0) { rightHip = skel.Joints[JointType.HipRight].Position; }
            }
            switch (seeking)
            {
                case "STILL":
                {
                    if (stillFramesCount == 15) { seeking = "START"; }
                    if (stillFramesCount < 15 && Math.Abs(prevOne.X - rightHand.X) < .08 && Math.Abs(prevOne.Y - rightHand.Y) < .08 && Math.Abs(prevTwo.X - rightHand.X) < .08 && Math.Abs(prevTwo.Y - rightHand.Y) < .08)
                    {
                        stillFramesCount++;
                        break;
                    }
                    else if (stillFramesCount < 15) { stillFramesCount = 0; }
                    break;
                }
                case "START":
                {
                    if (prevTwo.Y < (prevOne.Y - .01) && prevOne.Y < (rightHand.Y - .01))
                    {
                        framesInFirstBeat++;
                        break;
                    }
                    if (framesInFirstBeat >= 2 && prevTwo.Y > (prevOne.Y + .01) && prevOne.Y > (rightHand.Y + .01))
                    {
                        seeking = "MINIMUM";
                        break;
                    }
                    break;
                }
                case "MINIMUM":
                {
                    if (prevTwo.Y < (prevOne.Y - threshold) && prevOne.Y < (rightHand.Y - threshold))
                    {
                        xValues.Add(rightHand.X);
                        if (xValues.Count == 5) { xValues.RemoveAt(0); }
                        if (xValues.Count == 4) { if (xValues.Min() == xValues.ElementAt(3)) { volume = Math.Abs((rightHand.X - prevBeat) / .4) * 127; } }
                        prevBeat = rightHand.X;
                        seeking = "MAXIMUM";
                        break;
                    }
                    break;
                }
                case "MAXIMUM":
                {
                    if (prevTwo.Y > (prevOne.Y + threshold) && prevOne.Y > (rightHand.Y + threshold))
                    {
                        seeking = "MINIMUM";
                        break;
                    }
                    break;
                }
            }
            if (Math.Abs(volume - newVolume) > 10)
            {
                if (volume < newVolume) { if (newVolume > 0) { newVolume -= (2 * newVolume / 127); } }
                if (volume > newVolume) { if (newVolume < 127) { newVolume += (2 * (127 - newVolume) / 127); } }
            }
            Dispatch.TriggerVolumeChanged(newVolume / 127);
            prevTwo = prevOne;
            prevOne = rightHand;
        }
    }
}