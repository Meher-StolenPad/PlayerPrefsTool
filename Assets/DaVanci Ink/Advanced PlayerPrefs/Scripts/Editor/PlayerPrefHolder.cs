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
                    // BackupValues = arrayFloat;
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
                    Value = JsonConvert.SerializeObject(arrayInt);
                    break;
                case PlayerPrefsType.ArrayFloat:
                    Value = JsonConvert.SerializeObject(arrayFloat);
                    break;
                case PlayerPrefsType.ArrayBool:
                    Value = JsonConvert.SerializeObject(arrayBool);
                    break;
                case PlayerPrefsType.ArrayByte:
                    Value = JsonConvert.SerializeObject(arrayByte.Select(b => (int)b).ToArray());
                    break;
                case PlayerPrefsType.ArrayDouble:
                    Value = JsonConvert.SerializeObject(arrayDouble);
                    break;
                case PlayerPrefsType.ArrayVector3:
                    Value = JsonConvert.SerializeObject(arrayVector3.GetVector3DataArray());
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    Value = JsonConvert.SerializeObject(arrayVector3Int.GetVector3IntDataArray());
                    break;
                case PlayerPrefsType.ArrayString:
                    Value = JsonConvert.SerializeObject(arrayString);
                    break;
                case PlayerPrefsType.ArrayLong:
                    Value = JsonConvert.SerializeObject(arrayLong);
                    break;
                case PlayerPrefsType.ArrayVector2:
                    Value = JsonConvert.SerializeObject(arrayVector2.GetVector2DataArray());
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    Value = JsonConvert.SerializeObject(arrayVector2Int.GetVector2IntDataArray());
                    break;
                case PlayerPrefsType.ArrayVector4:
                    Value = JsonConvert.SerializeObject(arrayVector4.GetVector4DataArray());
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
                    TempValue = arrayInt;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayFloat:
                    AdvancedPlayerPrefs.SetArray(Key, arrayFloat, isEncrypted);
                    TempValue = arrayFloat;
                    BackupValues = TempValue;
                    break;

                case PlayerPrefsType.ArrayBool:
                    AdvancedPlayerPrefs.SetArray(Key, arrayBool, isEncrypted);
                    TempValue = arrayBool;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayByte:
                    AdvancedPlayerPrefs.SetArray(Key, arrayByte, isEncrypted);
                    TempValue = arrayByte;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayDouble:
                    AdvancedPlayerPrefs.SetArray(Key, arrayDouble, isEncrypted);
                    TempValue = arrayDouble;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayVector3:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector3, isEncrypted);
                    TempValue = arrayVector3;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector3Int, isEncrypted);
                    TempValue = arrayVector3Int;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayString:
                    AdvancedPlayerPrefs.SetArray(Key, arrayString, isEncrypted);
                    TempValue = arrayString;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayLong:
                    AdvancedPlayerPrefs.SetArray(Key, arrayLong, isEncrypted);
                    TempValue = arrayLong;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayVector2:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector2, isEncrypted);
                    TempValue = arrayVector2;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector2Int, isEncrypted);
                    TempValue = arrayVector2Int;
                    BackupValues = TempValue;

                    break;
                case PlayerPrefsType.ArrayVector4:
                    AdvancedPlayerPrefs.SetArray(Key, arrayVector4, isEncrypted);
                    TempValue = arrayVector4;
                    BackupValues = TempValue;

                    break;
                default:
                    break;
            }
        }
        public void BackUp()
        {
            GUI.FocusControl(null);
            TempValue = BackupValues;
            switch (type)
            {
                case PlayerPrefsType.ArrayInt:
                    if (BackupValues as int[] == null)
                    {
                        arrayInt = AdvancedPlayerPrefs.StringToArrayInt(BackupValues.ToString());
                    }
                    else
                    {
                        arrayInt = BackupValues as int[];
                    }
                    so.ApplyModifiedProperties();

                    break;
                case PlayerPrefsType.ArrayFloat:

                    if (BackupValues as float[] == null)
                    {
                        arrayFloat = AdvancedPlayerPrefs.StringToArrayFloat(BackupValues.ToString());
                    }
                    else
                    {
                        arrayFloat = BackupValues as float[];
                    }
                    so.ApplyModifiedProperties();
                    break;
                case PlayerPrefsType.ArrayBool:
                    if (BackupValues as bool[] == null)
                    {
                        arrayBool = AdvancedPlayerPrefs.StringToArrayBool(BackupValues.ToString());
                    }
                    else
                    {
                        arrayBool = BackupValues as bool[];
                    }
                    so.ApplyModifiedProperties();
                    break;
                case PlayerPrefsType.ArrayByte:
                    if (BackupValues as byte[] == null)
                    {
                        arrayByte = AdvancedPlayerPrefs.StringToArrayByte(BackupValues.ToString());
                    }
                    else
                    {
                        arrayByte = BackupValues as byte[];
                    }
                    so.ApplyModifiedProperties();
                    break;
                case PlayerPrefsType.ArrayDouble:
                    if (BackupValues as double[] == null)
                    {
                        arrayDouble = AdvancedPlayerPrefs.StringToArrayDouble(BackupValues.ToString());
                    }
                    else
                    {
                        arrayDouble = BackupValues as double[];
                    }
                    so.ApplyModifiedProperties();

                    break;
                case PlayerPrefsType.ArrayVector3:
                    if (BackupValues as Vector3[] == null)
                    {
                        arrayVector3 = AdvancedPlayerPrefs.StringToArrayVector3(BackupValues.ToString());
                    }
                    else
                    {
                        arrayVector3 = BackupValues as Vector3[];
                    }
                    so.ApplyModifiedProperties();

                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    if (BackupValues as Vector3Int[] == null)
                    {
                        arrayVector3Int = AdvancedPlayerPrefs.StringToArrayVector3Int(BackupValues.ToString());
                    }
                    else
                    {
                        arrayVector3Int = BackupValues as Vector3Int[];
                    }
                    so.ApplyModifiedProperties();

                    break;
                case PlayerPrefsType.ArrayString:
                    if (BackupValues as string[] == null)
                    {
                        arrayString = AdvancedPlayerPrefs.StringToArrayString(BackupValues.ToString());
                    }
                    else
                    {
                        arrayString = BackupValues as string[];
                    }
                    so.ApplyModifiedProperties();
                    break;
                case PlayerPrefsType.ArrayLong:
                    if (BackupValues as long[] == null)
                    {
                        arrayLong = AdvancedPlayerPrefs.StringToArrayLong(BackupValues.ToString());
                    }
                    else
                    {
                        arrayLong = BackupValues as long[];
                    }
                    so.ApplyModifiedProperties();

                    break;
                case PlayerPrefsType.ArrayVector2:
                    if (BackupValues as Vector2[] == null)
                    {
                        arrayVector2 = AdvancedPlayerPrefs.StringToArrayVector2(BackupValues.ToString());
                    }
                    else
                    {
                        arrayVector2 = BackupValues as Vector2[];
                    }
                    so.ApplyModifiedProperties();

                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    if (BackupValues as Vector2Int[] == null)
                    {
                        arrayVector2Int = AdvancedPlayerPrefs.StringToArrayVector2Int(BackupValues.ToString());
                    }
                    else
                    {
                        arrayVector2Int = BackupValues as Vector2Int[];
                    }
                    so.ApplyModifiedProperties();
                    break;
                case PlayerPrefsType.ArrayVector4:
                    if (BackupValues as Vector4[] == null)
                    {
                        arrayVector4 = AdvancedPlayerPrefs.StringToArrayVector4(BackupValues.ToString());
                    }
                    else
                    {
                        arrayVector4 = BackupValues as Vector4[];
                    }
                    so.ApplyModifiedProperties();
                    break;
            }
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
                    returnValue = hcc.Equals(hct);
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
                    if (TempValue as int[] == null)
                    {
                        returnValue = arrayInt.SequenceEqual(AdvancedPlayerPrefs.StringToArrayInt(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayInt.SequenceEqual(TempValue as int[]);
                    }
                    break;
                case PlayerPrefsType.ArrayFloat:
                    if (TempValue as float[] == null)
                    {
                        returnValue = arrayFloat.SequenceEqual(AdvancedPlayerPrefs.StringToArrayFloat(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayFloat.SequenceEqual(TempValue as float[]);
                    }
                    break;
                case PlayerPrefsType.ArrayBool:
                    if (TempValue as bool[] == null)
                    {
                        returnValue = arrayBool.SequenceEqual(AdvancedPlayerPrefs.StringToArrayBool(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayBool.SequenceEqual(TempValue as bool[]);
                    }
                    break;
                case PlayerPrefsType.ArrayByte:
                    if (TempValue as byte[] == null)
                    {
                        returnValue = arrayByte.SequenceEqual(AdvancedPlayerPrefs.StringToArrayByte(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayByte.SequenceEqual(TempValue as byte[]);
                    }
                    break;
                case PlayerPrefsType.ArrayDouble:
                    if (TempValue as double[] == null)
                    {
                        returnValue = arrayDouble.SequenceEqual(AdvancedPlayerPrefs.StringToArrayDouble(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayDouble.SequenceEqual(TempValue as double[]);
                    }
                    break;
                case PlayerPrefsType.ArrayVector3:
                    if (TempValue as Vector3[] == null)
                    {
                        returnValue = arrayVector3.SequenceEqual(AdvancedPlayerPrefs.StringToArrayVector3(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayVector3.SequenceEqual(TempValue as Vector3[]);
                    }
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    if (TempValue as Vector3Int[] == null)
                    {
                        returnValue = arrayVector3Int.SequenceEqual(AdvancedPlayerPrefs.StringToArrayVector3Int(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayVector3Int.SequenceEqual(TempValue as Vector3Int[]);
                    }
                    break;
                case PlayerPrefsType.ArrayString:
                    if (TempValue as string[] == null)
                    {
                        returnValue = arrayString.SequenceEqual(AdvancedPlayerPrefs.StringToArrayString(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayString.SequenceEqual(TempValue as string[]);
                    }
                    break;
                case PlayerPrefsType.ArrayLong:
                    if (TempValue as long[] == null)
                    {
                        returnValue = arrayLong.SequenceEqual(AdvancedPlayerPrefs.StringToArrayLong(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayLong.SequenceEqual(TempValue as long[]);
                    }
                    break;
                case PlayerPrefsType.ArrayVector2:
                    if (TempValue as Vector2[] == null)
                    {
                        returnValue = arrayVector2.SequenceEqual(AdvancedPlayerPrefs.StringToArrayVector2(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayVector2.SequenceEqual(TempValue as Vector2[]);
                    }
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    if (TempValue as Vector2Int[] == null)
                    {
                        returnValue = arrayVector2Int.SequenceEqual(AdvancedPlayerPrefs.StringToArrayVector2Int(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayVector2Int.SequenceEqual(TempValue as Vector2Int[]);
                    }
                    break;
                case PlayerPrefsType.ArrayVector4:
                    if (TempValue as Vector4[] == null)
                    {
                        returnValue = arrayVector4.SequenceEqual(AdvancedPlayerPrefs.StringToArrayVector4(TempValue.ToString()));
                    }
                    else
                    {
                        returnValue = arrayVector4.SequenceEqual(TempValue as Vector4[]);
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
