using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class StopGesture
    {
        SkeletonPoint leftHand;
        SkeletonPoint rightHand;
        SkeletonPoint rightHip;
        SkeletonPoint prevRightOne;
        SkeletonPoint prevLeftOne;
        int counter;

        public StopGesture() { Dispatch.SkeletonMoved += SkeletonMoved; }

        public void Unload() { Dispatch.SkeletonMoved -= this.SkeletonMoved; }

        void SkeletonMoved(float time, Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                rightHand = skel.Joints[JointType.HandRight].Position;
                leftHand = skel.Joints[JointType.HandLeft].Position;
                rightHip = skel.Joints[JointType.HipRight].Position;
            }
            if (Math.Abs(leftHand.Y - rightHand.Y) < .1)
            {          
                if (rightHand.X >= prevRightOne.X && leftHand.X <= prevLeftOne.X && rightHand.Y <= prevRightOne.Y && leftHand.Y <= prevLeftOne.Y) { counter += 1; }
                if (counter >= 4 && Math.Abs(rightHand.X - prevRightOne.X) < .1 && Math.Abs(leftHand.X - prevLeftOne.X) < .1 && Math.Abs(rightHand.Y - prevRightOne.Y) < .1 && Math.Abs(leftHand.Y - prevLeftOne.Y) < .1)
                {
                    counter = 0;
                    Dispatch.TriggerStop();
                }
            }
            prevRightOne.X = rightHand.X;
            prevRightOne.Y = rightHand.Y;
            prevLeftOne.X = leftHand.X;
            prevLeftOne.Y = leftHand.Y;
        }
    }
}