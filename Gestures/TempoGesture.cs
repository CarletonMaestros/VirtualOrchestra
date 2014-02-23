using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Kinect;

namespace Orchestra
{
    public class TempoGesture
    {
        Int32 counter = 1;
        Stopwatch stopwatch;
        String seeking;
        SkeletonPoint rightHand;
        SkeletonPoint rightHip;
        SkeletonPoint prevOne;
        SkeletonPoint prevTwo;
        float threshold;
        int stillFramesCount;
        int framesInFirstBeat;
        int startMarker = 0;
        bool locked = true;
        public bool beat = false;

        public TempoGesture()
        {
            stopwatch = new Stopwatch();
            counter = 0;
            seeking = "STILL";
            threshold = .002F;
            stillFramesCount = 0;
            framesInFirstBeat = 0;

            Dispatch.SkeletonMoved += SkeletonMoved;
            Dispatch.Stop += Stopped;
            Dispatch.Lock += Lock;
        }

        private void Stopped(float time) { seeking = "STILL"; }

        public void Unload() { Dispatch.SkeletonMoved -= this.SkeletonMoved; }

        private void Lock(bool songLocked) { locked = songLocked; }

        void SkeletonMoved(float time, Skeleton skel)
        {
            beat = false;
            if (locked) return;
            rightHand = skel.Joints[JointType.HandRight].Position;
            rightHip = skel.Joints[JointType.HipRight].Position;
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
                        if (framesInFirstBeat == 0) { stopwatch.Start(); }
                        framesInFirstBeat++;
                        break;
                    }
                    if (framesInFirstBeat >= 2 && prevTwo.Y > (prevOne.Y + .01) && prevOne.Y > (rightHand.Y + .01) && Math.Abs(rightHand.X - rightHip.X) < .15)
                    {
                        seeking = "MINIMUM";
                        Dispatch.TriggerStart();
                        break;
                    }
                    break;
                }
                case "MINIMUM":
                {
                    if (prevTwo.Y < (prevOne.Y - threshold) && prevOne.Y < (rightHand.Y - threshold))
                    {
                        if (counter == 5) { counter = 1; }
                        if (startMarker == 0)
                        {
                            startMarker = 1;
                            long firstTempo = stopwatch.ElapsedMilliseconds * 1000 / 2;
                        }
                        else { long tempo = stopwatch.ElapsedMilliseconds * 1000; }
                        stopwatch.Restart();
                        Dispatch.TriggerBeat(counter, "beat");
                        beat = true;
                        seeking = "MAXIMUM";
                        counter++;
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
            prevTwo = prevOne;
            prevOne = rightHand;
        }
    }
}