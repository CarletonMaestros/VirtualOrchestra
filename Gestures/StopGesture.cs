using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class StopGesture
    {
        float leftHandX;
        float leftHandY;
        float rightHandX;
        float rightHandY;
        float hipRightY;
        float hipRightZ;
        float prevRightYOne;
        float prevRightXOne;
        float prevLeftYOne;
        float prevLeftXOne;
        float counter;
        bool tooClose = false;

        public StopGesture()
        {
            Dispatch.SkeletonMoved += SkeletonMoved;
        }

        ~StopGesture() { Dispatch.SkeletonMoved -= this.SkeletonMoved; }

        void SkeletonMoved(float time, Skeleton skeleton)
        {
            foreach (Joint joint in skeleton.Joints)
            {
                if (joint.JointType == JointType.HandRight)
                {
                    rightHandX = joint.Position.X;
                    rightHandY = joint.Position.Y;
                }
                if (joint.JointType == JointType.HandLeft)
                {
                    leftHandX = joint.Position.X;
                    leftHandY = joint.Position.Y;
                }
                if (joint.JointType == JointType.HipRight) 
                { 
                    hipRightY = joint.Position.Y;
                    hipRightZ = joint.Position.Z;
                }
            }
            if (hipRightZ < .1) 
            { 
                tooClose = true; 
                //Console.WriteLine("TOO CLOSE"); 
            }
            else { tooClose = false; }
            if (Math.Abs(leftHandY - rightHandY) < .1 && tooClose == false)
            {          
                if (rightHandX >= prevRightXOne && leftHandX <= prevLeftXOne && rightHandY <= prevRightYOne && leftHandY <= prevLeftYOne) { counter += 1; }
                if (counter >= 4 && Math.Abs(rightHandX - prevRightXOne) < .1 && Math.Abs(leftHandX - prevLeftXOne) < .1 && Math.Abs(rightHandY - prevRightYOne) < .1 && Math.Abs(leftHandY - prevLeftYOne) < .1)
                {
                    counter = 0;
                    Dispatch.TriggerStop();
                }
            }
            prevRightXOne = rightHandX;
            prevRightYOne = rightHandY;
            prevLeftXOne = leftHandX;
            prevLeftYOne = leftHandY;
        }
    }
}