using UnityEditor;
using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class AdvancedPlayerPrefsInstallerPanel : EditorWindow
    {
        [MenuItem("DavanciCode/Setup", priority = 1)]
        public static void ShowWindow()
        {
            AdvancedPlayerPrefsInstallerPanel PlayerPrefsWindow = (AdvancedPlayerPrefsInstallerPanel)GetWindow(typeof(AdvancedPlayerPrefsInstallerPanel), true);
            PlayerPrefsWindow.titleContent = new GUIContent("Advanced PlayerPrefs Installer");
            Vector2 minSize = new Vector2(400, 500);
            Vector2 maxSize = new Vector2(400, 500);
            PlayerPrefsWindow.minSize = minSize;
            PlayerPrefsWindow.maxSize = maxSize;
            var position = PlayerPrefsWindow.position;
            position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
            PlayerPrefsWindow.position = position;
            PlayerPrefsWindow.Show();
        }
        private Texture Logo;
        private Texture cover;
        private Texture developedBy;
        private bool isAlreadyInstalled;
        private readonly string SetupButtonText = "Setup Encryption";
        private readonly string SelectButtonText = "Select Settings";
        private Texture2D ShowButtonNormal;
        private Texture2D ShowButtonHover;    
        private void OnEnable()
        {
            Logo = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/Logo.png", typeof(Texture));
            cover = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/Davanci_Banner.png", typeof(Texture));
            developedBy = (Texture)AssetDatabase.LoadAssetAtPath("Assets/DaVanci Ink/Advanced PlayerPrefs/Sprites/button_test.png", typeof(Texture));
            isAlreadyInstalled = AdvancedPlayerPrefs.SelectSettings(false);
            ShowButtonNormal = MakeBackgroundTexture(10, 10, AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor);
            ShowButtonHover = MakeBackgroundTexture(10, 10, AdvancedPlayerPrefsGlobalVariables.SetupButtonTextColor);
        }
        private void OnGUI()
        {
            var oldBackgroundColor = GUI.backgroundColor;
            GUIStyle style2 = new GUIStyle(GUI.skin.button);
            style2.fontSize = 12;
            style2.fontStyle = FontStyle.Bold;
            style2.alignment = TextAnchor.MiddleCenter;
            style2.normal.textColor = Color.white;
            style2.normal.background = Texture2D.whiteTexture;

            float buttonWidth = (EditorGUIUtility.currentViewWidth - 10);
            GUILayout.BeginArea(new Rect(0, 0, 825 / 2 - 10, 240 / 2), cover);

            GUILayout.EndArea();
            GUILayout.Space(110);
            //  DrawHorizontalLine(Color.gray);

            GUILayout.BeginVertical();
            GUILayout.Label("Version 1.0.0" + ColorString("[Release version]", Color.gray), EditorStyles.miniBoldLabel, GUILayout.Width(buttonWidth), GUILayout.Height(20));
            GUILayout.Label("Advanced player prefs Installed.Setup required to use encryption!", EditorStyles.miniBoldLabel, GUILayout.Width(buttonWidth), GUILayout.Height(20));
            GUILayout.EndVertical();

            DrawHorizontalLine(Color.gray);
            // GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            int iButtonWidth = 150;
            GUILayout.Space((Screen.width-20) / 5 - (iButtonWidth-5) / 5);


            GUI.backgroundColor = AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;
            style2.normal.textColor = AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColor;

            style2.hover.textColor = AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;
            style2.hover.background = MakeBackgroundTexture(10, 10, AdvancedPlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColor);
            //style2.onHover.textColor = PlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;
            //style2.onHover.background = MakeBackgroundTexture(10, 10, PlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsTextColor);
            //style2.onHover.textColor = PlayerPrefsGlobalVariables.ShowAdvancedPlayerPrefsButtonColor;

            if (GUILayout.Button("Show Player Prefs Tool\n CTRL+E", style2, GUILayout.Width(150), GUILayout.Height(40)))
            {
                PlayerPrefsWindow.ShowWindow();
            }

            GUI.backgroundColor = AdvancedPlayerPrefsGlobalVariables.SetupButtonColor;

            style2.normal.textColor = AdvancedPlayerPrefsGlobalVariables.SetupButtonTextColor;
            style2.normal.background = MakeBackgroundTexture(10, 10, AdvancedPlayerPrefsGlobalVariables.SetupButtonTextColor);

            style2.hover.background = MakeBackgroundTexture(10, 10, AdvancedPlayerPrefsGlobalVariables.SetupButtonColor);
            style2.hover.textColor = AdvancedPlayerPrefsGlobalVariables.SetupButtonTextColor;
            // style2.normal.textColor = Color.cyan;
            GUILayout.Space(5);

            if (GUILayout.Button(isAlreadyInstalled ? SelectButtonText : SetupButtonText, style2, GUILayout.Width(150), GUILayout.Height(40)))
            {
                if (isAlreadyInstalled)
                {
                    AdvancedPlayerPrefs.SelectSettings();
                }
                else
                {
                    AdvancedPlayerPrefs.CreateSettings();
                    AdvancedPlayerPrefs.SelectSettings();
                    isAlreadyInstalled = true;
                }
                //you code here
            }
            GUI.backgroundColor = oldBackgroundColor;
            style2.normal.textColor = Color.white;

            GUILayout.EndHorizontal();
            DrawHorizontalLine(Color.gray);
            GUILayout.BeginVertical();

            GUILayout.Label("Advanced player prefs Installed.Setup required to use encryption!\n" +
                "To Use encryption,you need to create an encryption settings file,\nwhere you can change and set your current encryption key.\n" +
                "Encrypction based on AES encryption (32 and 16 bytes)\n" +
                "You can export/import your current key in/from file and use it\n where ever you want.", EditorStyles.boldLabel, GUILayout.Width(buttonWidth), GUILayout.Height(90));
            //  GUILayout.FlexibleSpace();

            DrawHorizontalLine(Color.gray);
            DrawBottomButtons();
            // DrawHorizontalLine(Color.gray);


        }
        private void DrawBottomButtons()
        {

            EditorGUILayout.BeginHorizontal();
            float buttonWidth = (EditorGUIUtility.currentViewWidth - 10) / 2f;
            // Delete all PlayerPrefs
            if (GUILayout.Button("Website", GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Get Started", GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // DeleteAll();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            // Delete all PlayerPrefs
            if (GUILayout.Button("Changelogs", GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Documentation", GUILayout.Width(buttonWidth), GUILayout.Height(30)))
            {
                // DeleteAll();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(Screen.width / 2 - buttonWidth / 2);

            if (GUILayout.Button(developedBy, GUILayout.Width(buttonWidth), GUILayout.Height(50)))
            {
                //you code here
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
        private Texture2D MakeBackgroundTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D backgroundTexture = new Texture2D(width, height);

            backgroundTexture.SetPixels(pixels);
            backgroundTexture.Apply();

            return backgroundTexture;
        }
        public static string ColorString(string text, Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";
        }
    }
}