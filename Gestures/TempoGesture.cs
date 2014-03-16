using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Kinect;

namespace Orchestra
{
    public class TempoGesture
    {
        Int32 counter = 1;
        Stopwatch stopwatch;
        String seeking;
        SkeletonPoint rightHand;
        SkeletonPoint rightHip;
        SkeletonPoint prevOne;
        SkeletonPoint prevTwo;
        float threshold;
        int stillFramesCount;
        int framesInFirstBeat;
        int startMarker = 0;
        bool locked = true;
        public bool beat = false;

        // Loads the skeleton, stop, and lock into the dispatch
        // Resets the gesture
        public TempoGesture()
        {
            stopwatch = new Stopwatch();
            counter = 0;
            seeking = "STILL";
            threshold = .002F;
            stillFramesCount = 0;
            framesInFirstBeat = 0;
            Dispatch.SkeletonMoved += SkeletonMoved;
            Dispatch.Stop += Stopped;
            Dispatch.Lock += Lock;
        }

        // If the song is stopped, reset what gesture it is looking for to "still"
        private void Stopped(float time) { seeking = "STILL"; }

        // Unload the skeleton from the dispatch
        public void Unload() { Dispatch.SkeletonMoved -= this.SkeletonMoved; }

        // Lock or unlock the gesetures
        // When it is locked, no gestures are recognized when go to the song select menu
        private void Lock(bool songLocked) { locked = songLocked; }

        // Recognizes when a skeleton has moved
        void SkeletonMoved(float time, Skeleton skel)
        {
            beat = false;
            // If the song is locked, no gestures can be recognized, so return
            if (locked) return;
            // Gets the location information for each of the necessary joints
            rightHand = skel.Joints[JointType.HandRight].Position;
            rightHip = skel.Joints[JointType.HipRight].Position;
            // Initialize switch cases
            switch (seeking)
            {
                // To start the gesture, your hand must be still for 15 frames (about .5 seconds)
                case "STILL":
                    {
                        // If 15 frames have been counted, switch to seeking "start"
                        if (stillFramesCount == 15) { seeking = "START"; }
                        // If less than 15 frames, check to see if the hand is in roughly the same place as it was in the previous two frames
                        // If so, increment the counter
                        if (stillFramesCount < 15 && Math.Abs(prevOne.X - rightHand.X) < .08 && Math.Abs(prevOne.Y - rightHand.Y) < .08 && Math.Abs(prevTwo.X - rightHand.X) < .08 && Math.Abs(prevTwo.Y - rightHand.Y) < .08)
                        {
                            stillFramesCount++;
                            break;
                        }
                        // If the hand has moved, reset the count to 0
                        else if (stillFramesCount < 15) { stillFramesCount = 0; }
                        break;
                    }
                // Once your hand has stayed still for roughly 15 seconds, look for the starting beat
                case "START":
                    {
                        // Check to see if the right hand y-value has increased over the past two frames
                        if (prevTwo.Y > (prevOne.Y + .01) && prevOne.Y > (rightHand.Y + .01) && Math.Abs(rightHand.X - rightHip.X) < .15)
                        {
                            seeking = "MINIMUM";
                            Dispatch.TriggerStart();
                            break;
                        }
                        break;
                    }
                case "MINIMUM":
                    {
                        if (prevTwo.Y < (prevOne.Y - threshold) && prevOne.Y < (rightHand.Y - threshold))
                        {
                            if (counter == 5) { counter = 1; }
                            if (startMarker == 0)
                            {
                                startMarker = 1;
                                long firstTempo = stopwatch.ElapsedMilliseconds * 1000 / 2;
                            }
                            else { long tempo = stopwatch.ElapsedMilliseconds * 1000; }
                            stopwatch.Restart();
                            Dispatch.TriggerBeat(counter, "beat");
                            beat = true;
                            seeking = "MAXIMUM";
                            counter++;
                            break;
                        }
                        break;
                    }
                case "MAXIMUM":
                    {
                        if (prevTwo.Y > (prevOne.Y + threshold) && prevOne.Y > (rightHand.Y + threshold))
                        {
                            seeking = "MINIMUM";
                            break;
                        }
                        break;
                    }
            }
            prevTwo = prevOne;
            prevOne = rightHand;
        }
    }
}