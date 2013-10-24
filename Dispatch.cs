using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics
{
    class Dispatch
    {
        public delegate void SkeletonMovedDelegate(Skeleton skeleton);
        static public event SkeletonMovedDelegate SkeletonMoved;
        static public void TriggerSkeletonMoved(Skeleton skeleton) { SkeletonMoved(skeleton); }

        public delegate void VolumeChangedDelegate(float volume);
        static public event VolumeChangedDelegate VolumeChanged;
        static public void TriggerVolumeChanged(float volume) { VolumeChanged(volume); }
    }
}
