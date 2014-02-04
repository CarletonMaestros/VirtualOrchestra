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
        public Int32 counter = 0;
        public Stopwatch stopwatch;
        public String seeking;
        public float prevYOne;
        public float prevYTwo;
        public float prevXOne;
        public float prevXTwo;
        public float rightHandY;
        public float rightHandX;
        public float rightHipX;
        public float rightHipY;
        public float threshold;
        public int stillFramesCount;
        public int framesInFirstBeat;
        public int startMarker = 0;
        public Boolean stop = false;
        public float prevBeat;
        public float middleBeat;
        public int startingFour = 0;
        public List<float> maxX = new List<float>();
        public List<float> maxY = new List<float>();
        public List<float> minX = new List<float>();
        public List<float> minY = new List<float>();
        public Boolean volumeChanged = false;
        public double volume = 64;

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

        public void printArray<T>(IEnumerable<T> a)
        {
            foreach (var i in a)
            {
                Console.WriteLine(" ", i);
            }
        }

        ~TempoVolume()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
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
                if (joint.JointType == JointType.HipRight)
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
                        if (counter == 5)
                        {
                            counter = 1;
                        }
                        if (startMarker == 0)
                        {
                            startMarker = 1;
                            long firstTempo = stopwatch.ElapsedMilliseconds * 1000 / 2;
                        }
                        else
                        {
                            long tempo = stopwatch.ElapsedMilliseconds * 1000;
                        }
                        if (rightHandX - rightHipX > -.1 && rightHandX - rightHipX < .15 && Math.Abs(rightHandY - rightHipY) < .15)
                        {
                            minY.Add(rightHandY);
                            if (minY.Count == 4) { minY.RemoveAt(0); }
                        }
                        else if (rightHandX < 0)
                        {
                            minX.Add(rightHandX);
                            if (minX.Count == 4) { minX.RemoveAt(0); }
                        }
                        else if (rightHandX > 0)
                        {
                            maxX.Add(rightHandX);

                            if (maxX.Count == 4) { maxX.RemoveAt(0); }
                        }
                        //Dispatch.TriggerBeat(counter, "beat");
                        stopwatch.Restart();
                        seeking = "MAXIMUM";
                        counter++;
                        Dispatch.TriggerBeat(counter, "beat");
                        break;
                    }
                    break;
                }
                case "MAXIMUM":
                {
                    if (prevYTwo > (prevYOne + threshold) && prevYOne > (rightHandY + threshold))
                    {
                        seeking = "MINIMUM";
                        maxY.Add(rightHandY);
                        if (maxY.Count == 4) { maxY.RemoveAt(0); }
                        break;
                    }
                    break;
                }
            }
            //if (maxX.Count == 3 && minX.Count == 3 && maxY.Count == 3)
            //{
            //    if (maxX.ElementAt(2) < maxX.ElementAt(0) * .9 && minX.ElementAt(2) > minX.ElementAt(0) * .9 && maxY.ElementAt(2) < maxY.ElementAt(0) * .9)
            //    {
            //        //if (volume > 0) { volume -= (Math.Abs(maxX.ElementAt(2) / maxX.ElementAt(0)) * volume / 127); }
            //        if (volume > 0) { volume -= (5 * volume / 127); }
            //        volumeChanged = true;
            //    }
            //    if (maxX.ElementAt(2) > maxX.ElementAt(0) * 1.1 && minX.ElementAt(2) < minX.ElementAt(0) * 1.1 && maxY.ElementAt(2) > maxY.ElementAt(0) * 1.1)
            //    {
            //        Console.WriteLine("{0}", Math.Abs(maxX.ElementAt(2) / maxX.ElementAt(0)));
            //        //if (volume < 127) { volume += (Math.Abs(maxX.ElementAt(2) / maxX.ElementAt(0)) * (127 - volume) / 127); }
            //        if (volume < 127) { volume += (5 * (127 - volume) / 127); }
            //        volumeChanged = true;
            //    }
            //    //if (volumeChanged == true)
            //    //{
            //    //    maxX.Insert(1, maxX.ElementAt(0));
            //    //    maxX.Insert(2, maxX.ElementAt(0));
            //    //    minX.Insert(1, minX.ElementAt(0));
            //    //    minX.Insert(2, minX.ElementAt(0));
            //    //    maxY.Insert(1, maxY.ElementAt(0));
            //    //    maxY.Insert(2, maxY.ElementAt(0));
            //    //    volumeChanged = false;
            //    //}
            //    int intVolume = (int)volume;
            //    Dispatch.TriggerVolumeChanged(intVolume / 127f);
            //}
            prevYTwo = prevYOne;
            prevXTwo = prevXOne;
            prevYOne = rightHandY;
            prevXOne = rightHandX;
        }
    }
}