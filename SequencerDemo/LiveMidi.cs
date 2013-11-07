
using System;
using System.Collections.Generic;
using System.Text;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Kinect;

using System.ComponentModel; //AsyncCompletedEventArgs







namespace Orchestra.LiveMidi
{
    class LiveMidi
    {
        private Sequence sequence1;
        private Sequencer sequencer1;
        private OutputDevice outDevice;
        private int outDeviceID = 0;
        private Int32 TempoData;
        //private OutputDeviceDialog outDialog = new OutputDeviceDialog();

        public LiveMidi()
        {
            Dispatch.Beat += this.Beat;
            Dispatch.VolumeChanged += this.VolumeChanged;

            Initialize();
        }

        ~LiveMidi()
        {
            Dispatch.Beat -= this.Beat;
            Dispatch.VolumeChanged -= this.VolumeChanged;
        }

        private void Beat(int beat)
        {
            sequencer1.clock.Tempo = (60000000 / Convert.ToInt32(beat));
        }

        private void VolumeChanged(float volume)
        {
            outDevice.Send(new ChannelMessage(ChannelCommand.Controller, 11, (int)(volume*127)));
        }
        private void Initialize()
        {
            this.sequence1 = new Sanford.Multimedia.Midi.Sequence();
            this.sequencer1 = new Sanford.Multimedia.Midi.Sequencer();
            sequencer1.Stop();
            sequence1.LoadAsync("C:\\Users\\admin\\Desktop\\VirtualOrchestra\\Sample MIDIs\\g.mid");

            //sequencer1.Stop() followed by sequencer1.Continue could be used to handle changing tempo
            //also, perhaps sequencer1.position could be used (ticks)
            //sequence1.GetLength()
        }
        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            sequencer1.Start();
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            outDevice.Send(e.Message);
        }

        protected void OnLoad(EventArgs e)
        {
            if (OutputDevice.DeviceCount == 0)
            {
                Console.WriteLine("No MIDI output devices available.");
            }
            else
            {
                try
                {
                    outDevice = new OutputDevice(outDeviceID);
                    //sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    sequence1.LoadCompleted += HandleLoadCompleted;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message, "Error!");
                }
            }

            //base.OnLoad(e);
        }
    }
}
