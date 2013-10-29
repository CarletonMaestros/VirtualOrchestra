using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra.Sandbox.Gestures
{
    class TempoGesture
    {
        public Int32 counter;
        public int seeking;
        public int mod;
        public float prevYOne;
        public float prevYTwo;
        public float prevXOne;
        public float prevXTwo;
        public float rightHandY;
        public float rightHandX;
        public float threshold;

        public TempoGesture()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;

            counter = 0;
            seeking = 1;
            mod = 1;
            threshold = .002F;
        }

        ~TempoGesture()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        void SkeletonMoved(Skeleton skel)
        {
            if (counter % mod == 0)
            {
                foreach (Joint joint in skel.Joints)
                {
                    if (joint.JointType == JointType.HandRight)
                    {
                        rightHandY = joint.Position.Y;
                        rightHandX = joint.Position.X;
                        //Console.WriteLine(rightHandY);
                        //Console.WriteLine(prevYOne);
                    }
                }
                switch (seeking)
                {
                    case 1:
                        {
                            if (prevYTwo < (prevYOne -  threshold) && prevYOne < (rightHandY - threshold))
                            {
                                Dispatch.TriggerBeat(counter % 4 + 1);
                                seeking = 2;
                                counter++;
                                break;
                            }
                            break;
                        }
                    case 2:
                        {
                            if (prevYTwo > (prevYOne + threshold) && prevYOne > (rightHandY + threshold))
                            {
                                //Console.WriteLine((counter % 4) + 1);
                                seeking = 1;
                                break;
                            }
                            break;
                        }


                }
                prevYTwo = prevYOne;
                prevXTwo = prevXOne;
                prevYOne = rightHandY;
                prevXOne = rightHandX;
            }
             
        }
    }
}
