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
        // Options
        static Boolean verbose = false;

        // Supporting variables
        static Stopwatch stopwatch = new Stopwatch();
        static Timer timer = new Timer(1);
        static OutputDevice outDevice = new OutputDevice(0);

        // Per song variables
        static Sequence sequence = new Sanford.Multimedia.Midi.Sequence();
        static Sequencer sequencer = new Sanford.Multimedia.Midi.Sequencer();
        static int ppq = 0;
        static int[] curChannelInstruments = new int[16]; //keeps track of what instruments are on what channel
        static List<int> allInstrumentsUsed = new List<int>();
        static List<int[]> instrChanges = new List<int[]>(); //int[17] [0-15] = inst @ chan. [16] = tick
        static Dictionary<int, int[]> instrumentsAtTicks;
        static Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();

        // Volatile Variables
        private static Boolean songStarted = false;
        static int beatCount = -1; //So that we can increment immediately and start at 0
        static double[] lastBeatStarts = { -1, -1, -1, -1, -1 };

        // Properties
        static double Time { get { return stopwatch.ElapsedMilliseconds / 1000d; } }
        static double TimeSinceLastBeat { get { return Time - LastBeatStart; } }
        static double LastBPS { get { return 1d / LastBeatDuration; } }
        static double Last4BPS { get { return 4 / (lastBeatStarts[lastBeatStarts.Length - 1] - lastBeatStarts[lastBeatStarts.Length - 5]); } }
        static double LastBeatStart { get { return lastBeatStarts[lastBeatStarts.Length - 1]; } }
        static double LastBeatDuration { get { return lastBeatStarts[lastBeatStarts.Length - 1] - lastBeatStarts[lastBeatStarts.Length - 2]; } }
        static double Offset { get { return 1d * sequencer.Position / ppq - beatCount - Math.Min(1, TimeSinceLastBeat / LastBeatDuration); } }

        /// <summary>
        /// Activate the MIDI player
        /// </summary>
        public static void Load()
        {
            // Subscribe to dispatch events
            Dispatch.Play += Play;
            Dispatch.Stop += Stop;
            Dispatch.Beat += Beat;
            Dispatch.VolumeChanged += VolumeChanged;
            Dispatch.SkeletonMoved += SkeletonMoved;

            // Subscribe to MIDI events
            sequencer.ChannelMessagePlayed += MIDIChannelMessagePlayed;
            sequencer.MetaMessagePlayed += MIDIMetaMessagePlayed;
            sequencer.Chased += MIDIChased;
            sequencer.Stopped += Stopped;

            // Initialize MIDI
            sequencer.Sequence = sequence;
            LoadSong(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\s.mid");

            // Initialize timer
            timer.Elapsed += new ElapsedEventHandler(TimePassed);
            timer.Enabled = true;
        }

        private static void Stopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        /// <summary>
        /// Clock interrupt every 15.6 milliseconds (WARNING: artifact (but so far reliable))
        /// </summary>
        static void TimePassed(object sender, ElapsedEventArgs e)
        {
            if (TimeSinceLastBeat / LastBeatDuration > 1.75 || lastBeatStarts[0] < 0)
            {
                sequencer.Clock.Tempo = int.MaxValue;
            }
            else
            {
                double bps = LastBPS;
                sequencer.Clock.Tempo = (int)(Math.Min(int.MaxValue,1000000/Last4BPS));
            }
        }

        /// <summary>
        /// Beat gesture triggered
        /// </summary>
        static void Beat(float time, int beat)
        {
            // Update last beat starts
            beatCount++;
            for (int i = 0; i < lastBeatStarts.Length-1; ++i)
            {
                lastBeatStarts[i] = lastBeatStarts[i + 1];
            }
            lastBeatStarts[lastBeatStarts.Length - 1] = Time;

            if (verbose) { Console.WriteLine("\n\n****Beginning of Beat {0}****", beatCount); }

            // TEMPORARY: Start song on first beat
            if (!songStarted)
            {
                Play(0);
                songStarted = true;
                stopwatch.Restart();
            }
        }

        /// <summary>
        /// Inform the GUI of the sequencer position.
        /// </summary>
        private static void SkeletonMoved(float time, Skeleton skeleton)
        {
            Dispatch.TriggerTickInfo(sequencer.Position);
        }

        //private void 
        //adds to event dictionary, and check for collisions, instead of exception, edits val at key
        //dictionary properties: keys are absolute tick values for the NON data, elements are lists of int[4] arrays containing instr#, pitch, velocity, duration in ticks
        static Dictionary<int, List<int[]>> addToDictHandleCollisions(int key, int[] eventData, Dictionary<int, List<int[]>> eventsAtTicksDict)
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
        static void stripMetaMessages(Sequence sequence)
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
        static int[,] populateInstrChanges(Sequence sequence)
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
        static int[,] mergeInstrChangeData()
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

        static Dictionary<int, List<int[]>> getInstrumentNoteTimes(Track track, int[,] instrumentsAtTicks, Dictionary<int, List<int[]>> eventsAtTicksDict)
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

                        int[] data = new int[3] { abs, vel, channel };

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
                            int instr = -1; //deal with it hopfully/
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
                            int[] eventdata = new int[5] { instr, pitchNOFF_NOD, velNON_NOD, dur, channelNOFF_NOD }; //to be added to the dictionary of lists of lists
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
                            int[] eventdata = new int[5] { instr, pitchNOFF, velNON, dur, channelNOFF };
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

        static Dictionary<int, int[]> arrayToDict(int[,] instrumentChangesArray)
        {
            Dictionary<int, int[]> instrumentDict = new Dictionary<int, int[]>();
            for (int i=0; i < instrumentChangesArray.GetLength(0); i++)
            {
                int[] tempArray = new int[16];
                for (int j = 0; j < 16; j++)
                {
                    tempArray[j] = instrumentChangesArray[i,j];
                }
                instrumentDict.Add(instrumentChangesArray[i, 16], tempArray);
            }
            return instrumentDict;
        }

        public static void LoadSong(string file)
        {
            sequence.Load(file);
            
            stripMetaMessages(sequencer.Sequence);
            int[,] instrumentsAtTicksArray = populateInstrChanges(sequencer.Sequence);
            ppq = sequencer.Sequence.Division;

            IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            foreach (Track track in tracks)
            {
                eventsAtTicksDict = getInstrumentNoteTimes(track, instrumentsAtTicksArray, eventsAtTicksDict);
            }
            instrumentsAtTicks = arrayToDict(instrumentsAtTicksArray);
            SongData song = new SongData{ppq=ppq, beatsPerMeasure=4, eventsAtTicksDict=eventsAtTicksDict, instrumentsAtTicks=instrumentsAtTicks};
            Dispatch.TriggerSongLoaded(song);

        }

        static void Play(float time)
        {
            sequencer.Start();
        }

        static void Stop(float time)
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