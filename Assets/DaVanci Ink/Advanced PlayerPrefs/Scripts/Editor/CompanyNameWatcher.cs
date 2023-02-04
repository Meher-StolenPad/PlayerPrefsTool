using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CompanyNameChangeDetector : Editor
{
    private static string companyName;

    static CompanyNameChangeDetector()
    {
        companyName = PlayerSettings.companyName;
        EditorApplication.projectChanged += CheckCompanyNameChange;
    }

    private static void CheckCompanyNameChange()
    {
        if (PlayerSettings.companyName != companyName)
        {
            companyName = PlayerSettings.companyName;
            OnCompanyNameChanged();
        }
    }

    private static void OnCompanyNameChanged()
    {
        Debug.Log("Company name changed to: " + companyName);
    }
}
