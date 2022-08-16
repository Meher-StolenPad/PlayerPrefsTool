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

public class PlayerPrefsWindow : EditorWindow
{
    private static readonly System.Text.Encoding encoding = new System.Text.UTF8Encoding();

    private class PlayerPrefPair
    {
        public string Key;
        public string TempKey;

        public object Value;

        public object TempValue;

        public object BackupValues;

        public PlayerPrefsType type;

        private int backupIndex;

        public void SaveKey()
        {
            if (Key != TempKey)
            {
                PlayerPrefs.DeleteKey(Key);
                Key = TempKey;
            }
            Save();
        }

        public void Save()
        {
            BackupValues = Value;

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
                    PrefsSerialzer.SetVector3(Key, PrefsSerialzer.StringToVector3(Value.ToString()));
                    break;
                case PlayerPrefsType.Vector2:
                    PrefsSerialzer.SetVector2(Key, PrefsSerialzer.StringToVector2(Value.ToString()));
                    break;
                case PlayerPrefsType.Color:
                    PrefsSerialzer.SetColor(Key, PrefsSerialzer.StringToColor(Value.ToString()));
                    break;
                case PlayerPrefsType.Vector4:
                    PrefsSerialzer.SetVector4(Key, PrefsSerialzer.StringToVector4(Value.ToString()));
                    break;
                case PlayerPrefsType.Bool:
                    PrefsSerialzer.SetBool(Key, PrefsSerialzer.StringToBool(Value.ToString()));
                    break;
                default:
                    break;
            }
        }
        public void BackUp()
        {
            GUI.FocusControl(null);
            TempValue = BackupValues;
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(Key);
        }
    }

    internal void Import(string importCompanyName, string importProductName)
    {
        string currentCompanyName = PlayerSettings.companyName;
        string currentProductName = PlayerSettings.productName;

        PlayerSettings.productName = importProductName;
        PlayerSettings.companyName = importCompanyName;

        registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + importCompanyName + "\\" + importProductName);

        GetAllPlayerPrefs();

        PlayerSettings.productName = currentProductName;
        PlayerSettings.companyName = currentCompanyName;

        Debug.Log("import");
        foreach (var pref in deserializedPlayerPrefs)
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

    private Action OnSearchChanged;
    private void OnSearchTextChanged()
    {
        UpdateSearch();
    }
    private void UpdateSearch()
    {
        if (searchText.Equals(oldSearchFilter))
        {
            return;
        }
       // Debug.Log("update Search !");
        filtredPlayerPrefs.Clear();

        if (string.IsNullOrEmpty(searchText))
        {
            return;
        }

        int entryCount = deserializedPlayerPrefs.Count;

        for (int i = 0; i < entryCount; i++)
        {
            string fullKey = deserializedPlayerPrefs[i].Key;
            string displayKey = fullKey;

            if (displayKey.ToLower().Contains(searchText.ToLower()))
            {
                filtredPlayerPrefs.Add(deserializedPlayerPrefs[i]);
            }
            //// Else check value
            //else if (deserializedPlayerPrefs[i].Value.ToString().ToLower().Contains(searchText.ToLower()))
            //{
            //    filtredPlayerPrefs.Add(deserializedPlayerPrefs[i]);
            //}
        }
        oldSearchFilter = searchText;
    }

    private string searchText = "";


    List<PlayerPrefPair> deserializedPlayerPrefs = new List<PlayerPrefPair>();
    List<PlayerPrefPair> filtredPlayerPrefs = new List<PlayerPrefPair>();
    List<PlayerPrefPair> testPlayerPrefs = new List<PlayerPrefPair>();

    // int[] typeList;  

    Vector2 scrollView;
    RegistryKey registryKey;
    string companyName;
    string productName;


    // Create button as a MenuItem to call the ShowWindow method
    [MenuItem("DavanciCode/PlayerPrefs Manager")]
    public static void ShowWindow()
    {
        PlayerPrefsWindow window = (PlayerPrefsWindow)GetWindow(typeof(PlayerPrefsWindow));
        window.titleContent = new GUIContent("PlayerPrefs Manager");
        window.Show();
    }

    // Set variables at the beginning of window
    void OnEnable()
    {
        //searchText = "";

        companyName = PlayerSettings.companyName;
        productName = PlayerSettings.productName;

        registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + companyName + "\\" + productName);

        refreshIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/refresh_Icon.png", typeof(Texture));
        plusIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/plus_Icon.png", typeof(Texture));
        saveIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/save_Icon.png", typeof(Texture));
        resetIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/reset_Icon.png", typeof(Texture));
        deleteIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/delete_Icon.png", typeof(Texture));

        OnSearchChanged += OnSearchTextChanged;

        GetAllPlayerPrefs();
        filtredPlayerPrefs.Clear();
    }

    string oldSearchFilter = ".";
    // Called for rendering and handling GUI events
    void OnGUI()
    {
        GUILayout.BeginVertical();

        DrawToolbarGUI();
        // DrawAddValueArea();
        if (!String.IsNullOrEmpty(searchText))
        {
            UpdateSearch();
            DrawPlayerPrefs(filtredPlayerPrefs);
        }
        else
            DrawPlayerPrefs(deserializedPlayerPrefs);

        if (GUILayout.Button("Add"))
        {

        }
        GUILayout.EndVertical();
    }

    // Draw all toolbar items
    void DrawToolbarGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        //   DrawSortButton();
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
            UpdateRegistry();
            GetAllPlayerPrefs();
        }
    }

    // Draws area to add new PlayerPrefs' keys and values
    void DrawAddValueArea()
    {
        //GUILayout.BeginVertical("box");
        //GUILayout.Label("Add new PlayerPrefs", EditorStyles.boldLabel);

        //GUILayout.Space(20);
        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Key", GUILayout.MaxWidth(100));
        //newKey = GUILayout.TextField(newKey);
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Value", GUILayout.MaxWidth(100));
        //newValue = GUILayout.TextField(newValue);
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Type", GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
        //playerPrefsTypes = (PlayerPrefsType)EditorGUILayout.EnumPopup(playerPrefsTypes);
        //if (GUILayout.Button(new GUIContent(plusIcon, "Add new PlayerPrefs data"), GUILayout.MaxWidth(40), GUILayout.MaxHeight(40)))
        //{
        //    AddNewPlayerPrefsData();
        //    GetAllPlayerPrefs();
        //}
        //GUILayout.EndHorizontal();

        //GUILayout.EndVertical();
    }

    // Add new PlayerPrefs and data comes from key, value and type fields on window
    void AddNewPlayerPrefsData()
    {
        //switch (playerPrefsTypes)
        //{
        //    case PlayerPrefsType.Int:
        //        PlayerPrefs.SetInt(newKey, int.Parse(newValue));
        //        break;
        //    case PlayerPrefsType.Float:
        //        if (newValue.Contains("."))
        //            newValue = newValue.Replace('.', ',');
        //        PlayerPrefs.SetFloat(newKey, float.Parse(newValue));
        //        break;
        //    case PlayerPrefsType.String:
        //        PlayerPrefs.SetString(newKey, newValue);
        //        break;
        //}
        //newKey = "";
        //newValue = "";
    }

    // Gets all PlayerPrefs data that includes keys, values and types and adds them to arrays 

    private void UpdateRegistry()
    {
        registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + companyName + "\\" + productName);
    }
    private void GetAllPlayerPrefs(bool isImport = false)
    {
        if (registryKey == null) return;

        deserializedPlayerPrefs.Clear();
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
                        if (PrefsSerialzer.GetInt(key, -1) == -1 && PrefsSerialzer.GetInt(key, 0) == 0)
                        {
                            // Fetch the float value from PlayerPrefs in memory
                            string ambiguousValueSTR = ambiguousValue.ToString();
                            ambiguousValue = PrefsSerialzer.GetFloat(key);
                            pair.type = PlayerPrefsType.Float;
                        }
                        else
                        {
                            pair.type = PlayerPrefsType.Int;
                            //ambiguousValue = GetInt(key, (int));
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

                        ambiguousValue = PrefsSerialzer.TryGetCostumeType(key, out type, ambiguousValue.ToString());

                        pair.type = type;

                        //pair.type  = PlayerPrefsType.String;
                    }

                    pair.Value = ambiguousValue;
                    pair.TempValue = ambiguousValue;
                    pair.BackupValues = ambiguousValue;
                    pair.Key = key;
                    pair.TempKey = key;

                    // Assign the key and value into the respective record in our output array
                    tempPlayerPrefs[i] = pair;// new PlayerPrefPair() { Key = key, Value = ambiguousValue };
                    i++;
                }
                deserializedPlayerPrefs = tempPlayerPrefs.ToList();
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
    void DrawPlayerPrefs(List<PlayerPrefPair> _deserializedPlayerPrefs)
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
        GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
        GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
        GUILayout.Label("", EditorStyles.boldLabel, GUILayout.MinWidth(75), GUILayout.MaxWidth(75));
        GUILayout.EndHorizontal();
        scrollView = EditorGUILayout.BeginScrollView(scrollView);

        for (int i = 0; i < _deserializedPlayerPrefs.Count; i++)
        {
            GUILayout.BeginHorizontal();

            _deserializedPlayerPrefs[i].TempKey = GUILayout.TextField(_deserializedPlayerPrefs[i].TempKey, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

            switch (_deserializedPlayerPrefs[i].type)
            {
                case PlayerPrefsType.Int:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.IntField((int)_deserializedPlayerPrefs[i].TempValue, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Float:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.FloatField((float)_deserializedPlayerPrefs[i].TempValue, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.String:
                    _deserializedPlayerPrefs[i].TempValue = GUILayout.TextField(_deserializedPlayerPrefs[i].TempValue.ToString(), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Vector3:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector3Field("", PrefsSerialzer.StringToVector3(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Vector2:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector2Field("", PrefsSerialzer.StringToVector2(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Color:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.ColorField(PrefsSerialzer.StringToColor(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Vector4:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector4Field("", PrefsSerialzer.StringToVector4(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                    break;
                case PlayerPrefsType.Bool:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.ToggleLeft("", PrefsSerialzer.StringToBool(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.MinWidth(100), GUILayout.MaxWidth(200));

                    break;
                default:
                    break;
            }

            GUILayout.Label(_deserializedPlayerPrefs[i].type.ToString(), GUILayout.MinWidth(50), GUILayout.MaxWidth(50));

            if (GUILayout.Button(new GUIContent(saveIcon, "Save current data"), EditorStyles.miniButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                _deserializedPlayerPrefs[i].SaveKey();
            }
            if (GUILayout.Button(new GUIContent(resetIcon, "Reset data to default"), EditorStyles.miniButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                _deserializedPlayerPrefs[i].BackUp();
            }
            if (GUILayout.Button(new GUIContent(deleteIcon, "Delete PlayerPrefs data"), EditorStyles.miniButton, GUILayout.MaxWidth(35), GUILayout.MaxHeight(35)))
            {
                _deserializedPlayerPrefs[i].Delete();
                deserializedPlayerPrefs.Remove(_deserializedPlayerPrefs[i]);

                if (filtredPlayerPrefs.Contains(_deserializedPlayerPrefs[i]))
                    filtredPlayerPrefs.Remove(_deserializedPlayerPrefs[i]);
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
}

