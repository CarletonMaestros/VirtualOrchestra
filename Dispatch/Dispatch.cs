using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class Dispatch
    {
        public static void Load() { stopwatch.Restart(); }

        public delegate void TickInfoDelegate(int tick);
        public static event TickInfoDelegate TickInfo;
        public static void TriggerTickInfo(int tick) { if ( TickInfo != null) TickInfo(tick);}

        public delegate void SongLoadedDelegate(SongData song, string songName);
        public static event SongLoadedDelegate SongLoaded;
        public static void TriggerSongLoaded(SongData song, string songName) { if (SongLoaded != null) SongLoaded(song, songName); }

        public delegate void SkeletonMovedDelegate(float time, Skeleton skeleton);
        public static event SkeletonMovedDelegate SkeletonMoved;
        public static void TriggerSkeletonMoved(Skeleton skeleton) { if (SkeletonMoved != null) SkeletonMoved(Time, skeleton); }

        public delegate void VolumeChangedDelegate(float time, float volume);
        public static event VolumeChangedDelegate VolumeChanged;
        public static void TriggerVolumeChanged(float volume) { if (VolumeChanged != null) VolumeChanged(Time, volume); }

        public delegate void BeatDelegate(float time, int beat);
        public static event BeatDelegate Beat;
        public static void TriggerBeat(int beat, string type) { if (Beat != null) Beat(Time, beat); }
        public static void TriggerBeat(float time, int beat) { if (Beat != null) Beat(time, beat); }

        public delegate void StartDelegate(float time);
        public static event StartDelegate Start;
        public static void TriggerStart() { if (Start != null) Start(Time); }

        public delegate void StopDelegate(float time);
        public static event StopDelegate Stop;
        public static void TriggerStop() { if (Stop != null) Stop(Time); }

        public delegate void SongSelectedDelegate(string songFile, string songName);
        public static event SongSelectedDelegate SongSelected;
        public static void TriggerSongSelected(string songFile, string songName) { if (SongSelected != null) SongSelected(songFile, songName); }

        public delegate void LockDelegate(bool songLock);
        public static event LockDelegate Lock;
        public static void TriggerLock(bool songLock) { if (Lock != null) Lock(songLock); }

        private static Stopwatch stopwatch = new Stopwatch();

        private static float Time 
        {
            get { return stopwatch.ElapsedMilliseconds/1000f; }
        }
    }
}