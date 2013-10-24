using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class TempoGesture
    {
        public float minRightHandY = 100;
        public float maxRightHandY = 0;

        public TempoGesture()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;
        }

        ~TempoGesture()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        void SkeletonMoved(Skeleton skel)
        {
            float rightHandY = 0;
            foreach (Joint joint in skel.Joints)
            {
                if (joint.JointType == JointType.HandRight) {
                    rightHandY = joint.Position.Y;
                }
            }
            if (rightHandY > maxRightHandY)
            {
                maxRightHandY = rightHandY;
            }
            if (rightHandY < minRightHandY)
            {
                minRightHandY = rightHandY;
            }
            
        }
    }
}
