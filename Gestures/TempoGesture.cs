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
        float prevXOne;
        float prevYOne;
        float prevXTwo;
        float prevYTwo;
        float rightHandX;
        float rightHandY;
        float rightHandZ;        
        float rightHipX;
        float rightHipY;
        float rightHipZ;
        float threshold;
        float middleBeat;
        float prevBeat = 0;
        float prevXBeatValue;
        int stillFramesCount;
        int framesInFirstBeat;
        int startMarker = 0;
        bool locked = true;
        bool aboveHip = false;       
        bool tooClose = false;

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

        private void Stopped(float time)
        {
            seeking = "STILL";
        }

        ~TempoGesture()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        private void Lock(bool songLocked)
        {
            locked = songLocked;
        }

        void SkeletonMoved(float time, Skeleton skel)
        {
            if (locked) return;
            foreach (Joint joint in skel.Joints)
            {
                if (joint.JointType == JointType.HandRight)
                {
                    rightHandY = joint.Position.Y;
                    rightHandX = joint.Position.X;
                    rightHandZ = joint.Position.Z;
                }
                if (joint.JointType == JointType.HipRight)
                {
                    rightHipX = joint.Position.X;
                    rightHipY = joint.Position.Y;
                    rightHipZ = joint.Position.Z;
                }
            }
            if (rightHipZ < .1) { tooClose = true; Console.WriteLine("TOO CLOSE"); }
            else { tooClose = false; }
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
                    if (framesInFirstBeat >= 2 && prevYTwo > (prevYOne + .01) && prevYOne > (rightHandY + .01) && Math.Abs(rightHandX - rightHipX) < .15)
                    {
                        seeking = "MINIMUM";
                        Dispatch.TriggerStart();
                        break;
                    }
                    break;
                }
                case "MINIMUM":
                {
                    if (prevYTwo < (prevYOne - threshold) && prevYOne < (rightHandY - threshold) && tooClose == false)
                    {
                        if (counter == 5) { counter = 1; }
                        if (startMarker == 0)
                        {
                            startMarker = 1;
                            long firstTempo = stopwatch.ElapsedMilliseconds * 1000 / 2;
                        }
                        else
                        {
                            long tempo = stopwatch.ElapsedMilliseconds * 1000;
                        }
                        //if (prevBeat == 0 || prevXBeatValue < rightHandX - .05 && Math.Abs(rightHandX - rightHipX) < .1)
                        //{
                        //    prevBeat = 1;
                        //    Console.WriteLine("beat 1");
                        //}
                        //else if (prevXBeatValue > rightHandX + .05 && rightHandX + .05 < rightHipX)
                        //{
                        //    Console.WriteLine("beat 2");
                        //}
                        //else if (prevXBeatValue + .1 < rightHandX && rightHandX > rightHipX + .05)
                        //{
                        //    Console.WriteLine("beat 3");
                        //}
                        //else if (prevXBeatValue > rightHandX + .08 && Math.Abs(rightHandX - rightHipX) < .1)
                        //{
                        //    Console.WriteLine("beat 4");
                        //}
                        //else { Console.WriteLine("shit's wrong"); }
                        //if (rightHandX - rightHipX > -.1 && rightHandX - rightHipX < .15 && Math.Abs(rightHandY - rightHipY) < .15 && )
                        //{
                        //    //if (prevBeat == 1)
                        //    //{
                        //    //    //Dispatch.TriggerBeat(counter, "beat2");
                        //    //    Console.WriteLine("beat 2");
                        //    //    prevBeat = 2;
                        //    //    middleBeat = 1;
                        //    //}
                        //    //else if (prevBeat == 2 && middleBeat == 0)
                        //    //{
                        //    //    //Dispatch.TriggerBeat(counter, "beat3");
                        //    //    prevBeat = 3;
                        //    //    middleBeat = 1;
                        //    //    Console.WriteLine("beat 3");
                        //    //}
                        //    if (prevBeat == 3)
                        //    {
                        //        //Dispatch.TriggerBeat(counter, "beat4");
                        //        //prevXBeatValue = rightHandX;
                        //        prevBeat = 4;
                        //        middleBeat = 1;
                        //        Console.WriteLine("beat 4");
                        //    }
                        //    else
                        //    {
                        //        //Dispatch.TriggerBeat(counter, "beat1");
                        //        prevBeat = 1;
                        //        middleBeat = 1;
                        //        Console.WriteLine("beat 1");
                        //    }
                        //}
                        //else if (rightHandX < 0)
                        //{
                        //    //Dispatch.TriggerBeat(counter, "beat2");
                        //    prevBeat = 2;
                        //    middleBeat = 0;
                        //    Console.WriteLine("beat 2");
                        //}
                        //else if (rightHandX > 0)
                        //{
                        //    if (prevBeat == 2)
                        //    {
                        //        //Dispatch.TriggerBeat(counter, "beat3");
                        //        prevBeat = 3;
                        //        middleBeat = 0;
                        //        Console.WriteLine("beat 3");
                        //    }
                        //    //else if (prevBeat == 1)
                        //    //{
                        //    //    //Dispatch.TriggerBeat(counter, "beat2");
                        //    //    prevBeat = 2;
                        //    //    middleBeat = 0;
                        //    //    Console.WriteLine("beat 2");
                        //    //}
                        //}
                        stopwatch.Restart();
                        Dispatch.TriggerBeat(counter, "beat");
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
                        break;
                    }
                    break;
                }
            }
            prevYTwo = prevYOne;
            prevXTwo = prevXOne;
            prevYOne = rightHandY;
            prevXOne = rightHandX;
        }
    }
}