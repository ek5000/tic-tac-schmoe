﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace tic_tac_schmoe.Pages
{
    public class ListItem
    {
        public String name { get; set; }
        public String details { get; set; }
        public String id { get; set; }
        public ListItem(string n, string d, string i)
        {
            name = n;
            details = d;
            id = i;
        }
    }
    
    public static class ThemeColors
    {
        public static Color Lime = Color.FromArgb(255, 164, 196, 0);
        public static Color Green = Color.FromArgb(255, 96, 169, 23);
        public static Color Emerald = Color.FromArgb(255, 0, 138, 0);
        public static Color Teal = Color.FromArgb(255, 0, 171, 169);
        public static Color Cyan = Color.FromArgb(255, 27, 161, 226);
        public static Color Cobalt = Color.FromArgb(255, 0, 80, 239);
        public static Color Indigo = Color.FromArgb(255, 106, 0, 255);
        public static Color Violet = Color.FromArgb(255, 170, 0, 255);
        public static Color Pink = Color.FromArgb(255, 255, 114, 208);
        public static Color Magenta = Color.FromArgb(255, 216, 0, 115);
        public static Color Crimson = Color.FromArgb(255, 162, 0, 37);
        public static Color Red = Color.FromArgb(255, 229, 20, 0);
        public static Color Burnt_Orange = Color.FromArgb(255, 204, 85, 0);
        public static Color Orange = Color.FromArgb(255, 250, 104, 0);
        public static Color Amber = Color.FromArgb(255, 240, 163, 10);
        public static Color Yellow = Color.FromArgb(255, 227, 200, 0);
        public static Color Brown = Color.FromArgb(255, 130, 90, 44);
        public static Color Olive = Color.FromArgb(255, 109, 135, 100);
        public static Color Steel = Color.FromArgb(255, 100, 118, 135);
        public static Color Mauve = Color.FromArgb(255, 118, 96, 138);
        public static Color Taupe = Color.FromArgb(255, 135, 121, 78);

        public static Tuple<double, double, double> ColorToYUV(Color color)
        {
            double y = .299 * color.R + .587 * color.G + .114 * color.B;
            double u = .492 * (color.B - y);
            double v = .877 * (color.R - y);
            Tuple<double, double, double> yuv = new Tuple<double, double, double>(y, u, v);
            return yuv;
        }
        public static double ColorDistance(Color lhsColor, Color rhsColor)
        {
            Tuple<double, double, double> lhs = ColorToYUV(lhsColor);
            Tuple<double, double, double> rhs = ColorToYUV(rhsColor);
            double sum = 2 * (lhs.Item1 - rhs.Item1) * (lhs.Item1 - rhs.Item1) +
                        (lhs.Item2 - rhs.Item2) * (lhs.Item2 - rhs.Item2) +
                        (lhs.Item3 - rhs.Item3) * (lhs.Item3 - rhs.Item3);
            return Math.Sqrt(sum);
        }
        public static Color FindDifferentColor(Color c)
        {
            var colors = typeof(ThemeColors).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var color in colors)
            {
                Color themeColor = (Color)color.GetValue(null);
                if (ColorDistance(c, themeColor) < 70)
                {
                    return themeColor;
                }
            }
            return Colors.DarkGray;
        }
        public static Color FindDifferentColor(Color c1, Color c2)
        {
            var colors = typeof(ThemeColors).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            foreach (var color in colors)
            {
                Color themeColor = (Color)color.GetValue(null);
                if (ColorDistance(c1, themeColor) < 70 && ColorDistance(c2, themeColor) < 70)
                {
                    return themeColor;
                }
            }
            return Colors.DarkGray;
        }

        public static Color StringToColor(string color)
        {
            return (Color)typeof(ThemeColors).GetField(color, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).GetValue(null);
        }
    }
}