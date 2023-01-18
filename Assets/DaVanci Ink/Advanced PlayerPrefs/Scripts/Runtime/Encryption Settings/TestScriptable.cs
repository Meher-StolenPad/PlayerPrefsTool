using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[CreateAssetMenu(fileName ="test",menuName ="Test",order = 5)]
public class TestScriptable : ScriptableObject
{
    public int[] array;
    
}
[Serializable]
public class testtest
{
    public int te;
    public float ti;
    public GameObject GameObject;
    public Color color;
    public void test()
    {
     //   SerializedObject o = new SerializedObject(this);
    }
}
