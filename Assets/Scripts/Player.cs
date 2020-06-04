using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField]
    private Stat health;

    public Stat Health { get => health; set => health = value; }

    private void Awake()
    {
        Health.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
