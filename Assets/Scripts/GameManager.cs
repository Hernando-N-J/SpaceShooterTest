using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isGameOver;
    [SerializeField] private GameObject asteroidPrefab;

    private void Start()
    {
        //Instantiate(asteroidPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && isGameOver)
            SceneManager.LoadScene(1);

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }

    public void OnGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0.19f;
    }
}