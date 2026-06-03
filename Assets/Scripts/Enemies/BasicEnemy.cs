using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Random = UnityEngine.Random;

public class BasicEnemy : Enemy
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

    // protected override void OnTriggerEnter2D(Collider2D collision)
    // {
    //     base.OnTriggerEnter2D(collision);
    //     ProjectileController proj = collision.gameObject.GetComponent<ProjectileController>();
    //     if (collision.gameObject.GetComponent<ProjectileController>() != null)
    //     {
    //         if (!proj.IsHostile())
    //         {
    //             if(energy_life_ != null){
    //                 int chances = 0;
    //                 if(player_.GetComponent<CoreController>().current_player_life_ <= 33){
    //                     chances = 15;
    //                 } else if(player_.GetComponent<CoreController>().current_player_life_ <= 66){
    //                     chances = 10;
    //                 } else {
    //                     chances = 5;
    //                 }

    //                 int randomNum = Random.Range(0, 100);
    //                 if(randomNum <= chances){
    //                     GameObject generated = Instantiate(energy_life_, transform.position, Quaternion.identity);
    //                     generated.GetComponent<EnergyLifeController>().player_ = player_;
    //                 }
    //             }
    //             Destroy(collision.gameObject);
    //             Destroy(gameObject);
    //         }
    //     }
    // }
}
