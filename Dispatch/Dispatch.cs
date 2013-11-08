﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class Dispatch
    {
        public delegate void SkeletonMovedDelegate(Skeleton skeleton);
        public static event SkeletonMovedDelegate SkeletonMoved;
        public static void TriggerSkeletonMoved(Skeleton skeleton) { if (SkeletonMoved != null) SkeletonMoved(skeleton); }

        public delegate void VolumeChangedDelegate(float volume);
        public static event VolumeChangedDelegate VolumeChanged;
        public static void TriggerVolumeChanged(float volume) { if (VolumeChanged != null) VolumeChanged(volume); }

        public delegate void BeatDelegate(long beat);
        public static event BeatDelegate Beat;
        public static void TriggerBeat(long beat) { if (beat != null) Beat(beat); }

        public delegate void StartPieceDelegate();
        public static event StartPieceDelegate StartPiece;
        public static void TriggerStartPiece() { StartPiece(); }
    }
}