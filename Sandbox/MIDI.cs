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
    class MIDI
    {
        private Stopwatch stopwatch = new Stopwatch();
        private long ref_time = 0;
        private Sequence sequence1;
        private Sequencer sequencer1;
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        //private Int32 TempoData;
        //private OutputDeviceDialog outDialog = new OutputDeviceDialog();

        public MIDI()
        {
            Dispatch.Beat += Beat;
            Dispatch.StartPiece += StartPiece;
            Dispatch.VolumeChanged += VolumeChanged;

            Initialize();
        }

        ~MIDI()
        {
            Dispatch.Beat -= Beat;
            Dispatch.StartPiece -= StartPiece;
            Dispatch.VolumeChanged -= VolumeChanged;
        }

        private void StartPiece()
        {
            sequencer1.Start();
        }

        private void Beat(long beat)
        {
            sequencer1.clock.Tempo = (Convert.ToInt32(beat));

            //MidiInternalClock.SetTempo(500000);
            // ^^^
            // clock is a private variable (hence the error) anyway
            // so there HAS to be a better way to do this...
        }

        private void VolumeChanged(float volume)
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
            sequence1.LoadAsync(@"C:\Users\admin\Desktop\VirtualOrchestra\Sample MIDIs\s.mid");

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
            //this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            sequencer1.Chased += HandleChased;
            //this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var x = sequence1.GetLength();
            sequencer1.Sequence = sequence1;
        }



        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            outDevice.Send(e.Message);
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
