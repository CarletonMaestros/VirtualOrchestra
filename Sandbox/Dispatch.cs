using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    class Dispatch
    {
        public delegate void SkeletonMovedDelegate(Skeleton skeleton);
        static public event SkeletonMovedDelegate SkeletonMoved;
        static public void TriggerSkeletonMoved(Skeleton skeleton) { if (SkeletonMoved != null) SkeletonMoved(skeleton); }

        public delegate void VolumeChangedDelegate(float volume);
        static public event VolumeChangedDelegate VolumeChanged;
        static public void TriggerVolumeChanged(float volume) { if (VolumeChanged != null) VolumeChanged(volume); }
    }
}

