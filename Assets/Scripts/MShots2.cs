using System.Collections;
using UnityEngine;

public class MShots2 : MonoBehaviour
{
    public GameObject laserPrefab;
    public float laserRotation;
    public float lasersAmount;

    private void Start()
    {
        lasersAmount = 50;
        laserRotation = 5f;
        StartCoroutine(MShots2Routine());
    }

    IEnumerator MShots2Routine()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < lasersAmount; j++)
            {
                Instantiate(laserPrefab, transform.position, Quaternion.Euler(0, 0, laserRotation));
                laserRotation += 21f;
            }
            yield return new WaitForSeconds(1);
        }
        
    }
}
