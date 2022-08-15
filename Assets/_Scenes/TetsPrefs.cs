using Microsoft.Win32;
using System;
using UnityEditor;
using UnityEngine;

using System.Security.Permissions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

public class TetsPrefs : MonoBehaviour
{
    private enum PlayerPrefsType
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

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("TestAgain", 10.2247745f);
        SetVector3("TestVector3", new Vector3(0.5f, 55, 10));
        SetVector2("TestVector2", new Vector2(0.4f, 0.3f));
        SetVector4("TestVector4", new Vector4(55, 66f,77f,88f));
        SetColor("TestColor", Color.green);
        SetBool("TestBool", true);
    }

    // Update is called once per frame
    void Update()
    {
    
    }
    public void SetVector3(string key, Vector3 _value)
    {
        Serialzer<Vector3> serialzer = new Serialzer<Vector3>();
        serialzer.type = PlayerPrefsType.Vector3;
        serialzer.value = _value;

        var jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        string jsonString = JsonUtility.ToJson(serialzer);

        Debug.Log("string vector 3 ; " + jsonString);

        PlayerPrefs.SetString(key, jsonString);
    }

    public void SetVector2(string key, Vector2 _value)
    {
        Serialzer<Vector2> serialzer = new Serialzer<Vector2>();
        serialzer.type = PlayerPrefsType.Vector2;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        Debug.Log("string Vector 2; " + jsonString);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static void SetColor(string key, Color _value)
    {
        Serialzer<Color> serialzer = new Serialzer<Color>();
        serialzer.type = PlayerPrefsType.Color;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);
        Debug.Log("string color; " + jsonString);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static void SetVector4(string key, Vector4 _value)
    {
        Serialzer<Vector4> serialzer = new Serialzer<Vector4>();
        serialzer.type = PlayerPrefsType.Vector4;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static void SetBool(string key, bool _value)
    {
        Serialzer<bool> serialzer = new Serialzer<bool>();
        serialzer.type = PlayerPrefsType.Bool;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    [Serializable]
    private class Serialzer<T>
    {
        public PlayerPrefsType type;
        public T value;
    }
}   