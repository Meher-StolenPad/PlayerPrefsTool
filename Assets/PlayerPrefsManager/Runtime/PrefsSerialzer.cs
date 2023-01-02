using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static class PrefsSerialzer
{
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public static void DeleteKey(string key)
    {
         PlayerPrefs.DeleteKey(key);
    }
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
    public static Vector2 GetVector2(string key,Vector2 defaultValue)
    {
        return GetCosutomTypeValue<Vector2>(key, defaultValue);
    }
    public static Vector3 GetVector3(string key, Vector3 defaultValue)
    {
        return GetCosutomTypeValue<Vector3>(key, defaultValue);
    }
    public static Vector4 GetVector4(string key, Vector4 defaultValue)
    {
        return GetCosutomTypeValue<Vector4>(key, defaultValue);
    }
    public static Color GetColor(string key, Color defaultValue)
    {
        return GetCosutomTypeValue<Color>(key, defaultValue);
    }
    public static bool GetBool(string key, bool defaultValue)
    {
        return GetCosutomTypeValue<bool>(key, defaultValue);
    }

    private static T GetCosutomTypeValue<T>(string key,T defaultValue)
    {
        object returnvalue = default;
        Serialzer<T> serialzer = JsonUtility.FromJson<Serialzer<T>>(PlayerPrefs.GetString(key));
        if(serialzer != null)
        {
            returnvalue = serialzer.value;
        }
        else
        {
            returnvalue = defaultValue;
        }
        return (T)returnvalue;  
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
                case PlayerPrefsType.DateTime:
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
    public static Vector2 StringToVector2(string s)
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

    public static void SetDateTime(string key, DateTime _value)
    {
        Serialzer<DateTime> serialzer = new Serialzer<DateTime>();

        serialzer.type = PlayerPrefsType.DateTime; 

        serialzer.value = _value;
        //JsonConvert.SerializeObject(DateTime.Now)
        string jsonString = JsonConvert.SerializeObject(serialzer);
        PlayerPrefs.SetString(key, jsonString);
    }
    public static DateTime? StringToDateTime(string s)
    {
        //date = JsonConvert.DeserializeObject<DateTime>(s);

        if (DateTime.TryParse(s,out DateTime d))
        {
            return  d;
        }
        else
        {
            return null;
        }
    }

    public static void CopyToClipboard(this string s)
    {
        TextEditor te = new TextEditor();
        te.text = s;
        te.SelectAll();
        te.Copy();
    }
    private static string numberPattern = " ({0})";

    public static string NextAvailableFilename(string path)
    {
        // Short-cut if already available
        if (!File.Exists(path))
            return path;

        // If path has extension then insert the number pattern just before the extension and return next filename
        if (Path.HasExtension(path))
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

        // Otherwise just append the pattern to the path and return next filename
        return GetNextFilename(path + numberPattern);
    }

    private static string GetNextFilename(string pattern)
    {
        string tmp = string.Format(pattern, 1);
        if (tmp == pattern)
            throw new ArgumentException("The pattern must include an index place-holder", "pattern");

        if (!File.Exists(tmp))
            return tmp; // short-circuit if no matches

        int min = 1, max = 2; // min is inclusive, max is exclusive/untested

        while (File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            int pivot = (max + min) / 2;
            if (File.Exists(string.Format(pattern, pivot)))
                min = pivot;
            else
                max = pivot;
        }

        return string.Format(pattern, max);
    }
    public static bool ValidateJSON(this string s)
    {
        try
        {
            JToken.Parse(s);
            return true;
        }
        catch (JsonReaderException ex)
        {
            Trace.WriteLine(ex);
            return false;
        }
    }
    public static bool TryParseJson<T>(this string @this, out T result)
    {
        bool success = true;
        var settings = new JsonSerializerSettings
        {
            Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
            MissingMemberHandling = MissingMemberHandling.Error
        };
        result = JsonConvert.DeserializeObject<T>(@this, settings);
        return success;
    }
}
[Serializable]
public class Serialzer<T>
{
    public PlayerPrefsType type;
    [SerializeField]
    public T value;
}
[Serializable]
public class ExportSerialzer
{
    public string key;
    public PlayerPrefsType type;
    public string value;
}
[Serializable]
public class ExportSerialzerHolder
{
    public List<ExportSerialzer> exportlist = new List<ExportSerialzer>();
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
    Bool,   
    DateTime
}
