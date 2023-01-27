using DaVanciInk.AdvancedPlayerPrefs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    [SerializeField] private Material[] TowerMaterials;
    public LayerMask planeLayer;
    private Ray ray;
    RaycastHit hit;
    private void Start()
    {
       //TowersPosition = AdvancedPlayerPrefs.GetList<Vector3>("ADPP_TowerPositions");
        TowersPositionColors = AdvancedPlayerPrefs.GetList<Vector4>("ADPP_TowerPositionsColors");

        NoAds = AdvancedPlayerPrefs.GetBool("ADPP_NoAds", false);

        CoinsCount = AdvancedPlayerPrefs.GetInt("ADPP_Coins", 100);
        GemsCount = AdvancedPlayerPrefs.GetInt("ADPP_Gems", 15);

        UnlockedSkins = AdvancedPlayerPrefs.GetList("ADPP_UnlockedSkins", new List<byte>(0));

        dailyLogin = AdvancedPlayerPrefs.GetArray("ADPP_DailyLogin", dailyLogin);

        FriendsList = AdvancedPlayerPrefs.GetList<string>("ADPP_Friends");

        InitNoAdsImage();
        //foreach (var postion in TowersPosition)
        //{   
        //    SpawnTower(postion);
        //}
        foreach (var postion in TowersPositionColors)
        {
            SpawnTower(postion);
        }
    }
    public void BuyNoAds()
    {
        if (!NoAds)
        {
            NoAds = true;
            AdvancedPlayerPrefs.SetBool("ADPP_NoAds", NoAds);
            NoAdsImage.color = Color.green;
        } 
    }
    private void InitNoAdsImage()
    {
        NoAdsImage.color = NoAds ? Color.green : Color.red;
    }

    private void SpawnTower(Vector3 _position)
    {
        var tower=  Instantiate(TowerPrefab, TowerParent);
        tower.transform.localPosition = _position;
    }
    private void SpawnTower(Vector4 _positionColor)
    {
        var tower = Instantiate(TowerPrefab, TowerParent);
        tower.transform.localPosition =new Vector3 (_positionColor.x, _positionColor.y, _positionColor.z);
        var rendrer = tower.GetComponentInChildren<Renderer>();
        rendrer.material = TowerMaterials[(int)_positionColor.w];
    }
    private void SaveTowerPositions(Vector3 _position)
    {
        TowersPosition.Add(_position);
        AdvancedPlayerPrefs.SetList("ADPP_TowerPositions", TowersPosition);
    }
    private void SaveTowerPositionsColors(Vector4 _positionColor)
    {
        TowersPositionColors.Add(_positionColor);
        AdvancedPlayerPrefs.SetList("ADPP_TowerPositionsColors", TowersPositionColors);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, planeLayer))
            {
                int colorType = Random.Range(0, 4);

                //SpawnTower(hit.point);
                //SaveTowerPositions(hit.point);
                Vector4 posColor = (Vector4)hit.point;
                posColor.w = colorType;

                SpawnTower(posColor);
                SaveTowerPositionsColors(posColor);
            }
        }
    }
} 
