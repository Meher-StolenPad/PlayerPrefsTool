//// TODOs
////
//// Sort Button Functionality
//// Import Functionality
//// Export Functionality
//// Searchfield Functionality
//// Save Current Functionality

//// BUGFIX
////
//// Fix reading of float PlayerPrefs
//// Fix non-editable types because of that doesn't have references
//// Fix Delete All button that causes errors

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum PlayerPrefsType
{
    Int,
    Float,
    String,
    Vector2,
    Vector3,
    Vector4,
    Color,
    Bool
}

public class PlayerPrefsWindow : EditorWindow
{
    private static readonly System.Text.Encoding encoding = new System.Text.UTF8Encoding();

    private class PlayerPrefPair
    {
        public string Key;

        public object Value;

        public object TempValue;

        public List<object> BackupValues = new List<object>();

        public PlayerPrefsType type;

        private int backupIndex;

        public void Save()
        {
            BackupValues.Add(Value);

            Value = TempValue;

            switch (type)
            {
                case PlayerPrefsType.Int:
                    PlayerPrefs.SetInt(Key, (int)Value);
                    break;
                case PlayerPrefsType.Float:
                    PlayerPrefs.SetFloat(Key, (float)Value);
                    break;
                case PlayerPrefsType.String:
                    PlayerPrefs.SetString(Key, Value.ToString());
                    break;
                case PlayerPrefsType.Vector3:
                    SetVector3(Key, StringToVector3(Value.ToString()));
                    break;
                case PlayerPrefsType.Vector2:
                    SetVector2(Key, StringToVector2(Value.ToString()));
                    break;
                case PlayerPrefsType.Color:
                    SetColor(Key, StringToColor(Value.ToString()));
                    break;
                case PlayerPrefsType.Vector4:
                    SetVector4(Key, StringToVector4(Value.ToString()));
                    break;
                case PlayerPrefsType.Bool:
                    SetBool(Key, StringToBool(Value.ToString()));
                    break;
                default:
                    break;
            }
        }
        public void BackUp()
        {
            if (BackupValues.Count <= 0) return;

            backupIndex = BackupValues.Count - 1;

            TempValue = BackupValues[backupIndex];
            Debug.Log("get back up for : " + Key + " : " + BackupValues[backupIndex]);
            backupIndex--;

            if (backupIndex <= 0)
                backupIndex = 0;
        }

    }

