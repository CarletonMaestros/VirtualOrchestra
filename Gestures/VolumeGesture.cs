using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class VolumeGesture
    {
        SkeletonPoint leftWrist;
        SkeletonPoint leftHip;
        SkeletonPoint startValue;
        float prevYValue;
        float yAverage;
        float xAverage;
        float changeInYVal;
        float changeVolume;
        float increaseToDecrease = 1; // 0 means hand is moving down, 1 moving up
        float volume = 64;
        int count;
        bool aboveHip = false;
        bool outsideBox = true;
        bool tooClose = false;

        public VolumeGesture()
        {
            Dispatch.SkeletonMoved += SkeletonMoved;
            Dispatch.SongLoaded += SongLoaded;
        }

        public void Unload() { Dispatch.SkeletonMoved -= SkeletonMoved; }

        void SongLoaded(SongData song, string songName, string songFile) { Dispatch.TriggerVolumeChanged(0.5f); }

        void SkeletonMoved(float time, Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                leftWrist = skel.Joints[JointType.WristLeft].Position;
                leftHip = skel.Joints[JointType.HipLeft].Position;
                if (leftWrist.Y > leftHip.Y)
                {
                    aboveHip = true;
                    outsideBox = false;
                }
                else
                {
                    aboveHip = false;
                    count = 0;
                    startValue.Y = 0;
                    startValue.X = 0;
                    yAverage = 0;
                    xAverage = 0;
                }
                if (joint.JointType == JointType.WristLeft)
                {
                    if (count == 15)
                    {
                        startValue.Y = yAverage;
                        startValue.X = xAverage;
                        count += 1;
                    }
                    else if (count < 15)
                    {
                        if (aboveHip == true && yAverage == 0 && xAverage == 0)
                        {
                            count += 1;
                            yAverage = leftWrist.Y;
                            xAverage = leftWrist.X;
                        }
                        else if (aboveHip == true && yAverage != 0 && xAverage != 0)
                        {
                            if (yAverage - leftWrist.Y > -.1 && yAverage - leftWrist.Y < .1 && xAverage - leftWrist.X > -.1 && xAverage - leftWrist.X < .1)
                            {
                                yAverage = (yAverage + leftWrist.Y) / 2;
                                xAverage = (xAverage + leftWrist.X) / 2;
                                count += 1;
                            }
                            else
                            {
                                yAverage = leftWrist.Y;
                                xAverage = leftWrist.X;
                                count = 0;
                            }
                        }
                    }
                    else if (aboveHip == true && startValue.Y != 0 && startValue.X != 0 && tooClose == false)
                    {
                        if ((startValue.X - leftWrist.X) < .1 && (startValue.X - leftWrist.X) > -.1 && Math.Abs(leftWrist.X - leftHip.X) < .3 && outsideBox == false) // if wrist is within the acceptable "box" of x-ranges
                        {
                            changeInYVal += (leftWrist.Y - prevYValue);
                            if (increaseToDecrease == 0 && leftWrist.Y - prevYValue > 0) //hand is moving up now
                            {
                                increaseToDecrease = 1;
                                changeVolume = changeInYVal;
                            }
                            else if (increaseToDecrease == 1 && leftWrist.Y - prevYValue < 0) //hand is moving down now
                            {
                                increaseToDecrease = 0;
                                changeVolume = changeInYVal;
                            }
                            else { changeInYVal += (leftWrist.Y - prevYValue); }
                            prevYValue = leftWrist.Y;
                            if (changeInYVal - changeVolume > .1)
                            {
                                if (volume < 127) { volume += (5 * (127 - volume) / 127); }
                            }
                            else if (changeInYVal - changeVolume < -.1)
                            {
                                if (volume > 0) { volume -= (5 * volume / 127); }
                            }
                            int intVolume = (int)volume;
                            Dispatch.TriggerVolumeChanged(intVolume / 127f);
                        }
                        else { outsideBox = true; }
                    }
                }
            }
        }
    }
}