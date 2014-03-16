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

        // Initializes the skeleton, adds it to the dispatch
        public StopGesture() { Dispatch.SkeletonMoved += SkeletonMoved; }

        // Unloads the skeleton from the dispatch
        public void Unload() { Dispatch.SkeletonMoved -= this.SkeletonMoved; }

        // Called when the skeleton is moving, handles the stop gesture
        void SkeletonMoved(float time, Skeleton skel)
        {
            // Gets the location information for each of the necessary joints
            foreach (Joint joint in skel.Joints)
            {
                rightHand = skel.Joints[JointType.HandRight].Position;
                leftHand = skel.Joints[JointType.HandLeft].Position;
                rightHip = skel.Joints[JointType.HipRight].Position;
            }
            // Check to see if both hands are at roughly the same height
            if (Math.Abs(leftHand.Y - rightHand.Y) < .1)
            {
                // If both hands are moving diagonally down and away from each other, increase the counter by one      
                if (rightHand.X >= prevRightOne.X && leftHand.X <= prevLeftOne.X && rightHand.Y <= prevRightOne.Y && leftHand.Y <= prevLeftOne.Y) { counter += 1; }
                // If hands have moved diagonally down and away from each other for at least 4 frames, reset the counter, and stop the song
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