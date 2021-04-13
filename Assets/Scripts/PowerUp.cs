using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] AudioClip powerupSound;
    
    [SerializeField] private float speed = 3f;
    [SerializeField] private int powerupID; 

    void Update()
    {
        MovePowerup();

        if (transform.position.y < -6f)
            Destroy(gameObject);
    }

    private void MovePowerup()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(powerupSound, transform.position, 0.3f);

            Player player = other.GetComponent<Player>();
            
            if (player != null)
            {
                switch (powerupID)
                {
                    case 0: player.ActivateTripleLaser(); break;
                    case 1: player.ActivateSpeedBoost(); break;
                    case 2: player.ActivateShield(); break;
                    case 3: player.RefillAmmo();break;
                    case 4: player.RecoverHealth(); break;
                    case 5: player.ActivateMultipleShot(); break;
                    case 6: player.ActivateMshots2(); break;
                    default: Debug.Log("Default message in switch"); break;
                }
            }
            else Debug.LogError("There is no Player script in player (other)");

            Destroy(gameObject);
        }
    }
}
