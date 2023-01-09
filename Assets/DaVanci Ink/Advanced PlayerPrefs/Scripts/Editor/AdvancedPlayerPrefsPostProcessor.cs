using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class AdvancedPlayerPrefsPostProcessor : AssetPostprocessor
    {
#if UNITY_2021_OR_NEWER
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths,bool didDomainReload)
        {
            var inPackages = string.Empty;

             inPackages =
                 importedAssets.FirstOrDefault(path => path.Contains("Advanced PlayerPrefs"));

            if (!string.IsNullOrEmpty(inPackages))
            {
                //Show Advanced PlayerPrefs Package Installer
                PlayerPrefsWindow.ShowWindow();
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
                PlayerPrefsWindow.ShowWindow();
            }
        }
#endif
    }
}