    internal void Import(string importCompanyName, string importProductName)
    {
        registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + importCompanyName + "\\" + importProductName);
        GetAllPlayerPrefs(true);
        Debug.Log("import");
        foreach (var pref in ImportedPlayerPrefs)
        {
            pref.Save();
        }
    }

    private PlayerPrefsType playerPrefsTypes;

    Texture refreshIcon;
    Texture plusIcon;
    Texture saveIcon;
    Texture resetIcon;
    Texture deleteIcon;


    string searchText;
    string newKey;
    string newValue;

    List<PlayerPrefPair> deserializedPlayerPrefs = new List<PlayerPrefPair>();
    List<PlayerPrefPair> ImportedPlayerPrefs = new List<PlayerPrefPair>();

    // int[] typeList;

    Vector2 scrollView;
    RegistryKey registryKey;
    string companyName;
    string productName;

    private int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    private float GetFloat(string key, float defaultValue = 0.0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    private string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    private object TryGetCostumeType(string key, out PlayerPrefsType playerPrefsType, string defaultValue = "")
    {
        string json = PlayerPrefs.GetString(key, defaultValue);

        string retunValue = json;

        //Debug.Log(json);

        if (String.IsNullOrEmpty(json))
        {
            playerPrefsType = PlayerPrefsType.String;
            retunValue = json;
            Debug.Log(key + " Is empty !");
        }
        else if (json.TryParseJson(out Serialzer<object> t))
        {
           // Debug.Log(json);
            playerPrefsType = t.type;
            switch (t.type)
            {
                case PlayerPrefsType.Vector3:

                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Vector2:
                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Color:
                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Vector4:
                    retunValue = t.value.ToString();
                    break;
                case PlayerPrefsType.Bool:
                    retunValue = t.value.ToString();
                    break;
            }
        }
        else
        {
            playerPrefsType = PlayerPrefsType.String;
            retunValue = json;
        }
        return retunValue;
    }

    public static void SetVector3(string key, Vector3 _value)
    {
        Serialzer<Vector3> serialzer = new Serialzer<Vector3>();
        serialzer.type = PlayerPrefsType.Vector3;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Vector3 StringToVector3(string s)
    {
        Vector3 outVector3 = Vector3.zero;

        if (s.Contains("{"))
        {
            outVector3 = JsonUtility.FromJson<Vector3>(s);
        }
        else
        {
            //Debug.Log(s);

            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);

            // Build new Vector3 from array elements

            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
            outVector3.z = float.Parse(splitString[2]);
        }

        return outVector3;
    }
    public static void SetBool(string key, bool _value)
    {
        Serialzer<bool> serialzer = new Serialzer<bool>();
        serialzer.type = PlayerPrefsType.Bool;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static bool StringToBool(string s)
    {
        bool outBool = false;

        if (s == "True")
        {
            outBool = true;
        }
        return outBool;
    }
    public static void SetVector2(string key, Vector2 _value)
    {
        Serialzer<Vector2> serialzer = new Serialzer<Vector2>();
        serialzer.type = PlayerPrefsType.Vector2;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Vector3 StringToVector2(string s)
    {
        Vector2 outVector3 = Vector2.zero;

        if (s.Contains("{"))
        {
            outVector3 = JsonUtility.FromJson<Vector2>(s);
        }
        else
        {
            //Debug.Log(s);

            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);

            // Build new Vector3 from array elements

            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
        }

        return outVector3;
    }

    public static void SetVector4(string key, Vector4 _value)
    {
        Serialzer<Vector4> serialzer = new Serialzer<Vector4>();
        serialzer.type = PlayerPrefsType.Vector4;
        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Vector4 StringToVector4(string s)
    {
        Vector4 outVector3 = Vector4.zero;

        if (s.Contains("{"))
        {
            outVector3 = JsonUtility.FromJson<Vector4>(s);
        }
        else
        {
            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);

            // Build new Vector3 from array elements

            outVector3.x = float.Parse(splitString[0]);
            outVector3.y = float.Parse(splitString[1]);
            outVector3.z = float.Parse(splitString[2]);
            outVector3.w = float.Parse(splitString[3]);
        }

        return outVector3;
    }

    public static void SetColor(string key, Color _value)
    {
        Serialzer<Color> serialzer = new Serialzer<Color>();

        serialzer.type = PlayerPrefsType.Color;

        serialzer.value = _value;

        string jsonString = JsonUtility.ToJson(serialzer);

        PlayerPrefs.SetString(key, jsonString);
    }
    public static Color StringToColor(string s)
    {
        Color outColor = Color.white;

        if (ColorUtility.TryParseHtmlString("#" + s, out Color _Color))
        {
            outColor = _Color;
        }
        else if (s.Contains("{"))
        {
            s = s.Replace("{", "");
            s = s.Replace("}", "");
            s = s.Replace('"', ' ');
            s = s.Replace("r", "");
            s = s.Replace("g", "");
            s = s.Replace("b", "");
            s = s.Replace("a", "");
            s = s.Replace(":", "");
            s = s.Replace(" ", "");

            var splitString = s.Split(","[0]);


            //    // Build new Vector3 from array elements

            outColor.r = float.Parse(splitString[0]);
            outColor.g = float.Parse(splitString[1]);
            outColor.b = float.Parse(splitString[2]);
            outColor.a = float.Parse(splitString[3]);

        }
        else
        {
            s = s.Replace("RGBA", "");
            s = s.Replace("#", "");
            s = s.Replace("(", "");
            s = s.Replace(")", "");

            var splitString = s.Split(","[0]);


            //    // Build new Vector3 from array elements

            outColor.r = float.Parse(splitString[0]);
            outColor.g = float.Parse(splitString[1]);
            outColor.b = float.Parse(splitString[2]);
            outColor.a = float.Parse(splitString[3]);
        }

        return outColor;
    }


    // Create button as a MenuItem to call the ShowWindow method
    [MenuItem("JokerCoder/PlayerPrefs Manager")]
    public static void ShowWindow()
    {
        PlayerPrefsWindow window = (PlayerPrefsWindow)GetWindow(typeof(PlayerPrefsWindow));
        window.titleContent = new GUIContent("PlayerPrefs Manager");
        window.Show();
    }

    // Set variables at the beginning of window
    void OnEnable()
    {
        searchText = "";
        newKey = "";
        newValue = "";
        companyName = PlayerSettings.companyName;
        productName = PlayerSettings.productName;

        registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + companyName + "\\" + productName);

        refreshIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/refresh_Icon.png", typeof(Texture));
        plusIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/plus_Icon.png", typeof(Texture));
        saveIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/save_Icon.png", typeof(Texture));
        resetIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/reset_Icon.png", typeof(Texture));
        deleteIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/delete_Icon.png", typeof(Texture));

        GetAllPlayerPrefs();
    }
    public void ImportPlayerPres()
    {

    }
    // Called for rendering and handling GUI events
    void OnGUI()
    {
        GUILayout.BeginVertical();

        DrawToolbarGUI();
        DrawAddValueArea();
        DrawPlayerPrefs(GetDeserializedPlayerPrefs());

        GUILayout.EndVertical();
    }

    // Draw all toolbar items
    void DrawToolbarGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        DrawSortButton();
        DrawMoreButton();
        DrawSearchField();
        DrawRefreshButton();

        GUILayout.EndHorizontal();
    }

    // Sort all PlayerPrefs alphabetically
    void DrawSortButton()
    {
        if (GUILayout.Button("Sort", EditorStyles.toolbarPopup, GUILayout.MaxWidth(50)))
        {

        }
    }

    // Shows popup that includes more options to use
    void DrawMoreButton()
    {
        if (GUILayout.Button("More", EditorStyles.toolbarDropDown, GUILayout.MaxWidth(50)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete All"), false, DeleteAll);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Import"), false, Import);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Export"), false, Export);

            menu.ShowAsContext();
        }
    }

    // Draws search field for finding specific PlayerPrefs
    void DrawSearchField()
    {
        searchText = GUILayout.TextField(searchText, 25, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.MinWidth(150)); // It's name written wrong by team in Unity
        if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) // It's name written wrong by team in Unity
        {
            // Remove focus if cleared
            searchText = "";
            GUI.FocusControl(null);
        }
    }

    // Draws button to refresh all PlayerPrefs data
    void DrawRefreshButton()
    {
        if (GUILayout.Button(new GUIContent(refreshIcon, "Refresh all PlayerPrefs data"), EditorStyles.toolbarButton, GUILayout.MaxWidth(30)))
        {
            GetAllPlayerPrefs();
        }
    }

    // Draws area to add new PlayerPrefs' keys and values
    void DrawAddValueArea()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("Add new PlayerPrefs", EditorStyles.boldLabel);

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Key", GUILayout.MaxWidth(100));
        newKey = GUILayout.TextField(newKey);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Value", GUILayout.MaxWidth(100));
        newValue = GUILayout.TextField(newValue);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Type", GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
        playerPrefsTypes = (PlayerPrefsType)EditorGUILayout.EnumPopup(playerPrefsTypes);
        if (GUILayout.Button(new GUIContent(plusIcon, "Add new PlayerPrefs data"), GUILayout.MaxWidth(40), GUILayout.MaxHeight(40)))
        {
            AddNewPlayerPrefsData();
            GetAllPlayerPrefs();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    // Add new PlayerPrefs and data comes from key, value and type fields on window
    void AddNewPlayerPrefsData()
    {
        switch (playerPrefsTypes)
        {
            case PlayerPrefsType.Int:
                PlayerPrefs.SetInt(newKey, int.Parse(newValue));
                break;
            case PlayerPrefsType.Float:
                if (newValue.Contains("."))
                    newValue = newValue.Replace('.', ',');
                PlayerPrefs.SetFloat(newKey, float.Parse(newValue));
                break;
            case PlayerPrefsType.String:
                PlayerPrefs.SetString(newKey, newValue);
                break;
        }
        newKey = "";
        newValue = "";
    }

    // Gets all PlayerPrefs data that includes keys, values and types and adds them to arrays 
    private void GetAllPlayerPrefs(bool isImport = false)
    {
        if (registryKey == null) return;

        foreach (string item in registryKey.GetValueNames())
        {

            //  Debug.Log("Key : " + key.ToString() + registryKey.GetValue(key).GetType().ToString());

            //  string _key = item.Remove(item.LastIndexOf('_'));
            //keyList[counter] = _key;
            // pair.Key = _key;

            if (registryKey != null)
            {
                // Get an array of what keys (registry value names) are stored
                string[] valueNames = registryKey.GetValueNames();

                // Create the array of the right size to take the saved PlayerPrefs
                PlayerPrefPair[] tempPlayerPrefs = new PlayerPrefPair[valueNames.Length];

                // Parse and convert the registry saved PlayerPrefs into our array
                int i = 0;
                foreach (string valueName in valueNames)
                {
                    string key = valueName;

                    // Remove the _h193410979 style suffix used on PlayerPref keys in Windows registry
                    int index = key.LastIndexOf("_");
                    key = key.Remove(index, key.Length - index);

                    // Get the value from the registry
                    object ambiguousValue = registryKey.GetValue(valueName);
                    PlayerPrefPair pair = new PlayerPrefPair();
                    // Unfortunately floats will come back as an int (at least on 64 bit) because the float is stored as
                    // 64 bit but marked as 32 bit - which confuses the GetValue() method greatly!
                    if (ambiguousValue.GetType() == typeof(int) || ambiguousValue.GetType() == typeof(long))
                    {
                        // If the PlayerPref is not actually an int then it must be a float, this will evaluate to true
                        // (impossible for it to be 0 and -1 at the same time)
                        if (GetInt(key, -1) == -1 && GetInt(key, 0) == 0)
                        {
                            // Fetch the float value from PlayerPrefs in memory
                            string ambiguousValueSTR = ambiguousValue.ToString();
                            Debug.Log(ambiguousValueSTR);
                            ambiguousValue = GetFloat(key,float.Parse(ambiguousValueSTR));
                            pair.type = PlayerPrefsType.Float;
                        }
                        else
                        {
                            pair.type = PlayerPrefsType.Int;
                            ambiguousValue = GetInt(key, (int)ambiguousValue);
                        }
                        //else if (GetBool(key, true) != true || GetBool(key, false) != false)
                        //{
                        //    // If it reports a non default value as a bool, it's a bool not a string
                        //    ambiguousValue = GetBool(key);
                        //}
                    }
                    else if (ambiguousValue.GetType() == typeof(byte[]))
                    {
                        // On Unity 5 a string may be stored as binary, so convert it back to a string
                        ambiguousValue = encoding.GetString((byte[])ambiguousValue).TrimEnd('\0');

                        PlayerPrefsType type = PlayerPrefsType.String;

                        ambiguousValue = TryGetCostumeType(key, out type, ambiguousValue.ToString());

                        pair.type = type;

                        //pair.type  = PlayerPrefsType.String;
                    }


                    pair.Value = ambiguousValue;
                    pair.TempValue = ambiguousValue;
                    pair.Key = key;

                    // Assign the key and value into the respective record in our output array
                    tempPlayerPrefs[i] = pair;// new PlayerPrefPair() { Key = key, Value = ambiguousValue };
                    i++;
                }
                if (isImport)
                {
                    ImportedPlayerPrefs = tempPlayerPrefs.ToList();

                }
                else
                {
                    deserializedPlayerPrefs = tempPlayerPrefs.ToList();
                }
                // Return the results
                // return tempPlayerPrefs;
            }
            else
            {
                // No existing PlayerPrefs saved (which is valid), so just return an empty array
                //return new PlayerPrefPair[0];
            }
        }
    }

    private List<PlayerPrefPair> GetDeserializedPlayerPrefs()
    {
        return deserializedPlayerPrefs;
    }

    // Draw Scrollable view for PlayerPrefs list and PlayerPrefs rows that gets data from registryKey
    void DrawPlayerPrefs(List<PlayerPrefPair> deserializedPlayerPrefs)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
        GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
        GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
        GUILayout.Label("", EditorStyles.boldLabel, GUILayout.MinWidth(75), GUILayout.MaxWidth(75));
        GUILayout.EndHorizontal();
        scrollView = EditorGUILayout.BeginScrollView(scrollView);

        for (int i = 0; i < deserializedPlayerPrefs.Count; i++)
        {
            GUILayout.BeginHorizontal();

            var t = GUILayout.TextField(deserializedPlayerPrefs[i].Key, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

            switch (deserializedPlayerPrefs[i].type)
            {
                case PlayerPrefsType.Int:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.IntField((int)deserializedPlayerPrefs[i].TempValue, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Float:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.FloatField((float)deserializedPlayerPrefs[i].TempValue, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.String:
                    deserializedPlayerPrefs[i].TempValue = GUILayout.TextField(deserializedPlayerPrefs[i].TempValue.ToString(), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Vector3:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector3Field("", StringToVector3(deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Vector2:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector2Field("", StringToVector2(deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Color:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.ColorField(StringToColor(deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Vector4:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector4Field("", StringToVector4(deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Bool:
                    deserializedPlayerPrefs[i].TempValue = EditorGUILayout.ToggleLeft("", StringToBool(deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

                    break;
                default:
                    break;
            }

            PlayerPrefsType type = deserializedPlayerPrefs[i].type;

            type = (PlayerPrefsType)EditorGUILayout.EnumPopup(deserializedPlayerPrefs[i].type, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

            if (GUILayout.Button(new GUIContent(saveIcon, "Save current data"), EditorStyles.miniButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                deserializedPlayerPrefs[i].Save();
            }
            if (GUILayout.Button(new GUIContent(resetIcon, "Reset data to default"), EditorStyles.miniButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                deserializedPlayerPrefs[i].BackUp();
            }
            if (GUILayout.Button(new GUIContent(deleteIcon, "Delete PlayerPrefs data"), EditorStyles.miniButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                //PlayerPrefs.DeleteKey(keyList[i]);
                //GetAllPlayerPrefs();
            }

            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
    }

    // Call this function when Delete All button clicked
    void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    // Call this function when Import button clicked
    void Import()
    {
        ImportPrefsWizard wizard = ScriptableWizard.DisplayWizard<ImportPrefsWizard>("Import PlayerPrefs", "Import");

        Debug.Log("Import PlayerPrefs");
    }

    // Call this function when Export button clicked
    void Export()
    {
        Debug.Log("Export PlayerPrefs");

        // string addItemJsonString = "{\"fields\":{\"ID\":\"" + ID + "\",\"DeviceName\":\"" + deviceName + "\"}}";

    }

    //public void SetVector3(string key, Vector3 _value)
    //{
    //    Serialzer<Vector3> serialzer = new Serialzer<Vector3>();
    //    serialzer.type = PlayerPrefsType.Vector3;
    //    serialzer.value = _value;

    //    string jsonString = JsonUtility.ToJson(serialzer);

    //    Debug.Log("string vector 3 ; " + jsonString);

    //    PlayerPrefs.SetString(key, jsonString);
    //}
    [Serializable]
    private class Serialzer<T>
    {
        public PlayerPrefsType type;
        public T value;
    }
}

