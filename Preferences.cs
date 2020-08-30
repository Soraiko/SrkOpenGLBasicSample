using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class Preferences
    {
        static List<string> preferences = new List<string>(0);
        public static float StartupWidth;
        public static float StartupHeight;
        public static int SampleCount;
        public static float FrameRate;
        public static bool Fullscreen;

        public static void Initialize()
        {
            Fullscreen = false;
            StartupWidth = 0.75f;
            StartupHeight = 0.75f;
            FrameRate = 60f;
            SampleCount = 4;
        }
        public static void ApplyPreference(string name, string setting_value)
        {
            switch (name)
            {
                case "screen_resolution":
                    StartupWidth = Single.Parse(setting_value.Split(',')[0]);
                    StartupHeight = Single.Parse(setting_value.Split(',')[1]);
                    break;
                case "sample_count":
                    SampleCount = int.Parse(setting_value);
                    break;
                case "frame_rate":
                    FrameRate = Single.Parse(setting_value);
                    break;
                case "start_in_fullscreen":
                    Preferences.Fullscreen = setting_value.ToLower().Contains("true");
                break;
            }
        }

        public static void Get()
        {
            if (System.IO.File.Exists("preferences.ini"))
            {
                preferences = new List<string>(System.IO.File.ReadAllLines("preferences.ini"));
                for (int i = 0; i < preferences.Count; i++)
                {
                    string[] spli = preferences[i].Split('=');
                    if (spli.Length > 1)
                        ApplyPreference(spli[0].ToLower(), spli[1]);
                }
            }
        }


        public static bool GetPreference(string name, out string preference)
        {
            preference = "";
            for (int i = 0; i < preferences.Count; i++)
                if (preferences[i].Split('=')[0].ToLower() == name.ToLower())
                {
                    preference = preferences[i].Split('=')[1];
                    return true;
                }
            return false;
        }

        public static void SetPreference(string name, string setting_value)
        {
            bool set = false;
            for (int i = 0; i < preferences.Count; i++)
            {
                if (preferences[i].Split('=')[0].ToLower() == name.ToLower())
                {
                    set = true;
                    preferences[i] = name + "=" + setting_value;
                    break;
                }
            }
            if (!set)
                preferences.Add(name + "=" + setting_value);

            System.IO.File.WriteAllLines("preferences.ini", preferences);
        }
    }
}
