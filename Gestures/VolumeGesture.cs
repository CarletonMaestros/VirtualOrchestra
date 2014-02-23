using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class VolumeGesture
    {
        SkeletonPoint leftHand;
        SkeletonPoint leftHip;
        SkeletonPoint startValue;
        SkeletonPoint average;
        float prevYValue;        
        float changeInYVal;
        float changeVolume;
        float increaseToDecrease = 1; // 0 means hand is moving down, 1 moving up
        float volume = 64;
        int count;
        bool aboveHip = false;
        bool outsideBox = true;

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
                leftHip = skel.Joints[JointType.HipLeft].Position;
                leftHand = skel.Joints[JointType.HandLeft].Position;
                if (leftHand.Y > joint.Position.Y)
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
                    average = startValue;
                }
                if (count == 15)
                {
                    startValue = average;
                    count += 1;
                }
                else if (count < 15)
                {
                    if (aboveHip == true && average.Y == 0 && average.X == 0)
                    {
                        count += 1;
                        average = leftHand;
                    }
                    else if (aboveHip == true && average.Y != 0 && average.X != 0)
                    {
                        if (average.Y - leftHand.Y > -.1 && average.Y - leftHand.Y < .1 && average.X - leftHand.X > -.1 && average.X - leftHand.X < .1)
                        {
                            average.Y = (average.Y + leftHand.Y) / 2;
                            average.X = (average.X + leftHand.X) / 2;
                            count += 1;
                        }
                        else
                        {
                            average = leftHand;
                            count = 0;
                        }
                    }
                } 
                else if (aboveHip == true && startValue.Y != 0 && startValue.X != 0)
                {
                    if ((startValue.X - leftHand.X) < .1 && (startValue.X - leftHand.X) > -.1 && Math.Abs(leftHand.X - leftHip.X) < .3 && outsideBox == false) // if wrist is within the acceptable "box" of x-ranges
                    {
                        changeInYVal += (leftHand.Y - prevYValue);
                        if (increaseToDecrease == 0 && leftHand.Y - prevYValue > 0) //hand is moving up now
                        {
                            increaseToDecrease = 1;
                            changeVolume = changeInYVal;
                        }
                        else if (increaseToDecrease == 1 && leftHand.Y - prevYValue < 0) //hand is moving down now
                        {
                            increaseToDecrease = 0;
                            changeVolume = changeInYVal;
                        }
                        else { changeInYVal += (leftHand.Y - prevYValue); }
                        prevYValue = leftHand.Y;
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