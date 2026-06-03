using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    // General attributes
    private Vector3 goal_pos_;
    private Rigidbody2D rb;
    public GameObject core_ = null;
    public GameObject player_ = null;   
    public GameObject energy_life_ = null;   
    public bool can_shoot_;
    public float speed_ = 2.0f;
    public float exp_to_add_ = 1.0f; // por defecto
    [SerializeField]
    public GameObject exp_prefab_;
    public int shot_type_;
    public int movement_type_;

    public void SetObjectives(GameObject core, GameObject player, GameObject energyLife)
    {
        core_ = core;
        player_ = player;
        energy_life_ = energyLife;
    }
    
    public bool IsOnTheScreen(SpriteRenderer spriteRenderer)
    {
        Bounds spriteBounds = spriteRenderer.bounds;
        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(spriteBounds.min.x, spriteBounds.min.y, 0);
        corners[1] = new Vector3(spriteBounds.min.x, spriteBounds.max.y, 0);
        corners[2] = new Vector3(spriteBounds.max.x, spriteBounds.min.y, 0);
        corners[3] = new Vector3(spriteBounds.max.x, spriteBounds.max.y, 0);

        for(int i = 0; i < 4; i++)
        {
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(corners[i]);
            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                viewportPoint.z > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnEXP()
    {
        if (exp_prefab_)
        {
            GameObject exp = Instantiate(exp_prefab_, transform.position, Quaternion.identity);
            exp.GetComponent<EXP>().SetPlayer(core_);
            exp.GetComponent<EXP>().SetTargetPos(core_.GetComponent<CoreController>().exp_target_pos_.transform.position);
            exp.GetComponent<EXP>().SetExpToAdd(exp_to_add_);
        }
        
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        ProjectileController proj = collision.gameObject.GetComponent<ProjectileController>();
        if (proj != null && IsOnTheScreen(GetComponent<SpriteRenderer>()))
        {
            if (!proj.IsHostile())
            {
                // Destroy shadow trail
                if(movement_type_ == 2){
                    transform.GetComponent<MovementController>().DestroyShadowTrail();
                }

                // Divided shot power up
                if(player_.GetComponent<Railgun>().shot_divide_){
                    transform.GetComponent<ShootController>().LateralShotPowUp();
                }

                // Turret type 3
                if(GameManager.Instance_.current_ship_selected_index_ == 2){
                    transform.GetComponent<ShootController>().CircularShot();
                }

                // Particle animation
                collision.GetComponent<ProjectileController>().Explode(new Color(0.0f, 1.0f, 0.9157f, 1.0f), 0.3f, player_.GetComponent<Railgun>().piercing_shot_);

                if (GameObject.FindWithTag("SPAWNER") != null)
                {
                    GameObject.FindWithTag("SPAWNER").GetComponent<EnemySpawner>().EnemyKilled();
                }

                if (GameObject.FindWithTag("SPAWNER")){
                    if(GameManager.Instance_.time_left_ > 0){
                        SpawnEXP();
                    }
                    
                }else if (SceneManager.GetActiveScene().name == "Begin")
                {
                    SpawnEXP();
                }

                if (energy_life_ != null && core_ != null)
                {
                    int chances = 0;
                    if (core_.GetComponent<CoreController>().current_player_life_ <= 33)
                    {
                        chances = 50;
                    }
                    else if (core_.GetComponent<CoreController>().current_player_life_ <= 66)
                    {
                        chances = 35;
                    }
                    else
                    {
                        chances = 20;
                    }

                    int randomNum = Random.Range(0, 100);
                    if (randomNum <= chances)
                    {
                        GameObject generated = Instantiate(energy_life_, transform.position, Quaternion.identity);
                        generated.GetComponent<EnergyLifeController>().core_ = core_;
                    }
                }
                GameManager.Instance_.current_player_score_ += 20;
                //Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        if(core_ == null && player_ == null)
        {
            if(GameObject.FindWithTag("SPAWNER") != null)
            {
                player_ = GameObject.FindWithTag("SPAWNER").GetComponent<EnemySpawner>().player_;
                core_ = GameObject.FindWithTag("SPAWNER").GetComponent<EnemySpawner>().core_;
            }
            
        }

        transform.GetComponent<MovementController>().MovementStart();

        if(transform.GetComponent<ShootController>() != null && shot_type_ == 1)
        {
            transform.GetComponent<ShootController>().shoot_cadence_ *= 0.5f;
        }

        exp_to_add_ = 0.1f;
    }

    void Update()
    {
        if(!GameManager.Instance_.is_game_paused_){
            transform.GetComponent<MovementController>().Move();
            if(transform != null && can_shoot_)
            {
                transform.GetComponent<ShootController>().CheckShooting();
            }
        }
    }

    public void SetRB(Rigidbody2D newRB)
    {
        rb = newRB;
    }

    public GameObject GetCore()
    {
        return core_;
    }

    public GameObject GetPlayer()
    {
        return player_;
    }

    public Vector3 GetGoalPos()
    {
        return goal_pos_;
    }

    public void SetGoalPos(Vector3 goal)
    {
        goal_pos_ = goal;
    }
}
