using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal enum SortType
    {
        None,
        Name,
        Type
    }
    [Serializable]
    internal class PinnedPrefrences
    {

        [SerializeField] List<string> PinnedKeys = new List<string>();

        public PinnedPrefrences(bool t)
        {
            LoadInit();
        }

        public PinnedPrefrences()
        {
        }

        internal bool ContainsKey(string key)
        {
            return PinnedKeys.Contains(key);
        }
        internal void HandlePrefrence(string key, bool isPinned)
        {
            if (isPinned)
            {
                RemovePinned(key);
                DavanciDebug.Log(key + " Unpinned ", Color.red);
            }
            else
            {
                AddPinned(key);
                DavanciDebug.Log(key + " pinned ", Color.green);
            }
        }
        private void AddPinned(string key)
        {
            if (PinnedKeys.Contains(key)) return;
            PinnedKeys.Add(key);
            Save();
        }
        private void RemovePinned(string key)
        {
            if (!PinnedKeys.Contains(key)) return;
            PinnedKeys.Remove(key);
            Save();
        }
        internal void Save()
        {
            string t = JsonUtility.ToJson(this);
            EditorPrefs.SetString("PinnedPrefrences", t);
        }
        internal List<string> Load()
        {
            var t = JsonUtility.FromJson<PinnedPrefrences>(EditorPrefs.GetString("PinnedPrefrences"));

            if (t == null) return new List<string>();
            return t.PinnedKeys;
        }
        internal void LoadInit()
        {
            string tt = EditorPrefs.GetString("PinnedPrefrences");
            var t = JsonUtility.FromJson<PinnedPrefrences>(tt);

            if (t == null)
            {
                PinnedKeys = new List<string>();
                Save();
            }
            else
            {
                PinnedKeys = t.PinnedKeys;
            }
        }
    }

    internal class AdvancedPlayerPrefsTool : EditorWindow
    {
        #region Private Variables
        private static readonly System.Text.Encoding encoding = new System.Text.UTF8Encoding();

        private List<PlayerPrefHolder> PlayerPrefHolderList = new List<PlayerPrefHolder>();
        private readonly List<PlayerPrefHolder> FiltredPlayerPrefHolderList = new List<PlayerPrefHolder>();

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
        private Texture PinButtonIcon;
        private Texture UnpinButtonIcon;
        private string ExportPath
        {
            get => EditorPrefs.GetString(nameof(AdvancedPlayerPrefsTool) + "." + nameof(ExportPath));
            set
            {
                if (SavePathType == SavePathType.Absolute)
                    EditorPrefs.SetString(nameof(AdvancedPlayerPrefsTool) + "." + nameof(ExportPath), value);
                else
                    tempExportPath = value;
            }
        }
        private SavePathType SavePathType
        {
            get => (SavePathType)EditorPrefs.GetInt(nameof(AdvancedPlayerPrefsTool) + "." + nameof(SavePathType), 0);
            set => EditorPrefs.SetInt(nameof(AdvancedPlayerPrefsTool) + "." + nameof(SavePathType), (int)value);
        }
        private SavePathType oldSavePathType = SavePathType.None;

        [SerializeField] string Key = "";

        [SerializeField] PlayerPrefsType type;

        [SerializeField] object value;

        private int valuetempint;
        private float valuetempfloat;
        private string valuetempString;
        private double valuetempDouble;
        private long valuetempLong;
        private byte valuetempByte;
        private Vector2 valuetempVector2;
        private Vector2Int valuetempVector2Int;
        private Vector3 valuetempVector3;
        private Vector3Int valuetempVector3Int;
        private Vector4 valuetempVecotr4;
        private Color valuetempColor = Color.white;
        private Color valuetempHDRColor = Color.white;
        private bool valuetempBool;
        private DateTime valueDateTime = DateTime.Now;

        public int[] arrayInt;
        public float[] arrayFloat;
        public string[] arrayString;
        public bool[] arrayBool;
        public byte[] arrayByte;
        public double[] arrayDouble;
        public long[] arrayLong;
        public Vector3[] arrayVector3;
        public Vector3Int[] arrayVector3Int;
        public Vector2[] arrayVector2;
        public Vector2Int[] arrayVector2Int;
        public Vector4[] arrayVector4;


        public SerializedProperty ValueProperty;
        public SerializedObject so;

        private bool UseEncryption;
        private readonly bool DisplayAddPlayerPrefs;
        private bool DisplayExportPlayerPrefs;
        private string tempExportPath;
        private string ImportCompanyName;
        private string ImportProductName;

        private bool isProSKin;
        private bool UseAutoEncryption;

        private PinnedPrefrences pinnedPrefrences;
        #endregion
        private static AdvancedPlayerPrefsTool AdvancedPlayerPrefsToolInstance;
        #region Unity editor Tool   
        [MenuItem(AdvancedPlayerPrefsGlobalVariables.AdvancedPlayerPrefsToolMenuName, priority = 1)]
        internal static void ShowWindow()
        {
            AdvancedPlayerPrefsToolInstance = (AdvancedPlayerPrefsTool)GetWindow(typeof(AdvancedPlayerPrefsTool));
            AdvancedPlayerPrefsToolInstance.titleContent = new GUIContent(AdvancedPlayerPrefsGlobalVariables.AdvancedPlayerPrefsToolTitle);
            AdvancedPlayerPrefsToolInstance.Show();
            Vector2 minSize = AdvancedPlayerPrefsToolInstance.minSize;
            minSize.x = 600;
            minSize.y = 500;
            AdvancedPlayerPrefsToolInstance.minSize = minSize;
        }

        private void OnEnable()
        {
            pinnedPrefrences = new PinnedPrefrences(true);

            CompanyName = PlayerSettings.companyName;
            ProductName = PlayerSettings.productName;

            RegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + CompanyName + "\\" + ProductName);

            RefreshButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.RefreshButtonIconTexturePath, typeof(Texture));
            SaveButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.SaveButtonIconTexturePath, typeof(Texture));
            RevertButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.RevertButtonIconTexturePath, typeof(Texture));
            DeleteButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.DeleteButtonIconTexturePath, typeof(Texture));
            ApplyAllButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.ApplyAllButtonIconTexturePath, typeof(Texture));
            ExportButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.ExportButtonIconTexturePath, typeof(Texture));
            PinButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.PinButtonIconTexturePath, typeof(Texture));
            UnpinButtonIcon = (Texture)AssetDatabase.LoadAssetAtPath(AdvancedPlayerPrefsGlobalVariables.UnpinButtonIconTexturePath, typeof(Texture));
            GetAllPlayerPrefs();
            FiltredPlayerPrefHolderList.Clear();
            EncryptionSettingsFounded = AdvancedPlayerPrefs.SelectSettings(false);
            so = new SerializedObject(this);

            isProSKin = EditorGUIUtility.isProSkin;
            UseAutoEncryption = (bool)AdvancedPlayerPrefs.AutoEncryption;
            AdvancedPlayerPrefs.OnPreferenceUpdated += OnPrefrenceUpdated;
            //tempExportPath = ExportPath;
        }
        private void OnDisable()
        {
            so?.Dispose();
            AdvancedPlayerPrefs.OnPreferenceUpdated -= OnPrefrenceUpdated;
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
            EditorGUI.BeginChangeCheck();

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
                RefreshWithLog();
            }
        }
        private void DrawShowEditorPrefsButton()
        {
            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
            FullbuttonWidth = (FullbuttonWidth / 7) * 4;
            ShowEditorPrefs = GUILayout.Toggle(ShowEditorPrefs, "Show Editor prefs", EditorStyles.miniButton, GUILayout.Width(FullbuttonWidth + 4));
            if (EditorPrefsAvailable != ShowEditorPrefs)
            {
                RefreshWithoutLog();
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
            FullbuttonWidth /= 7;

            if (GUILayout.Button(new GUIContent(ApplyAllButtonIcon, "Save all Changes"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
            {
                foreach (var item in PlayerPrefHolderList)
                {
                    item.Save();
                    DavanciDebug.Log("All Changes are saved !", Color.green);

                }
            }
        }
        private void DrawRevertAll()
        {
            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth) / 2f;
            FullbuttonWidth /= 7;

            if (GUILayout.Button(new GUIContent(RevertButtonIcon, "Revert all changes"), EditorStyles.toolbarButton, GUILayout.Width(FullbuttonWidth)))
            {
                foreach (var item in PlayerPrefHolderList)
                {
                    item.BackUp();
                    DavanciDebug.Log("Revert all Changes !", Color.cyan);
                }
            }
        }
        private void DrawTitles()
        {
            var style = new GUIStyle(EditorStyles.toolbar)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            var oldBackgroundColor = GUI.backgroundColor;

            GUIStyle styletoolbar = new GUIStyle(EditorStyles.toolbarDropDown)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            GUI.backgroundColor = new Color(0.56f, 0.56f, 0.56f);
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            float FullbuttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 10;

            if (GUILayout.Toggle(PlayerPrefsSortType == SortType.Name, "Key", styletoolbar, GUILayout.Width(FullbuttonWidth * 3)))
            {
                if (PlayerPrefsSortType != SortType.Name)
                {
                    PlayerPrefsSortType = SortType.Name;
                    RefreshWithoutLog();
                }

            }
            GUILayout.Label("Value", style, GUILayout.MinWidth(200), GUILayout.Width(FullbuttonWidth * 3.5f));

            if (GUILayout.Toggle(PlayerPrefsSortType == SortType.Type, "Type", styletoolbar, GUILayout.Width(FullbuttonWidth * 1.5f)))
            {
                if (PlayerPrefsSortType != SortType.Type)
                {
                    PlayerPrefsSortType = SortType.Type;
                    RefreshWithoutLog();
                }
            }
            GUILayout.Label("Modify", style, GUILayout.MinWidth(110), GUILayout.Width((FullbuttonWidth * 2f) + 10));

            GUI.backgroundColor = oldBackgroundColor;

            GUILayout.EndHorizontal();
        }
        private void DecideFieldColor(PlayerPrefHolder playerPrefHolder, bool inSearch, GUIStyle style)
        {
            if (playerPrefHolder.isEncrypted)
            {
                style.normal.textColor = isProSKin ? AdvancedPlayerPrefsGlobalVariables.ProEncryptedTextColor : AdvancedPlayerPrefsGlobalVariables.NormalEncryptedTextColor; //Color.magenta;
            }
            if (!playerPrefHolder.IsEqual())
            {
                style.normal.textColor = isProSKin ? AdvancedPlayerPrefsGlobalVariables.ProChangedTextColor : AdvancedPlayerPrefsGlobalVariables.NormalChangedTextColor;
                style.fontStyle = FontStyle.Bold;
            }

            if (inSearch)
            {
                switch (playerPrefHolder.InSearch)
                {
                    case FoundInSearch.Key:
                        style.normal.textColor = isProSKin ? AdvancedPlayerPrefsGlobalVariables.ProSearchTextColor : AdvancedPlayerPrefsGlobalVariables.NormalSearchTextColor;  //Color.yellow;
                        style.fontStyle = FontStyle.Normal;
                        break;
                    case FoundInSearch.Value:

                        break;
                }
            }
        }
        private void DrawPlayerPrefs(List<PlayerPrefHolder> _playerPrefsHolderList, bool isSearchDraw = false)
        {
            DrawTitles();
            bool lastPrefDrawnPinned = false;
            Color oldBackgroundColor = GUI.backgroundColor;
            GUIStyle style3 = EditorStyles.textField;


            float FullWindowWidth = (EditorGUIUtility.currentViewWidth - 20) / 10;
            float valueLength = FullWindowWidth * 3.5f;

            ScrollViewPosition = GUILayout.BeginScrollView(ScrollViewPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Width(EditorGUIUtility.currentViewWidth));

            for (int i = 0; i < _playerPrefsHolderList.Count; i++)
            {
                if (lastPrefDrawnPinned && !_playerPrefsHolderList[i].Pinned)
                {
                    //draw separation
                    GUILayout.Space(3f);

                    DrawHorizontalLine(Color.grey, 3f);
                    GUILayout.Space(1f);
                    DrawHorizontalLine(Color.grey, 3f);
                    GUILayout.Space(3f);

                }
                lastPrefDrawnPinned = _playerPrefsHolderList[i].Pinned;

                if (!ShowEditorPrefs)
                {
                    if (_playerPrefsHolderList[i].TempKey.ToLower().Contains("unity"))
                        continue;
                }

                GUILayout.BeginHorizontal();

                Color oldstylecolor = style3.normal.textColor;

                style3.fontStyle = FontStyle.Normal;
                style3.normal.textColor = oldstylecolor;

                DecideFieldColor(_playerPrefsHolderList[i], isSearchDraw, style3);

                GUILayout.Label(_playerPrefsHolderList[i].TempKey, style3, GUILayout.Width(FullWindowWidth * 3f));

                if (isSearchDraw)
                {
                    switch (_playerPrefsHolderList[i].InSearch)
                    {
                        case FoundInSearch.None:
                            style3.normal.textColor = oldstylecolor;
                            style3.fontStyle = FontStyle.Normal;
                            break;
                        case FoundInSearch.Key:
                            style3.normal.textColor = oldstylecolor;
                            style3.fontStyle = FontStyle.Normal;
                            break;
                        case FoundInSearch.Value:
                            style3.normal.textColor = isProSKin ? AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColor : AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;  //Color.yellow;
                            style3.fontStyle = FontStyle.Normal;
                            break;
                    }
                }
                else
                {
                    style3.normal.textColor = oldstylecolor;
                    style3.fontStyle = FontStyle.Normal;
                }

                switch (_playerPrefsHolderList[i].type)
                {
                    case PlayerPrefsType.Int:
                        if (_playerPrefsHolderList[i].isEncrypted)
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.IntField(AdvancedPlayerPrefs.StringToInt(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        }
                        else
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.IntField((int)_playerPrefsHolderList[i].TempValue, GUILayout.Width(valueLength));
                        }
                        break;
                    case PlayerPrefsType.Float:
                        if (_playerPrefsHolderList[i].isEncrypted)
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.FloatField(AdvancedPlayerPrefs.StringToFloat(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        }
                        else
                        {
                            _playerPrefsHolderList[i].TempValue = EditorGUILayout.FloatField((float)_playerPrefsHolderList[i].TempValue, GUILayout.Width(valueLength));
                        }
                        break;
                    case PlayerPrefsType.String:
                        _playerPrefsHolderList[i].TempValue = GUILayout.TextArea(_playerPrefsHolderList[i].TempValue.ToString(), EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Vector3:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector3Field("", AdvancedPlayerPrefs.StringToVector3(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Vector2:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector2Field("", AdvancedPlayerPrefs.StringToVector2(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Color:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.ColorField(GUIContent.none, AdvancedPlayerPrefs.StringToColor(_playerPrefsHolderList[i].TempValue.ToString()), true, true, false, GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.HDRColor:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.ColorField(GUIContent.none, AdvancedPlayerPrefs.StringToColor(_playerPrefsHolderList[i].TempValue.ToString()), true, true, true, GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Vector4:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector4Field("", AdvancedPlayerPrefs.StringToVector4(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Bool:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Toggle("", AdvancedPlayerPrefs.StringToBool(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.DateTime:
                        GUILayout.TextArea(AdvancedPlayerPrefs.StringToDateTime(_playerPrefsHolderList[i].TempValue.ToString()).ToString(), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Byte:
                        _playerPrefsHolderList[i].TempValue = Mathf.Clamp(EditorGUILayout.IntField((int)AdvancedPlayerPrefs.StringToByte(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength)), 0, 255);
                        break;
                    case PlayerPrefsType.Double:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.DoubleField(AdvancedPlayerPrefs.StringToDouble(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Long:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.LongField(AdvancedPlayerPrefs.StringToLong(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Vector2Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector2IntField("", AdvancedPlayerPrefs.StringToVector2Int(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.Vector3Int:
                        _playerPrefsHolderList[i].TempValue = EditorGUILayout.Vector3IntField("", AdvancedPlayerPrefs.StringToVector3Int(_playerPrefsHolderList[i].TempValue.ToString()), GUILayout.Width(valueLength));
                        break;
                    case PlayerPrefsType.ArrayInt:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();

                        }
                        break;
                    case PlayerPrefsType.ArrayFloat:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;

                    case PlayerPrefsType.ArrayBool:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayByte:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayDouble:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayVector3:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayVector3Int:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayString:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayLong:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayVector2:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayVector2Int:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    case PlayerPrefsType.ArrayVector4:
                        if (_playerPrefsHolderList[i].ValueProperty != null && _playerPrefsHolderList[i].so != null)
                        {
                            _playerPrefsHolderList[i].so.Update();
                            EditorGUILayout.PropertyField(_playerPrefsHolderList[i].ValueProperty, GUIContent.none, true, GUILayout.Width(valueLength));
                            _playerPrefsHolderList[i].so.ApplyModifiedProperties();
                        }
                        break;
                    default:
                        break;
                }
                style3.normal.textColor = oldstylecolor;

                GUIStyle style2 = EditorStyles.miniTextField;
                style2.fontSize = 12;
                style2.fontStyle = FontStyle.Bold;
                style2.alignment = TextAnchor.MiddleCenter;

                GUILayout.Label(AdvancedPlayerPrefsGlobalVariables.TypeList[(int)_playerPrefsHolderList[i].type], style2, GUILayout.Width(FullWindowWidth * 1.5f));

                var pinned = _playerPrefsHolderList[i].Pinned;
                var key = _playerPrefsHolderList[i].Key;

                GUI.backgroundColor = pinned ? AdvancedPlayerPrefsGlobalVariables.UnpinButtonColor : AdvancedPlayerPrefsGlobalVariables.PinButtonColor;

                GUIContent g = new GUIContent(
                    pinned ? UnpinButtonIcon : PinButtonIcon,
                    pinned ? "Unpin <" + key + ">" : "Pin <" + key + ">");
                if (GUILayout.Button(g, EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.4f)))
                {
                    pinnedPrefrences.HandlePrefrence(key, pinned);
                    _playerPrefsHolderList[i].Pinned = !pinned;
                    PlayerPrefHolderList = PlayerPrefHolderList
                   .OrderByDescending(x => x.Pinned)
                   .ThenBy(x => x.originalIndex)
                   .ToList();
                    //RefreshWithoutLog();
                    //  Repaint();
                }

                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(new GUIContent(SaveButtonIcon, "Save <" + key + "> current data"), EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.5f)))
                {
                    _playerPrefsHolderList[i].SaveKey();
                    DavanciDebug.Log("Save " + key, Color.green);
                }

                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button(new GUIContent(RevertButtonIcon, "Reset <" + key + "> data to default"), EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.5f)))
                {
                    DavanciDebug.Log("Reset " + key, Color.cyan);
                    _playerPrefsHolderList[i].BackUp();
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button(new GUIContent(DeleteButtonIcon, "Delete <" + key + "> PlayerPrefs data"), EditorStyles.miniButton, GUILayout.Width(FullWindowWidth * 0.45f)))
                {

                    _playerPrefsHolderList[i].Delete();
                    OnDeleteElement += RefreshWithoutLog;

                    if (FiltredPlayerPrefHolderList.Contains(_playerPrefsHolderList[i]))
                        FiltredPlayerPrefHolderList.Remove(_playerPrefsHolderList[i]);
                    DavanciDebug.Log(_playerPrefsHolderList[i].Key + " is Deleted!", Color.red);

                }
                GUI.backgroundColor = oldBackgroundColor;
                GUILayout.EndHorizontal();
                style3.normal.textColor = oldstylecolor;
            }
            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();
            OnDeleteElement?.Invoke();
            OnDeleteElement -= RefreshWithoutLog;

        }
        private void DrawHorizontalLine(Color color, float fixedHeight = 1)
        {
            var horizontalLine = new GUIStyle();
            horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            horizontalLine.fixedHeight = fixedHeight;
            var c = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }
        private void DrawValueField()
        {
            float FullWindowWidth = (EditorGUIUtility.currentViewWidth - 20) / 10;


            GUILayout.Label("Add Player new Prefs", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 5));
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

            Key = GUILayout.TextField(Key, GUILayout.Width(FullWindowWidth * 4f));

            EditorGUILayout.Space(3);

            type = (PlayerPrefsType)EditorGUILayout.Popup((int)type, AdvancedPlayerPrefsGlobalVariables.EnumList, GUILayout.Width(FullWindowWidth * 4f));

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
                case PlayerPrefsType.Long:
                    valuetempLong = EditorGUILayout.LongField(valuetempLong, GUILayout.Width(FullWindowWidth * 4f));
                    value = valuetempLong;
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
                case PlayerPrefsType.ArrayInt:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayInt");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayFloat:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayFloat");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayBool:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayBool");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayDouble:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayDouble");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayByte:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayByte");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayVector3:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayVector3");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayVector3Int");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayString:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayString");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayLong:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayLong");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayVector2:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayVector2");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayVector2Int");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;
                case PlayerPrefsType.ArrayVector4:
                    so.Update();
                    ValueProperty = so.FindProperty("arrayVector4");
                    if (ValueProperty != null)
                    {
                        EditorGUILayout.PropertyField(ValueProperty, GUIContent.none, true, GUILayout.Width(FullWindowWidth * 4));
                        so.ApplyModifiedProperties();
                    }
                    break;

            }
            EditorGUILayout.Space(3);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUI.enabled = EncryptionSettingsFounded;
            GUI.enabled = !UseAutoEncryption;

            GUILayout.Label("Encryption", EditorStyles.boldLabel, GUILayout.Width(FullWindowWidth * 1.1f));
            EditorGUILayout.Space(3);
            UseAutoEncryption = (bool)AdvancedPlayerPrefs.AutoEncryption;

            if (UseAutoEncryption)
            {
                UseEncryption = UseAutoEncryption;
            }
            UseEncryption = EditorGUILayout.Toggle(UseEncryption);
            GUI.enabled = true;

            EditorGUILayout.Space(1);

            if (GUILayout.Button(EncryptionSettingsFounded ? "Select Settings" : "Create Settings"))
            {
                EncryptionSettingsFounded = AdvancedPlayerPrefs.SelectSettings();
                if (!EncryptionSettingsFounded)
                {
                    int dialogResult = EditorUtility.DisplayDialogComplex(
            "No Davanced Playerprefs Settings founded !",
            "Do you want to create a settings file ?",
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
                            DavanciDebug.Warning("Something went wrong when creating settings keys");
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

            if (GUILayout.Button("Add <" + Key + "> Prefs", GUILayout.Width(buttonWidth)))
            {
                DavanciDebug.Log("a " + AdvancedPlayerPrefsGlobalVariables.EnumList[(int)type] + " with key <" + Key + "> added !", Color.green);

                AddPlayerPref(Key, type, value, UseEncryption);
            }
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear", GUILayout.Width(buttonWidth)))
            {
                DavanciDebug.Log("All Fields are clear !", Color.cyan);

                InitValuesData();

                Key = "";
                valuetempint = 0;
                type = PlayerPrefsType.Int;
                UseEncryption = false;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void InitValuesData()
        {
            valuetempint = 0;
            valuetempfloat = 0;
            valuetempString = string.Empty;
            valuetempDouble = 0;
            valuetempLong = 0;
            valuetempByte = 0;
            valuetempVector2 = Vector2.zero;
            valuetempVector2Int = Vector2Int.zero;
            valuetempVector3 = Vector3.zero;
            valuetempVector3Int = Vector3Int.zero;
            valuetempVector3Int = Vector3Int.zero;
            valuetempVecotr4 = Vector4.zero;
            valuetempColor = Color.white;
            valuetempHDRColor = Color.white;
            valuetempBool = false;
            valueDateTime = DateTime.Now;
            arrayInt = new int[0];
            arrayFloat = new float[0];
            arrayBool = new bool[0];
            arrayByte = new byte[0];
            arrayDouble = new double[0];
            arrayLong = new long[0];
            arrayVector3 = new Vector3[0];
            arrayVector3Int = new Vector3Int[0];
            arrayVector2 = new Vector2[0];
            arrayVector2Int = new Vector2Int[0];
            arrayVector4 = new Vector4[0];
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

                SavePathType = (SavePathType)EditorGUILayout.Popup((int)SavePathType, AdvancedPlayerPrefsGlobalVariables.PathTypeList, GUILayout.Width(FullWindowWidth * 4.5f));

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
        private void OnPrefrenceUpdated(string _key, PlayerPrefsType playerPrefsType, bool isEncrypted)
        {
            PlayerPrefHolder updatePrefHolder = PlayerPrefHolderList.FirstOrDefault(x => x.Key == _key);
            object savedValue = null;
            if (isEncrypted)
            {
                savedValue = AdvancedPlayerPrefs.TryGetCostumeType(_key, out ReturnType returnValues);
            }
            else
            {
                switch (playerPrefsType)
                {
                    case PlayerPrefsType.Int:
                        savedValue = PlayerPrefs.GetInt(_key);
                        break;
                    case PlayerPrefsType.Float:
                        savedValue = PlayerPrefs.GetFloat(_key);
                        break;
                    case PlayerPrefsType.String:
                        savedValue = PlayerPrefs.GetString(_key);
                        break;
                    default:
                        savedValue = AdvancedPlayerPrefs.TryGetCostumeType(_key, out ReturnType returnValues);
                        break;
                }
            }
          

            if (updatePrefHolder != null)
            {
                Debug.Log("update ");
                //Update the value
                updatePrefHolder.Value = savedValue;
                updatePrefHolder.TempValue = savedValue;
                updatePrefHolder.BackupValues = savedValue;
                updatePrefHolder.isEncrypted= isEncrypted;
                updatePrefHolder.Init();
            }
            else
            {
                //add it to the list 
                Debug.Log("can't find it,create it !");
                PlayerPrefHolder newPrefHolder = ScriptableObject.CreateInstance<PlayerPrefHolder>();
                newPrefHolder.Value = savedValue;
                newPrefHolder.TempValue = savedValue;
                newPrefHolder.BackupValues = savedValue;
                newPrefHolder.Key = _key;
                newPrefHolder.TempKey = _key;
                newPrefHolder.Pinned = pinnedPrefrences.ContainsKey(_key);
                newPrefHolder.type = playerPrefsType;
                newPrefHolder.originalIndex = (ushort)(PlayerPrefHolderList.Count - 1);
                newPrefHolder.isEncrypted = isEncrypted;
                newPrefHolder.Init();
                PlayerPrefHolderList.Add(newPrefHolder);
                if (newPrefHolder.Pinned)
                    PlayerPrefHolderList = PlayerPrefHolderList.OrderByDescending(x => x.Pinned).ToList();
            }
            Repaint();
        }
        private void GetAllPlayerPrefs()
        {
            PlayerPrefHolderList.Clear();
#if UNITY_EDITOR_OSX
            string playerPrefsPath;

            string plistFilename = $"unity.{CompanyName}.{ProductName}.plist";
            // Now construct the fully qualified path
            playerPrefsPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences"), plistFilename);


            // Parse the PlayerPrefs file if it exists
            if (File.Exists(playerPrefsPath))
            {
                // Parse the plist then cast it to a Dictionary
                object plist = Plist.readPlist(playerPrefsPath);

                Dictionary<string, object> parsed = plist as Dictionary<string, object>;

                // Convert the dictionary data into an array of PlayerPrefPairs
                List<PlayerPrefHolder> tempPlayerPrefs = new List<PlayerPrefHolder>(parsed.Count);
                int i = 0;
                foreach (KeyValuePair<string, object> pair in parsed)
                {
                    PlayerPrefHolder playerPrefHolder = new PlayerPrefHolder();

                    playerPrefHolder.Key = pair.Key;
                    playerPrefHolder.Value = pair.Value;
                    var savedValue = pair.Value;
                    if (pair.Value.GetType() == typeof(double))
                    {
                        playerPrefHolder.type = PlayerPrefsType.Float;
                        playerPrefHolder.Value = (float)(double)pair.Value;

                        // Some float values may come back as double, so convert them back to floats
                       // tempPlayerPrefs.Add(new PlayerPrefHolder() { Key = pair.Key, Value = (float)(double)pair.Value });
                    }
                    else if (pair.Value.GetType() == typeof(bool))
                    {
                        // Unity PlayerPrefs API doesn't allow bools, so ignore them
                    }
                    else if (pair.Value.GetType() == typeof(int) || pair.Value.GetType() == typeof(long))
                    {
                        if (AdvancedPlayerPrefs.GetInt(pair.Key, -1) == -1 && AdvancedPlayerPrefs.GetInt(pair.Key, 0) == 0)
                        {
                            string savedStringValue = pair.Value.ToString();
                            savedValue = AdvancedPlayerPrefs.GetFloat(pair.Key);
                            playerPrefHolder.type = PlayerPrefsType.Float;
                        }
                        else
                        {
                            playerPrefHolder.type = PlayerPrefsType.Int;
                        }
                    }
                    else
                    {
                        ReturnType returnValues = null;

                        savedValue = AdvancedPlayerPrefs.TryGetCostumeType(pair.Key, out returnValues, savedValue.ToString());

                        playerPrefHolder.type = returnValues.PlayerPrefsType;
                        playerPrefHolder.isEncrypted = returnValues.IsEncrypted;
                        //tempPlayerPrefs.Add(new PlayerPrefHolder() { Key = pair.Key, Value = pair.Value });
                    }

                    playerPrefHolder.Value = savedValue;
                    playerPrefHolder.TempValue = savedValue;
                    playerPrefHolder.BackupValues = savedValue;
                    playerPrefHolder.Key = pair.Key;
                    playerPrefHolder.TempKey = pair.Key;
                    playerPrefHolder.Pinned = pinnedPrefrences.ContainsKey(pair.Key);
                    playerPrefHolder.Init();
                    tempPlayerPrefs.Add(playerPrefHolder);
                    playerPrefHolder.originalIndex = (ushort)i;
                    i++;
                }

                // Return the results
                PlayerPrefHolderList = tempPlayerPrefs.ToList();
            }
            else
            {
                // No existing PlayerPrefs saved (which is valid), so just return an empty array
                PlayerPrefHolderList = new List<PlayerPrefHolder>();
            }
#elif UNITY_EDITOR_WIN
            if (RegistryKey == null) return;

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

                            savedValue = AdvancedPlayerPrefs.TryGetCostumeType(key, out ReturnType returnValues, savedValue.ToString());

                            pair.type = returnValues.PlayerPrefsType;
                            pair.isEncrypted = returnValues.IsEncrypted;
                        }

                        pair.Value = savedValue;
                        pair.TempValue = savedValue;
                        pair.BackupValues = savedValue;
                        pair.Key = key;
                        pair.TempKey = key;
                        pair.originalIndex = (ushort)i;
                        pair.Pinned = pinnedPrefrences.ContainsKey(pair.Key);

                        pair.Init();

                        tempPlayerPrefs[i] = pair;

                        i++;
                    }
                    PlayerPrefHolderList = tempPlayerPrefs.ToList();

                }
            }
#else
     throw new NotSupportedException("Advanced PlayerPrefs doesn't support this Unity Editor platform");
#endif

            switch (PlayerPrefsSortType)
            {
                case SortType.Name:
                    PlayerPrefHolderList = PlayerPrefHolderList.OrderBy(go => go.Key).ToList();
                    break;
                case SortType.Type:
                    PlayerPrefHolderList = PlayerPrefHolderList.OrderBy(go => go.type).ToList();
                    break;
            }

            //Show pinned prefs first
            PlayerPrefHolderList = PlayerPrefHolderList.OrderByDescending(x => x.Pinned).ToList();

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
                PlayerPrefHolderList[i].InSearch = FoundInSearch.None;

                string fullKey = PlayerPrefHolderList[i].Key;
                string displayKey = fullKey;

                string fullvalue = PlayerPrefHolderList[i].TempValue.ToString();

                if (displayKey.ToLower().Contains(SearchText.ToLower()))
                {
                    FiltredPlayerPrefHolderList.Add(PlayerPrefHolderList[i]);
                    PlayerPrefHolderList[i].InSearch = FoundInSearch.Key;
                }
                if (fullvalue.ToLower().Contains(SearchText.ToLower()))
                {
                    if (!FiltredPlayerPrefHolderList.Contains(PlayerPrefHolderList[i]))
                    {
                        FiltredPlayerPrefHolderList.Add(PlayerPrefHolderList[i]);
                    }
                    PlayerPrefHolderList[i].InSearch = FoundInSearch.Value;
                }
            }
            OldSearchFilter = SearchText;
        }
        private void RefreshWithLog()
        {
            UpdateRegistry();
            GetAllPlayerPrefs();
            DavanciDebug.Log("Refreshing Prefs!", Color.cyan);
        }
        private void RefreshWithoutLog()
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
                case PlayerPrefsType.Long:
                    AdvancedPlayerPrefs.SetLong(key, (long)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector2Int:
                    AdvancedPlayerPrefs.SetVector2Int(key, (Vector2Int)value, useEncryption);
                    break;
                case PlayerPrefsType.Vector3Int:
                    AdvancedPlayerPrefs.SetVector3Int(key, (Vector3Int)value, useEncryption);
                    break;//valueArrayInt
                case PlayerPrefsType.ArrayInt:

                    AdvancedPlayerPrefs.SetArray(key, arrayInt, useEncryption);
                    break;
                case PlayerPrefsType.ArrayFloat:

                    AdvancedPlayerPrefs.SetArray(key, arrayFloat, useEncryption);
                    break;
                case PlayerPrefsType.ArrayBool:

                    AdvancedPlayerPrefs.SetArray(key, arrayBool, useEncryption);
                    break;
                case PlayerPrefsType.ArrayDouble:
                    AdvancedPlayerPrefs.SetArray(key, arrayDouble, useEncryption);
                    break;
                case PlayerPrefsType.ArrayByte:
                    AdvancedPlayerPrefs.SetArray(key, arrayByte, useEncryption);
                    break;
                case PlayerPrefsType.ArrayVector3:
                    AdvancedPlayerPrefs.SetArray(key, arrayVector3, useEncryption);
                    break;
                case PlayerPrefsType.ArrayVector3Int:
                    AdvancedPlayerPrefs.SetArray(key, arrayVector3Int, useEncryption);
                    break;
                case PlayerPrefsType.ArrayString:
                    AdvancedPlayerPrefs.SetArray(key, arrayString, useEncryption);
                    break;
                case PlayerPrefsType.ArrayLong:
                    AdvancedPlayerPrefs.SetArray(key, arrayLong, useEncryption);
                    break;
                case PlayerPrefsType.ArrayVector2:
                    AdvancedPlayerPrefs.SetArray(key, arrayVector2, useEncryption);
                    break;
                case PlayerPrefsType.ArrayVector2Int:
                    AdvancedPlayerPrefs.SetArray(key, arrayVector2Int, useEncryption);
                    break;
                case PlayerPrefsType.ArrayVector4:
                    AdvancedPlayerPrefs.SetArray(key, arrayVector4, useEncryption);
                    break;
                default:
                    break;
            }
            RefreshWithoutLog();
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
                    DavanciDebug.Log("All prefs are deleted !", Color.red);
                    RefreshWithoutLog();

                    break;
                case 1: //Don't create a backup
                    PlayerPrefs.DeleteAll();
                    DavanciDebug.Log("All prefs are deleted !", Color.red);
                    RefreshWithoutLog();
                    break;
                case 2: //Cancel process (Basically do nothing for now.)
                    break;
                default:
                    DavanciDebug.Warning("Something went wrong when clearing player prefs");
                    break;
            }
        }
        #endregion

        #region Import Export Region
        internal void Import(string importCompanyName, string importProductName)
        {
            string currentCompanyName = PlayerSettings.companyName;
            string currentProductName = PlayerSettings.productName;

            PlayerSettings.productName = importProductName;
            PlayerSettings.companyName = importCompanyName;

            RegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + importCompanyName + "\\" + importProductName);

            GetAllPlayerPrefs();
            int t = PlayerPrefHolderList.Count;

            PlayerSettings.productName = currentProductName;
            PlayerSettings.companyName = currentCompanyName;

            foreach (var pref in PlayerPrefHolderList)
            {
                pref.Save();
            }

            RefreshWithoutLog();

            if (t > 0)
            {
                DavanciDebug.Log(t + " Prefs Imported from < " + importProductName + "/" + importCompanyName + ">", Color.green);
            }
            else
            {
                DavanciDebug.Warning("No Prefs founded at < " + importProductName + "/" + importCompanyName + ">");
            }
        }
        private static List<PlayerPrefHolder> GetPlayerPrefs(string companyName, string productName)
        {
            List<PlayerPrefHolder> tempPlayerPrefs = new List<PlayerPrefHolder>();

#if UNITY_EDITOR_OSX

            string playerPrefsPath;

            string plistFilename = $"unity.{companyName}.{productName}.plist";
            // Now construct the fully qualified path
            playerPrefsPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Preferences"), plistFilename);

            // Parse the PlayerPrefs file if it exists
            if (File.Exists(playerPrefsPath))
            {
                // Parse the plist then cast it to a Dictionary
                object plist = Plist.readPlist(playerPrefsPath);

                Dictionary<string, object> parsed = plist as Dictionary<string, object>;

                // Convert the dictionary data into an array of PlayerPrefPairs
              
                foreach (KeyValuePair<string, object> pair in parsed)
                {
                    PlayerPrefHolder playerPrefHolder = new PlayerPrefHolder();

                    playerPrefHolder.Key = pair.Key;
                    playerPrefHolder.Value = pair.Value;
                    var savedValue = pair.Value;
                    if (pair.Value.GetType() == typeof(double))
                    {
                        playerPrefHolder.type = PlayerPrefsType.Float;
                        playerPrefHolder.Value = (float)(double)pair.Value;

                        // Some float values may come back as double, so convert them back to floats
                       // tempPlayerPrefs.Add(new PlayerPrefHolder() { Key = pair.Key, Value = (float)(double)pair.Value });
                    }
                    else if (pair.Value.GetType() == typeof(bool))
                    {
                        // Unity PlayerPrefs API doesn't allow bools, so ignore them
                    }
                    else if (pair.Value.GetType() == typeof(int) || pair.Value.GetType() == typeof(long))
                    {
                        if (AdvancedPlayerPrefs.GetInt(pair.Key, -1) == -1 && AdvancedPlayerPrefs.GetInt(pair.Key, 0) == 0)
                        {
                            string savedStringValue = pair.Value.ToString();
                            savedValue = AdvancedPlayerPrefs.GetFloat(pair.Key);
                            playerPrefHolder.type = PlayerPrefsType.Float;
                        }
                        else
                        {
                            playerPrefHolder.type = PlayerPrefsType.Int;
                        }
                    }
                    else
                    {
                        ReturnType returnValues = null;

                        savedValue = AdvancedPlayerPrefs.TryGetCostumeType(pair.Key, out returnValues, savedValue.ToString());

                        playerPrefHolder.type = returnValues.PlayerPrefsType;
                        playerPrefHolder.isEncrypted = returnValues.IsEncrypted;
                        //tempPlayerPrefs.Add(new PlayerPrefHolder() { Key = pair.Key, Value = pair.Value });
                    }

                    playerPrefHolder.Value = savedValue;
                    playerPrefHolder.TempValue = savedValue;
                    playerPrefHolder.BackupValues = savedValue;
                    playerPrefHolder.Key = pair.Key;
                    playerPrefHolder.TempKey = pair.Key;
                    playerPrefHolder.Init();

                    if (!tempPlayerPrefs.Exists(p => p.Key == playerPrefHolder.Key) && !pair.Key.ToLower().Contains("unity"))
                    {
                        tempPlayerPrefs.Add(playerPrefHolder);
                    }
                }
            }
#elif UNITY_EDITOR_WIN
            RegistryKey RegistryKey = Registry.CurrentUser.OpenSubKey(@"Software\Unity\UnityEditor\" + companyName + "\\" + productName);

            if (RegistryKey == null) return new List<PlayerPrefHolder>();

            foreach (string item in RegistryKey.GetValueNames())
            {
                if (RegistryKey != null)
                {
                    string[] valueNames = RegistryKey.GetValueNames();
                    //int i = 0;
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


                            savedValue = AdvancedPlayerPrefs.TryGetCostumeType(key, out ReturnType returnValues, savedValue.ToString());

                            pair.type = returnValues.PlayerPrefsType;
                            pair.isEncrypted = returnValues.IsEncrypted;


                        }

                        pair.Value = savedValue;
                        pair.TempValue = savedValue;
                        pair.BackupValues = savedValue;
                        pair.Key = key;
                        pair.TempKey = key;

                        pair.Init();

                        if (!tempPlayerPrefs.Exists(p => p.Key == pair.Key) && !pair.Key.ToLower().Contains("unity"))
                        {
                            tempPlayerPrefs.Add(pair);
                        }
                    }
                }
            }
#endif
            return tempPlayerPrefs;
        }
        internal static void ImportFrom(string importCompanyName, string importProductName)
        {
            string currentCompanyName = PlayerSettings.companyName;
            string currentProductName = PlayerSettings.productName;

            PlayerSettings.productName = importProductName;
            PlayerSettings.companyName = importCompanyName;

            var prefs = GetPlayerPrefs(importCompanyName, importProductName);
            int t = prefs.Count;

            PlayerSettings.productName = currentProductName;
            PlayerSettings.companyName = currentCompanyName;

            foreach (var pref in prefs)
            {
                pref.Save();
            }
            if (t > 0)
            {
                DavanciDebug.Log(t + " Prefs Imported from < " + importProductName + "/" + importCompanyName + ">", Color.green);
            }
            else
            {
                DavanciDebug.Warning("No Prefs founded at < " + importProductName + "/" + importCompanyName + ">");
            }
        }
        internal static int GetPlayerPrefsCount(string importCompanyName, string importProductName)
        {
            string currentCompanyName = PlayerSettings.companyName;
            string currentProductName = PlayerSettings.productName;

            PlayerSettings.productName = importProductName;
            PlayerSettings.companyName = importCompanyName;

            var t = GetPlayerPrefs(importCompanyName, importProductName).Count;

            PlayerSettings.productName = currentProductName;
            PlayerSettings.companyName = currentCompanyName;
            return t;
        }
        private void GetBackupFromFile(string tempPath)
        {
            //AdvancedPlayerPrefsExportManager.m_exportPath = tempExportPath;

            var backupPairs = AdvancedPlayerPrefsExportManager.ReadBackupFile(tempPath);
            if (backupPairs != null)
            {
                RefreshWithoutLog();
            }
        }
        private void Export()
        {
            AdvancedPlayerPrefsExportManager.Export(PlayerPrefHolderList, ExportPath, SavePathType);
        }
        #endregion
    }

}