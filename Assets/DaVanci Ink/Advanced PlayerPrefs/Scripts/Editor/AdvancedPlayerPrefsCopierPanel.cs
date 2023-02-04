using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class AdvancedPlayerPrefsCopierPanel : EditorWindow
    {
        private static AdvancedPlayerPrefsCopierPanel PlayerPrefsWindow;
        //[MenuItem(AdvancedPlayerPrefsGlobalVariables.AdvancedPlayerPrefsSetupMenuName)]
        public static void ShowWindow()
        {
            PlayerPrefsWindow = (AdvancedPlayerPrefsCopierPanel)GetWindow(typeof(AdvancedPlayerPrefsCopierPanel), true);
            PlayerPrefsWindow.titleContent = new GUIContent("Advanced PlayerPrefs Immegration Panel");
            Vector2 minSize = new Vector2(400, 500);
            Vector2 maxSize = new Vector2(400, 500);
            PlayerPrefsWindow.minSize = minSize;
            PlayerPrefsWindow.maxSize = maxSize;
            var position = PlayerPrefsWindow.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            PlayerPrefsWindow.position = position;
            PlayerPrefsWindow.Show();
            IsActive = true;
        }

        private Texture cover;
        private Texture developedBy;

        internal static bool IsActive;

        internal static int PlayerPrefsFound;
        internal static string CompanyName; 
        internal static string ProductName; 

        internal static void Init(int count,string companyName,string productName)
        {
            PlayerPrefsFound = count;
            CompanyName = companyName;
            ProductName = productName;
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }
        private void OnEnable()
        {
            cover = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/AdvancedPlayerPrefsPROCover.png", typeof(Texture));

            string developedByName = EditorGUIUtility.isProSkin ? "DavanciButtonPro.png" : "DavanciButton.png";
            developedBy = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/"+ developedByName, typeof(Texture));
        }
        void OnDisable()
        {
            IsActive = false;
            EditorCallBackHolder.UpadteInfo();
        }
        private void OnGUI()
        {
            var oldBackgroundColor = GUI.backgroundColor;
            GUIStyle style2 = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style2.normal.textColor = Color.white;
            //style2.wordWrap = true;

            // style2.normal.background = Texture2D.whiteTexture;

            GUIStyle style3 = new GUIStyle(EditorStyles.objectFieldThumb)
            {
                fontSize = 12,
                fontStyle = FontStyle.BoldAndItalic,
                alignment = TextAnchor.UpperLeft
            };
            style3.normal.textColor = Color.white;
            style3.wordWrap = true;

            float buttonWidth = (EditorGUIUtility.currentViewWidth - 10);
            GUILayout.BeginArea(new Rect(0, 0, 825 / 2 - 10, 240 / 2), cover);

            GUILayout.EndArea();
            GUILayout.Space(110);

            GUILayout.BeginVertical();
            GUILayout.Label("Version 1.0.0" + " [Release version]", EditorStyles.miniBoldLabel, GUILayout.Width(buttonWidth), GUILayout.Height(20));
            GUILayout.Label("Product Name/Company Name has been changed !", EditorStyles.miniBoldLabel, GUILayout.Width(buttonWidth), GUILayout.Height(20));
            GUILayout.EndVertical();

            DrawHorizontalLine(Color.gray);
            GUILayout.BeginHorizontal();
            int iButtonWidth = 120;
            GUILayout.Space((Screen.width-20) / 4 - (iButtonWidth-5) / 5);


            style2.hover.textColor = EditorGUIUtility.isProSkin ? AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColor : AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColorNormal;
            //style2.normal.background = ShowButtonNormal;

            style2.normal.textColor = AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;
            int oldfontSize = style2.fontSize;
            style2.fontSize = (int)(50 * (Screen.width / 1920f));
            // style2.hover.background = ShowButtonHover;

            if (GUILayout.Button("Show Player Prefs Tool\n CTRL+E", style2, GUILayout.Width(120), GUILayout.Height(40)))
            {
                AdvancedPlayerPrefsTool.ShowWindow();
            }

            GUILayout.Space(5);
            style2.fontSize = oldfontSize;
            style2.hover.textColor = EditorGUIUtility.isProSkin ? AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColor : AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColorNormal;
            //style2.normal.background = ShowButtonNormal;

            style2.normal.textColor = AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;
            // style2.hover.background = ShowButtonHover;
            GUI.enabled = false;
            if (GUILayout.Button("Create Backup", style2, GUILayout.Width(120), GUILayout.Height(40)))
            {
                AssetDatabase.ImportPackage(AdvancedPlayerPrefsGlobalVariables.SamplesPackagePath, true);
            }

            GUI.backgroundColor = oldBackgroundColor;
            style2.normal.textColor = Color.white;
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            DrawHorizontalLine(Color.gray);
            GUILayout.Space(5);

            GUILayout.BeginVertical();

            GUILayout.Label(
                "NOTE : When you change the " +
                GetStyledText("ProductName/CompanyName", EditorGUIUtility.isProSkin ? Color.grey : Color.black ) +
                "of unity project,You will lose all your current PlayerPrefs.\n\n" +
                "With Advanced PlayerPrefs editor tool,you can immegrate all your PlayerPrefs.If you close this panel,the process will be canceled.\n\n" +
                 GetStyledText(PlayerPrefsFound.ToString(), EditorGUIUtility.isProSkin ? Color.grey : Color.black) +
                " Player Prefs Can be moved from " +
                GetStyledText(ProductName+"/"+CompanyName+
                "\n\nDo you want to import them ? ", EditorGUIUtility.isProSkin ? Color.grey : Color.black)
                , style3, GUILayout.Width(buttonWidth), GUILayout.Height(155));
            DrawHorizontalLine(Color.gray);
            DrawBottomButtons();
        }

        private void DrawBottomButtons()
        {

            float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Import", GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                AdvancedPlayerPrefsTool.ImportFrom(CompanyName, ProductName);

                EditorCallBackHolder.UpadteInfo();
                Debug.Log("Prefs Auto Upadted : " + CompanyName + "/" + ProductName);
                PlayerPrefsWindow.Close();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Documentation", GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                Application.OpenURL(AdvancedPlayerPrefsGlobalVariables.DocumentationLink);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width / 2 - buttonWidth / 2);

            if (GUILayout.Button(developedBy, GUILayout.Width(buttonWidth), GUILayout.Height(50)))
            {
                Application.OpenURL(AdvancedPlayerPrefsGlobalVariables.DavanciInkLink);
            }
            EditorGUILayout.EndHorizontal();
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
        private string GetStyledText(string text, Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text +" "+ "</color>";
        }
    }
}