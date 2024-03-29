using System;
using System.Collections.Generic;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal class TestPrefs : MonoBehaviour
    {
        public Color m_Color = Color.yellow;
        public string testEncription; 
        public string CriptedEncription;

        public string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0"; //set any string of 32 chars
        public string iv = "1234567887654321"; //set any string of 16 chars

      
        private void Start()
        {
            Debug.LogWarning("dpzk^pdkzpdpzd$^lz");
            Debug.Log("poidpoid");
            AdvancedPlayerPrefs.SetFloat("TestAgain", 10.2247745f,true);
            AdvancedPlayerPrefs.SetVector3("TestVector3", new Vector3(0.5f, 55, 10), true);
            AdvancedPlayerPrefs.SetVector2("TestVector2", new Vector2(0.4f, 0.3f), true);
            AdvancedPlayerPrefs.SetVector4("TestVector4", new Vector4(55, 66f, 77f, 88f), true);
            AdvancedPlayerPrefs.SetColor("TestColor", Color.green,false, true);
            AdvancedPlayerPrefs.SetColor("TestColorHDR", Color.blue,true, true);
            AdvancedPlayerPrefs.SetBool("TestBool", true, true);
            AdvancedPlayerPrefs.SetDateTime("TestDateTime", DateTime.Now, true);
            AdvancedPlayerPrefs.SetByte("TestByte", 5,true);
            AdvancedPlayerPrefs.SetDoube("TestDouble", 54.444484, true);
            AdvancedPlayerPrefs.SetVector2Int("TestVector2Int", new Vector2Int(5,6), true);
            AdvancedPlayerPrefs.SetVector3Int("TestVector3Int", new Vector3Int(5,6,7), true);
            AdvancedPlayerPrefs.SetArray("TestArrayInt", new int[4] {1,2,3,4}, true);
            AdvancedPlayerPrefs.SetArray("TestArraybyte", new byte[2] {1,2}, true);
            AdvancedPlayerPrefs.SetArray("TestArrayDouble", new double[2] {1,2}, true);
            AdvancedPlayerPrefs.SetArray("TestArrayVector3", new Vector3[2] {Vector3.zero,Vector3.up}, true);
            AdvancedPlayerPrefs.SetArray("testintint", new int[2] {0, 55}, true);
            AdvancedPlayerPrefs.SetArray("testStringArray", new string[2] {"test","Test2"}, true);
            AdvancedPlayerPrefs.SetLong("testLong", 55, true);
            AdvancedPlayerPrefs.SetArray("test Vector2 Array", new Vector2[2] { Vector2.down, Vector2.left}, true);
            AdvancedPlayerPrefs.SetArray("test Vector2Int Array", new Vector2Int[2] { Vector2Int.down, Vector2Int.left }, true);
            AdvancedPlayerPrefs.SetArray("test Vector4Int Array", new Vector4[2] { Vector4.one, Vector4.zero }, true);
            //AdvancedPlayerPrefs.SetLong("testLong", 55, true);
            //    AdvancedPlayerPrefs.SetArray("TestArrayBool", new bool[4] {true,false, true, false }, true);

            //AdvancedPlayerPrefs.SetArray("TestArrayFloat", new float[4] {1.1f,2.2f,3.3f,4.4f}, true);

            Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgain"));
            Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgaidddn", 1f));
            Debug.Log("Vector 2 " + AdvancedPlayerPrefs.GetVector2("TestVector2", Vector2.zero));
            Debug.Log("Vector 3 " + AdvancedPlayerPrefs.GetVector3("TestVector3", Vector3.zero));
            Debug.Log("Vector 4 " + AdvancedPlayerPrefs.GetVector4("TestVector4", Vector4.zero));
            Debug.Log("Color " + AdvancedPlayerPrefs.GetColor("TestColor", Color.white,false));;
            Debug.Log("bool " + AdvancedPlayerPrefs.GetBool("TestBool", false));    
            Debug.Log("byte " + AdvancedPlayerPrefs.GetByte("TestByte", 15));
            Debug.Log("double " + AdvancedPlayerPrefs.GetDouble("TestDouble", 15));
            Debug.Log("Vector2Int " + AdvancedPlayerPrefs.GetVector2Int("TestVector2Int", Vector2Int.zero));
            Debug.Log("Vector3Int " + AdvancedPlayerPrefs.GetVector3Int("TestVector3Int", Vector3Int.zero));
            Debug.Log("testLong " + AdvancedPlayerPrefs.Getlong("testLong"));

            List<Vector3Int> t = AdvancedPlayerPrefs.GetList("TestArrayVector3Int", new List<Vector3Int>());

            foreach (var item in t)
            {
                Debug.Log("array  item : " + item);

            }
            var tt = AdvancedPlayerPrefs.GetList("TestArrayBool", new List<bool>());

            foreach (var item in tt)
            {
                Debug.Log("array  bool item : " + item);

            }
            AdvancedPlayerPrefs.SetList("TestArrayInt", t, true);

            //m_Color = AdvancedPlayerPrefs.GetColor("TestColor", Color.white,false);
        }
        public void Update()
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
               // outVector3 = JsonUtility.FromJson<Vector3[]>(" "+"{"x":0.0,"y":0.0,"z":0.0},{"x":0.0,"y":1.0,"z":0.0}");
                Debug.Log(CriptedEncription);
            }
            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //    Debug.Log(AdvancedPlayerPrefs.Decryption(CriptedEncription));
            //}
        }
      
    }
}