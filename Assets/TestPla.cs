using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPla : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetFloat("Testfloat", 5f);
        PlayerPrefs.SetString("TestString", "blabla");
        PlayerPrefs.SetInt("testInt", 50);
    }
}
