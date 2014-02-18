using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Kinect;

namespace Orchestra.Playback 
{
    class Playback
    {
        static float lastBeatTime = 0;
        static int lastBeat = 0;

        static Dictionary<String,float> RowDict(string[] row, string[] headers)
        {
            var dict = new Dictionary<String, float>();
            for (var i = 0; i < row.Length; ++i)
            {
                float x;
                bool valid = float.TryParse(row[i], out x);
                dict[headers[i]] = valid ? x : float.NaN;
            }
            return dict;
        }

        static void ReadCSV(string filename)
        {
            // Parse CSV
            var rows = File.ReadAllLines(filename)
                       .Select(r => r.Split(';')[0].Split(','));

            // Read headers
            var headers = rows.First();

            // Read data
            foreach (var row in rows.Skip(1))
            {
                //var d = RowDict(row, headers);
                //Console.WriteLine(d["Beat"]);
                //if ((int)d["Beat"] != lastBeat)
                //{
                //    lastBeat = (int)d["Beat"]; 
                //    Thread.Sleep((int)(1000 * (d["Time"] - lastBeatTime)));
                //    Dispatch.TriggerBeat(d["Time"], (int)d["Beat"]);
                //    lastBeatTime = d["Time"];
                //}

                Thread.Sleep(600);
                Dispatch.TriggerBeat(1, "WTF");
            }
        }

        static void Main(string[] args)
        {
            MIDI.Load();
            Dispatch.TriggerStart();
            ReadCSV(@"C:\Users\admin\Desktop\Dance.csv");
            Console.ReadLine();
        }
    }
}