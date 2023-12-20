using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum FoundInSearch
    {
        None,
        Key,
        Value
    }
    internal class PlayerPrefHolder : ScriptableObject
    {
        public string Key;
        public string TempKey;

        public object Value;
        public object TempValue;

        public object BackupValues;

        public PlayerPrefsType type;

        public FoundInSearch InSearch;
        public bool isEncrypted = false;
        public bool Pinned = true;
        public SerializedProperty ValueProperty;
        public SerializedObject so;

        public object[] ArrayHolder;

        public int[] arrayInt;
        public float[] arrayFloat;
        public string[] arrayString;
        public bool[] arrayBool;
        public byte[] arrayByte;
        public double[] arrayDouble;
        public long[] arrayLong;
        public Vector3[] arrayVector3;
        public Vector3Int[] arrayVector3Int;
        public Vector2Int[] arrayVector2Int;
        public Vector2[] arrayVector2;
        public Vector4[] arrayVector4 = null;
        public ushort originalIndex; 
        public void RefreshField()
        {
            Init();
        }
        public void Init()
        {
            this.hideFlags = HideFlags.DontSave;
        }
        public void SaveKey()
        {
            if (Key != TempKey)
            {
                PlayerPrefs.DeleteKey(Key);
                Key = TempKey;
            }
            Save();
        }
        public void Save()
        {
            BackupValues = Value;

            Value = TempValue;

            switch (type)
            {
                case PlayerPrefsType.Int:
                    AdvancedPlayerPrefs.SetInt(Key, (int)Value, isEncrypted);
                    break;
                case PlayerPrefsType.Float:
                    AdvancedPlayerPrefs.SetFloat(Key, Value.ToString(), isEncrypted);
                    break;
                case PlayerPrefsType.String:
                    AdvancedPlayerPrefs.SetString(Key, Value.ToString(), isEncrypted);
                    break;
            
                case PlayerPrefsType.Byte:
                    AdvancedPlayerPrefs.SetByte(Key, AdvancedPlayerPrefs.StringToByte(Value.ToString()), isEncrypted);
                    break;
                default:
                    break;
            }
        }
        public void BackUp()
        {
            GUI.FocusControl(null);
            TempValue = BackupValues;
        }
        public void Delete()
        {
            PlayerPrefs.DeleteKey(Key);
        }
        public bool IsEqual()
        {
            bool returnValue = false;

            switch (type)
            {
                case PlayerPrefsType.Int:
                    if (isEncrypted)
                    {
                        var ic = AdvancedPlayerPrefs.StringToInt(Value.ToString());
                        var it = AdvancedPlayerPrefs.StringToInt(TempValue.ToString());
                        returnValue = ic == it;
                    }
                    else
                    {
                        returnValue = (int)TempValue == (int)Value;
                    }
                    break;
                case PlayerPrefsType.Float:
                    if (isEncrypted)
                    {
                        var ic = AdvancedPlayerPrefs.StringToFloat(Value.ToString());
                        var it = AdvancedPlayerPrefs.StringToFloat(TempValue.ToString());
                        returnValue = Mathf.Approximately(ic, it);
                    }
                    else
                    {
                        returnValue = float.Parse((TempValue.ToString())) == float.Parse((Value.ToString()));
                    }
                    break;
                case PlayerPrefsType.String:
                    returnValue = String.Equals(TempValue, Value);
                    break;  
                case PlayerPrefsType.Byte:
                    var byc = AdvancedPlayerPrefs.StringToByte(Value.ToString());
                    var byt = AdvancedPlayerPrefs.StringToByte(TempValue.ToString());
                    returnValue = byc == byt;
                    break;
               
                default:
                    break;
            }
            return returnValue;
        }
    }
}
