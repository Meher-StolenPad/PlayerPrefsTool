using DaVanciInk.AdvancedPlayerPrefs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private GameObject TowerPrefab;
        [SerializeField] private Transform TowerParent;
        [SerializeField] private Transform Ground;  
        public LayerMask planeLayer;
        private Ray ray;
        RaycastHit hit;
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

            foreach (var postion in TowersPosition)
            {   
                SpawnTower(postion,true);
            }
           
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

        private Vector3 SpawnTower(Vector3 _position,bool isLocalpos=false)
        {
            if (isLocalpos)
            {
                var tower = Instantiate(TowerPrefab, TowerParent);
                tower.transform.localPosition=_position;
                return _position;
            }
            else
            {
                var tower = Instantiate(TowerPrefab, _position, Quaternion.identity);
                tower.transform.SetParent(TowerParent);
                return tower.transform.localPosition;

            }
        }
    
        private void SaveTowerPositions(Vector3 _position)
        {
            TowersPosition.Add(_position);
            AdvancedPlayerPrefs.SetList("ADPP_TowerPositions", TowersPosition);
        }
        private void Update()
        {
            Ground.Rotate(Vector3.up,50f*Time.deltaTime);
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, planeLayer))
                {
                    int colorType = Random.Range(0, 4);
                    SaveTowerPositions(SpawnTower(hit.point));
                }
            }

        }
        public void ShowTool()
        {
           // AdvancedPlayerPrefsTool.ShowWindow();
        }

    }


}
