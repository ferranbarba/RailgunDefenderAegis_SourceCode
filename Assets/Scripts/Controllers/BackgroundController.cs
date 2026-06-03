using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public int star_density_;
    public int star_speed_min_;
    public int star_speed_max_;
    public GameObject star_prefab_;
    public Sprite[] backgrounds_;
    public Sprite[] planets_;
    public Sprite[] dust_;
    private GameObject[] stars_;
    private float[] speed_;
    private GameObject planets_local_;
    private GameObject dust_local_;

    private GameObject GenerateStar(float y_){
        return Instantiate(star_prefab_, new Vector3(Random.Range(-886, 886) * 0.01f, y_, 0), transform.rotation);
    }

    void Start()
    {
        stars_ = new GameObject[star_density_];
        speed_ = new float[star_density_];
        dust_local_ = new GameObject();
        planets_local_ = new GameObject();

        // Stars
        for(int i = 0; i < star_density_; i++){
            stars_[i] = GenerateStar(Random.Range(-500, 500) * 0.01f);
            speed_[i] = Random.Range(star_speed_min_, star_speed_max_) * 0.001f;
            float scale_ = Random.Range(1, 3) * 0.01f;
            stars_[i].transform.localScale = new Vector3(scale_, scale_, 0);
        }
        this.GetComponent<SpriteRenderer>().sprite = backgrounds_[Random.Range(0, backgrounds_.Length - 1)];
        
        // Dust generation
        dust_local_.AddComponent<SpriteRenderer>();
        dust_local_.GetComponent<SpriteRenderer>().sprite = dust_[Random.Range(0, dust_.Length - 1)];
        dust_local_.GetComponent<SpriteRenderer>().sortingOrder = -4;
        dust_local_.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.7f);
        float dustSize = Random.Range(0.5f, 1.0f);
        dust_local_.transform.localScale = new Vector3(dustSize, dustSize, 0);
        dust_local_.transform.position = new Vector3(Random.Range(-886, 886) * 0.01f, Random.Range(0, 1500) * 0.01f, 0);

        // Planet Generation
        planets_local_.AddComponent<SpriteRenderer>();
        planets_local_.GetComponent<SpriteRenderer>().sprite = planets_[Random.Range(0, planets_.Length - 1)];
        planets_local_.GetComponent<SpriteRenderer>().sortingOrder = -4;
        float planetSize = Random.Range(0.07f, 1.0f);
        planets_local_.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        planets_local_.transform.localScale = new Vector3(planetSize, planetSize, 0);
        planets_local_.transform.position = new Vector3(Random.Range(-886, 886) * 0.01f, Random.Range(0, 1000) * 0.01f, 0);

    }

    void Update()
    {
        for(int i = 0; i < star_density_; i++){
            float newPos = stars_[i].transform.position.y - speed_[i]  * Time.deltaTime * 1000;
            stars_[i].transform.position = new Vector3(stars_[i].transform.position.x, newPos, stars_[i].transform.position.y);
            if(stars_[i].transform.position.y < -5){
                Destroy(stars_[i]);
                stars_[i] = GenerateStar(5.0f);
                speed_[i] = Random.Range(star_speed_min_, star_speed_max_) * 0.001f;
            }
        }

        planets_local_.transform.position = new Vector3(planets_local_.transform.position.x, planets_local_.transform.position.y - 0.0001f * Time.deltaTime * 1000, planets_local_.transform.position.z);
        dust_local_.transform.position = new Vector3(dust_local_.transform.position.x, dust_local_.transform.position.y - 0.0001f * Time.deltaTime * 1000, dust_local_.transform.position.z);

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.0001f * Time.deltaTime * 1000, this.transform.position.z);

        if(this.transform.position.y <= -5.25f){
            this.transform.position = new Vector3(this.transform.position.x, 5.25f, this.transform.position.z);
        }
    }
}
