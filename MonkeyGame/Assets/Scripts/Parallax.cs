using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float lenght, starpos;
    public GameObject came;
    public float parallaxEffect;
    public bool move;

    void Start()
    {
        if (move) starpos = transform.position.x;
        else starpos = transform.position.x;

        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {

        if (move)
        {
            float temp = (came.transform.position.x * (1 - parallaxEffect));
            float distance = (came.transform.position.x * parallaxEffect);
            transform.position = new Vector3(transform.position.x - parallaxEffect * Time.deltaTime, transform.position.y, transform.position.z);

            if (temp > transform.position.x + lenght) transform.position = new Vector2(transform.position.x + lenght, transform.position.y);
            else if (temp < transform.position.x - lenght) transform.position = new Vector2(transform.position.x - lenght, transform.position.y); ;
        }
        else
        {
            float temp = (came.transform.position.x * (1 - parallaxEffect));
            float distance = (came.transform.position.x * parallaxEffect);

            float tempy = (came.transform.position.y * (1 - parallaxEffect));
            float distancey = (came.transform.position.y * parallaxEffect);

            transform.position = new Vector3(starpos + distance, starpos + distancey, transform.position.z);

            if (temp > starpos + lenght) starpos += lenght;
            else if (temp < starpos - lenght) starpos -= lenght;
        }
    }
}

