﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Sanford.Multimedia.Midi;

namespace Orchestra
{
    public class MIDI
    {
        static Stopwatch    stopwatch = new Stopwatch();
        static Sequence     sequence  = new Sanford.Multimedia.Midi.Sequence();
        static Sequencer    sequencer = new Sanford.Multimedia.Midi.Sequencer();
        static OutputDevice outDevice = new OutputDevice(0);
        private int[] curChannelInstruments = new int[16]; //keeps track of what instruments are on what channel
        private List<int>   allInstrumentsUsed = new List<int>();
        //yolO(n) time
        private List<int[]> instrChanges = new List<int[]>(); //int[17] [0-15] = inst @ chan. [16] = tick
        
        //Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();

        static float lastBeat = 0;

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

            // Initialize MIDI
            sequencer.Sequence = sequence;
            LoadSong(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.mid");

            // Other messages that might be useful
            //this.sequencer1.PlayingCompleted += new System.EventHandler(PlayingCompleted);
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            //this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
        }

        //private void 
        //adds to dictionary, and check for collisions, instead of exception, edits val at key
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
                //this is ridiculous. Whatever, I suppose
            }
            else
            {
                keyValHolder.Add(eventData);
                eventsAtTicksDict.Add(key, keyValHolder);
                return eventsAtTicksDict;
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

                    switch (midievent.MidiMessage.Status>>4)
                    {
                        case 0xC:
                            {
                                int[] deltaEvent = new int[18];
                                int locationInDE;
                                byte[] byteHold = new byte[4];
                                int timestamp;

                                
                                locationInDE = midievent.MidiMessage.Status & 0x0F;
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
            int[,] instrChangesArray = new int[uniqueDeltaEvents.Count,17];
            uniqueDeltaEvents.Sort();
            int[] uniqueDeltaEventsArray = new int[uniqueDeltaEvents.Count];
            uniqueDeltaEvents.CopyTo(uniqueDeltaEventsArray);
            for (int i = 0; i<uniqueDeltaEvents.Count; ++i)
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
                //Console.Write("{0:X}, {1}, {2} {3}", midievent.MidiMessage.Status, midievent.DeltaTicks, midievent.AbsoluteTicks, "bytes: ");
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
                        try
                        {
                            noteDurationData.Add(key, data);
                        }
                        catch (ArgumentException)
                        {
                            //Console.WriteLine("too stupid for real msg"); // i no longer trust Sanford's merge method. slut. probably a guy, actually. bitch. Interesting gender norms appearing here.
                            //MULTIPLE NOTES ARE ALLOWED TO BE PLAYED BY THE SAME INSTRUMENT AT THE SAME TIME
                        }
                        break;
                        
                    case 0x8:
                        
                        byte[] byteHoldNOFF = new byte[4];
                        byteHoldNOFF = midievent.MidiMessage.GetBytes();
                        int channelNOFF = byteHoldNOFF[0] & 0x0F;//bitwise and to get last 4 bits
                        int pitchNOFF = byteHoldNOFF[1];
                        int absNOFF = midievent.AbsoluteTicks;
                        Tuple<int, int> keyNOFF = new Tuple<int, int>(channelNOFF, pitchNOFF);
                        try
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
                                    instr = instrumentsAtTicks[i, channelNOFF]; //added this case because I was wrong in the previous comment. Fuck you t-2 Joe Brown.
                                }
                            }
                            int[] eventdata = new int[4] { instr, pitchNOFF, velNON, dur };
                            eventsAtTicksDict = addToDictHandleCollisions(absNON, eventdata, eventsAtTicksDict);
                        }
                        catch (KeyNotFoundException)
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
            //Dispatch.TriggerSongLoaded();

            //Oh golly gee wiz, the engineers are not going to like this...
            MIDI preprocessor = new MIDI();
            int[,] instrumentsAtTicks = preprocessor.populateInstrChanges(sequencer.Sequence); //efficiency has been checked into the ICU and is in a vegetative state
            List<int> allInstrumentsUsed = preprocessor.allInstrumentsUsed;
            


            //proving we can extract tempo
            Console.Write("Sequencer division: ");
            Console.WriteLine(sequencer.Sequence.Division);
            
            //Joe is no longer doing things he understands
            /*
             * he is trying to add a structure that gets filled with NONs
             * as the sequence goes on, and eliminates them when the 
             * corresponding NOFs happen. When this happens, the abs
             * time between the events is recorded and sent to 
             * the dictionary as the note duration
             */
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

            Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();

            IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            foreach (Track track in tracks)
            {
                eventsAtTicksDict = preprocessor.getInstrumentNoteTimes(track, instrumentsAtTicks, eventsAtTicksDict);
            }
            
        }

        static void Play(float time)
        {
            sequencer.Start();
        }

        static void Pause(float time)
        {
            sequencer.Stop();
        }

        static void Beat(float time, int beat)
        {
            // Adjust clock based on how long the last beat took
            sequencer.Clock.Tempo = (int)(1000000 * (time - lastBeat));
            lastBeat = time;
        }

        static void VolumeChanged(float time, float volume)
        {
            // Send a volume change message to each channel
            for (int i = 0; i < 16; i++)
            {
                outDevice.Send(new ChannelMessage(ChannelCommand.Controller, i , 11, (int)(volume*127)));
            }
        }

        static void MIDIChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            outDevice.Send(e.Message);
            if (e.Message.MidiChannel == 4)
            {
                Console.WriteLine(e.Message.Message);
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