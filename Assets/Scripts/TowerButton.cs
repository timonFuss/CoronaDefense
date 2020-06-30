using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private int price;

    public int Price
    {
        get
        {
            return price;
        }
    }

    [SerializeField]
    private Text priceText;

    public GameObject TowerPrefab
    {
        get
        {
            return towerPrefab;
        }
    }

    public Sprite Sprite
    {
        get
        {
            return sprite;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        priceText.text = price + "$";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     public void ShowInfo(string type){
         string tooltip = string.Empty;

         switch (type){
            case "Soundbox":
                Tower soundbox = towerPrefab.GetComponentInChildren<Tower>();
                tooltip = string.Format("<color=#ffa500ff><size=20><b>Soundbox</b></size></color>\nDamage: {0}", soundbox.Damage);
                break;

            case "DesinfectSpray":
                Tower desinfectSpray = towerPrefab.GetComponentInChildren<Tower>();
                tooltip = string.Format("<color=#3b7eb6><size=20><b>Desinfect Spray</b></size></color>\nDamage: {0}", desinfectSpray.Damage);
                break;
         }
        GameManager.Instance.SetTooltipText(tooltip);
        GameManager.Instance.ShowStats();
    }
}
