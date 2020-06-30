using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    public int Price {get; private set; }

    public int Damage {get; private set; }

    public float DebuffDuration {get; private set; }

    public float SlowingFactor {get; private set;}

    public int SpecialDamage {get; private set; }

    public TowerUpgrade(int price, int damage, float debuffDuration){
        this.Damage = damage;
        this.DebuffDuration = debuffDuration;
        this.Price = price;
    }

    public TowerUpgrade(int price, int damage, float debuffDuration, float slowingFactor){
        this.Damage = damage;
        this.DebuffDuration = debuffDuration;
        
        this.SlowingFactor = slowingFactor;
        this.Price = price;
    }

    public TowerUpgrade(int price, int damage, float debuffDuration, int specialDamage){
        this.Damage = damage;
        this.DebuffDuration = debuffDuration;
        this.SpecialDamage = SpecialDamage;
        this.Price = price;
    }

}
