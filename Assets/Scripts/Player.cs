using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables

    [SerializeField] private AudioClip laserSound;
    [SerializeField] private GameObject multipleShotPrefab;
    [SerializeField] private GameObject mShots2Prefab;
    [SerializeField] private GameObject firepoint;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject tripleLaserPrefab;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject leftEngine;
    [SerializeField] private GameObject rightEngine;
    [SerializeField] private UIManager uiManager;

    [SerializeField] private bool hasAmmo;
    [SerializeField] private bool isMultipleShotActive;
    [SerializeField] private bool isMShots2Active;
    [SerializeField] private bool isTripleLaserActive;
    [SerializeField] private bool isSpeedBoostActive;
    [SerializeField] private bool isShieldActive;
    [SerializeField] private float canFire = 0f;
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float speedUpSpeed = 2f;
    [SerializeField] private float leftShiftSpeedMult = 1.5f;
    [SerializeField] private float totalSpeed;
    [SerializeField] private float fireRate = 2.0f;
    [SerializeField] private float MultipleShotFireRate = 5.0f;
    [SerializeField] private int lives = 3;
    [SerializeField] private int score;
    [SerializeField] private int ammoCount;

    private SpawnManager spawnManager;
    private AudioSource audioSource;

    private bool isLeftShiftKeyPressed;

    #endregion

    private void Start()
    {
        transform.position = new Vector2(0f, -3.0f);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError("There is no AudioSource component in Player script");

        audioSource.clip = laserSound;

        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        if (spawnManager == null)
            Debug.LogError("There is no SpawnManager script in spawnManager");

        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (uiManager == null)
            Debug.LogError("No UI Manager found. Null");

        shield.SetActive(false);
        leftEngine.SetActive(false);
        rightEngine.SetActive(false);

        //Limit the lasers fired by the player to only 15 shots.
        hasAmmo = true;
        //ammoCount = 15;
        uiManager.UpdateAmmo(ammoCount);
        uiManager.thrusterBarImage.fillAmount = 0.5f;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            isLeftShiftKeyPressed = true;
        else isLeftShiftKeyPressed = false;
            
        MovePlayer();

        // if Time.time = 5, Time.time > 0f => true
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > canFire && hasAmmo)
        {
            // player must wait until Time.time = 5.8f to shoot again
            FireLaser();
        }
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 moveDirection = new Vector2(horizontal, vertical).normalized;

        // Increase speed with speedUp powerup
        if (isSpeedBoostActive)
        {
            totalSpeed = playerSpeed * speedUpSpeed;
            transform.Translate(moveDirection * totalSpeed * Time.deltaTime);
            IncreaseThrusterBar();
        }
        // Increase speed with Left shift key
        else if (isLeftShiftKeyPressed)
        {
            totalSpeed = playerSpeed * leftShiftSpeedMult;
            transform.Translate(moveDirection * totalSpeed * Time.deltaTime);
            IncreaseThrusterBar();
        }
        // Normal Player movement
        else
        {
            totalSpeed = playerSpeed; // Just for testing
            transform.Translate(moveDirection * playerSpeed * Time.deltaTime);
            DecreaseThrusterBar();
        }

        // Set up player movement constraints 
        float xPos = transform.position.x;
        float yPos = transform.position.y;

        if (yPos >= 0f) transform.position = new Vector2(xPos, 0f);
        else if (yPos <= -3.5f) transform.position = new Vector2(xPos, -3.5f);

        if (xPos >= 10.4f) transform.position = new Vector2(-10.4f, yPos);
        else if (xPos <= -10.4f) transform.position = new Vector2(10.4f, yPos);
    }

    private void IncreaseThrusterBar()
    {
        uiManager.thrusterBarImage.fillAmount += 0.005f;
    }

    private void DecreaseThrusterBar()
    {
        if ((!isLeftShiftKeyPressed || !isSpeedBoostActive))
            uiManager.thrusterBarImage.fillAmount -= 0.005f;
        
        if (uiManager.thrusterBarImage.fillAmount <= 0.5f)
            uiManager.thrusterBarImage.fillAmount = 0.5f;
    }

    private void FireLaser()
    {
        // FIX: code logic modified due to a bug, improving code
        // ASK: Can be these conditions used in a switch?
        if (isMShots2Active)
        {
            SetCanFire(5f);
            Instantiate(mShots2Prefab, firepoint.transform.position, Quaternion.identity);
            isMShots2Active = false;
        }

        // Fire MultipleShot
        else if (isMultipleShotActive)
        {
            SetCanFire(MultipleShotFireRate);
            Instantiate(multipleShotPrefab, firepoint.transform.position, Quaternion.identity);
            isMultipleShotActive = false;
        }
        // Shot triple laser
        else if (isTripleLaserActive)
        {
            SetCanFire(fireRate);
            Instantiate(tripleLaserPrefab, transform.position, Quaternion.identity);
        }
        // Shot one laser
        else
        {
            SetCanFire(fireRate);
            Vector2 laserPos = transform.position + new Vector3(0, 1.0f);
            Instantiate(laserPrefab, laserPos, Quaternion.identity);
        }
        audioSource.Play();
        ammoCount--;

        if (ammoCount >= 1)
            uiManager.UpdateAmmo(ammoCount);

        else
        {
            uiManager.OnEmptyAmmo();
            hasAmmo = false;
        }
    }

    private void SetCanFire(float rate)
    {
        canFire = Time.time + rate;
    }

    public void Damage(int damage)
    {
        lives -= damage;
        uiManager.UpdateLives(lives);
        ChangeShieldColor();
        if (lives == 0) Destroy(gameObject);
        //Camera.main.GetComponent<CameraEffects>().Shake();
    }

    public void AddScore(int points)
    {
        score += points;
        uiManager.UpdateScore(score);
    }

    #region Activate powerups mechanics
    public void ActivateTripleLaser()
    {
        isTripleLaserActive = true;
        StartCoroutine(TripleLaserRoutine());
    }

    IEnumerator TripleLaserRoutine()
    {
        yield return new WaitForSeconds(5f);
        isTripleLaserActive = false;
    }

    public void ActivateSpeedBoost()
    {
        isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostRoutine());
    }

    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(5f);
        isSpeedBoostActive = false;
    }

    public void ActivateShield()
    {
        isShieldActive = true;
        shield.SetActive(true);
        lives = 3;
        ChangeShieldColor();
        uiManager.UpdateLives(lives);
    }

    public void ChangeShieldColor()
    {
        if (isShieldActive)
        {
            switch (lives)// Change shield color
            {
                case 3:
                    shield.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f, 0.7f);
                    break;
                case 2:
                    shield.GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0f, 0.7f);
                    leftEngine.SetActive(true);
                    break;
                case 1:
                    shield.GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 0f, 0.6f);
                    rightEngine.SetActive(true);
                    break;
                case 0:
                    spawnManager.OnPlayerDestroyed();
                    Destroy(gameObject);
                    break;
            }
        }
    }

    public void RefillAmmo()
    {
        ammoCount = 15;
        hasAmmo = true;
        uiManager.UpdateAmmo(ammoCount);
        uiManager.OnFullAmmo();
    }

    public void RecoverHealth()
    {
        if (lives >= 3) return;
        lives++;
        uiManager.UpdateLives(lives);
        ChangeShieldColor();
    }

    public void ActivateMshots2()
    {
        // Shoot it in FireLaser
        isMShots2Active = true;
        ammoCount++;
        uiManager.UpdateAmmo(ammoCount);
    }
    //Create a new form of projectile such as a new multidirection shot
    // Replace the standard fire to 5 seconds
    // Spawns rarely

    // Create pic
    // Create prefab
    // Create mechanic
    // Create script
    // gameobject array laser
    // for each obj in array
    // random float to rotate
    // Instantiate
    // transform.translate(forw * speed * t.delt)
    public void ActivateMultipleShot()
    {
        isMultipleShotActive = true;
        ammoCount++;
        uiManager.UpdateAmmo(ammoCount);
    }

    #endregion
}

// TODO: decide if reparing engines with new shield
// DONE: make a moving background
// INFO: a new mechanic can be created to reduce shield keeping player lives. If so, set isShieldActive to false where needed