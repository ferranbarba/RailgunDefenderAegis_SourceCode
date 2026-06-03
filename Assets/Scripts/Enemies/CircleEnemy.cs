using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Random = UnityEngine.Random;

public class CircleEnemy : Enemy
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

