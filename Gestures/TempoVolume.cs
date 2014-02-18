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
        public float rightHipX = 0;
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
        public float volumeFloat;
        public int allChanged = 1;
        public Boolean changeVolume = false;

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
                       // Console.WriteLine(rightHandX - rightHipX);
                        //Console.WriteLine(Math.Abs(rightHandY - rightHipY));
                        if (rightHandX - rightHipX > -.1 && rightHandX - rightHipX < .15 && Math.Abs(rightHandY - rightHipY) < .15)
                        {
                            minY.Add(rightHandY);
                            if (minY.Count == 4) { minY.RemoveAt(0); }
                            allChanged++;
                            //Console.WriteLine("volume: beat 1 or 4");
                        }
                        else if (rightHandX < 0)
                        {
                            //if (minX.Count == 0)
                            //{
                            //    for (int i = 0; i < 3; i++) { minX.Add(rightHandX); }
                            //}
                            //else { minX.Add(rightHandX); }
                            //if (minX.Count == 3) { minX.RemoveAt(0); }
                            volume = Math.Abs((rightHandX - rightHipX + .22) / -.41);
                            //Console.WriteLine("rightHandX " + rightHandX);
                            //Console.WriteLine("rightHipX: " + rightHipX);
                            //Console.WriteLine("volume: " + volume);
                            allChanged++;
                            //Console.WriteLine("volume: beat 2");
                        }
                        else
                        {
                            if (maxX.Count == 0)
                            {
                                for (int i = 0; i < 4; i++) { maxX.Add(rightHandX); }
                            }
                            if (maxX.Count == 4) { maxX.RemoveAt(0); }
                            allChanged = 1;
                            //Console.WriteLine("volume: beat 3");
                        }
                        if (maxX.Count == 3 && minX.Count == 3 && maxY.Count == 3)
                        {
                            if (maxX.ElementAt(2) < maxX.ElementAt(0) * .9 && minX.ElementAt(2) > minX.ElementAt(0) * .9 && allChanged % 3 == 0)
                            {
                                changeVolume = true;
                            }
                            else if (maxX.ElementAt(2) > maxX.ElementAt(0) * 1.1 && minX.ElementAt(2) < minX.ElementAt(0) * 1.1 && allChanged % 3 == 0)
                            {
                                changeVolume = true;
                            }
                            else { changeVolume = false; }
                        }
                        stopwatch.Restart();
                        if (volume > 1) { volume = 1; }
                        volumeFloat = (float)volume;
                        Dispatch.TriggerVolumeChanged(volumeFloat);
                        seeking = "MAXIMUM";
                        counter++;
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
                        //allChanged++;
                        break;
                    }
                    break;
                }
            }
            //if (maxX.Count == 3 && minX.Count == 3 && maxY.Count == 3)
            //{
            //    if (maxX.ElementAt(2) < maxX.ElementAt(0) * .9 && minX.ElementAt(2) > minX.ElementAt(0) * .9 && maxY.ElementAt(2) < maxY.ElementAt(0) * .9 && allChanged == 3)
            //    {
            //        if (volume > 0) { volume -= (Math.Abs(maxX.ElementAt(2) - maxX.ElementAt(0)) * volume / 127); }
            //        //if (volume > 0) { volume -= ((5 * volume) / 127); }
            //        //volumeChanged = true;
            //        allChanged = 0;
            //    }
            //    if (maxX.ElementAt(2) > maxX.ElementAt(0) * 1.1 && minX.ElementAt(2) < minX.ElementAt(0) * 1.1 && maxY.ElementAt(2) > maxY.ElementAt(0) * 1.1 && allChanged == 3)
            //    {
            //        if (volume < 127) { volume += (Math.Abs(maxX.ElementAt(2) - maxX.ElementAt(0)) * (127 - volume) / 127); }
            //        //if (volume < 127) { volume += (5 * (127 - volume) / 127); }
            //        //volumeChanged = true;
            //        allChanged = 0;
            //    }
            //    //foreach (var i in maxX) { Console.WriteLine(i); }
            //    //if (volumeChanged == true)
            //    //{
            //    //    maxX[1] = maxX.ElementAt(0);
            //    //    maxX[2] = maxX.ElementAt(0);
            //    //    minX[1] = minX.ElementAt(0);
            //    //    minX[2] = minX.ElementAt(0);
            //    //    maxY[1] = maxY.ElementAt(0);
            //    //    maxY[2] = maxY.ElementAt(0);
            //    //    volumeChanged = false;
            //    //}
            //    int intVolume = (int)volume;
            //    Dispatch.TriggerVolumeChanged(intVolume / 127f);
            //}
            //change 1/4 of volume per beat, find difference between first and last, divide by 4, change volume by that
            prevYTwo = prevYOne;
            prevXTwo = prevXOne;
            prevYOne = rightHandY;
            prevXOne = rightHandX;
        }
    }
}