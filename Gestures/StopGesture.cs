using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class StopGesture
    {
        public float leftHandX;
        public float leftHandY;
        public float rightHandY;
        public float rightHandX;
        public float prevRightYOne;
        public float prevRightYTwo;
        public float prevRightXOne;
        public float prevRightXTwo;
        public float prevLeftYOne;
        public float prevLeftYTwo;
        public float prevLeftXOne;
        public float prevLeftXTwo;
        public float hipRightY;
        public float counter;

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
                if (joint.JointType == JointType.HipRight) { hipRightY = joint.Position.Y; }
            }
            if (Math.Abs(leftHandY - rightHandY) < .1 && rightHandY > 5)
            {
                if (counter < 10)
                {
                    if (rightHandY < prevRightYTwo && leftHandY < prevLeftYTwo) 
                    {
                        if (rightHandX + leftHandX > prevRightXTwo + prevLeftXTwo)
                        { 
                            counter += 1;
                            Console.WriteLine(counter);
                        }
                    }
                }
                else
                {

                }               
            }
            prevRightXTwo = prevRightXOne;
            prevRightYTwo = prevRightYOne;
            prevRightXOne = rightHandX;
            prevRightYOne = rightHandY;
            prevLeftXTwo = prevLeftXOne;
            prevLeftYTwo = prevLeftYOne;
            prevLeftXOne = leftHandX;
            prevLeftYOne = leftHandY;
        }
        
    }
}
