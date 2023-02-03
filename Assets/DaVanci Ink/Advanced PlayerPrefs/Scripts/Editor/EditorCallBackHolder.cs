using UnityEditor;
using UnityEngine;

public static class ProductNameChangedCallback
{
    private static string previousProductName;
    private static string previousCompanyName;

    private static string CurrentCompanyName;   
    private static string CurrentProductName;

    private static float lastCheckTime;     

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        previousProductName = PlayerSettings.productName;
        previousCompanyName = PlayerSettings.companyName;
        EditorApplication.update += CheckForProductNameChange;
    }

    private static void CheckForProductNameChange()
    {
        float currentTime = Time.realtimeSinceStartup;

        if (currentTime - lastCheckTime >= 6.0f)
        {
            CurrentProductName = PlayerSettings.productName;
            CurrentCompanyName = PlayerSettings.companyName;

            if (!previousProductName.Equals(CurrentProductName) || !previousCompanyName.Equals(CurrentCompanyName))
            {
                // Perform your desired action, such as logging the new product name
                Debug.Log("Product name changed to: " + CurrentProductName);
                previousProductName = CurrentProductName;
                previousCompanyName = CurrentCompanyName;   
            }

            lastCheckTime = currentTime;
        }
    }
}