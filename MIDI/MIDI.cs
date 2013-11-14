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
        private Stopwatch stopwatch = new Stopwatch();
        private Sequence sequence1;
        private Sequencer sequencer1;
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        //private Int32 TempoData;
        //private OutputDeviceDialog outDialog = new OutputDeviceDialog();

        float lastBeat = 0;

        public MIDI()
        {
            Dispatch.Play += Play;
            Dispatch.Beat += Beat;
            Dispatch.VolumeChanged += VolumeChanged;

            Initialize();
        }

        ~MIDI()
        {
            Dispatch.Play -= Play;
            Dispatch.Beat -= Beat;
            Dispatch.VolumeChanged -= VolumeChanged;
        }

        private void Play(float time)
        {
            sequencer1.Start();
        }

        private void Beat(float time, int beat)
        {
            sequencer1.Clock.Tempo = (int)(1000000 * (time - lastBeat));

            //MidiInternalClock.SetTempo(500000);
            lastBeat = time;
        }

        private void VolumeChanged(float time, float volume)
        {
            for (int i = 0; i < 16; i++)
            {
                outDevice.Send(new ChannelMessage(ChannelCommand.Controller, i , 11, (int)(volume*127)));
            }
        }

        private void Initialize()
        {
            this.sequence1 = new Sanford.Multimedia.Midi.Sequence();
            this.sequencer1 = new Sanford.Multimedia.Midi.Sequencer();
            sequence1.LoadAsync(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\r.mid");

            //sequencer1.Stop() followed by sequencer1.Continue could be used to handle changing tempo
            //also, perhaps sequencer1.position could be used (ticks)
            //sequence1.GetLength()

            if (OutputDevice.DeviceCount == 0)
            {
                Console.WriteLine("No MIDI output devices available.");
            }
            else
            {
                try
                {
                    outDevice = new OutputDevice(outDeviceID);
                    sequence1.LoadCompleted += HandleLoadCompleted;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message, "Error!");
                }
            }


            //this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
            sequencer1.ChannelMessagePlayed += HandleChannelMessagePlayed;
            sequencer1.MetaMessagePlayed += HandleMetaMessagePlayed;
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            sequencer1.Chased += HandleChased;
            //this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            sequencer1.Sequence = sequence1;
            //sequencer1.Start();
            IEnumerable<Track> tracks = sequencer1.Sequence.AsEnumerable();
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
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            outDevice.Send(e.Message);
        }

        private void HandleMetaMessagePlayed(object sender, MetaMessageEventArgs e)
        {
            var m = e.Message;
            Console.Write("{0}    ", m.MetaType);
            foreach (var b in m.GetBytes()) Console.Write("{0:X} ", b);
            Console.WriteLine();
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }

        }
    }
}
