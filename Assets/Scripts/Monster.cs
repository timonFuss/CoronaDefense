﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Stack<Node> path;

    public Point GridPosition { get; set; }

    private Vector3 destination;

    private Animator myAnimator;

    [SerializeField]
    private Stat health;

    public bool Alive 
    {
        get 
        { 
            return health.CurrentVal > 0;
        }
    }

    public bool IsActive { get; set; }

    private void Awake()
    {
        health.Initialize();

        //for Animations
        myAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.startTile.transform.position;

        this.health.Bar.Reset();
        this.health.MaxVal = health;
        this.health.CurrentVal = this.health.MaxVal;

        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1), false));

        SetPath(LevelManager.Instance.Path);
    }

    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        float progress = 0;

        while( progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);

            progress += Time.deltaTime;

            yield return null;
        }

        transform.localScale = to;

        IsActive = true;

        if (remove)
        {
            Realease();
        }
    }

    private void Move()
    {
        if (IsActive)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (transform.position == destination)
            {
                if (path != null && path.Count > 0)
                {
                    GridPosition = path.Peek().GridPosition;
                    destination = path.Pop().WorldPosition;
                }
            }
        }
    }

    private void SetPath(Stack<Node> newPath)
    {
        if(newPath != null)
        {
            this.path = newPath;

            GridPosition = path.Peek().GridPosition;

            destination = path.Pop().WorldPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "DespawnTag")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            //shouldnt be a constant. 
            Player.Instance.Health.CurrentVal -= 20;
        }
    }

    //first need to add Animations
    private void Animate(Point currentPos, Point newPos)
    {
        if(currentPos.Y > newPos.Y)
        {
            //moving down
        }
        else if(currentPos.Y > newPos.Y)
        {
            //moving up
        }
        if(currentPos.Y == newPos.Y)
        {
            if(currentPos.X > newPos.X)
            {
                //Move left
            }
            else if(currentPos.X < newPos.X)
            {
                //Move right
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsActive)
        {
            health.CurrentVal -= damage;

            if(health.CurrentVal <= 0)
            {
                //wenn währung implementiert wurde
                GameManager.Instance.Currency += 1;

                //hier nur entfernen der Monster. Falls Animation vorhanden, Tutorial anschauen.
                StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

                IsActive = false;
            }
        }
    }

    //Reset Attributes of monster
    private void Realease()
    {
        IsActive = false;
        GridPosition = LevelManager.Instance.startTile.GridPosition;
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GameManager.Instance.RemoveMonster(this);
    }
}
