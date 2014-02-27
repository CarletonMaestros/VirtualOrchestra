using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Orchestra
{
    public partial class App : Application
    {
        public static StartScreen startScreen;
        public static SongSelectWindow songSelect;
        public static TutorialWindow tutorial;
        public static MainWindow main;
        //public static SineTracker sineTracker;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Dispatch.Load();
            Kinect.Load();
            MIDI.Load();

            startScreen = new StartScreen();
            songSelect = new SongSelectWindow();
            main = new MainWindow();
            //sineTracker = new SineTracker();

            ShowStartScreen();
            //sineTracker.Show();
        }

        public static void ShowStartScreen()
        {
            Gestures.Unload();
            Dispatch.TriggerStop();

            startScreen.Show();
            startScreen.Activate();
        }

        public static void RunTutorial()
        {
            tutorial = new TutorialWindow();
            tutorial.Show();
            tutorial.Activate();
        }

        public static void SelectSong()
        {
            Gestures.Unload();
            Dispatch.TriggerStop();

            songSelect.Clean();
            songSelect.SongSelector.SelectedIndex = -1;

            songSelect.Show();
            songSelect.Activate();
        }

        public static void PlaySong(string songFile, string songName, bool tutorial)
        {
            main.Show();
            //songSelect.StopAllMusic();
            if (tutorial) { main.Hide(); }
            main.Activate();
            Gestures.Load();
            MIDI.LoadSong(songFile, songName);
        }
    }
}