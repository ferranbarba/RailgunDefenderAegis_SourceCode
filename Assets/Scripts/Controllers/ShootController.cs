using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    // Shoot related attributes
    private bool can_shoot_timer_ = false;
    private float last_shoot_time = 0.0f;
    private Vector3 projectile_direction_;
    
    public GameObject projectile_prefab_ = null;
    public float shoot_cadence_ = 2.0f;
    public float shoot_life_time_ = 4.0f;
    public float shoot_speed_ = 2.0f;

    public void CheckShooting()
    {
        if (can_shoot_timer_ && transform.GetComponent<Enemy>().IsOnTheScreen(GetComponent<SpriteRenderer>()) && transform.GetComponent<Enemy>().GetCore() != null)
        {
            if(transform.GetComponent<Enemy>().shot_type_ == 0){
                BasicShot();
            } else if(transform.GetComponent<Enemy>().shot_type_ == 1){
                LateralShot();
            }
        }
        else
        {
            if(Time.time - last_shoot_time > shoot_cadence_)
            {
                can_shoot_timer_ = true;
            }
        }
    }

    private void BasicShot()
    {
        can_shoot_timer_ = false;
        last_shoot_time = Time.time;
        ProjectileController sp = Instantiate(projectile_prefab_, transform.position, Quaternion.identity).GetComponent<ProjectileController>();
        sp.SetLifeTime(shoot_life_time_);
        sp.SetHostile(true);
        sp.GetComponent<SpriteRenderer>().color = Color.red;
        Vector3 target_look = transform.GetComponent<Enemy>().GetPlayer().transform.position - sp.transform.position;
        float angle_to_look = (Mathf.Atan2(target_look.y, target_look.x) * Mathf.Rad2Deg) - 90.0f;
        sp.transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);
        Rigidbody2D rb_projectile_;
        if (projectile_prefab_.GetComponent<Rigidbody2D>() != null)
        {
            if(transform.GetComponent<Enemy>().GetPlayer() != null)
            {
                rb_projectile_ = sp.GetComponent<Rigidbody2D>();
                projectile_direction_ = transform.GetComponent<Enemy>().GetPlayer().transform.position - transform.position;
                rb_projectile_.velocity = projectile_direction_.normalized * shoot_speed_;
            }
            
        }
    }

    private void LateralShot()
    {
        can_shoot_timer_ = false;
        for(int i = 0; i < 2; i++){
            last_shoot_time = Time.time;
            ProjectileController sp = Instantiate(projectile_prefab_, transform.position, Quaternion.identity).GetComponent<ProjectileController>();
            sp.SetLifeTime(shoot_life_time_);
            sp.SetHostile(true);
            sp.GetComponent<SpriteRenderer>().color = Color.red;
            Rigidbody2D rb_projectile_;
            if (projectile_prefab_.GetComponent<Rigidbody2D>() != null)
            {
                if(transform.GetComponent<Enemy>().GetPlayer() != null)
                {
                    rb_projectile_ = sp.GetComponent<Rigidbody2D>();
                    Vector3 direction = transform.up;
                    if (i == 0){
                        projectile_direction_ = new Vector3(direction.y, -direction.x, direction.z);
                    } else {
                        projectile_direction_ = new Vector3(-direction.y, direction.x, direction.z);
                    }
                    float angle_to_look = (Mathf.Atan2(projectile_direction_.y, projectile_direction_.x) * Mathf.Rad2Deg) - 90.0f;
                    sp.transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);
                    rb_projectile_.velocity = projectile_direction_.normalized * shoot_speed_;
                }
            }
        }
    }

    public void LateralShotPowUp(){
        for(int i = 0; i < 2; i++){
            ProjectileController proj = Instantiate(projectile_prefab_, transform.position, transform.rotation).GetComponent<ProjectileController>();
            proj.is_gen_by_powup = true;
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector3 direction = transform.up;
                if (i == 0){
                    projectile_direction_ = new Vector3(direction.y, -direction.x, direction.z);
                } else {
                    projectile_direction_ = new Vector3(-direction.y, direction.x, direction.z);
                }
                float angle_to_look = (Mathf.Atan2(projectile_direction_.y, projectile_direction_.x) * Mathf.Rad2Deg) - 90.0f;
                proj.transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);
                rb.velocity = projectile_direction_ * transform.GetComponent<Enemy>().GetPlayer().GetComponent<Railgun>().shoot_speed_ 
                    * transform.GetComponent<Enemy>().GetPlayer().GetComponent<Railgun>().mult_shot_speed_;
            }
        }
    }

    public void CircularShot(){
    for(int i = 0; i < 6; i++){
        GameObject proj = Instantiate(projectile_prefab_, transform.position, transform.rotation);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float angle = i * 60.0f;
            float radian = angle * Mathf.Deg2Rad;
            projectile_direction_ = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0);
            float angle_to_look = (Mathf.Atan2(projectile_direction_.y, projectile_direction_.x) * Mathf.Rad2Deg) - 90.0f;
            proj.transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);
            rb.velocity = projectile_direction_ * transform.GetComponent<Enemy>().GetPlayer().GetComponent<Railgun>().shoot_speed_/1.5f 
                * transform.GetComponent<Enemy>().GetPlayer().GetComponent<Railgun>().mult_shot_speed_;
        }
    }
}
}
