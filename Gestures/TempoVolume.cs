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
        Int32 counter = 0;
        Stopwatch stopwatch;
        String seeking;
        float prevYOne;
        float prevYTwo;
        float prevXOne;
        float prevXTwo;
        float rightHandY;
        float rightHandX;
        float rightHipX = 0;
        float rightHipY;
        float threshold;
        int stillFramesCount;
        int framesInFirstBeat;
        double volume = 64;
        float floatVolume;
        float newVolume = 64;
        int beat = 1;
        float prevBeat;
        float prevBeatOne;
        float minX;
        float maxX;
        float maxY;
        double curVolume;
        float curVolumeFloat;
        List<float> xValues = new List<float>();
        bool smallestX;

        public TempoVolume()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;
            stopwatch = new Stopwatch();
            counter = 0;
            seeking = "STILL";
            threshold = .002F;
            stillFramesCount = 0;
            framesInFirstBeat = 0;
        }

        public void print(float a) { Console.WriteLine(a); }

        public void print(int a) { Console.WriteLine(a); }

        public void print(string a) { Console.WriteLine(a); }

        public void print(double a) { Console.WriteLine(a); }

        ~TempoVolume()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        void SongLoaded(SongData song, string songName, string songFile)
        {
            Dispatch.TriggerVolumeChanged(0.5f);
        }

        void SkeletonMoved(float time, Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                if (joint.JointType == JointType.HandRight)
                {
                    rightHandY = joint.Position.Y;
                    rightHandX = joint.Position.X;
                }
                if (joint.JointType == JointType.HipRight && rightHipX == 0)
                {
                    rightHipX = joint.Position.X;
                    rightHipY = joint.Position.Y;
                }
            }
            switch (seeking)
            {
                case "STILL":
                {
                    if (stillFramesCount == 15)
                    {
                        seeking = "START";
                    }
                    if (stillFramesCount < 15 && Math.Abs(prevXOne - rightHandX) < .08 && Math.Abs(prevYOne - rightHandY) < .08 && Math.Abs(prevXTwo - rightHandX) < .08 && Math.Abs(prevYTwo - rightHandY) < .08)
                    {
                        stillFramesCount++;
                        break;
                    }
                    else if (stillFramesCount < 15)
                    {
                        stillFramesCount = 0;
                    }
                    break;
                }
                case "START":
                {
                    if (prevYTwo < (prevYOne - .01) && prevYOne < (rightHandY - .01))
                    {
                        if (framesInFirstBeat == 0)
                        {
                            stopwatch.Start();
                        }
                        framesInFirstBeat++;
                        break;
                    }
                    if (framesInFirstBeat >= 2 && prevYTwo > (prevYOne + .01) && prevYOne > (rightHandY + .01))
                    {
                        seeking = "MINIMUM";
                        break;
                    }
                    break;
                }
                case "MINIMUM":
                {
                    if (prevYTwo < (prevYOne - threshold) && prevYOne < (rightHandY - threshold))
                    {
                        xValues.Add(rightHandX);
                        if (xValues.Count == 5)
                        {
                            xValues.RemoveAt(0);
                        }
                        if (xValues.Count == 4)
                        {
                            if (xValues.Min() == xValues.ElementAt(3)) { volume = Math.Abs(rightHandX * 2 / -.7) * 127; }
                        }
                        seeking = "MAXIMUM";
                        break;
                    }
                    break;
                }
                case "MAXIMUM":
                {
                    if (prevYTwo > (prevYOne + threshold) && prevYOne > (rightHandY + threshold))
                    {
                        seeking = "MINIMUM";
                        break;
                    }
                    break;
                }
            }
            if (Math.Abs(volume - newVolume) > 5)
            {
                if (volume < newVolume)
                {
                    if (newVolume > 0) { newVolume -= (3 * newVolume / 127); }
                }
                if (volume > newVolume)
                {
                    if (newVolume < 127) { newVolume += (3 * (127 - newVolume) / 127); }
                }
            }
            Dispatch.TriggerVolumeChanged(newVolume / 127);
            prevYTwo = prevYOne;
            prevXTwo = prevXOne;
            prevYOne = rightHandY;
            prevXOne = rightHandX;
        }
    }
}