using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public TowerButton ClickedButton { get; private set; }

    public ObjectPool Pool { get; set; }

    private int wave = 0;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private GameObject waveBtn;

    private List<Monster> activeMonsters = new List<Monster>();


    private Tower selectedTower;

    public bool WaveActive
    {
        get
        {
            return activeMonsters.Count > 0;
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickTower(TowerButton towerButton)
    {
        if (!WaveActive)
        {
            this.ClickedButton = towerButton;
            Hover.Instance.Activate(towerButton.Sprite);
        }
    }

    public void BuyTower()
    {
        ClickedButton = null;
    }

    public void StartWave()
    {
        wave++;

        waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", wave);

        StartCoroutine(SpawnWave());

        waveBtn.SetActive(false);
    }

    private IEnumerator SpawnWave()
    {
        LevelManager.Instance.GeneratePath();

        for(int i = 0; i < wave * 2; i++)
        {
            int monsterIndex = Random.Range(0, 2);

            string type = string.Empty;

            switch (monsterIndex)
            {
                case 0:
                    type = "covid_01";
                    break;
                case 1:
                    type = "bat_01";
                    break;
            }

            Monster monster = Pool.GetObject(type).GetComponent<Monster>();

            monster.Spawn();

            activeMonsters.Add(monster);

            yield return new WaitForSeconds(2.5f);
        }
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = tower;
        selectedTower.Select();
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }

        selectedTower = null;
    }

    public void RemoveMonster(Monster monster)
    {
        activeMonsters.Remove(monster);

        if (!WaveActive)
        {
            waveBtn.SetActive(true);
        }
    }
}
