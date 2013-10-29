using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra.Loggers
{
    class BeatLogger
    {
        public BeatLogger()
        {
            Dispatch.Beat += this.Beat;
        }

        ~BeatLogger()
        {
            Dispatch.Beat -= this.Beat;
        }

        void Beat(int beat)
        {
            Console.WriteLine("Beat " + beat);
        }
    }
}
