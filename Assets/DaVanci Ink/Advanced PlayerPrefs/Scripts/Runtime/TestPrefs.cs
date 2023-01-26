using UnityEngine;
namespace DaVanciInk.AdvancedPlayerPrefs
{
    internal class TestPrefs : MonoBehaviour
    {
        private void Start()
        {
            AdvancedPlayerPrefs.SetFloat("TestAgain", 10.2247745f, true);
            Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgain"));
            Debug.Log("Float " + AdvancedPlayerPrefs.GetFloat("TestAgaidddn", 1f));
        }

    }
}