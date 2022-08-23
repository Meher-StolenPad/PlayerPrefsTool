using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PrefsSerialzer
{
    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }
    public static float GetFloat(string key, float defaultValue = 0.0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }
    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }
    public static object TryGetCostumeType(string key, out PlayerPrefsType playerPrefsType, string defaultValue = "")
    {
        string json = PlayerPrefs.GetString(key, defaultValue);

        string retunValue = json;

        //Debug.Log(json);

        if (String.IsNullOrEmpty(json))
        {
            playerPrefsType = PlayerPrefsType.String;
            retunValue = json;
            Debug.Log(key + " Is empty !");
        }
        else if (json.TryParseJson(out Serialzer<object> t))
        {
            // Debug.Log(json);
            playerPrefsType = t.type;
            switch (t.type)
            {
                case PlayerPrefsType.Vector3:

                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Vector2:
                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Color:
                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Vector4:
                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Bool:
                    retunValue = t.value.ToString();
                    break;
            }
        }
        else
        {
            playerPrefsType = PlayerPrefsType.String;
            retunValue = json;
        }
        return retunValue;
    }
    public static void SetVector3(string key, Vector3 _value)
    {
        Serialzer<Vector3> serialzer = new Serialzer<Vector3>();
        serialzer.type = PlayerPrefsType.Vector3;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Vector3 StringToVector3(string s)
    {
        Vector3 outVector3 = Vector3.zero;

        if (s.Contains("{"))
        {
            outVector3 = JsonUtility.FromJson<Vector3>(s);
        }
        else
        {
            //Debug.Log(s);

            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);

            // Build new Vector3 from array elements

            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
            outVector3.z = float.Parse(splitString[2]);
        }

        return outVector3;
    }
    public static void SetBool(string key, bool _value)
    {
        Serialzer<bool> serialzer = new Serialzer<bool>();
        serialzer.type = PlayerPrefsType.Bool;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static bool StringToBool(string s)
    {
        bool outBool = false;

        if (s == "True")
        {
            outBool = true;
        }
        return outBool;
    }
    public static void SetVector2(string key, Vector2 _value)
    {
        Serialzer<Vector2> serialzer = new Serialzer<Vector2>();
        serialzer.type = PlayerPrefsType.Vector2;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Vector3 StringToVector2(string s)
    {
        Vector2 outVector3 = Vector2.zero;

        if (s.Contains("{"))
        {
            outVector3 = JsonUtility.FromJson<Vector2>(s);
        }
        else
        {
            //Debug.Log(s);

            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);

            // Build new Vector3 from array elements

            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
        }

        return outVector3;
    }
    public static void SetVector4(string key, Vector4 _value)
    {
        Serialzer<Vector4> serialzer = new Serialzer<Vector4>();
        serialzer.type = PlayerPrefsType.Vector4;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Vector4 StringToVector4(string s)
    {
        Vector4 outVector3 = Vector4.zero;

        if (s.Contains("{"))
        {
            outVector3 = JsonUtility.FromJson<Vector4>(s);
        }
        else
        {
            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);

            // Build new Vector3 from array elements

            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
            outVector3.z = float.Parse(splitString[2]);
            outVector3.w = float.Parse(splitString[3]);
        }

        return outVector3;
    }
    public static void SetColor(string key, Color _value)
    {
        Serialzer<Color> serialzer = new Serialzer<Color>();

        serialzer.type = PlayerPrefsType.Color;

        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Color StringToColor(string s)
    {
        Color outColor = Color.white;

        if (ColorUtility.TryParseHtmlString("#" + s, out Color _Color))
        {
            outColor = _Color;
        }
        else if (s.Contains("{"))
        {
            s = s.Replace("{", "");
            s = s.Replace("}", "");
            s = s.Replace('"', ' ');
            s = s.Replace("r", "");
            s = s.Replace("g", "");
            s = s.Replace("b", "");
            s = s.Replace("a", "");
            s = s.Replace(":", "");
            s = s.Replace(" ", "");

            var splitString = s.Split(","[0]);


            //    // Build new Vector3 from array elements

            outColor.r = float.Parse(splitString[0]);
            outColor.g = float.Parse(splitString[1]);
            outColor.b = float.Parse(splitString[2]);
            outColor.a = float.Parse(splitString[3]);

        }
        else
        {
            s = s.Replace("RGBA", "");
            s = s.Replace("#", "");
            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);


            //    // Build new Vector3 from array elements

            outColor.r = float.Parse(splitString[0]);
            outColor.g = float.Parse(splitString[1]);
            outColor.b = float.Parse(splitString[2]);
            outColor.a = float.Parse(splitString[3]);
        }

        return outColor;
    }

    public static void CopyToClipboard(this string s)
    {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }
}
[Serializable]
public class Serialzer<T>
{
    public PlayerPrefsType type;
    public T value;
}
public enum PlayerPrefsType
{
    Int,
    Float,
    String,
    Vector2,
    Vector3,
    Vector4,
    Color,
    Bool
}