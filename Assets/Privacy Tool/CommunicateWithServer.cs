using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using System;
using Facebook.Unity.Settings;

public class CommunicateWithServer : MonoBehaviour
{
    private string serverURL = "https://stolenpad-studio.com/Games/CreatePage.php";
    private string FacebookserverURL = "https://stolenpad-studio.com/FacebookListner.php";

    // [SerializeField]private string GameName;
    private string privacyLink;
    public Action<string> OnLinkCreated;
    public Action<string> OnFacebookAdded;

    [ContextMenu("Test SERVER")]    
    public void SendPrivacyRequest(string gameName)
    {
        StartCoroutine(SendPrivacyData(gameName));
    }

    public IEnumerator SendPrivacyData(string GameName)
    {
        if (String.IsNullOrEmpty(GameName))
        {
            GameName = Application.productName;

        }

        //GameName = "Privacy" + GameName;

        WWWForm form = new WWWForm();
        form.AddField("GameName", GameName);
        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);
        yield return www.SendWebRequest();


        // check for errors
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW Ok!: " + www.downloadHandler.text);

            GameName = GameName.Replace(" ", "");
            privacyLink = "https://stolenpad-studio.com/Games/" + "Privacy" + GameName + ".html";
            
            OnLinkCreated?.Invoke(privacyLink);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }

         yield return privacyLink;
    }

    public void SendFacebookRequest(string FacebookId)
    {
        StartCoroutine(SendFacebookData(FacebookId));
    }
    public IEnumerator SendFacebookData(string FacebookId)
    {
        WWWForm form = new WWWForm();

        string faceboookId = '\n' +"facebook.com, XX, RESELLER, c3e20eee3f780d68";

        if(String.IsNullOrEmpty(FacebookId))
        {
            faceboookId = faceboookId.Replace("XX", FacebookSettings.AppId);
        }
        else
        {
            faceboookId = faceboookId.Replace("XX", FacebookId);
        }

        form.AddField("FacebookId", faceboookId);
       //Debug.Log(faceboookId);
        UnityWebRequest www = UnityWebRequest.Post(FacebookserverURL, form);
        yield return www.SendWebRequest();

        // check for errors
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Ok! : " + www.downloadHandler.text);
            OnFacebookAdded?.Invoke(faceboookId);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }

        yield return faceboookId;
    }
}