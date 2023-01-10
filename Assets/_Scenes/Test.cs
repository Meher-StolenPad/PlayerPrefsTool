using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DaVanciInk.AdvancedPlayerPrefs;
public class Test : MonoBehaviour
{
    public bool test;
    public Color ShowAdvancedPlayerPrefsButtonColor;
    public Color ShowAdvancedPlayerPrefsTextColor = new Color(255, 216, 116, 1);

    public Color SetupButtonTextColor = new Color(20, 180, 255, 1);
    public Color SetupButtonButtonColor = new Color(20, 89, 255, 1);
    // Start is called before the first frame update
    void Start()
    {
    
        ShowAdvancedPlayerPrefsButtonColor = new Color32(255, 109, 2, 255);
        test = AdvancedPlayerPrefs.GetBool("TestBool",false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
