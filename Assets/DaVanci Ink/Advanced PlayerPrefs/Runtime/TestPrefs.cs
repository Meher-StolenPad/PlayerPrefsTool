using DaVanciInk.AdvancedPlayerPrefs;
using System;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class TestPrefs : MonoBehaviour
    {
        [ColorUsage(true,true)]
        public Color m_Color;

        private void Start()
        {
            PlayerPrefs.SetFloat("TestAgain", 10.2247745f);
            PrefsSerialzer.SetVector3("TestVector3", new Vector3(0.5f, 55, 10));
            PrefsSerialzer.SetVector2("TestVector2", new Vector2(0.4f, 0.3f));
            PrefsSerialzer.SetVector4("TestVector4", new Vector4(55, 66f, 77f, 88f));
            PrefsSerialzer.SetColor("TestColor", Color.green,false);
            PrefsSerialzer.SetColor("TestColorHDR", Color.blue,true);
            PrefsSerialzer.SetBool("TestBool", true);
            PrefsSerialzer.SetDateTime("TestDateTime", DateTime.Now);
            PrefsSerialzer.SetByte("TestByte", 5);
            PrefsSerialzer.SetDoube("TestDouble", 54.444484);
            PrefsSerialzer.SetVector2Int("TestVector2Int", new Vector2Int(5,6));
            PrefsSerialzer.SetVector3Int("TestVector3Int", new Vector3Int(5,6,7));

            Debug.Log("Vector 2 " + PrefsSerialzer.GetVector2("TestVector2", Vector2.zero));
            Debug.Log("Vector 3 " + PrefsSerialzer.GetVector3("TestVector3", Vector3.zero));
            Debug.Log("Vector 4 " + PrefsSerialzer.GetVector4("TestVector4", Vector4.zero));
            Debug.Log("Color " + PrefsSerialzer.GetColor("TestColor", Color.white,false));;
            Debug.Log("bool " + PrefsSerialzer.GetBool("TestBool", false));
            Debug.Log("byte " + PrefsSerialzer.GetByte("TestByte", 15));
            Debug.Log("double " + PrefsSerialzer.GetDouble("TestDouble", 15));
            Debug.Log("Vector2Int " + PrefsSerialzer.GetVector2Int("TestVector2Int", Vector2Int.zero));
            Debug.Log("Vector3Int " + PrefsSerialzer.GetVector3Int("TestVector3Int", Vector3Int.zero));

            m_Color = PrefsSerialzer.GetColor("TestColor", Color.white,false);
        }
    }
}