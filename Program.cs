using System;
using System.Collections.Generic;
using System.IO;

namespace SrkOpenGLBasicSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = Compatibility.us_cultureinfo_for_decimal_separator;

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
            if (read_preferences) Preferences.Get();

            using (var renderer_window = new RendererWindow(
                (int)(Preferences.StartupWidth * OpenTK.DisplayDevice.Default.Width),
                (int)(Preferences.StartupHeight * OpenTK.DisplayDevice.Default.Height), Preferences.SampleCount))
            {
                renderer_window.Run(Preferences.FrameRate);
            }
        }


    }
}
