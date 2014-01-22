using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestra
{
    public class SongData
    {
        public int ppq;
        public int beatsPerMeasure;
        public Dictionary<int, List<int[]>> eventsAtTicksDict;
        public Dictionary<int, int[]> instrumentsAtTicks;
    }
}
