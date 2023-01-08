﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    public enum SortType
    {
        None,
        Name,
        Type
    }
    public class PlayerPrefsWindow : EditorWindow
    {
        #region Player Pref Holder Class
        private class PlayerPrefHolder
        {
            public string Key;
            public string TempKey;

            public object Value;
            public object TempValue;

            public object BackupValues;

            public PlayerPrefsType type;

            public bool isKeyFounded = false;
            public bool isValueFounded = false;
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
                        PrefsSerialzer.SetColor(Key, PrefsSerialzer.StringToColor(Value.ToString()),false);
                        break;
                    case PlayerPrefsType.HDRColor:
                        PrefsSerialzer.SetColor(Key, PrefsSerialzer.StringToColor(Value.ToString()), true);
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
                    case PlayerPrefsType.Byte:
                        PrefsSerialzer.SetByte(Key, PrefsSerialzer.StringToByte(Value.ToString()));
                        break;
                    case PlayerPrefsType.Double:
                        PrefsSerialzer.SetDoube(Key, PrefsSerialzer.StringToDouble(Value.ToString()));
                        break;
                    case PlayerPrefsType.Vector2Int:
                        PrefsSerialzer.SetVector2Int(Key, PrefsSerialzer.StringToVector2Int(Value.ToString()));
                        break;
                    case PlayerPrefsType.Vector3Int:
                        PrefsSerialzer.SetVector3Int(Key, PrefsSerialzer.StringToVector3Int(Value.ToString()));
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
                bool returnValue = false;

                switch (type)
                {
                    case PlayerPrefsType.Int:
                        returnValue = (int)TempValue == (int)Value;
                        break;
                    case PlayerPrefsType.Float:
                        returnValue = (float)TempValue == (float)Value;
                        break;
                    case PlayerPrefsType.String:
                        returnValue = String.Equals(TempValue.ToString(), Value.ToString());
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
                    case PlayerPrefsType.HDRColor:
                        var hcc = PrefsSerialzer.StringToColor(Value.ToString());
                        var hct = PrefsSerialzer.StringToColor(TempValue.ToString());
                        returnValue = hcc == hct;
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
                    case PlayerPrefsType.Byte:
                        var byc = PrefsSerialzer.StringToByte(Value.ToString());
                        var byt = PrefsSerialzer.StringToByte(TempValue.ToString());
                        returnValue = byc == byt;
                        break;
                    case PlayerPrefsType.Double:
                        var dc = PrefsSerialzer.StringToDouble(Value.ToString());
                        var dt = PrefsSerialzer.StringToDouble(TempValue.ToString());
                        returnValue = dc == dt;
                        break;
                    case PlayerPrefsType.Vector2Int:
                        var v2ic = PrefsSerialzer.StringToVector2Int(Value.ToString());
                        var v2it = PrefsSerialzer.StringToVector2Int(TempValue.ToString());
                        returnValue = v2ic == v2it;
                        break;
                    case PlayerPrefsType.Vector3Int:
                        var v3ic = PrefsSerialzer.StringToVector3Int(Value.ToString());
                        var v3it = PrefsSerialzer.StringToVector3Int(TempValue.ToString());
                        returnValue = v3ic == v3it;
                        break;
                    default:
                        break;
                }
                return returnValue;
            }
        }
        #endregion

        private static readonly System.Text.Encoding encoding = new System.Text.UTF8Encoding();

        private List<PlayerPrefHolder> PlayerPrefHolderList = new List<PlayerPrefHolder>();
        private readonly List<PlayerPrefHolder> FiltredPlayerPrefHolderList = new List<PlayerPrefHolder>();

        private SortType SortType;

        private bool ShowEditorPrefs = true;
        private bool EditorPrefsAvailable = true;

        private string SearchText = "";
        private string OldSearchFilter = ".";

        private Action OnDeleteElement;

        private RegistryKey RegistryKey;
        private string CompanyName;
        private string ProductName;

        private Vector2 ScrollViewPosition;
        private Texture RefreshButtonIcon;
        private Texture SaveButtonIcon;
        private Texture RevertButtonIcon;
        private Texture DeleteButtonIcon;
        private Texture ApplyAllButtonIcon;

        [MenuItem("DavanciCode/PlayerPrefs Manager")]
        public static void ShowWindow()
        {
            PlayerPrefsWindow PlayerPrefsWindow = (PlayerPrefsWindow)GetWindow(typeof(PlayerPrefsWindow));
            PlayerPrefsWindow.titleContent = new GUIContent("Player Prefs Manager");
            PlayerPrefsWindow.Show();
            Vector2 minSize = PlayerPrefsWindow.minSize;
            minSize.x = 600;
            PlayerPrefsWindow.minSize = minSize;
        }
        private void OnEnable()
        {
            CompanyName = PlayerSettings.companyName;
            ProductName = PlayerSettings.productName;

            RegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + CompanyName + "\\" + ProductName);

            RefreshButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/refresh_Icon.png", typeof(Texture));
            SaveButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/save_Icon.png", typeof(Texture));
            RevertButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/reset_Icon.png", typeof(Texture));
            DeleteButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/delete_Icon.png", typeof(Texture));
            ApplyAllButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/apply_Icon.png", typeof(Texture));

            GetAllPlayerPrefs();
            FiltredPlayerPrefHolderList.Clear();
        }

        #region GUI Region
        private void OnGUI()
        {
            GUILayout.BeginVertical();

            DrawToolbarGUI();

            if (!String.IsNullOrEmpty(SearchText))
            {
                UpdateSearch();
                DrawPlayerPrefs(FiltredPlayerPrefHolderList, true);
            }
            else
            {
                DrawPlayerPrefs(PlayerPrefHolderList);
            }
            EditorGUILayout.Space(10);
            DrawBottomButtons();
            EditorGUILayout.Space(10);

            GUILayout.EndVertical();
        }
        private void DrawToolbarGUI()
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
        private void DrawImpExpButton()
        {
            float buttonWidth = (EditorGUIUtility.currentViewWidth) / 4f;
            if (GUILayout.Button("Import/Export", EditorStyles.toolbarDropDown, GUILayout.Width((EditorGUIUtility.currentViewWidth) / 4f)))
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
        private void DrawSearchField()
        {
            float buttonWidth = (EditorGUIUtility.currentViewWidth) / 4f;

            SearchText = GUILayout.TextField(SearchText, 25, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(buttonWidth));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                SearchText = "";
                GUI.FocusControl(null);
            }
        }
        private void DrawRefreshButton()
        {
            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
            FullbuttonWidth = (FullbuttonWidth / 7) / 1.5f;

            if (GUILayout.Button(new GUIContent(RefreshButtonIcon, "Refresh all PlayerPrefs data"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
            {
                Refresh();
            }
        }
        private void DrawShowEditorPrefsButton()
        {
            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
            FullbuttonWidth = (FullbuttonWidth / 7) * 4;
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

            if (GUILayout.Button(new GUIContent(ApplyAllButtonIcon, "Save all Changes"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
            {
                foreach (var item in PlayerPrefHolderList)
                {
                    item.Save();
                }
            }
        }
        private void DrawRevertAll()
        {
            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
            FullbuttonWidth = (FullbuttonWidth / 7);

            if (GUILayout.Button(new GUIContent(RevertButtonIcon, "Revert all changes"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
            {
                foreach (var item in PlayerPrefHolderList)
                {
                    item.BackUp();
                }
            }
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

            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 10;

            if (GUILayout.Toggle(SortType == SortType.Name, "Key", styletoolbar, GUILayout.Width(FullbuttonWidth * 3)))
            {
                if (SortType != SortType.Name)
                {
                    SortType = SortType.Name;
                    Refresh();
                }

            }
            GUILayout.Label("Value", style, GUILayout.MinWidth(200), GUILayout.Width(FullbuttonWidth * 4));
            if (GUILayout.Toggle(SortType == SortType.Type, "Type", styletoolbar, GUILayout.Width(FullbuttonWidth * 1.5f)))
            {
                if (SortType != SortType.Type)
                {
                    SortType = SortType.Type;
                    Refresh();
                }
            }
            GUILayout.Label("Modify", style, GUILayout.MinWidth(110), GUILayout.Width((FullbuttonWidth * 1.5f)+10));

            GUI.backgroundColor = oldBackgroundColor;

            GUILayout.EndHorizontal();
        }
        private void DrawPlayerPrefs(List<PlayerPrefHolder> _playerPrefsHolderList, bool isSearchDraw = false)
        {
            GUIStyle style;
            Color oldBackgroundColor;

            DrawTitles(out style, out oldBackgroundColor);
            float FullWindowWidth = (EditorGUIUtility.currentViewWidth - 20) / 10;

            ScrollViewPosition = GUILayout.BeginScrollView(ScrollViewPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(EditorGUIUtility.currentViewWidth));


            for (int i = 0; i < _playerPrefsHolderList.Count; i++)
            {
                if (!ShowEditorPrefs)
                {
                    if (_playerPrefsHolderList[i].TempKey.ToLower().Contains("unity"))
                        continue;
                }

                GUILayout.BeginHorizontal();
                GUIStyle style3 = EditorStyles.toolbar;
                GUIStyle style4 = EditorStyles.toggle;
                style3 = EditorStyles.textField;
                Color oldstylecolor = style.normal.textColor;

                style4.normal.textColor = Color.red;
                if (!_playerPrefsHolderList[i].isEqual())
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
                    if (_playerPrefsHolderList[i].isKeyFounded)
                    {
                        style3.normal.textColor = Color.yellow;

                        GUILayout.Label(_playerPrefsHolderList[i].TempKey, style3, GUILayout.Width(FullWindowWidth * 3f));
                        style3.normal.textColor = oldstylecolor;
                    }
                    else
                    {
                        GUILayout.Label(_playerPrefsHolderList[i].TempKey, style3, GUILayout.Width(FullWindowWidth * 3f));
                    }
                }
                else
                {
                    GUILayout.Label(_playerPrefsHolderList[i].TempKey, style3, GUILayout.Width(FullWindowWidth * 3f));
                }
                if (isSearchDraw)
                {
                    if (_playerPrefsHolderList[i].isValueFounded)
                    {
                        style3.normal.textColor = Color.yellow;
                        style3.fontStyle = FontStyle.Bold;
                    }
                    else
                    {
                        style3.normal.textColor = Color.white;
                        style3.fontStyle = FontStyle.Normal;
                    }
                }
                else
                {
                    style3.normal.textColor = Color.white;
                    style3.fontStyle = FontStyle.Normal;
                }
                switch (_playerPrefsHolderList[i].type)
                {
                    case PlayerPrefsType.Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.IntField((int)_playerPrefsHolderList[i].TempValue, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Float:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.FloatField((float)_playerPrefsHolderList[i].TempValue, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.String:
                        _playerPrefsHolderList[i].TempValue = GUILayout.TextArea(_playerPrefsHolderList[i].TempValue.ToString(), EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector3:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector3Field("", PrefsSerialzer.StringToVector3(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector2:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector2Field("", PrefsSerialzer.StringToVector2(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Color:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.ColorField(GUIContent.none,PrefsSerialzer.StringToColor(_playerPrefsHolderList[i].TempValue.ToString()),true,true, false,GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.HDRColor:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.ColorField(GUIContent.none, PrefsSerialzer.StringToColor(_playerPrefsHolderList[i].TempValue.ToString()), true, true, true, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector4:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector4Field("", PrefsSerialzer.StringToVector4(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Bool:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Toggle("", PrefsSerialzer.StringToBool(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.DateTime:
                        GUILayout.TextArea(PrefsSerialzer.StringToDateTime(_playerPrefsHolderList[i].TempValue.ToString()).ToString(), EditorStyles.toolbarTextField, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Byte:
                        _playerPrefsHolderList[i].TempValue = Mathf.Clamp(EditorGUILayout.IntField((int)PrefsSerialzer.StringToByte(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4)),0,255);
                        break;
                    case PlayerPrefsType.Double:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.DoubleField(PrefsSerialzer.StringToDouble(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector2Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector2IntField("", PrefsSerialzer.StringToVector2Int(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector3Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector3IntField("", PrefsSerialzer.StringToVector3Int(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    default:
                        break;
                }
                style3.normal.textColor = oldstylecolor;

                GUIStyle style2 = EditorStyles.miniTextField;
                style2.fontSize = 12;
                style2.fontStyle = FontStyle.Bold;
                style2.alignment = TextAnchor.MiddleCenter;

                GUILayout.Label(_playerPrefsHolderList[i].type.ToString(), style2, GUILayout.Width(FullWindowWidth * 1.5f));

                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(new GUIContent(SaveButtonIcon, "Save current data"), EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.45f)))
                {
                    _playerPrefsHolderList[i].SaveKey();
                }

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(new GUIContent(RevertButtonIcon, "Reset data to default"), EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.5f)))
                {
                    _playerPrefsHolderList[i].BackUp();
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button(new GUIContent(DeleteButtonIcon, "Delete PlayerPrefs data"), EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.45f)))
                {
                    _playerPrefsHolderList[i].Delete();
                    OnDeleteElement += Refresh;

                    if (FiltredPlayerPrefHolderList.Contains(_playerPrefsHolderList[i]))
                        FiltredPlayerPrefHolderList.Remove(_playerPrefsHolderList[i]);
                }
                GUI.backgroundColor = oldBackgroundColor;
                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
            OnDeleteElement?.Invoke();
            OnDeleteElement -= Refresh;
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

            if (GUILayout.Button("Delete All", GUILayout.Width(buttonWidth)))
            {
                DeleteAll();
            }

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        private void UpdateRegistry()
        {
            RegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + CompanyName + "\\" + ProductName);
        }
        private void GetAllPlayerPrefs()
        {
            if (RegistryKey == null) return;

            PlayerPrefHolderList.Clear();
            foreach (string item in RegistryKey.GetValueNames())
            {
                if (RegistryKey != null)
                {
                    string[] valueNames = RegistryKey.GetValueNames();
                    PlayerPrefHolder[] tempPlayerPrefs = new PlayerPrefHolder[valueNames.Length];

                    int i = 0;
                    foreach (string valueName in valueNames)
                    {
                        string key = valueName;
                        int index = key.LastIndexOf("_");
                        key = key.Remove(index, key.Length - index);

                        object savedValue = RegistryKey.GetValue(valueName);
                        PlayerPrefHolder pair = new PlayerPrefHolder();

                        if (savedValue.GetType() == typeof(int) || savedValue.GetType() == typeof(long))
                        {
                            if (PrefsSerialzer.GetInt(key, -1) == -1 && PrefsSerialzer.GetInt(key, 0) == 0)
                            {
                                string savedStringValue = savedValue.ToString();
                                savedValue = PrefsSerialzer.GetFloat(key);
                                pair.type = PlayerPrefsType.Float;
                            }
                            else
                            {
                                pair.type = PlayerPrefsType.Int;
                            }
                        }
                        else if (savedValue.GetType() == typeof(byte[]))
                        {
                            savedValue = encoding.GetString((byte[])savedValue).TrimEnd('\0');

                            PlayerPrefsType type = PlayerPrefsType.String;

                            savedValue = PrefsSerialzer.TryGetCostumeType(key, out type, savedValue.ToString());

                            pair.type = type;
                        }

                        pair.Value = savedValue;
                        pair.TempValue = savedValue;
                        pair.BackupValues = savedValue;
                        pair.Key = key;
                        pair.TempKey = key;

                        tempPlayerPrefs[i] = pair;
                        i++;
                    }
                    PlayerPrefHolderList = tempPlayerPrefs.ToList();
                    switch (SortType)
                    {
                        case SortType.Name:
                            PlayerPrefHolderList = PlayerPrefHolderList.OrderBy(go => go.Key).ToList();
                            break;
                        case SortType.Type:
                            PlayerPrefHolderList = PlayerPrefHolderList.OrderBy(go => go.type).ToList();
                            break;
                    }
                }
            }
        }
        private void UpdateSearch()
        {
            if (SearchText.Equals(OldSearchFilter))
            {
                return;
            }
            FiltredPlayerPrefHolderList.Clear();

            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }

            int entryCount = PlayerPrefHolderList.Count;

            for (int i = 0; i < entryCount; i++)
            {
                PlayerPrefHolderList[i].isKeyFounded = false;
                PlayerPrefHolderList[i].isValueFounded = false;

                string fullKey = PlayerPrefHolderList[i].Key;
                string displayKey = fullKey;

                string fullvalue = PlayerPrefHolderList[i].TempValue.ToString();

                if (displayKey.ToLower().Contains(SearchText.ToLower()))
                {
                    FiltredPlayerPrefHolderList.Add(PlayerPrefHolderList[i]);
                    PlayerPrefHolderList[i].isKeyFounded = true;
                }
                if (fullvalue.ToLower().Contains(SearchText.ToLower()))
                {
                    if (!FiltredPlayerPrefHolderList.Contains(PlayerPrefHolderList[i]))
                    {
                        FiltredPlayerPrefHolderList.Add(PlayerPrefHolderList[i]);
                    }
                    PlayerPrefHolderList[i].isValueFounded = true;
                }
            }
            OldSearchFilter = SearchText;
        }
        private void Refresh()
        {
            UpdateRegistry();
            GetAllPlayerPrefs();
        }
        internal void AddPlayerPref(string key, PlayerPrefsType playerPrefsType, object value)
        {
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
                    PrefsSerialzer.SetColor(key, (Color)value,false);
                    break;
                case PlayerPrefsType.HDRColor:
                    PrefsSerialzer.SetColor(key, (Color)value, true);
                    break;
                case PlayerPrefsType.Bool:
                    PrefsSerialzer.SetBool(key, (bool)value);
                    break;
                case PlayerPrefsType.Byte:
                    PrefsSerialzer.SetByte(key, (byte)value);
                    break;
                case PlayerPrefsType.Double:
                    PrefsSerialzer.SetDoube(key, (double)value);
                    break;
                case PlayerPrefsType.Vector2Int:
                    PrefsSerialzer.SetVector2Int(key, (Vector2Int)value);
                    break;
                case PlayerPrefsType.Vector3Int:
                    PrefsSerialzer.SetVector3Int(key, (Vector3Int)value);
                    break;
                default:
                    break;
            }
            Refresh();
        }
        private void DeleteAll()
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

        #region Import Export Region
        public void Import(string importCompanyName, string importProductName)
        {
            string currentCompanyName = PlayerSettings.companyName;
            string currentProductName = PlayerSettings.productName;

            PlayerSettings.productName = importProductName;
            PlayerSettings.companyName = importCompanyName;

            RegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + importCompanyName + "\\" + importProductName);

            GetAllPlayerPrefs();

            PlayerSettings.productName = currentProductName;
            PlayerSettings.companyName = currentCompanyName;

            Debug.Log("import");
            foreach (var pref in PlayerPrefHolderList)
            {
                pref.Save();
            }

        }
        private void Import()
        {
            ImportPrefsWizard wizard = ScriptableWizard.DisplayWizard<ImportPrefsWizard>("Import PlayerPrefs", "Import");

            Debug.Log("Import PlayerPrefs");
        }
        private void Export()
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

            foreach (var item in PlayerPrefHolderList)
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
        private void ReadBackupFile()
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
            List<PlayerPrefHolder> pairs = new List<PlayerPrefHolder>();

            foreach (var item in exportSerialzerHolder.exportlist)
            {
                PlayerPrefHolder ppp = new PlayerPrefHolder();
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
                    case PlayerPrefsType.HDRColor:
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
                    case PlayerPrefsType.Byte:
                        ppp.Value = PrefsSerialzer.StringToByte(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Double:
                        ppp.Value = PrefsSerialzer.StringToDouble(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector2Int:
                        ppp.Value = PrefsSerialzer.StringToVector2Int(item.value);
                        ppp.BackupValues = ppp.Value;
                        ppp.TempValue = ppp.Value;
                        break;
                    case PlayerPrefsType.Vector3Int:
                        ppp.Value = PrefsSerialzer.StringToVector3Int(item.value);
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
        #endregion
    }

}