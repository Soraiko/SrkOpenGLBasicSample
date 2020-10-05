using System;
using System.Collections.Generic;
using System.IO;

namespace SrkOpenGLBasicSample
{
    public class Program
    {
        public static string ExecutableDirectory;
        public static void Main(string[] args)
        {
            ExecutableDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            System.Globalization.CultureInfo.CurrentCulture = Compatibility.us_cultureinfo_for_decimal_separator;
            System.Globalization.CultureInfo.CurrentUICulture = Compatibility.us_cultureinfo_for_decimal_separator;

            System.Threading.Thread.CurrentThread.CurrentUICulture = Compatibility.us_cultureinfo_for_decimal_separator;
            System.Threading.Thread.CurrentThread.CurrentCulture = Compatibility.us_cultureinfo_for_decimal_separator;

            Directory.SetCurrentDirectory(@"KHDebug_files");
            bool read_preferences = true;

            for (int i=0;i< args.Length;i++)
            {
                switch (args[i].ToLower())
                {
                    case "ignore_preferences":
                        read_preferences = false;
                        break;
                }
            }

            Preferences.Initialize();
            if (read_preferences) Preferences.GetFromFile();
            /*Preferences.SetPreference(Preferences.PrefName.DEBUG, "true");
            Preferences.SetPreference(Preferences.PrefName.SCREEN_LOCATION, "1,0");
            Preferences.SetPreference(Preferences.PrefName.START_IN_FULLSCREEN,"true");*/


            using (var renderer_window = new RendererWindow(
                (int)(Preferences.StartX * OpenTK.DisplayDevice.Default.Width),
                (int)(Preferences.StartY * OpenTK.DisplayDevice.Default.Width),
                (int)(Preferences.StartWidth * OpenTK.DisplayDevice.Default.Width),
                (int)(Preferences.StartHeight * OpenTK.DisplayDevice.Default.Height), Preferences.SampleCount))
            {
                renderer_window.Run(Preferences.FrameRate);
            }
        }
    }
}
