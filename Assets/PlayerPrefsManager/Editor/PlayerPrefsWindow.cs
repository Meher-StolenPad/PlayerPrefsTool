//// TODOs
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
public enum SortType
{
    None,
    Name,
    Type
}
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
        public bool isKeyFounded=false;
        public bool isValueFounded=false;   
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
                case PlayerPrefsType.DateTime:
                    if (PrefsSerialzer.StringToDateTime(Value.ToString()) != null)
                    {
                        PrefsSerialzer.SetDateTime(Key, ((DateTime)PrefsSerialzer.StringToDateTime(Value.ToString())));
                    }
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
        public bool isEqual()
        {   
            bool returnValue = false ;

            switch (type)
            {
                case PlayerPrefsType.Int:
                    returnValue = (int)TempValue == (int)Value;
                    break;
                case PlayerPrefsType.Float:
                    returnValue = (float)TempValue == (float)Value;
                    break;
                case PlayerPrefsType.String:
                    returnValue = String.Equals(TempValue.ToString(),Value.ToString());
                    break;
                case PlayerPrefsType.Vector2:
                    var v2c = PrefsSerialzer.StringToVector2(Value.ToString());
                    var v2t = PrefsSerialzer.StringToVector2(TempValue.ToString());
                    returnValue = v2c == v2t;

                    break;
                case PlayerPrefsType.Vector3:
                    var v3c = PrefsSerialzer.StringToVector3(Value.ToString());
                    var v3t = PrefsSerialzer.StringToVector3(TempValue.ToString());
                    returnValue = v3c == v3t;
                    break;
                case PlayerPrefsType.Vector4:
                    var v4c = PrefsSerialzer.StringToVector4(Value.ToString());
                    var v4t = PrefsSerialzer.StringToVector4(TempValue.ToString());
                    returnValue = v4c == v4t;
                    break;
                case PlayerPrefsType.Color:
                    var cc = PrefsSerialzer.StringToColor(Value.ToString());
                    var ct = PrefsSerialzer.StringToColor(TempValue.ToString());
                    returnValue = cc == ct;
                    break;
                case PlayerPrefsType.Bool:
                    var bc = PrefsSerialzer.StringToBool(Value.ToString());
                    var bt = PrefsSerialzer.StringToBool(TempValue.ToString());
                    returnValue = bc == bt;
                    break;
                case PlayerPrefsType.DateTime:
                    var tc = PrefsSerialzer.StringToDateTime(Value.ToString());
                    var tt = PrefsSerialzer.StringToDateTime(TempValue.ToString());
                    returnValue = tc == tt;
                    break;
                default:
                    break;
            }
            return returnValue;
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
    internal void AddPlayerPref(string key, PlayerPrefsType playerPrefsType, object value)
    {
        Debug.Log("AddPlayerPref");
        switch (playerPrefsType)
        {
            case PlayerPrefsType.Int:
                PlayerPrefs.SetInt(key, (int)value);
                break;
            case PlayerPrefsType.Float:
                PlayerPrefs.SetFloat(key, (float)value);
                break;
            case PlayerPrefsType.String:
                PlayerPrefs.SetString(key, (string)value);
                break;
            case PlayerPrefsType.Vector2:
                PrefsSerialzer.SetVector2(key, (Vector2)value);
                break;
            case PlayerPrefsType.Vector3:
                PrefsSerialzer.SetVector3(key, (Vector3)value);
                break;
            case PlayerPrefsType.Vector4:
                PrefsSerialzer.SetVector4(key, (Vector4)value);
                break;
            case PlayerPrefsType.Color:
                PrefsSerialzer.SetColor(key, (Color)value);
                break;
            case PlayerPrefsType.Bool:
                PrefsSerialzer.SetBool(key, (bool)value);
                break;
            default:
                break;
        }
        Refresh();
    }
    private SortType SortType;
    Texture refreshIcon;
    Texture plusIcon;
    Texture saveIcon;
    Texture resetIcon;
    Texture deleteIcon;
    Texture ApplyAllIcon;

    private Action OnSearchChanged;
    public bool ShowEditorPrefs = true;
    public bool EditorPrefsAvailable = true;
    public bool TypeSortActive = false;
    public bool ApplySort = false;
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
        filtredPlayerPrefs.Clear();

        if (string.IsNullOrEmpty(searchText))
        {
            return;
        }

        int entryCount = deserializedPlayerPrefs.Count;

        for (int i = 0; i < entryCount; i++)
        {
            deserializedPlayerPrefs[i].isKeyFounded = false;
            deserializedPlayerPrefs[i].isValueFounded = false;

            string fullKey = deserializedPlayerPrefs[i].Key;
            string displayKey = fullKey;

            string fullvalue = deserializedPlayerPrefs[i].TempValue.ToString();
            
            if (displayKey.ToLower().Contains(searchText.ToLower()))
            {
                filtredPlayerPrefs.Add(deserializedPlayerPrefs[i]);
                deserializedPlayerPrefs[i].isKeyFounded = true;
            }
            if (fullvalue.ToLower().Contains(searchText.ToLower()))
            {
                if (!filtredPlayerPrefs.Contains(deserializedPlayerPrefs[i]))
                {
                    filtredPlayerPrefs.Add(deserializedPlayerPrefs[i]);
                }
                deserializedPlayerPrefs[i].isValueFounded = true;
            }
        }
        oldSearchFilter = searchText;
    }

    private string searchText = "";


    List<PlayerPrefPair> deserializedPlayerPrefs = new List<PlayerPrefPair>();
    List<PlayerPrefPair> filtredPlayerPrefs = new List<PlayerPrefPair>();

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
        Vector2 minSize = window.minSize;
        minSize.x = 600;
        window.minSize = minSize;
    }

    // Set variables at the beginning of window
    void OnEnable()
    {
        companyName = PlayerSettings.companyName;
        productName = PlayerSettings.productName;

        registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + companyName + "\\" + productName);

        refreshIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/refresh_Icon.png", typeof(Texture));
        plusIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/plus_Icon.png", typeof(Texture));
        saveIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/save_Icon.png", typeof(Texture));
        resetIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/reset_Icon.png", typeof(Texture));
        deleteIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/delete_Icon.png", typeof(Texture));
        ApplyAllIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/PlayerPrefsManager/Icons/apply_Icon.png", typeof(Texture));

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
        if (!String.IsNullOrEmpty(searchText))
        {
            UpdateSearch();
            DrawPlayerPrefs(filtredPlayerPrefs,true);
        }
        else
            DrawPlayerPrefs(deserializedPlayerPrefs);
        EditorGUILayout.Space(10);
        DrawBottomButtons();
        EditorGUILayout.Space(10);

        GUILayout.EndVertical();
    }

    // Draw all toolbar items
    void DrawToolbarGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        DrawImpExpButton();
        DrawSearchField();
        DrawRefreshButton();
        DrawShowEditorPrefsButton();
        DrawApplyAll();
        DrawRevertAll();
        GUILayout.EndHorizontal();
    }
    private void DrawBottomButtons()
    {
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;
        // Delete all PlayerPrefs
        if (GUILayout.Button("Add New Pref", GUILayout.Width(buttonWidth)))
        {
            CreatePrefWizard wizard = ScriptableWizard.DisplayWizard<CreatePrefWizard>("AddPlayerPref");
        }

        GUILayout.FlexibleSpace();

        // Mainly needed for OSX, this will encourage PlayerPrefs to save to file (but still may take a few seconds)
        if (GUILayout.Button("Delete All", GUILayout.Width(buttonWidth)))
        {
            DeleteAll();
        }

        EditorGUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //Color oldBackgroundColor = GUI.backgroundColor;

        //GUI.backgroundColor = oldBackgroundColor;

        //GUI.backgroundColor = Color.green;
        //float FullbuttonWidth = (EditorGUIUtility.currentViewWidth-10) / 2f;

        //if (GUILayout.Button(new GUIContent(plusIcon, "Add New Player Pref"), EditorStyles.miniButtonLeft, GUILayout.Width(FullbuttonWidth)))
        //{
        //    CreatePrefWizard wizard = ScriptableWizard.DisplayWizard<CreatePrefWizard>("AddPlayerPref");
        //}
        //GUILayout.FlexibleSpace();

        //GUI.backgroundColor = Color.red;

        //if (GUILayout.Button(new GUIContent(deleteIcon, "Delete all PlayerPrefs data"), EditorStyles.miniButtonRight, GUILayout.Width(FullbuttonWidth)))
        //{
        //    DeleteAll();
        //}
        //GUI.backgroundColor = oldBackgroundColor;
    }
    // Shows popup that includes Import/Export options
    void DrawImpExpButton()
    {
        float buttonWidth = (EditorGUIUtility.currentViewWidth) / 4f;
        if (GUILayout.Button("Import/Export", EditorStyles.toolbarDropDown, GUILayout.Width((EditorGUIUtility.currentViewWidth) /4f)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Import directly"), false, Import);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Import from file"), false, ReadBackupFile);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Export"), false, Export);

            menu.ShowAsContext();
        }
    }

    // Draws search field for finding specific PlayerPrefs
    void DrawSearchField()
    {
        float buttonWidth = (EditorGUIUtility.currentViewWidth) / 4f;

        searchText = GUILayout.TextField(searchText, 25, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(buttonWidth)); // It's name written wrong by team in Unity
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
        float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
        FullbuttonWidth = (FullbuttonWidth / 7)/1.5f;

        if (GUILayout.Button(new GUIContent(refreshIcon, "Refresh all PlayerPrefs data"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
        {
            Refresh();
        }
    }
    void DrawShowEditorPrefsButton()
    {
        float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
        FullbuttonWidth = (FullbuttonWidth / 7)*4;
        ShowEditorPrefs = GUILayout.Toggle(ShowEditorPrefs, "Show Editor prefs", EditorStyles.miniButton, GUILayout.Width(FullbuttonWidth));
        if (EditorPrefsAvailable != ShowEditorPrefs)
        {
            Refresh();
            EditorPrefsAvailable = ShowEditorPrefs;
        }
    }
    private void DrawApplyAll()
    {
        float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
        FullbuttonWidth = (FullbuttonWidth / 7);

        if (GUILayout.Button(new GUIContent(ApplyAllIcon, "Save all Changes"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
        {
            foreach (var item in deserializedPlayerPrefs)
            {
                item.Save();
            }
        }
    }
    private void DrawRevertAll()
    {
        float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
        FullbuttonWidth = (FullbuttonWidth /7);

        if (GUILayout.Button(new GUIContent(resetIcon, "Revert all changes"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
        {
            foreach (var item in deserializedPlayerPrefs)
            {
                item.BackUp();
            }
        }
    }
    private void Refresh()
    {
        UpdateRegistry();
        GetAllPlayerPrefs();
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
                // Get Sort Type
                switch (SortType)
                {
                    case SortType.Name:
                        deserializedPlayerPrefs = deserializedPlayerPrefs.OrderBy(go => go.Key).ToList();
                        break;
                    case SortType.Type:
                        deserializedPlayerPrefs = deserializedPlayerPrefs.OrderBy(go => go.type).ToList();
                        break;
                }
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
    void DrawPlayerPrefs(List<PlayerPrefPair> _deserializedPlayerPrefs,bool isSearchDraw = false)
    {
        GUIStyle style;
        Color oldBackgroundColor;

        DrawTitles(out style, out oldBackgroundColor);
        float FullbuttonWidth = (EditorGUIUtility.currentViewWidth-20) / 10;
        //float valuebuttonWidth = (EditorGUIUtility.currentViewWidth) / 10;
            
        // scrollView = GUI.BeginScrollView(scrollView, new Rect(0, 0, 220, 200));

        //scrollView = GUILayout.BeginScrollView(scrollView, false, true, GUILayout.Width(EditorGUIUtility.currentViewWidth));

        scrollView = GUILayout.BeginScrollView(scrollView, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(EditorGUIUtility.currentViewWidth));


        for (int i = 0; i < _deserializedPlayerPrefs.Count; i++)
        {
            if (!ShowEditorPrefs)
            {
                if (_deserializedPlayerPrefs[i].TempKey.ToLower().Contains("unity"))
                    continue;
            }

            GUILayout.BeginHorizontal();
            GUIStyle style3 = EditorStyles.toolbar;
            GUIStyle style4 = EditorStyles.toggle;
            style3 = EditorStyles.textField;
            Color oldstylecolor = style.normal.textColor;

            style4.normal.textColor = Color.red;
            if (!_deserializedPlayerPrefs[i].isEqual())
            {
                style3.fontStyle = FontStyle.Bold;
                style3.normal.textColor = Color.green;
            }
            else
            {
                style3.fontStyle = FontStyle.Normal;
                style.normal.textColor = Color.white;
            }

            if (isSearchDraw)
            {
                if (_deserializedPlayerPrefs[i].isKeyFounded)
                {
                    style3.normal.textColor = Color.yellow;

                    GUILayout.Label(_deserializedPlayerPrefs[i].TempKey, style3, GUILayout.Width(FullbuttonWidth*3f));
                    style3.normal.textColor = oldstylecolor;
                }
                else
                {
                    GUILayout.Label(_deserializedPlayerPrefs[i].TempKey, style3, GUILayout.Width(FullbuttonWidth * 3f));
                }
            }
            else
            {
                GUILayout.Label(_deserializedPlayerPrefs[i].TempKey, style3, GUILayout.Width(FullbuttonWidth * 3f));
            }
            if (isSearchDraw)
            {
                if (_deserializedPlayerPrefs[i].isValueFounded)
                {
                    style3.normal.textColor = Color.yellow;
                }
                
            }
            style3.fontStyle = FontStyle.Normal;
            style3.normal.textColor = Color.white;

            // GUILayout.TextArea(_deserializedPlayerPrefs[i].TempKey, EditorStyles.textField, GUILayout.ExpandHeight(true));
            switch (_deserializedPlayerPrefs[i].type)
            {
                case PlayerPrefsType.Int:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.IntField((int)_deserializedPlayerPrefs[i].TempValue, GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.Float:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.FloatField((float)_deserializedPlayerPrefs[i].TempValue, GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.String:
                    _deserializedPlayerPrefs[i].TempValue = GUILayout.TextArea(_deserializedPlayerPrefs[i].TempValue.ToString(), EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.Vector3:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector3Field("", PrefsSerialzer.StringToVector3(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.Vector2:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector2Field("", PrefsSerialzer.StringToVector2(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.Color:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.ColorField(PrefsSerialzer.StringToColor(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.Vector4:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Vector4Field("", PrefsSerialzer.StringToVector4(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.Bool:
                    _deserializedPlayerPrefs[i].TempValue = EditorGUILayout.Toggle("", PrefsSerialzer.StringToBool(_deserializedPlayerPrefs[i].TempValue.ToString()), GUILayout.Width(FullbuttonWidth * 4));
                    break;
                case PlayerPrefsType.DateTime:
                    GUILayout.TextArea(PrefsSerialzer.StringToDateTime(_deserializedPlayerPrefs[i].TempValue.ToString()).ToString(), EditorStyles.toolbarTextField, GUILayout.Width(FullbuttonWidth * 4));
                    break;
                default:
                    break;
            }
            style3.normal.textColor = oldstylecolor;

            GUIStyle style2 = EditorStyles.miniTextField;
            style2.fontSize = 12;
            style2.fontStyle = FontStyle.Bold;
            style2.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label(_deserializedPlayerPrefs[i].type.ToString(), style2, GUILayout.Width(FullbuttonWidth * 1.5f));
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button(new GUIContent(saveIcon, "Save current data"), EditorStyles.miniButton, GUILayout.Width(FullbuttonWidth * 0.45f)))
            {
                _deserializedPlayerPrefs[i].SaveKey();
            }
            GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button(new GUIContent(resetIcon, "Reset data to default"), EditorStyles.miniButton, GUILayout.Width(FullbuttonWidth * 0.5f)))
            {
                _deserializedPlayerPrefs[i].BackUp();
            }
            GUI.backgroundColor = oldBackgroundColor;

            GUI.backgroundColor = Color.red;

            if (GUILayout.Button(new GUIContent(deleteIcon, "Delete PlayerPrefs data"), EditorStyles.miniButton, GUILayout.Width(FullbuttonWidth * 0.45f)))
            {
                _deserializedPlayerPrefs[i].Delete();
                OnDeleteElement += Refresh;

                if (filtredPlayerPrefs.Contains(_deserializedPlayerPrefs[i]))
                    filtredPlayerPrefs.Remove(_deserializedPlayerPrefs[i]);
            }
            GUI.backgroundColor = oldBackgroundColor;
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.EndVertical();
        OnDeleteElement?.Invoke();
        OnDeleteElement -= Refresh;
    }

    private void DrawTitles(out GUIStyle style, out Color oldBackgroundColor)
    {
        style = EditorStyles.toolbar;
        style.fontSize = 12;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        oldBackgroundColor = GUI.backgroundColor;
        GUIStyle styletoolbar = EditorStyles.toolbarDropDown;
        styletoolbar.fontSize = 12;
        styletoolbar.fontStyle = FontStyle.Bold;
        styletoolbar.alignment = TextAnchor.MiddleCenter;

        GUI.backgroundColor = new Color(0.56f, 0.56f, 0.56f);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();

        float FullbuttonWidth = (EditorGUIUtility.currentViewWidth-10)/10;

        if (GUILayout.Toggle(SortType == SortType.Name, "Key", styletoolbar, GUILayout.Width(FullbuttonWidth*3)))
        {
            if (SortType != SortType.Name)
            {
                SortType = SortType.Name;
                Refresh();
            }

        }


        // GUILayout.Toggle("Key", styletoolbar, GUILayout.MinWidth(205), GUILayout.MaxWidth(205));
        GUILayout.Label("Value", style, GUILayout.MinWidth(200), GUILayout.Width(FullbuttonWidth * 4));
        //GUILayout.Label("Type", styletoolbar, GUILayout.MinWidth(75), GUILayout.MaxWidth(75));
        if (GUILayout.Toggle(SortType == SortType.Type, "Type", styletoolbar, GUILayout.Width(FullbuttonWidth*1.5f)))
        {
            if (SortType != SortType.Type)
            {
                SortType = SortType.Type;
                Refresh();
            }
        }
        GUILayout.Label("Modify", style, GUILayout.MinWidth(110), GUILayout.Width(FullbuttonWidth *1.5f));

        GUI.backgroundColor = oldBackgroundColor;

        GUILayout.EndHorizontal();
    }

    private Action OnDeleteElement;

    // Call this function when Delete All button clicked
    void DeleteAll()
    {
        int dialogResult = EditorUtility.DisplayDialogComplex(
               "All Player Prefs Will be deleted !",
               "Do you want to create a backup file for your current prefs ?",
               "Yes", "Don't Create", "Cancel");

        switch (dialogResult)
        {
            case 0: //Create backup
                Export();
                PlayerPrefs.DeleteAll();
                Refresh();

                break;
            case 1: //Don't create a backup
                PlayerPrefs.DeleteAll();
                Refresh();
                break;
            case 2: //Cancel process (Basically do nothing for now.)
                break;
            default:
                Debug.LogWarning("Something went wrong when clearing player prefs");
                break;
        }
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
        var backupstring = CreateBackup();
        string newBackupString = PlayerPrefsGlobalVariables.CreatedText;
        string playerprefsSpecific = "//Player prefs for product  : " + Application.productName + " , Company :  " + Application.companyName + '\n'
            + "//Created at : " + DateTime.Now + "\n//Created by " + UnityEditor.CloudProjectSettings.userName + '\n';
        newBackupString += playerprefsSpecific;

        newBackupString += backupstring;

        string path = EditorUtility.OpenFolderPanel("Backup path", "", "PPbackup.txt");
        path += "/PPbackup.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, newBackupString);
        }
        else
        {
            path = PrefsSerialzer.NextAvailableFilename(path);
            File.WriteAllText(path, newBackupString);
        }
        Debug.Log(path);
    }
    private string CreateBackup()
    {
        ExportSerialzerHolder exportSerialzerHolder = new ExportSerialzerHolder();

        foreach (var item in deserializedPlayerPrefs)
        {
            ExportSerialzer toExport = new ExportSerialzer();
            toExport.type = item.type;
            toExport.key = item.Key;
            toExport.value = item.Value.ToString();
            exportSerialzerHolder.exportlist.Add(toExport);
        }
        string jsonString = JsonUtility.ToJson(exportSerialzerHolder, true);
        return jsonString;
    }
    public void ReadBackupFile()
    {
        string[] filters = new string[] { "text files", "txt", "All files", "*" };
        string path = EditorUtility.OpenFilePanelWithFilters("Load backup file", "", filters);

        if (string.IsNullOrEmpty(path)) return;



        var stringArray = File.ReadLines(path).Where(line => !line.StartsWith("//")).ToArray();
        var newString = string.Empty;

        foreach (var item in stringArray)
        {
            newString += item;
        }

        ExportSerialzerHolder exportSerialzerHolder = JsonUtility.FromJson<ExportSerialzerHolder>(newString);
        // create pair list from load
        List<PlayerPrefPair> pairs = new List<PlayerPrefPair>();

        foreach (var item in exportSerialzerHolder.exportlist)
        {
            PlayerPrefPair ppp = new PlayerPrefPair();
            ppp.Key = item.key;
            ppp.TempKey = item.key;
            ppp.type = item.type;

            switch (item.type)
            {
                case PlayerPrefsType.Int:
                    ppp.Value = int.Parse(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                case PlayerPrefsType.Float:
                    ppp.Value = float.Parse(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                case PlayerPrefsType.String:
                    ppp.Value = item.value;
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                case PlayerPrefsType.Vector2:
                    ppp.Value = PrefsSerialzer.StringToVector2(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                case PlayerPrefsType.Vector3:
                    ppp.Value = PrefsSerialzer.StringToVector3(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                case PlayerPrefsType.Vector4:
                    ppp.Value = PrefsSerialzer.StringToVector4(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;

                    break;
                case PlayerPrefsType.Color:
                    ppp.Value = PrefsSerialzer.StringToColor(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;

                    break;
                case PlayerPrefsType.Bool:
                    ppp.Value = PrefsSerialzer.StringToBool(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                case PlayerPrefsType.DateTime:
                    ppp.Value = PrefsSerialzer.StringToDateTime(item.value);
                    ppp.BackupValues = ppp.Value;
                    ppp.TempValue = ppp.Value;
                    break;
                default:
                    break;
            }
            ppp.Save();
        }
        Refresh();
    }

    private void SortWithName()
    {
        if (SortType != SortType.Name)
        {
            SortType = SortType.Name;
            Refresh();
        }
    }
    private void SortWithType()
    {
        if (SortType != SortType.Type)
        {
            SortType = SortType.Type;
            Refresh();
        }
    }
}

