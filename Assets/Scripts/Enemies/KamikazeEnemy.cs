using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeEnemy : Enemy
{
    void Start()
    {
        transform.GetComponent<MovementController>().MovementStart();
    }

    void Update()
    {
        transform.GetComponent<MovementController>().Move();
        if(transform != null)
        {
            transform.GetComponent<ShootController>().CheckShooting();
        }
    }
}
