using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawn : MonoBehaviour
{
    [HideInInspector]
    public float timer;
    [Header("Options")]
    public float spawnSeconds;
    public float LifeSeconds;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        timer = spawnSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = spawnSeconds;
            GameObject temp = Instantiate(obj, transform.position, Quaternion.identity);
            Destroy(temp, LifeSeconds);
        }
    }
}
