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

        /// <summary>
        /// Activate a bunch of gesture recognizers
        /// </summary>
        public static void Load(bool rightHandVolume)
        {
            Unload();
            if (rightHandVolume) { tempoVolume = new TempoVolume(); }
            else { volume = new VolumeGesture(); }
            tempo = new TempoGesture();
            stop = new StopGesture();
        }

        public static void Unload()
        {
            if (tempo != null) tempo.Unload();
            if (stop != null) stop.Unload();
            if (tempoVolume != null) tempoVolume.Unload();
            if (volume != null) volume.Unload();
        }
    }
}