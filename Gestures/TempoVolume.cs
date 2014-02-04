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
        public Boolean conducting;
        public int stillFramesCount;
        public int framesInFirstBeat;
        public float headX;
        public float headY;
        public int startMarker = 0;
        public Boolean stop = false;
        public float prevBeat;
        public float middleBeat;
        public int startingFour = 0;

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
            if (!stop)
            {
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
                                if (prevBeat == 1)
                                {
                                    prevBeat = 2;
                                    middleBeat = 1;
                                }
                                else if (prevBeat == 2 && middleBeat == 0)
                                {
                                    prevBeat = 3;
                                    middleBeat = 1;
                                }
                                else if (prevBeat == 3 && middleBeat == 0)
                                {
                                    prevBeat = 4;
                                    middleBeat = 1;
                                }
                                else
                                {
                                    prevBeat = 1;
                                    middleBeat = 1;
                                }
                            }
                            else if (rightHandX < 0)
                            {
                                prevBeat = 2;
                                middleBeat = 0;
                            }
                            else if (rightHandX > 0)
                            {
                                if (prevBeat == 2)
                                {
                                    prevBeat = 3;
                                    middleBeat = 0;
                                }
                                else if (prevBeat == 1)
                                {
                                    prevBeat = 2;
                                    middleBeat = 0;
                                }
                            }
                            if (startingFour <= 4)
                            {
                                startingFour++;
                                Console.WriteLine(startingFour);
                            }
                            else
                            {
                                Dispatch.TriggerBeat(counter, "beat");
                            }
                            stopwatch.Restart();
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
            }
            prevYTwo = prevYOne;
            prevXTwo = prevXOne;
            prevYOne = rightHandY;
            prevXOne = rightHandX;
        }
    }
}