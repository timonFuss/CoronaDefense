using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Monster target;
    private Tower parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    public void Initialize(Tower parent)
    {
        this.target = parent.Target;
        this.parent = parent;
    }

    private void MoveToTarget()
    {
        if (target != null && target.IsActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * parent.ProjecttileSpeed);
        }
        else if (!target.IsActive){
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Monster")
        {
            if (target.gameObject == collision.gameObject)
            {
                //geht noch nicht. projectile erkennt keinen Tower als parent
                //target.TakeDamage(parent.Damage);
                target.TakeDamage(5);

                GameManager.Instance.Pool.ReleaseObject(gameObject);
            }
        }
    }
}
