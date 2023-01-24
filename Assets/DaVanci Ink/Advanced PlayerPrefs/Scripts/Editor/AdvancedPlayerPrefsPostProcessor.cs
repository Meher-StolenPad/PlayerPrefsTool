using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class AdvancedPlayerPrefsPostProcessor : AssetPostprocessor
    {
#if UNITY_2021_2_OR_NEWER
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths,bool didDomainReload)
        {
            var inPackages = string.Empty;

             inPackages =
                 importedAssets.FirstOrDefault(path => path.Contains("Advanced PlayerPrefs"));

            if (!string.IsNullOrEmpty(inPackages))
            {
                //Show Advanced PlayerPrefs Package Installer
                AdvancedPlayerPrefsInstallerPanel.ShowWindow();
                DavanciDebug.Log("Advanced PlayerPrefs imported successfully !",Color.green);
            }
        }
#else

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var inPackages = string.Empty;

            inPackages =
                importedAssets.FirstOrDefault(path => path.Contains("Advanced PlayerPrefs"));

            if (!string.IsNullOrEmpty(inPackages))
            {
                //Show Advanced PlayerPrefs Package Installer
                // AdvancedPlayerPrefsInstallerPanel.ShowWindow();
                DavanciDebug.Log("Advanced PlayerPrefs imported successfully !", Color.green);

            }
        }
#endif
    }
}
