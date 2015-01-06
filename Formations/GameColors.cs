using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Formations
{
    [Serializable]
    class GameColors
    {


        //Inside Colors
        public static Color noUnitInsideColor = Color.Green;
        public static Color attUnitInsideColor = Color.Red;
        public static Color defUnitInsideColor = Color.Gray;
        public static Color magUnitInsideColor = Color.Purple;

        //Outside Colors
        public static Color noControlOutsideColor = Color.DarkGreen;
        public static Color HostControlOutsideColor = Color.PowderBlue;
        public static Color guestControlOutsideColor = Color.DarkOrange;
        public static Color bothControlOutsideColor = Color.Black;
        public static Color attUnitOutsideColor = attUnitInsideColor;
        public static Color defUnitOutsideColor = defUnitInsideColor;
        public static Color magUnitOutsideColor = magUnitInsideColor;

        //Border Colors
        public static Color hoverBorderColor = Color.Red;
        public static Color selectedBorderColor = Color.Purple;
        public static Color normalBorderColor = Color.Black;

        //Button Colors
        public static Color turnButtonInsideColor = Color.Blue;
        public static Color turnButtonOutsideColor = Color.DarkOrange;
        public static Color turnButtonInsideColorGuest = Color.Yellow;
        public static Color attButton = Color.Red;
        public static Color magicButton = Color.LightPink;
        public static Color moveButton = Color.Blue;

        //Background Colors
        public static Color boardAreaBackground = Color.DarkBlue;
        public static Color playerAreaBackground = Color.Black;
    }
}
