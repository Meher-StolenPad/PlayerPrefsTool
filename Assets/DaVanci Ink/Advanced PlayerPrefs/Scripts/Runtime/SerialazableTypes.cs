using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal static class SerialzablmeTypes
    {
        internal static Vector3Data[] GetVector3DataArray(this Vector3[] data)
        {   
            Vector3Data[] returnValue = new Vector3Data[data.Length];
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = (Vector3Data)data[i];
            }
            return returnValue;
        }
        internal static Vector3IntData[] GetVector3IntDataArray(this Vector3Int[] data)   
        {
            Vector3IntData[] returnValue = new Vector3IntData[data.Length];
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = (Vector3IntData)data[i];
            }
            return returnValue;
        }
        internal static Vector2Data[] GetVector2DataArray(this Vector2[] data)
        {
            Vector2Data[] returnValue = new Vector2Data[data.Length];
            for (int i = 0; i < returnValue.Length; i++)    
            {
                returnValue[i] = (Vector2Data)data[i];
            }
            return returnValue;
        }
        internal static Vector2IntData[] GetVector2IntDataArray(this Vector2Int[] data)
        {
            Vector2IntData[] returnValue = new Vector2IntData[data.Length];
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = (Vector2IntData)data[i];
            }
            return returnValue;
        }
        internal static Vector4Data[] GetVector4DataArray(this Vector4[] data)
        {
            Vector4Data[] returnValue = new Vector4Data[data.Length];
            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = (Vector4Data)data[i];
            }
            return returnValue;
        }
    }
    [Serializable]
    internal struct Vector3Data
    {
        public float x;
        public float y;
        public float z;
        internal Vector3Data(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static implicit operator Vector3Data(Vector3 x)
        {
            return new Vector3Data(x.x, x.y, x.z);
        }
    }
    [Serializable]
    internal struct Vector3IntData
    {
        public int x;
        public int y;
        public int z;
        public Vector3IntData(int x, int y, int z)
        {
            this.x = x;
            this.y = y; 
            this.z = z;
        }
        public static implicit operator Vector3IntData(Vector3Int x)
        {
            return new Vector3IntData(x.x, x.y, x.z);
        }   
    }
    [Serializable]
    internal struct Vector2IntData
    {
        public int x;
        public int y;
        public Vector2IntData(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2IntData(Vector2Int x)
        {
            return new Vector2IntData(x.x, x.y);
        }
    }
    internal struct Vector2Data
    {
        public float x;
        public float y;
        public Vector2Data(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public static implicit operator Vector2Data(Vector2 x)
        {
            return new Vector2Data(x.x, x.y);
        }
    }
    [Serializable]
    internal struct Vector4Data
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public Vector4Data(float x, float y, float z,float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Vector4Data(Vector4 x)
        {
            return new Vector4Data(x.x, x.y,x.z,x.w);
        }
    }
}

