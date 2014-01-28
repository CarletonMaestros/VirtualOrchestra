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
        public float counter1;
        public float counter2;
        public float counter3;
        public float startValueY;
        public float startValueX;
        public float xAverage;
        public float yAverage;

        public StopGesture()
        {
            Dispatch.SkeletonMoved += SkeletonMoved;
        }

        ~StopGesture()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }


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
            if (counter3 == 15)
            {
                //Console.WriteLine("Left hand - right hand: {0}", Math.Abs(leftHandY - rightHandY));
                //Console.WriteLine("Right hand Y : {0}", rightHandY);
            }
            else { counter3 += 1; }
            if (Math.Abs(leftHandY - rightHandY) < .1 && rightHandY - hipRightY > .15)
            {
                //Console.WriteLine("here");
                if (counter1 < 10)
                {
                    if (rightHandY < prevRightYTwo && leftHandY < prevLeftYTwo) 
                    {
                        if (rightHandX + leftHandX > prevRightXTwo + prevLeftXTwo)
                        { 
                            counter1 += 1;
                            //Console.WriteLine(counter1);
                        }
                    }
                }
                else
                {
                    if (rightHandX + leftHandX > prevRightXTwo + prevLeftXTwo)
                    {
                        //Console.WriteLine("yay!");
                        if (counter2 == 10)
                        {
                            startValueY = yAverage;
                            startValueX = xAverage;
                            counter2 += 1;
                            Console.WriteLine("STOP");
                        }
                        else if (counter2 < 10)
                        {
                            if (yAverage == 0 && xAverage == 0)
                            {
                                counter2 += 1;
                                yAverage = leftHandY;
                                xAverage = leftHandX;
                            }
                            else
                            {
                                if (yAverage - leftHandY > -.1 && yAverage - leftHandY < .1 && xAverage - leftHandY > -.1 && xAverage - leftHandX < .1)
                                {
                                    yAverage = (yAverage + leftHandY) / 2;
                                    xAverage = (xAverage + leftHandX) / 2;
                                    counter2 += 1;
                                }
                                else
                                {
                                    yAverage = leftHandY;
                                    xAverage = leftHandX;
                                    counter2 = 0;
                                }
                            }
                        }
                    }
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
