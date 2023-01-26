using System;
using System.Collections;
using System.Collections.Generic;
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

        public void RefreshField()
        {
            Init();
        }
        public void Init()
        {
            this.hideFlags = HideFlags.DontSave;

            switch (type)
            {
                case PlayerPrefsType.ArrayInt:
                    arrayInt = AdvancedPlayerPrefs.StringToArrayInt(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayInt");
                    break;
                case PlayerPrefsType.ArrayFloat:
                    arrayFloat = AdvancedPlayerPrefs.StringToArrayFloat(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayFloat");
                    break;
                case PlayerPrefsType.ArrayBool:
                    arrayBool = AdvancedPlayerPrefs.StringToArrayBool(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayBool");
                    break;
                case PlayerPrefsType.ArrayByte:
                    arrayByte = AdvancedPlayerPrefs.StringToArrayByte(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayByte");
                    break;
                case PlayerPrefsType.ArrayDouble:
                    arrayDouble = AdvancedPlayerPrefs.StringToArrayDouble(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayDouble");
                    break;
                case PlayerPrefsType.ArrayVector3:
                    arrayVector3 = AdvancedPlayerPrefs.StringToArrayVector3(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayVector3");
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    arrayVector3Int = AdvancedPlayerPrefs.StringToArrayVector3Int(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayVector3Int");
                    break;
                case PlayerPrefsType.ArrayString:
                    arrayString = AdvancedPlayerPrefs.StringToArrayString(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayString");
                    break;
                case PlayerPrefsType.ArrayLong:
                    arrayLong = AdvancedPlayerPrefs.StringToArrayLong(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayLong");
                    break;
                case PlayerPrefsType.ArrayVector2:
                    arrayVector2 = AdvancedPlayerPrefs.StringToArrayVector2(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayVector2");
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    arrayVector2Int = AdvancedPlayerPrefs.StringToArrayVector2Int(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayVector2Int");
                    break;
                case PlayerPrefsType.ArrayVector4:
                    arrayVector4 = AdvancedPlayerPrefs.StringToArrayVector4(Value.ToString());
                    so = new SerializedObject(this);
                    ValueProperty = so.FindProperty("arrayVector4");

                    break;
            }

        }
        public void SetValue()
        {
            switch (type)
            {
                case PlayerPrefsType.ArrayInt:
                    Value = arrayInt;
                    break;
                case PlayerPrefsType.ArrayFloat:
                    Value = arrayFloat;
                    break;
                case PlayerPrefsType.ArrayBool:
                    Value = arrayBool;
                    break;
                case PlayerPrefsType.ArrayByte:
                    Value = arrayByte; 
                    break;
                case PlayerPrefsType.ArrayDouble:
                    Value = arrayDouble;
                    break;
                case PlayerPrefsType.ArrayVector3:
                    Value = arrayVector3;
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    Value = arrayVector3Int;
                    break;
                case PlayerPrefsType.ArrayString:
                    Value = arrayString;
                    break;
                case PlayerPrefsType.ArrayLong:
                    Value = arrayLong;
                    break;
                case PlayerPrefsType.ArrayVector2:
                    Value = arrayVector2;
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    Value = arrayVector2Int;
                    break;
                case PlayerPrefsType.ArrayVector4:
                    Value = arrayVector4;
                    break;
            }
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
                case PlayerPrefsType.Vector3:
                    AdvancedPlayerPrefs.SetVector3(Key, AdvancedPlayerPrefs.StringToVector3(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Vector2:
                    AdvancedPlayerPrefs.SetVector2(Key, AdvancedPlayerPrefs.StringToVector2(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Color:
                    AdvancedPlayerPrefs.SetColor(Key, AdvancedPlayerPrefs.StringToColor(Value.ToString()), false, isEncrypted);
                    break;
                case PlayerPrefsType.HDRColor:
                    AdvancedPlayerPrefs.SetColor(Key, AdvancedPlayerPrefs.StringToColor(Value.ToString()), true, isEncrypted);
                    break;
                case PlayerPrefsType.Vector4:
                    AdvancedPlayerPrefs.SetVector4(Key, AdvancedPlayerPrefs.StringToVector4(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Bool:
                    AdvancedPlayerPrefs.SetBool(Key, AdvancedPlayerPrefs.StringToBool(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.DateTime:
                    if (AdvancedPlayerPrefs.StringToDateTime(Value.ToString()) != null)
                    {
                        AdvancedPlayerPrefs.SetDateTime(Key, ((DateTime)AdvancedPlayerPrefs.StringToDateTime(Value.ToString())), isEncrypted);
                    }
                    break;
                case PlayerPrefsType.Byte:
                    AdvancedPlayerPrefs.SetByte(Key, AdvancedPlayerPrefs.StringToByte(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Double:
                    AdvancedPlayerPrefs.SetDoube(Key, AdvancedPlayerPrefs.StringToDouble(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Long:
                    AdvancedPlayerPrefs.SetLong(Key, AdvancedPlayerPrefs.StringToLong(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Vector2Int:
                    AdvancedPlayerPrefs.SetVector2Int(Key, AdvancedPlayerPrefs.StringToVector2Int(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.Vector3Int:
                    AdvancedPlayerPrefs.SetVector3Int(Key, AdvancedPlayerPrefs.StringToVector3Int(Value.ToString()), isEncrypted);
                    break;
                case PlayerPrefsType.ArrayInt:
                    AdvancedPlayerPrefs.SetArray(Key, arrayInt, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayFloat:
                    AdvancedPlayerPrefs.SetArray(Key, arrayFloat, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayBool:
                    AdvancedPlayerPrefs.SetArray(Key, arrayBool, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayByte:
                    AdvancedPlayerPrefs.SetArray(Key, arrayByte, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayDouble:
                    AdvancedPlayerPrefs.SetArray(Key, arrayDouble, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayVector3:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector3, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector3Int, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayString:
                    AdvancedPlayerPrefs.SetArray(Key, arrayString, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayLong:
                    AdvancedPlayerPrefs.SetArray(Key, arrayLong, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayVector2:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector2, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector2Int, isEncrypted);
                    break;
                case PlayerPrefsType.ArrayVector4:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector4, isEncrypted);
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
                        returnValue = Mathf.Approximately(ic,it);
                    }
                    else
                    {
                        returnValue = float.Parse((TempValue.ToString())) == float.Parse((Value.ToString()));
                    }
                    break;
                case PlayerPrefsType.String:
                    returnValue = String.Equals(TempValue, Value);
                    break;
                case PlayerPrefsType.Vector2:
                    var v2c = AdvancedPlayerPrefs.StringToVector2(Value.ToString());
                    var v2t = AdvancedPlayerPrefs.StringToVector2(TempValue.ToString());
                    returnValue = v2c == v2t;

                    break;
                case PlayerPrefsType.Vector3:
                    var v3c = AdvancedPlayerPrefs.StringToVector3(Value.ToString());
                    var v3t = AdvancedPlayerPrefs.StringToVector3(TempValue.ToString());
                    returnValue = v3c == v3t;
                    break;
                case PlayerPrefsType.Vector4:
                    var v4c = AdvancedPlayerPrefs.StringToVector4(Value.ToString());
                    var v4t = AdvancedPlayerPrefs.StringToVector4(TempValue.ToString());
                    returnValue = v4c == v4t;
                    break;
                case PlayerPrefsType.Color:
                    var cc = AdvancedPlayerPrefs.StringToColor(Value.ToString());
                    var ct = AdvancedPlayerPrefs.StringToColor(TempValue.ToString());
                    returnValue = cc == ct;
                    break;
                case PlayerPrefsType.HDRColor:
                    var hcc = AdvancedPlayerPrefs.StringToColor(Value.ToString());
                    var hct = AdvancedPlayerPrefs.StringToColor(TempValue.ToString());
                    returnValue = hcc == hct;
                    break;
                case PlayerPrefsType.Bool:
                    var bc = AdvancedPlayerPrefs.StringToBool(Value.ToString());
                    var bt = AdvancedPlayerPrefs.StringToBool(TempValue.ToString());
                    returnValue = bc == bt;
                    break;
                case PlayerPrefsType.DateTime:
                    var tc = AdvancedPlayerPrefs.StringToDateTime(Value.ToString());
                    var tt = AdvancedPlayerPrefs.StringToDateTime(TempValue.ToString());
                    returnValue = tc == tt;
                    break;
                case PlayerPrefsType.Byte:
                    var byc = AdvancedPlayerPrefs.StringToByte(Value.ToString());
                    var byt = AdvancedPlayerPrefs.StringToByte(TempValue.ToString());
                    returnValue = byc == byt;
                    break;
                case PlayerPrefsType.Double:
                    var dc = AdvancedPlayerPrefs.StringToDouble(Value.ToString());
                    var dt = AdvancedPlayerPrefs.StringToDouble(TempValue.ToString());
                    returnValue = dc == dt;
                    break;
                case PlayerPrefsType.Long:
                    var lc = AdvancedPlayerPrefs.StringToLong(Value.ToString());
                    var lt = AdvancedPlayerPrefs.StringToLong(TempValue.ToString());
                    returnValue = lc == lt;
                    break;
                case PlayerPrefsType.Vector2Int:
                    var v2ic = AdvancedPlayerPrefs.StringToVector2Int(Value.ToString());
                    var v2it = AdvancedPlayerPrefs.StringToVector2Int(TempValue.ToString());
                    returnValue = v2ic == v2it;
                    break;
                case PlayerPrefsType.Vector3Int:
                    var v3ic = AdvancedPlayerPrefs.StringToVector3Int(Value.ToString());
                    var v3it = AdvancedPlayerPrefs.StringToVector3Int(TempValue.ToString());
                    returnValue = v3ic == v3it;
                    break;
                case PlayerPrefsType.ArrayInt:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayFloat:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayBool:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayByte:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayDouble:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayVector3:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayString:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayLong:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayVector2:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    returnValue = true;
                    break;
                case PlayerPrefsType.ArrayVector4:
                    returnValue = true;
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
