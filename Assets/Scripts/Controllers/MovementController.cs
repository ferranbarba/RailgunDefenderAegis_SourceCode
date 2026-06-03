using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    // Circle related attributes
    public float radius_circle_decay_ = 1.0f; // Velocidad de acercamiento
    private float angle_ = 0.0f; // Angulo para movimiento circular
    private float radius_circle_; // Radio dinamico

    // Erratic related attributes
    public float radius_erratic_decay_ = 1.5f;
    public GameObject shadow_ = null;
    public bool is_shadow_trail_active = true;
    private bool pos_checker_ = false;
    private GameObject shadow_trail_ = null;

    public void Move()
    {
        if (transform.GetComponent<Enemy>().GetCore() != null)
        {
            if(transform.GetComponent<Enemy>().movement_type_ == 0){
                BasicMovement();
            } else if(transform.GetComponent<Enemy>().movement_type_ == 1){
                CircularMovement();
            } else if(transform.GetComponent<Enemy>().movement_type_ == 2){
                ErraticMovement();
            }
        } 
    }

    public void MovementStart()
    {
        if(transform.GetComponent<Enemy>().movement_type_ == 0){
            // Basic and Kamikaze setup
            transform.GetComponent<Enemy>().SetGoalPos(new Vector3(Screen.width / 2, Screen.height / 2, 1.0f));
            Vector3 target_look = Vector3.zero;
            if (transform.GetComponent<Enemy>().GetCore() != null) target_look = transform.GetComponent<Enemy>().GetCore().transform.position - transform.position;
            float angle_to_look = (Mathf.Atan2(target_look.y, target_look.x) * Mathf.Rad2Deg) - 90.0f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);
        } else if(transform.GetComponent<Enemy>().movement_type_ == 1){
            // Circular setup
            transform.GetComponent<Enemy>().SetGoalPos(new Vector3(Screen.width / 2, Screen.height / 2, 1.0f));
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            radius_circle_ = Vector3.Distance(transform.position, transform.GetComponent<Enemy>().GetCore().transform.position);
            angle_ = Mathf.Atan2(transform.position.y - transform.GetComponent<Enemy>().GetCore().transform.position.y, 
                            transform.position.x - transform.GetComponent<Enemy>().GetCore().transform.position.x);
        } else if(transform.GetComponent<Enemy>().movement_type_ == 2){
            // Erratic setup
            transform.GetComponent<Enemy>().SetGoalPos(transform.GetComponent<Enemy>().GetCore().transform.position);
            Vector3 target_look = transform.GetComponent<Enemy>().GetCore().transform.position - transform.position;
            float angle_to_look = (Mathf.Atan2(target_look.y, target_look.x) * Mathf.Rad2Deg) - 90.0f;
            transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);
        }

        transform.GetComponent<Enemy>().SetRB(GetComponent<Rigidbody2D>());
    }

    private void BasicMovement()
    {
        Vector3 dist = transform.GetComponent<Enemy>().GetCore().transform.position - transform.position;
        transform.position = dist.normalized * transform.GetComponent<Enemy>().speed_ * Time.deltaTime + transform.position;
    }

    private void CircularMovement()
    {
        angle_ += transform.GetComponent<Enemy>().speed_ * Time.deltaTime * 0.5f;

        radius_circle_ = Mathf.Max(0, radius_circle_ - radius_circle_decay_ * Time.deltaTime);

        float x = Mathf.Cos(angle_) * radius_circle_ / 2;
        float y = Mathf.Sin(angle_) * radius_circle_ / 2;

        transform.position = new Vector3(x, y, 0f);
        Vector3 directionToCenter = (transform.GetComponent<Enemy>().GetCore().transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, directionToCenter); 
        transform.rotation = rotation;
    }

    private void ErraticMovement()
    {
        Vector3 dist = transform.GetComponent<Enemy>().GetGoalPos() - transform.position;
        transform.position = dist.normalized * transform.GetComponent<Enemy>().speed_ * Time.deltaTime + transform.position;

        if(!transform.GetComponent<Enemy>().IsOnTheScreen(GetComponent<SpriteRenderer>())){
            transform.GetComponent<Enemy>().SetGoalPos(transform.GetComponent<Enemy>().GetCore().transform.position);
            pos_checker_ = false;
        } 
        else if(!pos_checker_)
        {   
            pos_checker_ = true;
            if(is_shadow_trail_active) DestroyShadowTrail();

            SetGoalPos();
        }
        
        if(Vector3.Distance(transform.position, transform.GetComponent<Enemy>().GetGoalPos()) < 0.1f && pos_checker_)
        {
            if(is_shadow_trail_active) DestroyShadowTrail();
            SetGoalPos();
        }
    }

    // Sets shadow trail for erratic movement
    private void SetShadowTrail(Vector3 startPos, Vector3 endPos)
    {
        DestroyShadowTrail();

        shadow_trail_ = Instantiate(shadow_, startPos, Quaternion.identity);
        float distance = Vector3.Distance(startPos, endPos);

        shadow_trail_.transform.localScale = new Vector3(
            shadow_trail_.transform.localScale.x, // Mantener ancho
            distance / shadow_.GetComponent<SpriteRenderer>().bounds.size.y * 8.0f, // Ajustar largo
            1f
        );

        Vector3 direction = (endPos - startPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        shadow_trail_.transform.rotation = Quaternion.Euler(0, 0, angle);
        shadow_trail_.GetComponent<SpriteRenderer>().sortingOrder = -1;
        shadow_trail_.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
    }

    // Sets next position for erratic movement
    private void SetGoalPos()
    {
        Vector2 forwardDirection = transform.up.normalized;
        float randomAngle = Random.Range(-90f, 90f);
        Vector2 offsetDirection = Quaternion.Euler(0, 0, randomAngle) * forwardDirection;

        Vector2 randomOffset = offsetDirection * radius_erratic_decay_;
        transform.GetComponent<Enemy>().SetGoalPos(transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f));

        Vector3 target_look = transform.GetComponent<Enemy>().GetGoalPos() - transform.position;
        float angle_to_look = Mathf.Atan2(target_look.y, target_look.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle_to_look);

        if(is_shadow_trail_active) SetShadowTrail(transform.position + transform.up * 2.0f, transform.GetComponent<Enemy>().GetGoalPos());
    }

    public void DestroyShadowTrail(){
        if (shadow_trail_ != null)
        {
            Destroy(shadow_trail_);
        }
    }
}
