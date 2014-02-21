using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class VolumeGesture
    {
        float leftHandX;
        float leftHandY;
        float leftHipX;
        float leftHipZ;
        float startValueX;
        float startValueY;
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

        public void Unload()
        {
            Dispatch.SkeletonMoved -= SkeletonMoved;
        }

        void SongLoaded(SongData song, string songName, string songFile)
        {
            Dispatch.TriggerVolumeChanged(0.5f);
        }

        void SkeletonMoved(float time, Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.JointType == JointType.HipLeft)
                {
                    leftHipX = joint.Position.X;
                    leftHipZ = joint.Position.Z;
                    if (leftHandY > joint.Position.Y)
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
                if (leftHipZ < .1) 
                { 
                    tooClose = true; 
                }
                else { tooClose = false; }
                if (joint.JointType == JointType.WristLeft)
                {
                    leftHandY = joint.Position.Y;
                    leftHandX = joint.Position.X;
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
                            yAverage = leftHandY;
                            xAverage = leftHandX;
                        }
                        else if (aboveHip == true && yAverage != 0 && xAverage != 0)
                        {
                            if (yAverage - leftHandY > -.1 && yAverage - leftHandY < .1 && xAverage - leftHandX > -.1 && xAverage - leftHandX < .1)
                            {
                                yAverage = (yAverage + leftHandY) / 2;
                                xAverage = (xAverage + leftHandX) / 2;
                                count += 1;
                            }
                            else
                            {
                                yAverage = leftHandY;
                                xAverage = leftHandX;
                                count = 0;
                            }
                        }
                    } 
                    else if (aboveHip == true && startValueY != 0 && startValueX != 0 && tooClose == false)
                    {
                        if ((startValueX - leftHandX) < .1 && (startValueX - leftHandX) > -.1 && Math.Abs(leftHandX - leftHipX) < .3 && outsideBox == false) // if wrist is within the acceptable "box" of x-ranges
                        {
                            changeInYVal += (leftHandY - prevYValue);
                            if (increaseToDecrease == 0 && leftHandY - prevYValue > 0) //hand is moving up now
                            {
                                increaseToDecrease = 1;
                                changeVolume = changeInYVal;
                            }
                            else if (increaseToDecrease == 1 && leftHandY - prevYValue < 0) //hand is moving down now
                            {
                                increaseToDecrease = 0;
                                changeVolume = changeInYVal;
                            }
                            else { changeInYVal += (leftHandY - prevYValue); }
                            prevYValue = leftHandY;
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