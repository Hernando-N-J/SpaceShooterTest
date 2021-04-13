using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float laserSpeed = 5f;
    [SerializeField] private Player player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        MoveLaserUp();
    }

    public void MoveLaserUp()
    {
        transform.Translate(Vector2.up * laserSpeed * Time.deltaTime);

        if (transform.position.y >= 7.5f)
        {
            Destroy(gameObject);

            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            this.player.AddScore(10);
            Destroy(gameObject);
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.OnEnemyDestroyedByLaser();

        }
    }
}