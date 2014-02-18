using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchestra
{
    public class Gestures
    {
        /// <summary>
        /// Gesture recognizers (stored to prevent garbage collection)
        /// </summary>
        private static List<object> gestures = new List<object>();

        /// <summary>
        /// Activate a bunch of gesture recognizers
        /// </summary>
        public static void Load()
        {
            gestures.Add(new VolumeGesture());
            gestures.Add(new TempoGesture());
            gestures.Add(new StopGesture());
            //gestures.Add(new TempoVolume());
        }
    }
}