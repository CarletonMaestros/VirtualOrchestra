using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchestra.Sandbox.Loggers
{
    class Loggers
    {
        /// <summary>
        /// Loggers (stored to prevent garbage collection)
        /// </summary>
        private static List<object> loggers = new List<object>();

        /// <summary>
        /// Activate a bunch of loggers
        /// </summary>
        public static void Load()
        {
            loggers.Add(new BeatLogger());
        }
    }
}
