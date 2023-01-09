using System;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class TestPrefs : MonoBehaviour
    {
        [ColorUsage(true,true)]
        public Color m_Color;
        public string testEncription; 
        public string CriptedEncription;

        public string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0"; //set any string of 32 chars
        public string iv = "1234567887654321"; //set any string of 16 chars

      
        private void Start()
        {
            //PrefsSerialzer.SetFloat("TestAgain", 10.2247745f,true);
            //PrefsSerialzer.SetVector3("TestVector3", new Vector3(0.5f, 55, 10), true);
            //PrefsSerialzer.SetVector2("TestVector2", new Vector2(0.4f, 0.3f), true);
            //PrefsSerialzer.SetVector4("TestVector4", new Vector4(55, 66f, 77f, 88f), true);
            //PrefsSerialzer.SetColor("TestColor", Color.green,false, true);
            //PrefsSerialzer.SetColor("TestColorHDR", Color.blue,true, true);
            //PrefsSerialzer.SetBool("TestBool", true, true);
            //PrefsSerialzer.SetDateTime("TestDateTime", DateTime.Now, true);
            //PrefsSerialzer.SetByte("TestByte", 5,true);
            //PrefsSerialzer.SetDoube("TestDouble", 54.444484, true);
            //PrefsSerialzer.SetVector2Int("TestVector2Int", new Vector2Int(5,6), true);
            //PrefsSerialzer.SetVector3Int("TestVector3Int", new Vector3Int(5,6,7), true);

            Debug.Log("Float " + PrefsSerialzer.GetFloat("TestAgain", 1f));
            Debug.Log("Float " + PrefsSerialzer.GetFloat("TestAgaidddn", 1f));
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
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                CriptedEncription = PrefsSerialzer.Encryption(testEncription);
                Debug.Log(CriptedEncription);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log(PrefsSerialzer.Decryption(CriptedEncription));
            }
        }
      
    }
}