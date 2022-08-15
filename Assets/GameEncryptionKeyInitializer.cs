#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Sabresaurus.PlayerPrefsUtilities;

public static class GameEncryptionKeyInitializer
{
    private static readonly byte[] customKey = {209, 194, 253, 155, 249, 153, 57, 248, 68, 184, 138, 215, 138, 66, 240, 192, 104, 196, 103, 223, 190, 165, 42, 137, 131, 93, 56, 70, 32, 188, 70, 44};

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Initialize()
    {
        SimpleEncryption.SetCustomKey(customKey);
    }
}