using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject player_;
    public GameObject core_;
    public GameObject[] prefab_enemy_;
    public GameObject energy_life_;   
    public int spawning_radius_ = 10;
    public int spawning_min_distance_ = 5;
    private int max_enemies_on_screen_;      // max enemies alive at the same time
    public int current_enemies_;        // actual alive enemies
    public int current_enemies_on_screen;

    [SerializeField]
    private double time_to_spawn_enemy_ = 0.5;
    [SerializeField]
    public bool can_spawn_;
    public double last_spawn_;
    private Vector3 random_pos_;

    private const string ENEMY_TAG = "ENEMY";

    public void SetLevelSpawningParameters(int enemies_to_spawn, int max_on_screen)
    {
        current_enemies_ = enemies_to_spawn;
        current_enemies_on_screen = 0;
        max_enemies_on_screen_ = max_on_screen;
    }

    public void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(ENEMY_TAG);

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public int GetCurrentEnemiesAlive()
    {
        return current_enemies_on_screen;
    }
    public int GetEnemiesLeft()
    {
        return current_enemies_;
    }
    private Vector3 GetRandomPositionInRadius(float radius)
    {
        Quaternion quat = Quaternion.Euler(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), 1.0f);

        // get forward vector
        Vector3 forward = quat * Vector3.forward;

        // get random distance from center, and move that way to find position
        Vector3 position = forward * Random.Range(spawning_min_distance_, radius);

        return position;
    }

    // Start is called before the first frame update
    void Start()
    {
        can_spawn_ = false;
        last_spawn_ = 0.0;
    }

    public void SpawningEnemies(int max_prefab_index)
    {
        random_pos_ = GetRandomPositionInRadius(spawning_radius_); // no funciona como deberia

        GameObject enemy = Instantiate(prefab_enemy_[Random.Range(0, max_prefab_index)], random_pos_, transform.rotation);

        do
        {
            enemy.transform.position = GetRandomPositionInRadius(spawning_radius_);
        } while (enemy.GetComponent<Enemy>().IsOnTheScreen(enemy.transform.GetComponent<SpriteRenderer>()));

        enemy.GetComponent<Enemy>().SetObjectives(core_, player_, energy_life_);

        //Debug.Log("ENEMY SPAWNED");

        current_enemies_--;
        current_enemies_on_screen++;
        enemy.tag = ENEMY_TAG;
    }

    public bool SpawnMode(int max_index_prefab_enemy)
    {
        if (can_spawn_ && current_enemies_on_screen < max_enemies_on_screen_)
        {
            SpawningEnemies(max_index_prefab_enemy);
            can_spawn_ = false;
            last_spawn_ = Time.time;
        }
        else
        {
            if (Time.time - last_spawn_ > time_to_spawn_enemy_)
            {
                can_spawn_ = true;
            }
        }

        /*Debug.Log("Can spawn -> " + can_spawn_);
        Debug.Log("Time minus last spanw -> " + (Time.time - last_spawn_));
        Debug.Log("time_to_spawn_enemy_ -> " + time_to_spawn_enemy_);
        Debug.Log("Current on screen -> " + current_enemies_on_screen);
        Debug.Log("Max on screen -> " + max_enemies_on_screen_);
        Debug.Log("Current enemies -> " + current_enemies_);*/


        return current_enemies_ == 0;
    }

    public void EnemyKilled()
    {
        current_enemies_--;
        current_enemies_on_screen--;
    }
}
