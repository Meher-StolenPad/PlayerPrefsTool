using System;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal class TestPrefs : MonoBehaviour
    {
        [ColorUsage(true,true)]
        public Color m_Color;
        public string testEncription; 
        public string CriptedEncription;

        public string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0"; //set any string of 32 chars
        public string iv = "1234567887654321"; //set any string of 16 chars

      
        private void Start()
        {
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
            //Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgain", 1f));
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

            m_Color = AdvancedPlayerPrefs.GetColor("TestColor", Color.white,false);
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CriptedEncription = AdvancedPlayerPrefs.Encryption(testEncription);
                Debug.Log(CriptedEncription);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log(AdvancedPlayerPrefs.Decryption(CriptedEncription));
            }
        }
      
    }
}