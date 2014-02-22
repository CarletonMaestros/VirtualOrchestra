using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchestra
{
    public class Gestures
    {
        public static TempoGesture tempo;
        public static StopGesture stop;
        public static TempoVolume tempoVolume;
        public static VolumeGesture volume;

        public static bool rightHandVolume;

        /// <summary>
        /// Activate a bunch of gesture recognizers
        /// </summary>
        public static void Load()
        {
            Unload();
            if (rightHandVolume) { tempoVolume = new TempoVolume(); }
            else { volume = new VolumeGesture(); }
            tempo = new TempoGesture();
            stop = new StopGesture();
        }

        public static void Unload()
        {
            if (tempo != null) { tempo.Unload(); tempo = null; }
            if (stop != null) { stop.Unload(); stop = null; }
            if (tempoVolume != null) { tempoVolume.Unload(); tempoVolume = null; }
            if (volume != null) { volume.Unload(); volume = null; }
        }
    }
}