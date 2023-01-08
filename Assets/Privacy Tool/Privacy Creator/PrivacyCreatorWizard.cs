using DaVanciInk.AdvancedPlayerPrefs;
using Facebook.Unity.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrivacyCreatorWizard : ScriptableWizard
{
    public string GameName;
    public string FacebookId;
    private CommunicateWithServer cs;

    [MenuItem("DavanciCode/Add Privacy")]
    static void CreateWindow()
    {
        // Creates the wizard for display
        ScriptableWizard.DisplayWizard("Create Privacy Page",
            typeof(PrivacyCreatorWizard),
            "Create Privacy !", "Add Facebook Id");
    }
    void OnWizardUpdate()
    {
        helpString = "If The game product name is not the same on Playtstore,you can overwrite it from here to create the privacy Page with the CORRECT APP NAME";

        if (String.IsNullOrEmpty(GameName))
        {
            GameName = Application.productName;
        }
        if (String.IsNullOrEmpty(FacebookId))
        {
            FacebookId = FacebookSettings.AppId;
        }
    }
    GameObject ObjectHolder;

    private void OnWizardCreate()
    {
        //GameName = Application.productName;
        if (ObjectHolder == null)
        {
            ObjectHolder = new GameObject();
            cs = ObjectHolder.AddComponent<CommunicateWithServer>();
        }
        else
            cs = ObjectHolder.GetComponent<CommunicateWithServer>();

        //   cs = ((CommunicateWithServer)Resources.FindObjectsOfTypeAll(typeof(CommunicateWithServer))[0]);

        if (EditorUtility.DisplayDialog("Create Privacy page ?",
               "Are you sure you want to create privacy page for  " + GameName
               + " ?", "Create ", "Cancel"))
        {
            cs.OnLinkCreated += ShowPopup;
            cs.SendPrivacyRequest(GameName);
            Debug.Log("SendPrivacyRequest : "+ GameName);
        }
    }
    void OnWizardOtherButton()
    {
        if (ObjectHolder == null)
        {
            ObjectHolder = new GameObject();
            cs = ObjectHolder.AddComponent<CommunicateWithServer>();
        }
        else
            cs = ObjectHolder.GetComponent<CommunicateWithServer>();

        ShowPopupFacebook(FacebookId);
    }

    public void ShowPopup(string link)
    {
        link.CopyToClipboard();

        PrivacyPopup wizard = ScriptableWizard.DisplayWizard<PrivacyPopup>("Privacy Link", "Copy");
        wizard.PrivacyLink = link;
        wizard.helpString = link;

        Debug.Log("Link from wizard : " + link);
    }
    public void ShowPopupFacebook(string FacebookId)
    {
        if (EditorUtility.DisplayDialog("Add Facebook Id Added to AppTxt ?",
                "Are you sure you want to Add the Facebook ID : ' " + FacebookId
                + " ' to app-ads.txt ?", "Add", "Cancel"))
        {
            cs.SendFacebookRequest(FacebookId);
        }
    }
    private void OnDisable()
    {
       // DestroyImmediate(ObjectHolder);
    }
}
