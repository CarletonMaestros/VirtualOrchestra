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
        public float prevRightXOne;
        public float prevLeftYOne;
        public float prevLeftXOne;
        public float hipRightY;
        public float hipCenterX;
        public float counter;
        public Boolean start = false;
        public Boolean stop = false;

        public StopGesture()
        {
            Dispatch.SkeletonMoved += SkeletonMoved;
            Dispatch.Start += Start;
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
                if (joint.JointType == JointType.HipRight) { hipRightY = joint.Position.Y; }
                if (joint.JointType == JointType.HipCenter) { hipCenterX = joint.Position.X; }
            }
            if (Math.Abs(leftHandY - rightHandY) < .1 && rightHandY - hipRightY > .5) { start = true; }
            if (Math.Abs(leftHandY - rightHandY) < .1 && start == true && stop == false)
            {          
                if (rightHandX >= prevRightXOne && leftHandX <= prevLeftXOne && rightHandY <= prevRightYOne && leftHandY <= prevLeftYOne) { counter += 1; }
                //if ((rightHandX >= prevRightXOne && leftHandX <= prevLeftXOne && Math.Abs(rightHandY - prevRightYTwo) < .2 && Math.Abs(leftHandY - prevLeftYTwo) < .2) || (rightHandY <= prevRightYOne && leftHandY <= prevLeftYOne && Math.Abs(rightHandX - prevRightXTwo) < .2 && Math.Abs(leftHandX - prevLeftXTwo) < .2)) { counter += 1; }
                //Console.WriteLine("{0}", Math.Abs(rightHandX - prevRightXTwo));
                //if (Math.Abs(rightHandX - hipCenterX) - Math.Abs(leftHandX - hipCenterX) < .1 && Math.Abs(rightHandX - prevRightXTwo) >= .2)   { counter += 1; }
                if (counter >= 3 && Math.Abs(rightHandX - prevRightXOne) < .1 && Math.Abs(leftHandX - prevLeftXOne) < .1 && Math.Abs(rightHandY - prevRightYOne) < .1 && Math.Abs(leftHandY - prevLeftYOne) < .1)
                {
                    stop = true;
                    counter = 0;
                    Dispatch.TriggerStop();
                }
            }
            prevRightXOne = rightHandX;
            prevRightYOne = rightHandY;
            prevLeftXOne = leftHandX;
            prevLeftYOne = leftHandY;
        }

        private void Start(float time)
        {
            start = false;
            stop = false;
        }
    }
}