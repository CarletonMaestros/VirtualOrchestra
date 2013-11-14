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

            //Initialize MIDI
            sequencer.Sequence = sequence;
            sequence.Load(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.mid");
            sequencer.Start();

            // Print track data
            IEnumerable<Track> tracks = sequencer.Sequence.AsEnumerable();
            foreach (var track in tracks)
            {
                Console.WriteLine("______________________________________________________________________________________");
                IEnumerable<MidiEvent> midievents = track.Iterator();
                foreach (MidiEvent midievent in midievents)
                {
                    Console.WriteLine(midievent.MidiMessage);
                }
            }

            //foreach (var track in sequence1)
            //{
            //    for (int i = 0; i < track.Count; ++i)
            //    {
            //        var m = track.GetMidiEvent(i).MidiMessage;
            //        if (m.MessageType == MessageType.Channel) continue;
            //        Console.Write("{0} {1:X} ", m.MessageType, m.Status);
            //        foreach (var b in m.GetBytes()) Console.Write("{0} ", b);
            //        Console.WriteLine();
            //    }
            //}

            // Other messages that might be useful
            //this.sequencer1.PlayingCompleted += new System.EventHandler(PlayingCompleted);
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            //this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
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