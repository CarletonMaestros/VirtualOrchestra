using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Microsoft.Kinect;
using Sanford.Multimedia.Midi;
using System.Threading.Tasks;

namespace Orchestra
{
    public class MIDI
    {
        static Stopwatch stopwatch = new Stopwatch();
        static Sequence sequence = new Sanford.Multimedia.Midi.Sequence();
        static Sequencer sequencer = new Sanford.Multimedia.Midi.Sequencer();
        static OutputDevice outDevice = new OutputDevice(0);
        private int[] curChannelInstruments = new int[16]; //keeps track of what instruments are on what channel
        private List<int> allInstrumentsUsed = new List<int>();
        //yolO(n) time
        private List<int[]> instrChanges = new List<int[]>(); //int[17] [0-15] = inst @ chan. [16] = tick
        private static Boolean songStarted = false;

        //Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();

        static double lastBeat = -1;
        static double prevDeltaTime = 0;
        static double deltaTime = double.MaxValue;
        static long lastTeleport = 0;
        static int curTick = 0;
        static int ppq = 0;
        static int beatCount = 0;
        static Boolean verbose = false;
        static Boolean graph = false;
        static Timer timer = new Timer(1);
        

        static int timerCounter = 0;

        /// <summary>
        /// Activate the MIDI player
        /// </summary>
        public static void Load()
        {
            // Subscribe to dispatch events
            Dispatch.Play += Play;
            Dispatch.Beat += Beat;
            Dispatch.VolumeChanged += VolumeChanged;

            // Subscribe to MIDI events
            sequencer.ChannelMessagePlayed += MIDIChannelMessagePlayed;
            sequencer.MetaMessagePlayed += MIDIMetaMessagePlayed;
            sequencer.Chased += MIDIChased;
            timer.Elapsed += new ElapsedEventHandler(TimePassed);
            timer.Enabled = true;

            // Initialize MIDI
            sequencer.Sequence = sequence;
            LoadSong(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\s.mid");

            // Other messages that might be useful
            //this.sequencer1.PlayingCompleted += new System.EventHandler(PlayingCompleted);
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            //this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
        }

        private static void TimePassed(object sender, ElapsedEventArgs e)
        {
            // Stop if we've lost direction
            // one clock is 15.6 milliseconds
            //Console.WriteLine("{0}   {1}   {2}", stopwatch.ElapsedMilliseconds/1000d, lastBeat, deltaTime);
            if (stopwatch.ElapsedMilliseconds/1000d > lastBeat + deltaTime - 0.05)
            {
                if (verbose) { Console.WriteLine("#HANGING at {0}", sequencer.Position % ppq); }
                sequencer.Clock.Tempo = Int32.MaxValue; // 2 billion is very slow
                //if (stopwatch.ElapsedMilliseconds / 1000d > lastBeat + 2 * deltaTime - 0.05)
            }

            // Normal time (if not teleporting)
            else if (stopwatch.ElapsedMilliseconds - lastTeleport > .140)
            {
                //Console.WriteLine(deltaTime);
                sequencer.Clock.Tempo = (int)(1000000 * deltaTime);
                //if (verbose) { Console.WriteLine("Finished Teleport in {0} millis\nSequencer position is {1}\nSetting Tempo to {2}", stopwatch.ElapsedMilliseconds - temptime, sequencer.Position % ppq, newTempo); }
            }

            //if (verbose) { Console.WriteLine("\nWaited {0} millis\nLocalBeatCount is {1}\nBeatCount is {2}", stopwatch.ElapsedMilliseconds - testtime, localBeatCount, beatCount); }
        }

        static void Beat(float time, int beat)
        {
            time = stopwatch.ElapsedMilliseconds / 1000f;
            if (!songStarted)
            {
                Play(0);
                songStarted = true;
                stopwatch.Restart();
            }
            beatCount++;
            //if (verbose) { Console.WriteLine("\n\n****Beginning of Beat {0}****", beatCount); }
            float beatPercentCompleted = (sequencer.Position % ppq) / (float)ppq;
            float beatPercentRemaining = 1 - beatPercentCompleted;
            deltaTime = time - lastBeat;
            //Console.WriteLine("{0}   {1}   {2}", time, time - lastBeat, deltaTime);
            lastBeat = time;
            //if (verbose) { Console.WriteLine("DeltaTime is {0}\nSequencer position {1}\nBeatPercentComplete is {2}\nBeatPercentRemaining is {3}", deltaTime, sequencer.Position % ppq, beatPercentCompleted, beatPercentRemaining); }
            if (beatPercentCompleted < .9 && beatPercentCompleted > .1)
            {
                // Teleport
                int teleportSpeed = (int)(150000 / beatPercentRemaining);
                //if (verbose) { Console.WriteLine("#TELEPORTING with tempo {0}", teleportSpeed); }
                sequencer.Clock.Tempo = teleportSpeed;
                lastTeleport = stopwatch.ElapsedMilliseconds;
            }
            //if (verbose) { Console.WriteLine("Will check for hang in {0} millis", (deltaTime * 1000) - 10); }
            long testtime = stopwatch.ElapsedMilliseconds;
        }

        //static async void Hang(int millis, int localBeatCount, long testtime)
        //{
        //    await Task.Delay(millis);
        //    if (verbose) { Console.WriteLine("\nWaited {0} millis\nLocalBeatCount is {1}\nBeatCount is {2}", stopwatch.ElapsedMilliseconds - testtime, localBeatCount, beatCount); }
        //    if (localBeatCount == beatCount)
        //    {
        //        if (verbose) { Console.WriteLine("#HANGING at {0}", sequencer.Position % ppq); }
        //        sequencer.Clock.Tempo = Int32.MaxValue; // 2 billion is very slow
        //    }
        //    return;
        //}

        //private void 
        //adds to event dictionary, and check for collisions, instead of exception, edits val at key
        //dictionary properties: keys are absolute tick values for the NON data, elements are lists of int[4] arrays containing instr#, pitch, velocity, duration in ticks
        private Dictionary<int, List<int[]>> addToDictHandleCollisions(int key, int[] eventData, Dictionary<int, List<int[]>> eventsAtTicksDict)
        {
            List<int[]> keyValHolder = new List<int[]>();
            if (eventsAtTicksDict.ContainsKey(key))
            {
                keyValHolder = eventsAtTicksDict[key];
                keyValHolder.Add(eventData);
                eventsAtTicksDict.Remove(key);
                eventsAtTicksDict.Add(key, keyValHolder);
                return eventsAtTicksDict;
            }
            else
            {
                keyValHolder.Add(eventData);
                eventsAtTicksDict.Add(key, keyValHolder);
                return eventsAtTicksDict;
            }
        }
        //removes tempo meta messages
        private void stripMetaMessages(Sequence sequence)
        {
            IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            foreach (Track track in tracks)
            {
                int counter = 0;
                IEnumerable<MidiEvent> midievents = track.Iterator();
                foreach (MidiEvent midievent in midievents)
                {
                    //Console.WriteLine(midievent.MidiMessage.MessageType);
                    if (midievent.MidiMessage.MessageType == MessageType.Meta)
                    {
                        //Console.WriteLine(midievent.MidiMessage.MessageType);
                        track.RemoveAt(counter);
                    }
                    counter++;
                }
            }
        }
        // populates instrChanges
        private int[,] populateInstrChanges(Sequence sequence)
        {
            IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            foreach (Track track in tracks)
            {
                IEnumerable<MidiEvent> midievents = track.Iterator();
                foreach (MidiEvent midievent in midievents)
                {

                    switch (midievent.MidiMessage.Status >> 4)
                    {
                        case 0xC:
                            {
                                int[] deltaEvent = new int[18];
                                int locationInDE;
                                byte[] byteHold = new byte[4];


                                locationInDE = midievent.MidiMessage.Status & 0x0F; //grabs channel number (0-15)
                                byteHold = midievent.MidiMessage.GetBytes();
                                deltaEvent[locationInDE] = byteHold[1]; //second byte is instr number
                                if (!allInstrumentsUsed.Contains(byteHold[1]))
                                {
                                    allInstrumentsUsed.Add(byteHold[1]);
                                }
                                deltaEvent[16] = midievent.AbsoluteTicks;
                                deltaEvent[17] = locationInDE;

                                instrChanges.Add(deltaEvent); //start filling deltaEvent
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
            }
            allInstrumentsUsed.Sort();
            return mergeInstrChangeData();
        }

        /*
         * Merges the irresponsibly-generated instr-change data
         */
        private int[,] mergeInstrChangeData()
        {
            List<int> uniqueDeltaEvents = new List<int>();
            foreach (int[] deltaEvent in instrChanges)
            {
                if (uniqueDeltaEvents.Contains(deltaEvent[16]))
                {
                    continue;
                }
                else
                {
                    uniqueDeltaEvents.Add(deltaEvent[16]);
                }
            }
            int[,] instrChangesArray = new int[uniqueDeltaEvents.Count, 17];
            uniqueDeltaEvents.Sort();
            int[] uniqueDeltaEventsArray = new int[uniqueDeltaEvents.Count];
            uniqueDeltaEvents.CopyTo(uniqueDeltaEventsArray);
            for (int i = 0; i < uniqueDeltaEvents.Count; ++i)
            {
                instrChangesArray[i, 16] = uniqueDeltaEventsArray[i];
                foreach (int[] deltaEvent in instrChanges)
                {
                    if (deltaEvent[16] == uniqueDeltaEventsArray[i])
                    {
                        instrChangesArray[i, deltaEvent[17]] = deltaEvent[deltaEvent[17]];
                    }
                }
                if (i < uniqueDeltaEvents.Count - 1)
                {
                    for (int j = 0; j < 16; ++j)
                    {
                        instrChangesArray[i + 1, j] = instrChangesArray[i, j];
                    }
                }
            }

            return instrChangesArray;
        }

        private Dictionary<int, List<int[]>> getInstrumentNoteTimes(Track track, int[,] instrumentsAtTicks, Dictionary<int, List<int[]>> eventsAtTicksDict)
        {
            //Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();

            //dict to be modified to allow note duration calculations
            Dictionary<Tuple<int, int>, int[]> noteDurationData = new Dictionary<Tuple<int, int>, int[]>(); //key = <channel,pitch> data = <abs, vel>
            IEnumerable<MidiEvent> midievents = track.Iterator();
            foreach (MidiEvent midievent in midievents)
            {
                //Console.Write("{0:X}, {1}, {2}", midievent.MidiMessage.Status, midievent.AbsoluteTicks, "bytes: ");
                //foreach (byte b in midievent.MidiMessage.GetBytes()) Console.Write("{0:X}, {1}", b, " ");
                //Console.WriteLine();

                switch (midievent.MidiMessage.Status >> 4)
                {
                    case 0x9:

                        byte[] byteHold = new byte[4];
                        byteHold = midievent.MidiMessage.GetBytes();
                        int channel = byteHold[0] & 0x0F;//bitwise and to get last 4 bits
                        int pitch = byteHold[1];
                        int vel = byteHold[2];
                        int abs = midievent.AbsoluteTicks;
                        Tuple<int, int> key = new Tuple<int, int>(channel, pitch);

                        int[] data = new int[2] { abs, vel };

                        if (!noteDurationData.ContainsKey(key))
                        {
                            noteDurationData.Add(key, data);
                        }
                        else
                        {
                            //Console.WriteLine("too stupid for real msg");
                            //NOD stands for note-on duplicate. We need this because occasionally, songs like to send multiple NONs before a NOFF. I find this confusing and frustrating.
                            byte[] byteHoldNOFF_NOD = new byte[4];
                            byteHoldNOFF_NOD = midievent.MidiMessage.GetBytes();
                            int channelNOFF_NOD = byteHoldNOFF_NOD[0] & 0x0F;//bitwise and to get last 4 bits
                            int pitchNOFF_NOD = byteHoldNOFF_NOD[1];
                            int absNOFF_NOD = midievent.AbsoluteTicks;
                            Tuple<int, int> keyNOFF_NOD = new Tuple<int, int>(channelNOFF_NOD, pitchNOFF_NOD);

                            int[] NONdata_NOD = noteDurationData[keyNOFF_NOD];
                            noteDurationData.Remove(keyNOFF_NOD);
                            int absNON_NOD = NONdata_NOD[0];
                            int velNON_NOD = NONdata_NOD[1];
                            int dur = absNOFF_NOD - absNON_NOD;
                            int instr = -1;
                            //CALCULATE current instrument
                            for (int i = 0; i < instrumentsAtTicks.GetLength(0); ++i)
                            {
                                if (instrumentsAtTicks[i, 16] > absNON_NOD)
                                {
                                    instr = instrumentsAtTicks[i - 1, channelNOFF_NOD]; //this makes sense. finding a bigger time then backtracking should never fail
                                }
                                else if (i == instrumentsAtTicks.GetLength(0) - 1)
                                {
                                    instr = instrumentsAtTicks[i, channelNOFF_NOD]; //added this case because I was wrong in the previous comment.
                                }
                            }
                            int[] eventdata = new int[4] { instr, pitchNOFF_NOD, velNON_NOD, dur }; //to be added to the dictionary of lists of lists
                            eventsAtTicksDict = addToDictHandleCollisions(absNON_NOD, eventdata, eventsAtTicksDict);
                            //pretend nothing happened. To be clear, in this try block, we are killing the duplicated note and restarting with a new note.
                            noteDurationData.Add(key, data);

                        }
                        break;

                    case 0x8:

                        byte[] byteHoldNOFF = new byte[4];
                        byteHoldNOFF = midievent.MidiMessage.GetBytes();
                        int channelNOFF = byteHoldNOFF[0] & 0x0F;//bitwise and to get last 4 bits
                        int pitchNOFF = byteHoldNOFF[1];
                        int absNOFF = midievent.AbsoluteTicks;
                        Tuple<int, int> keyNOFF = new Tuple<int, int>(channelNOFF, pitchNOFF);
                        if (noteDurationData.ContainsKey(keyNOFF))
                        {
                            int[] NONdata = noteDurationData[keyNOFF];
                            noteDurationData.Remove(keyNOFF);
                            int absNON = NONdata[0];
                            int velNON = NONdata[1];
                            int dur = absNOFF - absNON;
                            int instr = -1;
                            //CALCULATE current instrument
                            for (int i = 0; i < instrumentsAtTicks.GetLength(0); ++i)
                            {
                                if (instrumentsAtTicks[i, 16] > absNON)
                                {
                                    instr = instrumentsAtTicks[i - 1, channelNOFF]; //this makes sense. finding a bigger time then backtracking should never fail
                                }
                                else if (i == instrumentsAtTicks.GetLength(0) - 1)
                                {
                                    instr = instrumentsAtTicks[i, channelNOFF]; //added this case because I was wrong in the previous comment.
                                }
                            }
                            int[] eventdata = new int[4] { instr, pitchNOFF, velNON, dur };
                            eventsAtTicksDict = addToDictHandleCollisions(absNON, eventdata, eventsAtTicksDict);
                        }
                        else
                        {
                            //as If I even care at this point
                        }
                        break;


                    default:
                        break;
                }
            }

            return eventsAtTicksDict;
        }

        public static void LoadSong(string file)
        {
            sequence.Load(file);
            Dispatch.TriggerSongLoaded();

            MIDI preprocessor = new MIDI();
            preprocessor.stripMetaMessages(sequencer.Sequence);
            int[,] instrumentsAtTicks = preprocessor.populateInstrChanges(sequencer.Sequence);
            List<int> allInstrumentsUsed = preprocessor.allInstrumentsUsed;
            ppq = sequencer.Sequence.Division;



            //proving we can extract tempo
            //Console.Write("Sequencer division: "); 
            //Console.WriteLine(ppq);

            Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();
            IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            foreach (Track track in tracks)
            {
                eventsAtTicksDict = preprocessor.getInstrumentNoteTimes(track, instrumentsAtTicks, eventsAtTicksDict);
            }


        //    Console.WriteLine("int[] tempArray = int[4];");
        //    Console.WriteLine("List<int[]> tempList = new List<int[]>;");
        //    Console.WriteLine("Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();");
        //    foreach (KeyValuePair<int, List<int[]>> kvp in eventsAtTicksDict)
        //    {
                
        //        Console.WriteLine("Key = {0}", kvp.Key);
        //        //Console.WriteLine("\ti#\tp\tvel\tdur");
        //        foreach (int[] NONevent in kvp.Value)
        //        {
        //            int iterationCount = 0;
        //            foreach (int NONsubEvent in NONevent)
        //            {
        //                Console.WriteLine("tempArray[{0}] = {1}", iterationCount, NONsubEvent);
        //                Console.Write("\t{0}", NONsubEvent);
        //                iterationCount++;
        //            }
        //            Console.WriteLine();
        //        }
        //    }

        }

        static void Play(float time)
        {
            sequencer.Start();
        }

        static void Pause(float time)
        {
            sequencer.Stop();
        }

        static void VolumeChanged(float time, float volume)
        {
            // Send a volume change message to each channel
            for (int i = 0; i < 16; i++)
            {
                outDevice.Send(new ChannelMessage(ChannelCommand.Controller, i, 11, (int)(volume * 127)));
            }
        }

        static void MIDIChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            outDevice.Send(e.Message);
            if (e.Message.MidiChannel == 4)
            {
                //Console.WriteLine(e.Message.Message);
            }
        }

        static void MIDIMetaMessagePlayed(object sender, MetaMessageEventArgs e)
        {
            var m = e.Message;
            Console.Write("{0}    ", m.MetaType);
            foreach (var b in m.GetBytes()) Console.Write("{0:X} ", b);
            Console.WriteLine();
        }

        static void MIDIChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }
    }
}








//IEnumerable<MidiEvent> midievents = allTracksMerged.Iterator();
//foreach (MidiEvent midievent in midievents)
//{
//    int[] eventData = new int[5];
//    //eventData[0] =
//    Console.Write("{0:X}, {1}, {2} {3}", midievent.MidiMessage.Status, midievent.DeltaTicks, midievent.AbsoluteTicks, "bytes: ");
//    switch (midievent.MidiMessage.Status)
//    {
//        case 3:
//            {
//                foreach (byte b in midievent.MidiMessage.GetBytes()) Console.Write((char)b);
//                break;
//            }
//        default:
//            {
//                foreach (byte b in midievent.MidiMessage.GetBytes()) Console.Write("{0:X}, {1}", b, " ");
//                break;
//            }
//    }
//    Console.WriteLine(" ");
//}



//Print track data
//Console.WriteLine("______________________________________________________________________________________");
//Console.Write("Sequencer division: ");
//Console.WriteLine(sequencer.Sequence.Division);
//int x ;
//IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
//foreach (var track in tracks)
//{
//    Console.WriteLine("______________________________________________________________________________________");
//    IEnumerable<MidiEvent> midievents = track.Iterator();
//    //MessageDispatcher md = new MessageDispatcher();
//    //int tickNumber = new int();
//    //ChannelChaser cc = new ChannelChaser();
//    //IEnumerable<int> dispathEvents = track.DispatcherIterator(md);
//    //IEnumerable<int> tickEvents = track.TickIterator(tickNumber, cc, md);

//    foreach (MidiEvent midievent in midievents)
//    {

//        //if (midievent.MidiMessage.MessageType == MessageType.Meta)
//        //{
//        //    Console.WriteLine(midievent.MidiMessage.MetaType);
//        //}
//        Console.Write("{0:X}, {1}, {2} {3}", midievent.MidiMessage.Status, midievent.DeltaTicks, midievent.AbsoluteTicks, "bytes: ");
//        switch (midievent.MidiMessage.Status)
//        {
//            case 3:
//                {
//                    foreach (var b in midievent.MidiMessage.GetBytes()) Console.Write((char)b);
//                    break;
//                }
//            default:
//                {
//                    foreach (var b in midievent.MidiMessage.GetBytes()) Console.Write("{0:X} ", b);
//                    break;
//                }
//        }
//        Console.WriteLine(" ");


//    }
//}

//Track allTracksMerged = new Track();

//IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
//foreach (Track track in tracks)
//{
//    if (track == tracks.First())
//    {
//        allTracksMerged = track;
//        continue;
//    }
//    allTracksMerged.Merge(track);
//}