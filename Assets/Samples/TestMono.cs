using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    [ContextMenuItem("TT", "ClearData")]
    public string T;

    public void ClearData()
    {
        Debug.Log("Data cleared.");
    }
}