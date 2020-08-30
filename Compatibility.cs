using System;
using System.Collections.Generic;
using System.Text;

namespace SrkOpenGLBasicSample
{
    public static class Compatibility
    {
        public static OpenTK.Input.Key FirstPerson_Forward;
        public static OpenTK.Input.Key FirstPerson_Left;
        public static OpenTK.Input.Key FirstPerson_Right;
        public static OpenTK.Input.Key FirstPerson_Backward;

        public static bool AZERTY_keyboard = System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToLower().Contains("fra") ||
                                                System.Globalization.CultureInfo.CurrentCulture.EnglishName.ToLower().Contains("belg");
        public static System.Globalization.CultureInfo us_cultureinfo_for_decimal_separator;

        static Compatibility()
        {
            if (AZERTY_keyboard)
            {
                FirstPerson_Forward = OpenTK.Input.Key.Z;
                FirstPerson_Left = OpenTK.Input.Key.Q;
            }
            else
            {
                FirstPerson_Forward = OpenTK.Input.Key.W;
                FirstPerson_Left = OpenTK.Input.Key.A;
            }
            FirstPerson_Backward = OpenTK.Input.Key.S;
            FirstPerson_Right = OpenTK.Input.Key.D;

            us_cultureinfo_for_decimal_separator = new System.Globalization.CultureInfo("en-US");
            us_cultureinfo_for_decimal_separator.NumberFormat.NumberDecimalSeparator = ".";
        }
    }
}
