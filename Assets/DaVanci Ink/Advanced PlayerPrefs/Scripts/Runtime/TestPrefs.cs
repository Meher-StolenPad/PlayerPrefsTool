using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal class TestPrefs : MonoBehaviour
    {
        public Color m_Color = Color.yellow;
        public string testEncription;
        public string CriptedEncription;

        public string key; //set any string of 32 chars
        public string iv = "1234567887654321"; //set any string of 16 chars
        public int CoinsCount;

        public float TraveledDistance { get; private set; }
        public string PlayerName { get; private set; }

        private void Start()
        {








            // Set a float value to the "TraveledDistance" key in AdvancedPlayerPrefs with encryption disabled.
            AdvancedPlayerPrefs.SetFloat("TraveledDistance", 222.22f, false);

            // Set a int value to the "CoinsCount" key in AdvancedPlayerPrefs with encryption enabled.
            AdvancedPlayerPrefs.SetInt("CoinsCount", CoinsCount, true);

            // Save a string value with the key "PlayerName" to the PlayerPrefs using AdvancedPlayerPrefs.
            AdvancedPlayerPrefs.SetString("PlayerName", "Walter White");

            // Retrieve the integer value with the key "CoinsCount" from the PlayerPrefs using AdvancedPlayerPrefs.
            CoinsCount = AdvancedPlayerPrefs.GetInt("CoinsCount");

            // Retrieve the float value with the key "TraveledDistance" from the PlayerPrefs using AdvancedPlayerPrefs.
            TraveledDistance = AdvancedPlayerPrefs.GetFloat("TraveledDistance");

            // Retrieve the string value with the key "PlayerName" from the PlayerPrefs using AdvancedPlayerPrefs.
            PlayerName = AdvancedPlayerPrefs.GetString("PlayerName");





















            // Set a float value to the "TraveledDistance" key in AdvancedPlayerPrefs with encryption disabled.
            AdvancedPlayerPrefs.SetFloat("TraveledDistance", 222.22f, false);

            // Set a Vector3 value to the "CoinsCount" key in AdvancedPlayerPrefs with encryption enabled .
            AdvancedPlayerPrefs.SetInt("CoinsCount", CoinsCount, true);

            // Set a boolean value to the "PlayerName" key in AdvancedPlayerPrefs
            AdvancedPlayerPrefs.SetString("PlayerName", "Walter White");

            // Retrieve the boolean value associated with the "NoAds" key from the player prefs
            bool NoAds = AdvancedPlayerPrefs.GetBool("NoAds");

            // Create a new list of 7 bool values to represent daily login status
            List<bool> DailyLogin = new List<bool>(7);

            // Save the list to player prefs using AdvancedPlayerPrefs, with encryption enabled
            AdvancedPlayerPrefs.SetList("DailyLogin", DailyLogin, true);

            // Retrieve the list from player prefs using AdvancedPlayerPrefs
            DailyLogin = AdvancedPlayerPrefs.GetList<bool>("DailyLogin");


            // Set a float value to the "TraveledDistance" key in AdvancedPlayerPrefs with encryption disabled.
            AdvancedPlayerPrefs.SetFloat("TraveledDistance", 222.22f, false);


            AdvancedPlayerPrefs.SetVector2("TestVector2", new Vector2(0.4f, 0.3f));
            AdvancedPlayerPrefs.SetVector4("TestVector4", new Vector4(55, 66f, 77f, 88f));
            AdvancedPlayerPrefs.SetColor("TestColor", Color.red, false);
            AdvancedPlayerPrefs.SetColor("TestColorHDR", Color.blue, true);
            AdvancedPlayerPrefs.SetDateTime("TestDateTime", DateTime.Now);
            AdvancedPlayerPrefs.SetByte("TestByte", 5);
            AdvancedPlayerPrefs.SetDoube("TestDouble", 54.444484, true);
            AdvancedPlayerPrefs.SetVector2Int("TestVector2Int", new Vector2Int(5, 6), true);
            AdvancedPlayerPrefs.SetVector3Int("TestVector3Int", new Vector3Int(5, 6, 7), true);
            AdvancedPlayerPrefs.SetArray("TestArrayInt", new int[4] { 1, 2, 3, 4 }, true);
            AdvancedPlayerPrefs.SetArray("TestArraybyte", new byte[2] { 1, 2 }, true);
            AdvancedPlayerPrefs.SetArray("TestArrayDouble", new double[2] { 1, 2 }, true);
            AdvancedPlayerPrefs.SetArray("TestArrayVector3", new Vector3[2] { Vector3.zero, Vector3.up }, true);
            AdvancedPlayerPrefs.SetArray("testintint", new int[2] { 0, 55 }, true);
            AdvancedPlayerPrefs.SetArray("testStringArray", new string[2] { "test", "Test2" }, true);
            AdvancedPlayerPrefs.SetLong("testLong", 55, true);
            AdvancedPlayerPrefs.SetArray("test Vector2 Array", new Vector2[2] { Vector2.down, Vector2.left }, true);
            AdvancedPlayerPrefs.SetArray("test Vector2Int Array", new Vector2Int[2] { Vector2Int.down, Vector2Int.left }, true);
            AdvancedPlayerPrefs.SetArray("test Vector4Int Array", new Vector4[2] { Vector4.one, Vector4.zero }, true);
            AdvancedPlayerPrefs.SetLong("testLong", 55, true);
            AdvancedPlayerPrefs.SetArray("TestArrayBool", new bool[4] { true, false, true, false }, true);
            AdvancedPlayerPrefs.SetArray("TestArrayFloat", new float[4] { 1.1f, 2.2f, 3.3f, 4.4f }, true);
            AdvancedPlayerPrefs.SetList("TestArrayVector4", new List<Vector4>() { Vector4.zero, Vector4.one }, true);
            DailyLogin.Add(NoAds);

            //Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgain"));
            //Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgaidddn", 1f));
            //Debug.Log("Vector 2 " + AdvancedPlayerPrefs.GetVector2("TestVector2", Vector2.zero));
            //Debug.Log("Vector 3 " + AdvancedPlayerPrefs.GetVector3("TestVector3", Vector3.zero));
            //Debug.Log("Vector 4 " + AdvancedPlayerPrefs.GetVector4("TestVector4", Vector4.zero));
            //Debug.Log("Color " + AdvancedPlayerPrefs.GetColor("TestColor", Color.white,false));;
            //Debug.Log("bool " + AdvancedPlayerPrefs.GetBool("TestBool", false));    
            //Debug.Log("byte " + AdvancedPlayerPrefs.GetByte("TestByte", 15));
            //Debug.Log("double " + AdvancedPlayerPrefs.GetDouble("TestDouble", 15));
            //Debug.Log("Vector2Int " + AdvancedPlayerPrefs.GetVector2Int("TestVector2Int", Vector2Int.zero));
            //Debug.Log("Vector3Int " + AdvancedPlayerPrefs.GetVector3Int("TestVector3Int", Vector3Int.zero));
            //Debug.Log("testLong " + AdvancedPlayerPrefs.Getlong("testLong"));

            //List<Vector3Int> t = AdvancedPlayerPrefs.GetList("TestArrayVector3Int", new List<Vector3Int>());

            //foreach (var item in t)
            //{
            //    Debug.Log("array  item : " + item);

            //}
            //var tt = AdvancedPlayerPrefs.GetList("TestArrayBool", new List<bool>());

            //foreach (var item in tt)
            //{
            //    Debug.Log("array  bool item : " + item);

            //}
            //AdvancedPlayerPrefs.SetList("TestArrayInt", t, true);

            //m_Color = AdvancedPlayerPrefs.GetColor("TestColor", Color.white,false);
        }
        public void Update()
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log(AdvancedPlayerPrefsSettings.Instance.Key);

            }
            if (Input.GetKeyDown(KeyCode.D))
            {
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log(key);
            }
        }
    }
}