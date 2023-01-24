using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Graphs.Styles;
using Color = UnityEngine.Color;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public static class DavanciDebug    
    {   
        public static void Log(string message,Color color)
        {
            Debug.Log("<color=#" + FromColor(color) + ">" + message + "</color>");
        }
        public static void Warning(string message)
        {
            Debug.LogWarning("<color=#" + FromColor(Color.yellow) + ">" + message + "</color>");
        }
        public static void Error(string message)
        {
            Debug.LogError("<color=#" + FromColor(Color.red) + ">" + message + "</color>");
        }
        public static string FromColor(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
            // int r = (int)color.r, g = (int)color.g, b = (int)color.b;
            // return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
}
