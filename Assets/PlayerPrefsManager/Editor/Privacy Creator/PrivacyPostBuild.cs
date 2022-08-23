using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
public class PrivacyPostBuild : MonoBehaviour
{

    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
       if(target == BuildTarget.Android)
        {
            PrivacyCreatorWizard wizard = ScriptableWizard.DisplayWizard<PrivacyCreatorWizard>("Add Privacy");
            Debug.Log(pathToBuiltProject);
        }
    }

}
