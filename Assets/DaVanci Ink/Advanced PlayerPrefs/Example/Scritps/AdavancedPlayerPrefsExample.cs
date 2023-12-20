using DaVanciInk.AdvancedPlayerPrefs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DaVanciInk.AdvancedPlayerPrefs
{
    public class AdavancedPlayerPrefsExample : MonoBehaviour
    {
        [SerializeField] private bool NoAds;
        [SerializeField] private int CoinsCount;
        [Header("References")]
        [SerializeField] private Transform Ground;
        [SerializeField] private bool SetSamplesPrefs;

        private float RotationSpeed = 50f;

        private void Start()
        {
            CoinsCount = AdvancedPlayerPrefs.GetInt("ADPP_Coins", 100);

            if (SetSamplesPrefs)
                SaveSamplesPrefs();
        }
     
        private void Update()
        {
            Ground.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        }
        private void SaveSamplesPrefs()
        {
            // Set a float value to the "TraveledDistance" key in AdvancedPlayerPrefs with encryption disabled.
            AdvancedPlayerPrefs.SetFloat("TraveledDistance", 222.22f, false);

            // Set a int value to the "CoinsCount" key in AdvancedPlayerPrefs with encryption enabled.
            AdvancedPlayerPrefs.SetInt("CoinsCount", CoinsCount, true);

            // Save a string value with the key "PlayerName" to the PlayerPrefs using AdvancedPlayerPrefs.
            AdvancedPlayerPrefs.SetString("PlayerName", "Walter White");

            // Retrieve the integer value with the key "CoinsCount" from the PlayerPrefs using AdvancedPlayerPrefs.
            CoinsCount = AdvancedPlayerPrefs.GetInt("CoinsCount");

            // Set a float value to the "TraveledDistance" key in AdvancedPlayerPrefs with encryption disabled.
            AdvancedPlayerPrefs.SetFloat("TraveledDistance", 222.22f, false);

            // Set a Vector3 value to the "CoinsCount" key in AdvancedPlayerPrefs with encryption enabled .
            AdvancedPlayerPrefs.SetInt("CoinsCount", CoinsCount, true);

            // Set a boolean value to the "PlayerName" key in AdvancedPlayerPrefs
            AdvancedPlayerPrefs.SetString("PlayerName", "Walter White");

            AdvancedPlayerPrefs.SetByte("ByteTest", 255);
        }

    }


}
