using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float lifetime_ = 10.0f;
    private Rigidbody2D rb;
    private bool is_hostile_ = false;
    public bool is_powup_active = false;
    public bool is_gen_by_powup = false;
    [SerializeField] 
    private GameObject explosion_animation_; 

    private Vector2 temp_velocity_;

    // void Start()
    // {
    //     rb = GetComponent<Rigidbody2D>();
    //     Destroy(gameObject, lifetime_);
    // }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke(nameof(DestroyProjectile), lifetime_);
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    void Update(){
        if(GameManager.Instance_.is_game_paused_ && GetComponent<Rigidbody2D>().velocity != Vector2.zero){
            temp_velocity_ = GetComponent<Rigidbody2D>().velocity;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        } else if(!GameManager.Instance_.is_game_paused_ && GetComponent<Rigidbody2D>().velocity == Vector2.zero){
            GetComponent<Rigidbody2D>().velocity = temp_velocity_;
            lifetime_ -= 0.1f;
        }
    }

    public void SetLifeTime(float lifetime)
    {
        lifetime_ = lifetime;
    }

    public float GetLifeTime()
    {
        return lifetime_;
    }

    public void SetHostile(bool hostile)
    {
        is_hostile_ = hostile;
    }

    public bool IsHostile()
    {
        return is_hostile_;
    }

    public void Explode(Color explosionColor, float expSize, bool isPowUpActive)
    {
        if (explosion_animation_ == null) return;

        Vector3 explosionPosition = transform.position;

        if (rb != null)
        {
            Vector3 direction = rb.velocity.normalized;
            explosionPosition += direction * 0.2f;
        }
        else
        {
            explosionPosition += transform.up * 0.2f;
        }

        GameObject animInstance = Instantiate(explosion_animation_, explosionPosition, Quaternion.identity);
        ParticleSystem[] particleSystems = animInstance.GetComponentsInChildren<ParticleSystem>();

        float maxDuration = 0f;
        foreach (ParticleSystem ps in particleSystems)
        {
            var main = ps.main;
            main.startSize = main.startSize.constant * expSize;
            main.startColor = explosionColor;

            float duration = main.duration + main.startLifetime.constantMax;
            if (duration > maxDuration)
            {
                maxDuration = duration;
            }
        }

        animInstance.transform.localScale = new Vector3(expSize, expSize, expSize);

        transform.GetComponent<AudioSource>().Play();

        Destroy(animInstance, maxDuration);
        if(!isPowUpActive)
        {
            Destroy(gameObject);
        } else {
            is_gen_by_powup = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy shots power up
        if(collision.GetComponent<ProjectileController>() != null && is_powup_active)
        {
            if(collision.GetComponent<ProjectileController>().IsHostile()){
                collision.GetComponent<ProjectileController>().Explode(new Color(0.0f, 1.0f, 0.9157f, 1.0f), 0.15f, false);
            }
        }

        if(collision.GetComponentInParent<Boss>() != null && collision.CompareTag("ENEMY")) // spawner boss shield
        {
            this.Explode(new Color(0.0f, 1.0f, 0.9157f, 1.0f), 0.3f, false);
        }
    }
}


