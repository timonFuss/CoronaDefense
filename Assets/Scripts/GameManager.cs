using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public TowerButton ClickedButton { get; private set; }

    public ObjectPool Pool { get; set; }

    private int currency;

    [SerializeField]
    private Text currencyText;

    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
            this.currency = value;
            this.currencyText.text = value.ToString() + "<color=lime>$</color>";
        }
    }

    private int wave = 0;

    private int remainingWaves = 0;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private GameObject waveBtn;

    private List<Monster> activeMonsters = new List<Monster>();


    private Tower selectedTower;

    private int monsterHealth = 15;

    private bool gameOver = false;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject nextLevelMenu;

    private bool fatBatSpawned = false;

    [SerializeField]
    private GameObject healthBar;

    [SerializeField]
    private SliderMenuAnim towerPanel;

    public bool WaveActive
    {
        get
        {
            return activeMonsters.Count > 0;
        }
    }

    public int RemainingWaves
    {
        get
        {
            return remainingWaves;
        }
        set { remainingWaves = value; }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Currency = 25;
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.Instance.Health.CurrentVal <= 0)
        {
            gameOverMenu.SetActive(true);
            gameOver = true;
        }
    }

    public void PickTower(TowerButton towerButton)
    {
        if (!WaveActive)
        {
            if (Currency >= towerButton.Price)
            {
                this.ClickedButton = towerButton;
                Hover.Instance.Activate(towerButton.Sprite);
            }
        }
    }

    public void BuyTower()
    {
        if (Currency >= ClickedButton.Price)
        {
            Currency -= ClickedButton.Price;
        }
        ClickedButton = null;
    }

    public void SellTower(Tower tower)
    {
        Destroy(tower.transform.parent.gameObject);
        Currency += tower.Price;
        
    }

    public void StartWave()
    {
        towerPanel.ShowSideMenu();
        if (LevelManager.Instance.LevelType == 2)
        {
            LevelManager.Instance.GeneratePath();
        }

        if(LevelManager.Instance.Path.Count > 0)
        {
            wave++;

            waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", wave);

            StartCoroutine(SpawnWave());

            waveBtn.SetActive(false);
        }
        else
        {
            Debug.LogError("KEIN GÜLTIGER PATH MÖGLICH!");
        }
        
    }

    private IEnumerator SpawnWave()
    {
        if(remainingWaves == 0 && LevelManager.Instance.LevelIdx == 1)
        {
            Monster monster = Pool.GetObject("bat_01 Variant").GetComponent<Monster>();
            monster.GetComponent<SpriteRenderer>().color = Color.red;
            monster.SetSpeed(25);
            monster.Spawn(5000000);
            activeMonsters.Add(monster);
            fatBatSpawned = true;
        }

        if (remainingWaves > 0)
        {
            remainingWaves--;
            //Amount of Monsters per wave
            for (int i = 0; i < wave * 2; i++)
            {
                int monsterIndex = Random.Range(0, 2);

                string type = string.Empty;

                switch (monsterIndex)
                {
                    case 0:
                        type = "covid_01 Variant";
                        break;
                    case 1:
                        type = "bat_01 Variant";
                        break;
                }

                Monster monster = Pool.GetObject(type).GetComponent<Monster>();

                monster.Spawn(monsterHealth);

                //health of monsters get increased every third wave
                if (wave % 3 == 0)
                {
                    monsterHealth += 5;
                }

                activeMonsters.Add(monster);

                yield return new WaitForSeconds(2.5f);
            }
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

        if (monster.GetComponent<SpriteRenderer>().color == Color.red && monster.name.Equals("bat_01 Variant"))
        {
            //Fette Fledermaus ist im Ziel!
            nextLevelMenu.SetActive(true);
        }

        if (!WaveActive && !gameOver)
        {
            if (remainingWaves == 0)
            {
                if(LevelManager.Instance.LevelIdx == 1 && !fatBatSpawned)
                {
                    //Im ersten Level die fette Fledermaus spawnen
                    waveTxt.enabled = false;
                    waveBtn.SetActive(false);
                    currencyText.enabled = false;
                    healthBar.SetActive(false);
                    StartCoroutine(SpawnWave());
                }
                else
                {
                    nextLevelMenu.SetActive(true);
                }

            }
            else
            {
                towerPanel.ShowSideMenu();
                waveBtn.SetActive(true);
            }
        }
    }

    public void LoadNextLevel()
    {
        var level = LevelManager.Instance.LevelIdx;
        level++;
        SceneManager.LoadScene("Level" + level);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenuScene");
    }
}
