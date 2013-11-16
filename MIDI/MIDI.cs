using System;
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
            LoadSong(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\s.mid");

            // Other messages that might be useful
            //this.sequencer1.PlayingCompleted += new System.EventHandler(PlayingCompleted);
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            //this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
        }

        public static void LoadSong(string file)
        {
            sequence.Load(file);
            Dispatch.TriggerSongLoaded();
            //Console.Write("tempo : ");
            //Console.WriteLine(sequencer.Clock.Tempo);
            //Dictionary<int, List<int[]>> eventsAtTicksDict = new Dictionary<int, List<int[]>>();
            //IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            //foreach (Track track in tracks)
            //{
            //    IEnumerable<MidiEvent> midievents = track.Iterator();
            //    foreach (MidiEvent midievent in midievents)
            //    {
            //        int[] eventData = new int[5];
            //        eventData[0] = 
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


            //Print track data
            //Console.WriteLine("______________________________________________________________________________________");
            //Console.Write("Sequencer division: ");
            //Console.WriteLine(sequencer.Sequence.Division);
            //IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            //foreach (var track in tracks)
            //{
            //    Console.WriteLine("______________________________________________________________________________________");
            //    IEnumerable<MidiEvent> midievents = track.Iterator();
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