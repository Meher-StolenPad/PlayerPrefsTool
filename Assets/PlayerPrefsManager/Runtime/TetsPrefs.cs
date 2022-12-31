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
    public Color color;
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
        PrefsSerialzer.SetVector3("TestVector3", new Vector3(0.5f, 55, 10));
        PrefsSerialzer.SetVector2("TestVector2", new Vector2(0.4f, 0.3f));
        PrefsSerialzer.SetVector4("TestVector4", new Vector4(55, 66f,77f,88f));
        PrefsSerialzer.SetColor("TestColor", Color.green);
        PrefsSerialzer.SetBool("TestBool", true);

        Debug.Log("Vector 2 " + PrefsSerialzer.GetVector2("TestVector2", Vector2.zero));
        Debug.Log("Vector 3 " + PrefsSerialzer.GetVector3("TestVector3", Vector3.zero));
        Debug.Log("Vector 4 " + PrefsSerialzer.GetVector4("TestVector4", Vector4.zero));
        Debug.Log("Color " + PrefsSerialzer.GetColor("TestColor", Color.white));
        Debug.Log("bool " + PrefsSerialzer.GetBool("TestBool", false));
        color = PrefsSerialzer.GetColor("TestColor", Color.white);
        //  Debug.Log("Vector 2 " + PrefsSerialzer.GetVector2("TestVector2"));
    }
    private void Update()
    {
      
    }
}   