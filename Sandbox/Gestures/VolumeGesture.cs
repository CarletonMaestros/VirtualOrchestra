using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra.Gestures
{
    class VolumeGesture
    {
        public VolumeGesture()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;
        }

        ~VolumeGesture()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        void SkeletonMoved(Skeleton skel)
        {
        }
    }
}
