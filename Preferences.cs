using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class Preferences
    {
        static List<string> preferences = new List<string>(0);
        public static float StartX;
        public static float StartY;
        public static float StartWidth;
        public static float StartHeight;
        public static int SampleCount;
        public static float FrameRate;
        public static bool Fullscreen;

        public static void Initialize()
        {
            Fullscreen = false;

            StartWidth = 0.75f;
            StartHeight = 0.75f;

            StartX = 0.5f - StartWidth / 2f;
            StartY = 0.5f - StartHeight / 2f;

            FrameRate = 60f;
            SampleCount = 4;
        }

        public enum PrefName
        {
            SCREEN_RESOLUTION = 0,
            SCREEN_LOCATION = 1,
            SAMPLE_COUNT = 2,
            FRAME_RATE = 3,
            START_IN_FULLSCREEN = 4

        }

        static string[] prefNamesString = new string[]
        {
            "SCREEN_RESOLUTION",
            "SCREEN_LOCATION",
            "SAMPLE_COUNT",
            "FRAME_RATE",
            "START_IN_FULLSCREEN"
        };

        public static void SetPreference(PrefName name, string setting_value)
        {
            bool set = false;
            for (int i = 0; i < preferences.Count; i++)
            {
                if (preferences[i].Split('=')[0].ToUpper() == prefNamesString[(int)name])
                {
                    set = true;
                    preferences[i] = name + "=" + setting_value;
                    break;
                }
            }
            if (!set)
                preferences.Add(name + "=" + setting_value);

            switch (name)
            {
                case PrefName.SCREEN_RESOLUTION:
                    StartWidth = Single.Parse(setting_value.Split(',')[0]);
                    StartHeight = Single.Parse(setting_value.Split(',')[1]);
                    break;
                case PrefName.SCREEN_LOCATION:
                    StartX = Single.Parse(setting_value.Split(',')[0]);
                    StartY = Single.Parse(setting_value.Split(',')[1]);
                    break;
                case PrefName.SAMPLE_COUNT:
                    SampleCount = int.Parse(setting_value);
                    break;
                case PrefName.FRAME_RATE:
                    FrameRate = Single.Parse(setting_value);
                    break;
                case PrefName.START_IN_FULLSCREEN:
                    Preferences.Fullscreen = setting_value.ToUpper().Contains("TRUE");
                break;
            }
            System.IO.File.WriteAllLines("preferences.ini", preferences);
        }

        public static void GetFromFile()
        {
            if (System.IO.File.Exists(@"preferences.ini"))
            {
                preferences = new List<string>(System.IO.File.ReadAllLines(@"preferences.ini"));
                for (int i = 0; i < preferences.Count; i++)
                {
                    string[] spli = preferences[i].Split('=');
                    if (spli.Length > 1)
                    {
                        int prefIndex = Array.IndexOf(prefNamesString, spli[0].ToUpper());
                        if (prefIndex > -1)
                        SetPreference((PrefName)prefIndex, spli[1]);
                    }
                }
            }
        }


        public static bool GetPreference(PrefName name, out string preference)
        {
            if (preferences==null)
                GetFromFile();

            preference = "";
            for (int i = 0; i < preferences.Count; i++)
                if (preferences[i].Split('=')[0].ToUpper() == prefNamesString[(int)name])
                {
                    preference = preferences[i].Split('=')[1];
                    return true;
                }
            return false;
        }

    }
}
