using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum SortType
    {
        None,
        Name,
        Type
    }
    internal class PlayerPrefsWindow : EditorWindow
    {
        #region Player Pref Holder Class
        internal class PlayerPrefHolder : ScriptableObject
        {
            public string Key;
            public string TempKey;

            public object Value;
            public object TempValue;

            public object BackupValues;

            public PlayerPrefsType type;

            public bool isKeyFounded = false;
            public bool isValueFounded = false;
            public bool isEncrypted = false;

            public SerializedProperty ValueProperty;
            private SerializedObject so;

            public int[] array;
            public void Init()
            {
                array = AdvancedPlayerPrefs.StringToArrayInt(Value.ToString());

                ScriptableObject t = this;

                so = new SerializedObject(t);

                ValueProperty = so.FindProperty("array");
                Debug.Log("array count : " + array.Length);
            }
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
                        AdvancedPlayerPrefs.SetInt(Key, (int)Value, isEncrypted);
                        break;
                    case PlayerPrefsType.Float:
                        AdvancedPlayerPrefs.SetFloat(Key, Value.ToString(), isEncrypted);
                        break;
                    case PlayerPrefsType.String:
                        AdvancedPlayerPrefs.SetString(Key, Value.ToString(), isEncrypted);
                        break;
                    case PlayerPrefsType.Vector3:
                        AdvancedPlayerPrefs.SetVector3(Key, AdvancedPlayerPrefs.StringToVector3(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Vector2:
                        AdvancedPlayerPrefs.SetVector2(Key, AdvancedPlayerPrefs.StringToVector2(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Color:
                        AdvancedPlayerPrefs.SetColor(Key, AdvancedPlayerPrefs.StringToColor(Value.ToString()), false, isEncrypted);
                        break;
                    case PlayerPrefsType.HDRColor:
                        AdvancedPlayerPrefs.SetColor(Key, AdvancedPlayerPrefs.StringToColor(Value.ToString()), true, isEncrypted);
                        break;
                    case PlayerPrefsType.Vector4:
                        AdvancedPlayerPrefs.SetVector4(Key, AdvancedPlayerPrefs.StringToVector4(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Bool:
                        AdvancedPlayerPrefs.SetBool(Key, AdvancedPlayerPrefs.StringToBool(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.DateTime:
                        if (AdvancedPlayerPrefs.StringToDateTime(Value.ToString()) != null)
                        {
                            AdvancedPlayerPrefs.SetDateTime(Key, ((DateTime)AdvancedPlayerPrefs.StringToDateTime(Value.ToString())), isEncrypted);
                        }
                        break;
                    case PlayerPrefsType.Byte:
                        AdvancedPlayerPrefs.SetByte(Key, AdvancedPlayerPrefs.StringToByte(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Double:
                        AdvancedPlayerPrefs.SetDoube(Key, AdvancedPlayerPrefs.StringToDouble(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Vector2Int:
                        AdvancedPlayerPrefs.SetVector2Int(Key, AdvancedPlayerPrefs.StringToVector2Int(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Vector3Int:
                        AdvancedPlayerPrefs.SetVector3Int(Key, AdvancedPlayerPrefs.StringToVector3Int(Value.ToString()), isEncrypted);
                        break;
                    case PlayerPrefsType.Array:
                        AdvancedPlayerPrefs.SetArray(Key,array, isEncrypted);
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
                        if (isEncrypted)
                        {
                            var ic = AdvancedPlayerPrefs.StringToInt(Value.ToString());
                            var it = AdvancedPlayerPrefs.StringToInt(TempValue.ToString());
                            returnValue = ic == it;
                        }
                        else
                        {
                            returnValue = (int)TempValue == (int)Value;
                        }
                        break;
                    case PlayerPrefsType.Float:
                        if (isEncrypted)
                        {
                            var ic = AdvancedPlayerPrefs.StringToFloat(Value.ToString());
                            var it = AdvancedPlayerPrefs.StringToFloat(TempValue.ToString());
                            returnValue = ic == it;
                        }
                        else
                        {
                            returnValue = (float)TempValue == (float)Value;
                        }
                        break;
                    case PlayerPrefsType.String:
                        returnValue = String.Equals(TempValue, Value);
                        break;
                    case PlayerPrefsType.Vector2:
                        var v2c = AdvancedPlayerPrefs.StringToVector2(Value.ToString());
                        var v2t = AdvancedPlayerPrefs.StringToVector2(TempValue.ToString());
                        returnValue = v2c == v2t;

                        break;
                    case PlayerPrefsType.Vector3:
                        var v3c = AdvancedPlayerPrefs.StringToVector3(Value.ToString());
                        var v3t = AdvancedPlayerPrefs.StringToVector3(TempValue.ToString());
                        returnValue = v3c == v3t;
                        break;
                    case PlayerPrefsType.Vector4:
                        var v4c = AdvancedPlayerPrefs.StringToVector4(Value.ToString());
                        var v4t = AdvancedPlayerPrefs.StringToVector4(TempValue.ToString());
                        returnValue = v4c == v4t;
                        break;
                    case PlayerPrefsType.Color:
                        var cc = AdvancedPlayerPrefs.StringToColor(Value.ToString());
                        var ct = AdvancedPlayerPrefs.StringToColor(TempValue.ToString());
                        returnValue = cc == ct;
                        break;
                    case PlayerPrefsType.HDRColor:
                        var hcc = AdvancedPlayerPrefs.StringToColor(Value.ToString());
                        var hct = AdvancedPlayerPrefs.StringToColor(TempValue.ToString());
                        returnValue = hcc == hct;
                        break;
                    case PlayerPrefsType.Bool:
                        var bc = AdvancedPlayerPrefs.StringToBool(Value.ToString());
                        var bt = AdvancedPlayerPrefs.StringToBool(TempValue.ToString());
                        returnValue = bc == bt;
                        break;
                    case PlayerPrefsType.DateTime:
                        var tc = AdvancedPlayerPrefs.StringToDateTime(Value.ToString());
                        var tt = AdvancedPlayerPrefs.StringToDateTime(TempValue.ToString());
                        returnValue = tc == tt;
                        break;
                    case PlayerPrefsType.Byte:
                        var byc = AdvancedPlayerPrefs.StringToByte(Value.ToString());
                        var byt = AdvancedPlayerPrefs.StringToByte(TempValue.ToString());
                        returnValue = byc == byt;
                        break;
                    case PlayerPrefsType.Double:
                        var dc = AdvancedPlayerPrefs.StringToDouble(Value.ToString());
                        var dt = AdvancedPlayerPrefs.StringToDouble(TempValue.ToString());
                        returnValue = dc == dt;
                        break;
                    case PlayerPrefsType.Vector2Int:
                        var v2ic = AdvancedPlayerPrefs.StringToVector2Int(Value.ToString());
                        var v2it = AdvancedPlayerPrefs.StringToVector2Int(TempValue.ToString());
                        returnValue = v2ic == v2it;
                        break;
                    case PlayerPrefsType.Vector3Int:
                        var v3ic = AdvancedPlayerPrefs.StringToVector3Int(Value.ToString());
                        var v3it = AdvancedPlayerPrefs.StringToVector3Int(TempValue.ToString());
                        returnValue = v3ic == v3it;
                        break;
                    case PlayerPrefsType.Array:
                        returnValue = true;
                        //var ttt = AdvancedPlayerPrefs.StringToArrayInt(array.ToString());
                        //var tttt = AdvancedPlayerPrefs.StringToArrayInt(array.ToString());
                        //returnValue = ttt == tttt;
                        break;
                    default:
                        break;
                }
                return returnValue;
            }
        }
        #endregion

        #region Private Variables
        private static readonly System.Text.Encoding encoding = new System.Text.UTF8Encoding();

        private List<PlayerPrefHolder> PlayerPrefHolderList = new List<PlayerPrefHolder>();
        private List<PlayerPrefHolder> FiltredPlayerPrefHolderList = new List<PlayerPrefHolder>();

        private SortType PlayerPrefsSortType;

        private bool ShowEditorPrefs = true;
        private bool EditorPrefsAvailable = true;
        private bool EncryptionSettingsFounded;

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
        private Texture ExportButtonIcon;


        private string ExportPath
        {
            get => EditorPrefs.GetString(nameof(PlayerPrefsWindow) + "." + nameof(ExportPath));
            set
            {
                if (SavePathType == SavePathType.Absolute)
                    EditorPrefs.SetString(nameof(PlayerPrefsWindow) + "." + nameof(ExportPath), value);
                else
                    tempExportPath = value;
            }
        }
        private SavePathType SavePathType
        {
            get => (SavePathType)EditorPrefs.GetInt(nameof(PlayerPrefsWindow) + "." + nameof(SavePathType), 0);
            set => EditorPrefs.SetInt(nameof(PlayerPrefsWindow) + "." + nameof(SavePathType), (int)value);
        }
        private SavePathType oldSavePathType;

        [SerializeField] string Key = "";

        [SerializeField] PlayerPrefsType type;

        [SerializeField] object value;

        private int valuetempint;
        private float valuetempfloat;
        private string valuetempString;
        private double valuetempDouble;
        private byte valuetempByte;
        private Vector2 valuetempVector2;
        private Vector2Int valuetempVector2Int;
        private Vector3 valuetempVector3;
        private Vector3Int valuetempVector3Int;
        private Vector4 valuetempVecotr4;
        private Color valuetempColor;
        private Color valuetempHDRColor;
        private bool valuetempBool;
        private DateTime valueDateTime;
        private string oldKey;

        private bool UseEncryption;
        private bool DisplayAddPlayerPrefs;
        private bool DisplayExportPlayerPrefs = true;
        private string tempExportPath;
        private string ImportCompanyName;
        private string ImportProductName;


        #endregion

        TestScriptable testScriptable;
        #region Unity editor Tool 
        [MenuItem(AdvancedPlayerPrefsGlobalVariables.AdvancedPlayerPrefsToolMenuName, priority = 2)]
        public static void ShowWindow()
        {
            PlayerPrefsWindow PlayerPrefsWindow = (PlayerPrefsWindow)GetWindow(typeof(PlayerPrefsWindow));
            PlayerPrefsWindow.titleContent = new GUIContent(AdvancedPlayerPrefsGlobalVariables.AdvancedPlayerPrefsToolTitle);
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

            RefreshButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.RefreshButtonIconTexturePath, typeof(Texture));
            SaveButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.SaveButtonIconTexturePath, typeof(Texture));
            RevertButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.RevertButtonIconTexturePath, typeof(Texture));
            DeleteButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.DeleteButtonIconTexturePath, typeof(Texture));
            ApplyAllButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.ApplyAllButtonIconTexturePath, typeof(Texture));
            ExportButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.ExportButtonIconTexturePath, typeof(Texture));
            testScriptable = Resources.Load<TestScriptable>("AdvancedPlayerPrefs/test");

            GetAllPlayerPrefs();
            FiltredPlayerPrefHolderList.Clear();
            EncryptionSettingsFounded = AdvancedPlayerPrefs.SelectSettings(false);

            ScriptableObject target = testScriptable;

            //tempExportPath = ExportPath;
        }
        #endregion

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
            EditorGUILayout.Space(5);
            DrawHorizontalLine(Color.grey);
            EditorGUILayout.Space(5);
            DrawValueField();
            EditorGUILayout.Space(5);
            DrawHorizontalLine(Color.grey);
            DrawExportFields();
            EditorGUILayout.Space(5);


            //so.ApplyModifiedProperties(); // Remember to apply modified properties

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        private void DrawToolbarGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            DrawShowEditorPrefsButton();
            DrawSearchField();
            DrawRefreshButton();
            DrawApplyAll();
            DrawRevertAll();
            DrawDeletAllButton();
            GUILayout.EndHorizontal();
        }
        private void DrawSearchField()
        {
            float buttonWidth = (EditorGUIUtility.currentViewWidth) / 3f;

            SearchText = GUILayout.TextField(SearchText, 25, GUI.skin.FindStyle(AdvancedPlayerPrefsGlobalVariables.ToolbarSeachTextField), GUILayout.Width(buttonWidth + 15));
            if (GUILayout.Button("", GUI.skin.FindStyle(AdvancedPlayerPrefsGlobalVariables.ToolbarSearchCancelButton)))
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
            ShowEditorPrefs = GUILayout.Toggle(ShowEditorPrefs, "Show Editor prefs", EditorStyles.miniButton, GUILayout.Width(FullbuttonWidth + 4));
            if (EditorPrefsAvailable != ShowEditorPrefs)
            {
                Refresh();
                EditorPrefsAvailable = ShowEditorPrefs;
            }
        }
        private void DrawDeletAllButton()
        {
            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 5f;
            FullbuttonWidth = (FullbuttonWidth / 7) * 4;

            if (GUILayout.Button("Delete All", GUILayout.Width(FullbuttonWidth + 4)))
            {
                DeleteAll();
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

            if (GUILayout.Toggle(PlayerPrefsSortType == SortType.Name, "Key", styletoolbar, GUILayout.Width(FullbuttonWidth * 3)))
            {
                if (PlayerPrefsSortType != SortType.Name)
                {
                    PlayerPrefsSortType = SortType.Name;
                    Refresh();
                }

            }
            GUILayout.Label("Value", style, GUILayout.MinWidth(200), GUILayout.Width(FullbuttonWidth * 4));

            if (GUILayout.Toggle(PlayerPrefsSortType == SortType.Type, "Type", styletoolbar, GUILayout.Width(FullbuttonWidth * 1.5f)))
            {
                if (PlayerPrefsSortType != SortType.Type)
                {
                    PlayerPrefsSortType = SortType.Type;
                    Refresh();
                }
            }
            GUILayout.Label("Modify", style, GUILayout.MinWidth(110), GUILayout.Width((FullbuttonWidth * 1.5f) + 10));

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
                    if (_playerPrefsHolderList[i].isEncrypted)
                    {
                        style3.normal.textColor = Color.magenta;
                    }
                    else
                    {
                        style3.normal.textColor = Color.white;
                    }
                    GUILayout.Label(_playerPrefsHolderList[i].TempKey, style3, GUILayout.Width(FullWindowWidth * 3f));

                    style3.normal.textColor = Color.white;
                    style3.fontStyle = FontStyle.Normal;
                }

                switch (_playerPrefsHolderList[i].type)
                {
                    case PlayerPrefsType.Int:
                        if (_playerPrefsHolderList[i].isEncrypted)
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.IntField(AdvancedPlayerPrefs.StringToInt(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        }
                        else
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.IntField((int)_playerPrefsHolderList[i].TempValue, GUILayout.Width(FullWindowWidth * 4));
                        }
                        break;
                    case PlayerPrefsType.Float:
                        if (_playerPrefsHolderList[i].isEncrypted)
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.FloatField(AdvancedPlayerPrefs.StringToFloat(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        }
                        else
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.FloatField((float)_playerPrefsHolderList[i].TempValue, GUILayout.Width(FullWindowWidth * 4));
                        }
                        break;
                    case PlayerPrefsType.String:
                        _playerPrefsHolderList[i].TempValue = GUILayout.TextArea(_playerPrefsHolderList[i].TempValue.ToString(), EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector3:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector3Field("", AdvancedPlayerPrefs.StringToVector3(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector2:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector2Field("", AdvancedPlayerPrefs.StringToVector2(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Color:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.ColorField(GUIContent.none, AdvancedPlayerPrefs.StringToColor(_playerPrefsHolderList[i].TempValue.ToString()), true, true, false, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.HDRColor:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.ColorField(GUIContent.none, AdvancedPlayerPrefs.StringToColor(_playerPrefsHolderList[i].TempValue.ToString()), true, true, true, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector4:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector4Field("", AdvancedPlayerPrefs.StringToVector4(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Bool:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Toggle("", AdvancedPlayerPrefs.StringToBool(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.DateTime:
                        GUILayout.TextArea(AdvancedPlayerPrefs.StringToDateTime(_playerPrefsHolderList[i].TempValue.ToString()).ToString(), EditorStyles.toolbarTextField, GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Byte:
                        _playerPrefsHolderList[i].TempValue = Mathf.Clamp(EditorGUILayout.IntField((int)AdvancedPlayerPrefs.StringToByte(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4)), 0, 255);
                        break;
                    case PlayerPrefsType.Double:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.DoubleField(AdvancedPlayerPrefs.StringToDouble(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector2Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector2IntField("", AdvancedPlayerPrefs.StringToVector2Int(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Vector3Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector3IntField("", AdvancedPlayerPrefs.StringToVector3Int(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(FullWindowWidth * 4));
                        break;
                    case PlayerPrefsType.Array:
                        //if (_playerPrefsHolderList[i].ValueProperty != null)
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        //   // True means show c$$anonymous$$ldren
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
        private void DrawHorizontalLine(Color color)
        {
            var horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = 1;
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }
        private void DrawValueField()
        {
            float FullWindowWidth = (EditorGUIUtility.currentViewWidth - 20) / 10;
            GUIStyle style3 = EditorStyles.textField;
            GUIStyle style4 = EditorStyles.popup;
            style4.alignment = TextAnchor.MiddleCenter;

            DisplayAddPlayerPrefs = EditorGUILayout.BeginFoldoutHeaderGroup(DisplayAddPlayerPrefs, "Add Player Prefs");

            if (DisplayAddPlayerPrefs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();

                GUILayout.Label("Key", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth));
                EditorGUILayout.Space(3);

                GUILayout.Label("Type", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth));
                EditorGUILayout.Space(3);

                GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth));
                EditorGUILayout.Space(3);

                GUILayout.EndVertical();
                GUILayout.BeginVertical();

                Key = GUILayout.TextField(Key, style3, GUILayout.Width(FullWindowWidth * 4f));

                EditorGUILayout.Space(3);

                type = (PlayerPrefsType)EditorGUILayout.EnumPopup(type, style4, GUILayout.Width(FullWindowWidth * 4f));

                EditorGUILayout.Space(3);

                switch (type)
                {
                    case PlayerPrefsType.Int:
                        valuetempint = EditorGUILayout.IntField(valuetempint, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempint;
                        break;
                    case PlayerPrefsType.Float:
                        valuetempfloat = EditorGUILayout.FloatField(valuetempfloat, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempfloat;
                        break;
                    case PlayerPrefsType.String:
                        valuetempString = EditorGUILayout.TextField(valuetempString, GUILayout.ExpandHeight(true), GUILayout.MinWidth(200), GUILayout.MinHeight(100), GUILayout.ExpandWidth(true));
                        value = valuetempString;
                        break;
                    case PlayerPrefsType.Vector2:
                        valuetempVector2 = EditorGUILayout.Vector2Field("", valuetempVector2, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempVector2;
                        break;
                    case PlayerPrefsType.Vector3:
                        valuetempVector3 = EditorGUILayout.Vector3Field("", valuetempVector3, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempVector3;
                        break;
                    case PlayerPrefsType.Vector4:
                        valuetempVecotr4 = EditorGUILayout.Vector4Field("", valuetempVecotr4, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempVecotr4;
                        break;
                    case PlayerPrefsType.Color:
                        valuetempColor = EditorGUILayout.ColorField(valuetempColor, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempColor;
                        break;
                    case PlayerPrefsType.Bool:
                        valuetempBool = EditorGUILayout.ToggleLeft("", valuetempBool, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempBool;
                        break;
                    case PlayerPrefsType.Byte:
                        valuetempByte = (byte)Mathf.Clamp(EditorGUILayout.IntField((int)valuetempByte, GUILayout.Width(FullWindowWidth * 4f)), 0, 255);
                        value = valuetempByte;
                        break;
                    case PlayerPrefsType.Double:
                        valuetempDouble = EditorGUILayout.DoubleField(valuetempDouble, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempDouble;
                        break;
                    case PlayerPrefsType.Vector2Int:
                        valuetempVector2Int = EditorGUILayout.Vector2IntField("", valuetempVector2Int, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempVector2Int;
                        break;
                    case PlayerPrefsType.Vector3Int:
                        valuetempVector3Int = EditorGUILayout.Vector3IntField("", valuetempVector3Int, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempVector3Int;
                        break;
                    case PlayerPrefsType.HDRColor:
                        valuetempHDRColor = EditorGUILayout.ColorField(GUIContent.none, valuetempHDRColor, true, true, true, GUILayout.Width(FullWindowWidth * 4f));
                        value = valuetempHDRColor;
                        break;
                    case PlayerPrefsType.DateTime:
                        value = DateTime.MinValue;
                        GUILayout.TextArea(valueDateTime.ToString(), EditorStyles.toolbarTextField, GUILayout.Width(FullWindowWidth * 4f));
                        break;
                }
                EditorGUILayout.Space(3);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                GUILayout.Label("Encryption", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 1.1f));
                EditorGUILayout.Space(3);
                GUI.enabled = EncryptionSettingsFounded;

                UseEncryption = EditorGUILayout.Toggle(UseEncryption);
                GUI.enabled = true;

                EditorGUILayout.Space(1);

                if (GUILayout.Button("Select Settings"))
                {
                    EncryptionSettingsFounded = AdvancedPlayerPrefs.SelectSettings();
                    if (!EncryptionSettingsFounded)
                    {
                        int dialogResult = EditorUtility.DisplayDialogComplex(
                "No Encryption Settings founded !",
                "Do you want to create an encryption settings file ?",
                "Yes", "Don't Create", "Cancel");

                        switch (dialogResult)
                        {
                            case 0: //Create backup
                                AdvancedPlayerPrefs.CreateSettings();
                                EncryptionSettingsFounded = AdvancedPlayerPrefs.SelectSettings();
                                break;
                            case 1: //Don't create a backup
                                break;
                            case 2: //Cancel process (Basically do nothing for now.)
                                break;
                            default:
                                Debug.LogWarning("Something went wrong when creating settings keys");
                                break;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space(7);

                EditorGUILayout.BeginHorizontal();
                float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;
                if (AdvancedPlayerPrefs.HasKey(Key) || string.IsNullOrEmpty(Key))
                {
                    GUI.enabled = false;
                }
                // Delete all PlayerPrefs
                if (GUILayout.Button("Add " + Key + " Prefs", GUILayout.Width(buttonWidth)))
                {
                    AddPlayerPref(Key, type, value, UseEncryption);
                }
                GUI.enabled = true;

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear", GUILayout.Width(buttonWidth)))
                {
                    Key = "";
                    valuetempint = 0;
                    type = PlayerPrefsType.Int;
                    UseEncryption = false;
                }

                EditorGUILayout.EndHorizontal();

            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        private void DrawExportFields()
        {
            float FullWindowWidth = (EditorGUIUtility.currentViewWidth - 20) / 20;
            GUIStyle style3 = EditorStyles.textField;
            GUIStyle style4 = EditorStyles.miniButton;
            style4.alignment = TextAnchor.MiddleCenter;


            GUIStyle LabelStyle = EditorStyles.boldLabel;
            LabelStyle.stretchWidth = false;

            DisplayExportPlayerPrefs = EditorGUILayout.BeginFoldoutHeaderGroup(DisplayExportPlayerPrefs, "Export/Import Settings");

            if (DisplayExportPlayerPrefs)
            {
                EditorGUILayout.Space(10);
                GUILayout.Label("Export settings :", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 4));
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Export path", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 3));

                SavePathType = (SavePathType)EditorGUILayout.EnumPopup(SavePathType, GUILayout.Width(FullWindowWidth * 3f));

                GUILayout.Space(5);
                GUI.enabled = false;

                tempExportPath = GUILayout.TextField(tempExportPath, style3, GUILayout.Width(FullWindowWidth * 10f));
                GUI.enabled = true;

                if (SavePathType != oldSavePathType && SavePathType != SavePathType.Absolute)
                {
                    tempExportPath = AdvancedPlayerPrefsExportManager.GetPathByType(SavePathType);
                    oldSavePathType = SavePathType;
                }
                if (SavePathType != oldSavePathType && SavePathType == SavePathType.Absolute)
                {
                    tempExportPath = ExportPath;
                    oldSavePathType = SavePathType;
                }
                GUILayout.Space(5);

                if (GUILayout.Button(new GUIContent("...", "Select Export Folder"), style4, GUILayout.Width(FullWindowWidth)))
                {
                    string path = EditorUtility.OpenFolderPanel("Backup path", "", "");
                    if (path.Length > 0)
                    {
                        SavePathType = SavePathType.Absolute;
                        ExportPath = path;
                        tempExportPath = ExportPath;
                    }
                }
                GUILayout.Space(5);

                if (GUILayout.Button(new GUIContent(ExportButtonIcon, "Open Export Folder"), EditorStyles.miniButtonMid, GUILayout.Width(FullWindowWidth / 1.5f)))
                {
                    AdvancedPlayerPrefsExportManager.ShowExplorer(tempExportPath);
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;
                GUILayout.Space(buttonWidth / 3 + 50);

                if (GUILayout.Button(new GUIContent("Export"), GUILayout.Width(buttonWidth / 2)))
                {
                    Export();
                }
                GUILayout.Space(5);


                if (GUILayout.Button("Import from file", GUILayout.Width(buttonWidth / 2)))
                {
                    GetBackupFromFile(tempExportPath);
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(10);
                GUILayout.Label("Import From settings :", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 5));
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Company Name", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 4));

                GUILayout.Space(5);

                ImportCompanyName = GUILayout.TextField(ImportCompanyName, style3, GUILayout.Width(FullWindowWidth * 10f));

                if (string.IsNullOrEmpty(ImportCompanyName) || string.IsNullOrEmpty(ImportProductName))
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button("Import from Settings", GUILayout.Width(buttonWidth / 2)))
                {
                    Import(ImportCompanyName, ImportProductName);
                }
                GUI.enabled = true;

                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();

                GUILayout.Label("Product Name", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 4));

                GUILayout.Space(5);

                ImportProductName = GUILayout.TextField(ImportProductName, style3, GUILayout.Width(FullWindowWidth * 10f));

                GUILayout.EndHorizontal();

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

        }
        #endregion

        #region Data Management Region
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
                        PlayerPrefHolder pair = ScriptableObject.CreateInstance<PlayerPrefHolder>();

                        if (savedValue.GetType() == typeof(int) || savedValue.GetType() == typeof(long))
                        {
                            if (AdvancedPlayerPrefs.GetInt(key, -1) == -1 && AdvancedPlayerPrefs.GetInt(key, 0) == 0)
                            {
                                string savedStringValue = savedValue.ToString();
                                savedValue = AdvancedPlayerPrefs.GetFloat(key);
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

                            ReturnType returnValues = null;

                            savedValue = AdvancedPlayerPrefs.TryGetCostumeType(key, out returnValues, savedValue.ToString());

                            pair.type = returnValues.PlayerPrefsType;
                            pair.isEncrypted = returnValues.IsEncrypted;
                           

                        }

                        pair.Value = savedValue;
                        pair.TempValue = savedValue;
                        pair.BackupValues = savedValue;
                        pair.Key = key;
                        pair.TempKey = key;
                        if (pair.type == PlayerPrefsType.Array)
                        {
                            pair.Init();
                        }
                        tempPlayerPrefs[i] = pair;

                        i++;
                    }
                    PlayerPrefHolderList = tempPlayerPrefs.ToList();
                    switch (PlayerPrefsSortType)
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
        #endregion

        #region ADD/Remove Prefs Region
        internal void AddPlayerPref(string key, PlayerPrefsType playerPrefsType, object value, bool useEncryption)
        {
            switch (playerPrefsType)
            {
                case PlayerPrefsType.Int:
                    AdvancedPlayerPrefs.SetInt(key, (int)value, useEncryption);
                    break;
                case PlayerPrefsType.Float:
                    AdvancedPlayerPrefs.SetFloat(key, value.ToString(), useEncryption);
                    break;
                case PlayerPrefsType.String:
                    AdvancedPlayerPrefs.SetString(key, (string)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector2:
                    AdvancedPlayerPrefs.SetVector2(key, (Vector2)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector3:
                    AdvancedPlayerPrefs.SetVector3(key, (Vector3)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector4:
                    AdvancedPlayerPrefs.SetVector4(key, (Vector4)value, useEncryption);
                    break;
                case PlayerPrefsType.Color:
                    AdvancedPlayerPrefs.SetColor(key, (Color)value, false, useEncryption);
                    break;
                case PlayerPrefsType.HDRColor:
                    AdvancedPlayerPrefs.SetColor(key, (Color)value, true, useEncryption);
                    break;
                case PlayerPrefsType.Bool:
                    AdvancedPlayerPrefs.SetBool(key, (bool)value, useEncryption);
                    break;
                case PlayerPrefsType.Byte:
                    AdvancedPlayerPrefs.SetByte(key, (byte)value, useEncryption);
                    break;
                case PlayerPrefsType.Double:
                    AdvancedPlayerPrefs.SetDoube(key, (double)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector2Int:
                    AdvancedPlayerPrefs.SetVector2Int(key, (Vector2Int)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector3Int:
                    AdvancedPlayerPrefs.SetVector3Int(key, (Vector3Int)value, useEncryption);
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
        #endregion

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

        private void GetBackupFromFile(string tempPath)
        {
            //AdvancedPlayerPrefsExportManager.m_exportPath = tempExportPath;

            var backupPairs = AdvancedPlayerPrefsExportManager.ReadBackupFile(tempPath);
            if (backupPairs != null)
            {
                Refresh();
            }
        }
        private void Export()
        {
            AdvancedPlayerPrefsExportManager.Export(PlayerPrefHolderList, ExportPath, SavePathType);
        }
        #endregion
    }

}