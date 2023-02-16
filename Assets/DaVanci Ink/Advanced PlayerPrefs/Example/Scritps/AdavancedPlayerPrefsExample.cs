using DaVanciInk.AdvancedPlayerPrefs;
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
        [SerializeField] private int GemsCount;
        [SerializeField] private List<byte> UnlockedSkins;
        [SerializeField] private bool[] dailyLogin = new bool[7];
        [SerializeField] private List<string> FriendsList;
        [SerializeField] private List<Vector3> TowersPosition = new List<Vector3>();
        [SerializeField] private List<Vector4> TowersPositionColors = new List<Vector4>();

        [Header("References")]
        [SerializeField] private Image NoAdsImage;
        [SerializeField] private AdvancedPlayerPrefsTower TowerPrefab;
        [SerializeField] private Transform TowerParent;
        [SerializeField] private Transform Ground;

        public LayerMask planeLayer;
        private Ray ray;
        private RaycastHit hit;
        private float RotationSpeed = 50f;
        private bool Locked = true;

        private void Start()
        {
            TowersPosition = AdvancedPlayerPrefs.GetList<Vector3>("ADPP_TowerPositions");

            NoAds = AdvancedPlayerPrefs.GetBool("ADPP_NoAds", false);

            CoinsCount = AdvancedPlayerPrefs.GetInt("ADPP_Coins", 100);
            GemsCount = AdvancedPlayerPrefs.GetInt("ADPP_Gems", 15);

            UnlockedSkins = AdvancedPlayerPrefs.GetList("ADPP_UnlockedSkins", new List<byte>(0));

            dailyLogin = AdvancedPlayerPrefs.GetArray("ADPP_DailyLogin", dailyLogin);

            FriendsList = AdvancedPlayerPrefs.GetList<string>("ADPP_Friends");

            InitNoAdsImage();
            StartCoroutine(SpawnTowersWithDelay());
        }
        public void BuyNoAds()
        {
            if (!NoAds)
            {
                NoAds = true;
                NoAdsImage.color = Color.green;
            }
            else
            {
                NoAds = false;
                NoAdsImage.color = Color.red;
            }
            AdvancedPlayerPrefs.SetBool("ADPP_NoAds", NoAds);

        }
        private void InitNoAdsImage()
        {
            NoAdsImage.color = NoAds ? Color.green : Color.red;
        }
        private IEnumerator SpawnTowersWithDelay()
        {
            Locked = true;

            foreach (var position in TowersPosition)
            {

                SpawnTower(position);
                yield return new WaitForSeconds(3f / TowersPosition.Count);
            }
            Locked = false;
        }
        private void SpawnTower(Vector3 _position)
        {
            var tower = Instantiate(TowerPrefab, TowerParent);
            tower.transform.localPosition = _position + Vector3.up * 10; ;
            tower.m_Position = _position;
            tower.MoveToPosition();
        }

        private void SaveTowerPositions(Vector3 _position)
        {
            TowersPosition.Add(_position);
            AdvancedPlayerPrefs.SetList("ADPP_TowerPositions", TowersPosition);
        }
        private void Update()
        {
            Ground.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);

            if (!Locked)
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, planeLayer))
                    {
                        Vector3 localPosition = TowerParent.InverseTransformPoint(hit.point);
                        SpawnTower(localPosition);
                        SaveTowerPositions(localPosition);
                    }
                }

        }
        public void ShowTool()
        {
            // AdvancedPlayerPrefsTool.ShowWindow();
        }

    }


}
