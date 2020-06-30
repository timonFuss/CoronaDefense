using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private Monster target;

    public Monster Target
    {
        get { return target;  }
    }

    private SpriteRenderer mySpriteRenderer;
    private Queue<Monster> monsters = new Queue<Monster>();

    [SerializeField]
    private string projectileType;

    public int Level { get; protected set; }

    [SerializeField]
    private float projectileSpeed;

    private Animator myAnimator;

    public float ProjecttileSpeed
    {
        get { return projectileSpeed; }
    }

    [SerializeField]
    private int damage;

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    private bool canAttack = true;

    private float attackTimer;

    [SerializeField]
    private float attackCooldown;

    public int Price { get; set; }

    public TowerUpgrade[] Upgrades { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = transform.parent.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        Level=1;
        Upgrades = new TowerUpgrade[]{
            new TowerUpgrade(2,3,0),
            new TowerUpgrade(5,5,0),
            new TowerUpgrade(10,8,0),
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        //Debug.Log(target);
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTooltip();
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        if (target == null && monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }
        if (target != null && target.IsActive)
        {
            if (canAttack)
            {
                if (this.projectileType == "soundProjectile" && (target.name != "covid_01 Variant" && target.name != "bat_01 Variant"))
                {
                    // Tritt ein, wenn die SoundBox einen Mensch in range hat --> nicht angreifen
                }
                else
                {
                    Shoot();

                    myAnimator.SetTrigger("Attack");

                    canAttack = false;
                }
            }
        }
        else if(monsters.Count > 0)
        {
            target = monsters.Dequeue();
        }
        if(target != null && !target.Alive)
        {
            target = null;
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        // ??????????????????????????????
        projectile.transform.position = transform.position;
        projectile.Initialize(this);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            target = null;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            monsters.Enqueue(other.GetComponent<Monster>());
        }
    }

    public string GetStats(){
        if (Upgrades.Length >Level){
            
            return string.Format("<color=#F0DF00ff><size=20><b>Upgrade</b></size></color>\nLevel: {0} \nDamage: {1} <color=#20D738ff>+{2}</color>",Level,damage,Upgrades[Level-1].Damage);
        }
        return string.Format("<color=#F0DF00ff><size=20><b>Upgrade</b></size></color>\nLevel: {0} \nDamage: {1}",Level, damage);
    }

    public void Upgrade(){
        GameManager.Instance.Currency -= Upgrades[Level-1].Price;
        Price += Upgrades[Level-1].Price;
        this.damage += Upgrades[Level-1].Damage;
        Level++;
        GameManager.Instance.UpdateUpgradeTooltip();
    }
}
