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

        public void Unload()
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
            if (rightHipZ < .2) 
            { 
                tooClose = true; 
                //Console.WriteLine("TOO CLOSE"); 
            }
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