using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class VolumeGesture
    {
        float leftWristY = 0;
        float leftWristX = 0;
        Boolean aboveHip = false;
        Boolean outsideBox = true;
        float startValueX = 0;
        float startValueY = 0;
        float changeInYVal = 0;
        float changeVolume = 0;
        float prevYValue = 0;
        float increaseToDecrease = 1; // 0 means hand is moving down, 1 moving up
        float volume = 64;
        int count = 0;
        float yAverage = 0;
        float xAverage = 0;

        public VolumeGesture()
        {
            Dispatch.SkeletonMoved += SkeletonMoved;
            Dispatch.SongLoaded += SongLoaded;
        }

        ~VolumeGesture()
        {
            Dispatch.SkeletonMoved -= SkeletonMoved;
        }

        void SongLoaded(SongData song)
        {
            Dispatch.TriggerVolumeChanged(0.5f);
        }

        void SkeletonMoved(float time, Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                

                if (joint.JointType == JointType.HipLeft)
                {
                    if (leftWristY > joint.Position.Y)
                    {
                        aboveHip = true;
                        outsideBox = false;
                    }
                    else
                    {
                        aboveHip = false;
                        count = 0;
                        startValueY = 0;
                        startValueX = 0;
                        yAverage = 0;
                        xAverage = 0;
                    }
                }

                if (joint.JointType == JointType.WristLeft)
                {
                    leftWristY = joint.Position.Y;
                    leftWristX = joint.Position.X;
                    if (count == 15)
                    {
                        startValueY = yAverage;
                        startValueX = xAverage;
                        count += 1;
                    }
                    else if (count < 15)
                    {
                        if (aboveHip == true && yAverage == 0 && xAverage == 0)
                        {
                            count += 1;
                            yAverage = leftWristY;
                            xAverage = leftWristX;
                        }
                        else if (aboveHip == true && yAverage != 0 && xAverage != 0)
                        {
                            if (yAverage - leftWristY > -.1 && yAverage - leftWristY < .1 && xAverage - leftWristX > -.1 && xAverage - leftWristX < .1)
                            {
                                yAverage = (yAverage + leftWristY) / 2;
                                xAverage = (xAverage + leftWristX) / 2;
                                count += 1;
                            }
                            else
                            {
                                yAverage = leftWristY;
                                xAverage = leftWristX;
                                count = 0;
                            }

                        }
                    }
                    else if (aboveHip == true && startValueY != 0 && startValueX != 0)
                    {
                        if ((startValueX - leftWristX) < .1 && (startValueX - leftWristX) > -.1 && outsideBox == false) // if wrist is within the acceptable "box" of x-ranges
                        {
                            changeInYVal += (leftWristY - prevYValue);
                            if (increaseToDecrease == 0 && leftWristY - prevYValue > 0) //hand is moving up now
                            {
                                increaseToDecrease = 1;
                                changeVolume = changeInYVal;
                            }
                            else if (increaseToDecrease == 1 && leftWristY - prevYValue < 0) //hand is moving down now
                            {
                                increaseToDecrease = 0;
                                changeVolume = changeInYVal;
                            }
                            else
                                changeInYVal += (leftWristY - prevYValue);

                            prevYValue = leftWristY;
                            if (changeInYVal - changeVolume > .1)
                            {
                                if (volume < 127)
                                    volume += (5 * (127 - volume) / 127);

                            }
                            else if (changeInYVal - changeVolume < -.1)
                            {
                                if (volume > 0)
                                    volume -= (5 * volume / 127);
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