using System.Text;
using UnityEngine;

namespace JamForge
{
    public static class StringExtensions
    {
        public static string Colorize(this string str, string color)
        {
            var builder = new StringBuilder();
            builder.Append("<color=");
            builder.Append(color);
            builder.Append(">");
            builder.Append(str);
            builder.Append("</color>");
            return builder.ToString();
        }

        public static string Colorize(this string str, Color color)
        {
            var builder = new StringBuilder();
            builder.Append("<color=#");
            builder.Append(ColorUtility.ToHtmlStringRGBA(color));
            builder.Append(">");
            builder.Append(str);
            builder.Append("</color>");
            return builder.ToString();
        }

        public static string Colorize(this string str, Color32 color)
        {
            var builder = new StringBuilder();
            builder.Append("<color=#");
            builder.Append(ColorUtility.ToHtmlStringRGBA(color));
            builder.Append(">");
            builder.Append(str);
            builder.Append("</color>");
            return builder.ToString();
        }

        public static string DyeRed(this string str)
        {
            return str.Colorize("red");
        }

        public static string DyeGreen(this string str)
        {
            return str.Colorize("green");
        }

        public static string DyeBlue(this string str)
        {
            return str.Colorize("blue");
        }

        public static string DyeYellow(this string str)
        {
            return str.Colorize("yellow");
        }

        public static string DyeMagenta(this string str)
        {
            return str.Colorize("magenta");
        }

        public static string DyeCyan(this string str)
        {
            return str.Colorize("cyan");
        }
    }
}
