using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 1f;

    private Vector2 moveDir;

    public void Launch(Vector2 direction)
    {
        moveDir = direction.normalized;
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy") || other.GetComponent<EnemyProjectile>() != null)
        {
            return; 
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}